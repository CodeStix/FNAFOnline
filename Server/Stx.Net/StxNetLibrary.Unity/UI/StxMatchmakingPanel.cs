using Stx.Net.RoomBased;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stx.Net.Unity.UI
{
    public class StxMatchmakingPanel : MonoBehaviour
    {
        [Range(1.5f,25f)]
        public float refreshTime;
        [Header("UI Items")]
        public StxMatchmakingPanelRoomItem roomListItemPrefab;
        public GameObject refreshingObject;
        [Header("Events")]
        public UnityVoidEvent onListRefreshing;
        public UnityVoidEvent onListRefreshed;
        public UnityVoidEvent onJoinFailed;

        private float rt;
        private List<StxMatchmakingPanelRoomItem> items = new List<StxMatchmakingPanelRoomItem>();

        private static StxMatchmakingPanel Instance = null;

        void Start()
        {
            if (Instance == null)
                Instance = this;

            if (refreshTime < 1.5f)
                refreshTime = 1.5f;

            rt = 0.5f;
        }

        void Update()
        {
            rt -= Time.deltaTime;

            if (rt < 0f)
            {
                rt = refreshTime;

                RefreshList();
            }
        }

        public void RefreshList()
        {
            onListRefreshing?.Invoke();
            refreshingObject?.SetActive(true);

            ClearList();

            StxUnityClient.F.GetMatchmakingAsync(MatchmakingQuery.Default, (state, e) =>
            {
                foreach (Room r in e.Rooms)
                {
                    AddToList(r);
                }

                onListRefreshed?.Invoke();
                refreshingObject?.SetActive(false);
            });
        }

        public void AddToList(Room r)
        {
            var v = Instantiate(roomListItemPrefab, transform);
            v.room = r;
            v.Show();

            items.Add(v);
        }

        void ClearList()
        {
            items.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        public void SetJoinButton(bool enabled)
        {
            foreach(var item in items)
            {
                item.joinButton.interactable = enabled;
            }
        }

        public static void DisableButtons()
        {
            Instance.SetJoinButton(false);
        }

        public static void EnableButtons()
        {
            Instance.SetJoinButton(true);
        }

        public static void JoiningFailed()
        {
            Instance.onJoinFailed?.Invoke();
        }
    }

    public class StxMatchmakingPanelRoomItem : MonoBehaviour
    {
        public Text nameText;
        public Text ownerText;
        public Text playerCountText;
        public GameObject locked;
        public Button joinButton;

        [HideInInspector]
        public Room room;

        public void Show()
        {
            nameText.text = room.Name;
            ownerText.text = room.OwnerDisplayName;
            playerCountText.text = $"{ room.PlayerCount }/{ room.MaxPlayers }";
            locked.SetActive(room.Locked);
        }

        public void Join()
        {
            StxMatchmakingPanel.DisableButtons();

            if (room.Locked)
            {
                StxUnityClient.Instance.AskForInput($"Please enter the room password for <color=#FFFF22>{ room.Name }</color>", "", (input) =>
                {
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        room.Join(StxUnityClient.C, (s, e) =>
                        {
                            if (s <= PacketResponseStatus.Okey)
                            {
                                StxUnityClient.Logger.Log("Joined OK", Logging.LoggedImportance.Debug);

                                StxUnityClient.Instance.SceneSwitchLobby();

                            }
                            else
                            {
                                StxUnityClient.Logger.Log("Joining failed", Logging.LoggedImportance.Debug);

                                // Try again.
                                // return false;

                                StxMatchmakingPanel.JoiningFailed();
                            }

                        }, input);
                    }

                    StxMatchmakingPanel.EnableButtons();

                    return true;
                });
            }
            else
            {
                room.Join(StxUnityClient.C, (s, e) =>
                {
                    if (s <= PacketResponseStatus.Okey)
                    {
                        StxUnityClient.Logger.Log("Joined OK!", Logging.LoggedImportance.Debug);

                        StxUnityClient.Instance.SceneSwitchLobby();
                    }
                    else
                    {
                        StxUnityClient.Logger.Log("Joining failed!", Logging.LoggedImportance.Debug);

                        StxMatchmakingPanel.JoiningFailed();
                    }
                });

                StxMatchmakingPanel.EnableButtons();
            }

            /*StxUnityClient.server.JoinRoomAsync(roomID, (state, e) =>
            {

            }, )*/
        }
    }
}
