using Stx.Net.RoomBased;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Stx.Net.Unity.UI
{
    public class StxLobbyPlayers : MonoBehaviour
    {
        public List<StxLobbyPlayerDisplay> playerSpots;
        [Space]
        public Sprite defaultAvatar;
        public Sprite nobodyAvatar;
        [Space]
        public UnityStringEvent onJoin;
        public UnityStringEvent onLeave;
        public UnityStringEvent onReady;
        public UnityStringEvent onUnReady;

        public static string LobbyOwnerID { get; private set; }
        public static bool IsLobbyOwner
        {
            get
            {
                return LobbyOwnerID == StxUnityClient.C.NetworkID;
            }
        }

        void Awake()
        {
            StxPlayerDisplay.DefaultAvatar = defaultAvatar;
            StxPlayerDisplay.NobodyAvatar = nobodyAvatar;
        }

        void OnEnable()
        {
            StxUnityClient.C.OnSomeoneJoinedRoom += Client_OnSomeoneJoinedRoom;
            StxUnityClient.C.OnSomeoneLeftRoom += Client_OnSomeoneLeftRoom;
            StxUnityClient.C.OnSomeoneChangedStatus += Client_OnSomeoneChangedStatus;
            StxUnityClient.C.OnSomeonePromoted += Client_OnSomeonePromoted;

            StxUnityClient.F.GetCurrentRoomAsync((state, e) =>
            {
                if (state == PacketResponseStatus.Responded)
                    SetupLobby(e);
            });
        }

        private void SetupLobby(Room r)
        {
            LobbyOwnerID = r.OwnerClientID;

            // Hide the unused player spots.
            while (r.MaxPlayers < playerSpots.Count)
            {
                var v = playerSpots.Last();
                v.gameObject.SetActive(false);
                playerSpots.Remove(v);
            }

            foreach (string c in r.ConnectedClients)
            {
                StxUnityClient.F.GetClientIdentityAsync(c, (state, e) =>
                {
                    if (state == PacketResponseStatus.Responded)
                        Client_OnSomeoneJoinedRoom(e);
                });
            }

            StxInfoText.Infos.currentRoomPlayerCount = r.PlayerCount;
            StxInfoText.Infos.currentRoomName = r.Name;
        }

        void OnDisable()
        {
            StxUnityClient.C.OnSomeoneJoinedRoom -= Client_OnSomeoneJoinedRoom;
            StxUnityClient.C.OnSomeoneLeftRoom -= Client_OnSomeoneLeftRoom;
            StxUnityClient.C.OnSomeoneChangedStatus -= Client_OnSomeoneChangedStatus;
            StxUnityClient.C.OnSomeonePromoted -= Client_OnSomeonePromoted;
        }

        private void Client_OnSomeoneLeftRoom(ClientIdentity leftClient)
        {
            StxInfoText.Infos.currentRoomPlayerCount--;

            var spot = playerSpots.FirstOrDefault((e) => e.ForClient.NetworkID == leftClient.NetworkID);

            if (spot == null)
                return;

            spot.StopUse();
            onLeave?.Invoke(leftClient.NetworkID);
        }

        private void Client_OnSomeonePromoted(string promotedClient)
        {
            var spot = playerSpots.FirstOrDefault((e) => e.ForClient.NetworkID == promotedClient);

            if (spot == null)
                return;

            LobbyOwnerID = promotedClient;

            // Refresh the UI for the new owner.
            foreach (var s in playerSpots)
                if (s.SpotUsed)
                    s.EnableUI();
                else
                    s.DisableUI();
        }

        private void Client_OnSomeoneJoinedRoom(ClientIdentity joinedClient)
        {
            StxInfoText.Infos.currentRoomPlayerCount++;
    
            var spot = playerSpots.FirstOrDefault((e) => !e.SpotUsed);

            if (spot == null)
            {
                StxUnityClient.Logger.Log("Too less player display spots to display new player.", Logging.LoggedImportance.CriticalWarning);

                return;
            }

            spot.UseFor(joinedClient);
            onJoin?.Invoke(joinedClient.NetworkID);
        }

        private void Client_OnSomeoneChangedStatus(ClientIdentity clientWhoChanged, ClientRoomStatus newStatus)
        {
            //Debug.Log("Who: " + clientWhoChanged + "; Status: " + newStatus);

            var spot = playerSpots.FirstOrDefault((e) => e.ForClient.NetworkID == clientWhoChanged.NetworkID);

            if (spot == null)
                return;

            if (newStatus == ClientRoomStatus.Ready)
                onReady?.Invoke(clientWhoChanged.NetworkID);
            else if (newStatus == ClientRoomStatus.NotReady)
                onUnReady?.Invoke(clientWhoChanged.NetworkID);

            spot.SetStatus(newStatus);
        }
    }

    public class StxPlayerDisplay : MonoBehaviour
    {
        public ClientIdentity ForClient { get; private set; }
        public bool SpotUsed { get; private set; }

        public UnityStringEvent onUse;
        public UnityStringEvent onStopUse;
        [Header("UI Items")]
        public Image avatarImage;
        public Text nameText;
        public Text levelText;
        public Button addFriendButton;

        public static Sprite DefaultAvatar { get; set; } = null;
        public static Sprite NobodyAvatar { get; set; } = null;
        public static string LoadingLevelText { get; set; } = "<color=#660000>LVL ?</color>";
        public static string LevelTextFormat { get; set; } = "LVL {0}";

        void Start()
        {
            DisableUI();
        }

        public void UseFor(ClientIdentity client)
        {
            ForClient = client;
            SpotUsed = true;

            EnableUI();
            SetDefaultAvatar();
            SetUIFor(client);

            onUse?.Invoke(client.NetworkID);
        }

        public void StopUse()
        {
            onStopUse?.Invoke(ForClient.NetworkID);

            ForClient = null;
            SpotUsed = false;

            DisableUI();
        }

        public void SetDefaultAvatar()
        {
            if (avatarImage == null)
                return;

            avatarImage.sprite = DefaultAvatar;
        }

        public void SetNobodyAvatar()
        {
            if (avatarImage == null)
                return;

            avatarImage.sprite = NobodyAvatar;
        }

        public virtual void SetUIFor(ClientIdentity client)
        {
            if (client == null)
                client = new ClientIdentity();

            if (client.Info.AvatarUrl != null)
                StartCoroutine(ImageDownloader.DownloadTo(client.Info.AvatarUrl, avatarImage));

            nameText.text = client.DisplayName;
            levelText.text = LoadingLevelText;

            StxUnityClient.F.GetClientLevelAsync(client.NetworkID, (state, e) =>
            {
                if (state == PacketResponseStatus.Responded)
                    levelText.text = string.Format(LevelTextFormat, e);
            });
        }

        public virtual void DisableUI()
        {
            nameText.enabled = false;
            levelText.enabled = false;
            SetButton(addFriendButton, false);

            if (NobodyAvatar == null)
                avatarImage.enabled = false;
            else
                SetNobodyAvatar();
        }

        public virtual void EnableUI()
        {
            if (!gameObject.activeInHierarchy)
                return;

            nameText.enabled = true;
            levelText.enabled = true;
            SetButton(addFriendButton, true);
            avatarImage.enabled = true;

            addFriendButton.interactable = false;

            SetNobodyAvatar();
        }

        protected void SetButton(Button b, bool enabled)
        {
            b.enabled = enabled;
            if (b.image != null)
                b.image.enabled = enabled;
            Text t = b.GetComponentInChildren<Text>();
            if (t != null)
                t.enabled = enabled;
        }

    }

    public class StxLobbyPlayerDisplay : StxPlayerDisplay
    {
        public Text statusText;
        public Button kickButton;
        public Image ownerImage;
        [Space]
        public string readyText = "<color=#33FF11>Is Ready!</color>";
        public string notReadyText = "<color=#FF5555>Not Ready.</color>";
        public string nobodyText = "<color=#9999BB><i>Waiting...</i></color>";
        [Space]
        public UnityVoidEvent onReady;
        public UnityVoidEvent onUnReady;

        public static string NoStatusText { get; set; } = "<color=#660000>No Status?</color>";

        public override void SetUIFor(ClientIdentity client)
        {
            base.SetUIFor(client);

            statusText.text = GetTextForStatus(client.RoomStatus);

            //kickButton.interactable = false;
        }

        public override void DisableUI()
        {
            base.DisableUI();

            SetButton(kickButton, false);
            if (string.IsNullOrEmpty(nobodyText))
                statusText.enabled = false;
            else
                statusText.text = nobodyText;
            ownerImage.enabled = false;
        }

        public override void EnableUI()
        {
            base.EnableUI();

            if (!gameObject.activeInHierarchy)
                return;

            SetButton(kickButton, true);
            statusText.enabled = true;

            kickButton.interactable = StxLobbyPlayers.IsLobbyOwner/* && ForClientID != StxUnityClient.C.ClientID*/;
            ownerImage.enabled = ForClient.NetworkID == StxLobbyPlayers.LobbyOwnerID;            
        }

        public void SetStatus(ClientRoomStatus newStatus)
        {
            statusText.text = GetTextForStatus(newStatus);

            if (newStatus == ClientRoomStatus.Ready)
                onReady?.Invoke();
            else if (newStatus == ClientRoomStatus.NotReady)
                onUnReady?.Invoke();
        }

        public void ClickKick()
        {
            if (!StxLobbyPlayers.IsLobbyOwner/* || ForClientID == StxUnityClient.C.ClientID*/)
                return;

            StxUnityClient.F.KickFromRoomAsync((state, r) =>
            {
                if (state <= PacketResponseStatus.Okey)
                    StxUnityClient.Instance.DisplayAlert("You kicked a player from your room.");

            }, new string[] { ForClient.NetworkID });
        }

        public string GetTextForStatus(ClientRoomStatus status)
        {
            switch(status)
            {
                case ClientRoomStatus.NotReady:
                    return notReadyText;
                case ClientRoomStatus.Ready:
                    return readyText;
                default:
                case ClientRoomStatus.None:
                    return NoStatusText;
            }
        }
    }
}
