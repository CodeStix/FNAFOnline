using Stx.Net.RoomBased;
using UnityEngine;
using UnityEngine.UI;

namespace Stx.Net.Unity.UI
{
    public class StxChatPanel : MonoBehaviour
    {
        public ChatSourceType chatSource = ChatSourceType.Global;
        public string personalChatReceiver = null;
        [Space]
        public StxChatPanelItem chatItemPrefab;
        public string chatTitleFormat = "------ {0} Chat {1} ------";
        [Header("UI")]
        public Text titleText;
        public InputField chatInput;
        public bool enableEnterSubmit = true;
        public Button chatSendButton;
        public ScrollRect chatRect;

        void Start()
        {
            UpdateChatTitle();
        }

        void Update()
        {
            if (enableEnterSubmit && chatInput.isFocused && Input.GetKey(KeyCode.Return))
            {
                SendChat();

                chatInput.Select();
                chatInput.ActivateInputField();
            }
        }

        void OnEnable()
        {
            StxUnityClient.C.OnChat += Client_OnChat;

            //handles = StxChatHandleToHandle(chatHandles);

            /*if (handles != null)
                StxUnityClient.C.DataReceiver.AddHandler(new DataHandler<ChatEntry>(handles, (e, p) =>
                {
                    StxUnityClient.F.GetClientInfoAsync(e.SenderID, (state, e2) =>
                    {
                        AddMessage(e2.Name, e.Message);
                    });
                }));*/
        }

        private void Client_OnChat(ChatEntry chatMessage)
        {
            if (chatMessage.SourceType == chatSource)
                AddMessage(chatMessage.SenderDisplayName, chatMessage.Message);
        }

        void OnDisable()
        {
            StxUnityClient.C.OnChat -= Client_OnChat;

            /*if (handles != null)
                StxUnityClient.C.DataReceiver.RemoveHandler(handles);*/
        }

        /*public string StxChatHandleToHandle(StxChatHandle chatHandle)
        {
            if (chatHandles == StxChatHandle.RoomChat)
                return "RoomChatEntry";
            else if (chatHandles == StxChatHandle.Nothing)
                return null;
            else
                return null;
        }*/

        public void UpdateChatTitle()
        {
            if (titleText != null)
            {
                if (chatSource == ChatSourceType.Global || chatSource == ChatSourceType.Room || chatSource == ChatSourceType.Other)
                    titleText.text = string.Format(chatTitleFormat, chatSource, "");
                else if (chatSource == ChatSourceType.Personal)
                    titleText.text = string.Format(chatTitleFormat, chatSource, personalChatReceiver);
            }
        }

        public void UseForReceiver(string receiverID)
        {
            if (chatSource != ChatSourceType.Personal)
            {
                StxUnityClient.Logger.Log("This chat panels chat source is not set to receive or send personal chat messages.", Logging.LoggedImportance.Warning);

                return;
            }

            personalChatReceiver = receiverID;

            UpdateChatTitle();
        }

        public void AddMessage(string sender, string message)
        {
            var v = Instantiate(chatItemPrefab, transform);
            v.senderText.text = sender;
            v.messageText.text = message;

            Canvas.ForceUpdateCanvases();

            

            if (chatRect != null)
                chatRect.verticalNormalizedPosition = 0f;
        }

        public void SendChat()
        {
            if (chatInput.text.Length < 1)
                return;

            chatInput.interactable = false;
            chatSendButton.interactable = false;

            switch (chatSource)
            {
                case ChatSourceType.Global:
                    StxUnityClient.F.ChatGloballyAsync(chatInput.text, (state) =>
                    {
                        if (state <= PacketResponseStatus.Okey)
                            chatInput.text = "";
                        chatInput.interactable = true;
                        chatSendButton.interactable = true;
                    });
                    break;

                case ChatSourceType.Personal:
                    StxUnityClient.F.ChatPersonalAsync(chatInput.text, personalChatReceiver, (state) =>
                    {
                        if (state <= PacketResponseStatus.Okey)
                            chatInput.text = "";
                        chatInput.interactable = true;
                        chatSendButton.interactable = true;
                    });
                    break;

                case ChatSourceType.Room:
                    StxUnityClient.F.ChatInRoomAsync(chatInput.text, (state) =>
                    {
                        if (state <= PacketResponseStatus.Okey)
                            chatInput.text = "";
                        chatInput.interactable = true;
                        chatSendButton.interactable = true;
                    });
                    break;

                default:
                case ChatSourceType.Other:
                    break;
            }

          
                

            /*ChatEntry ce = new ChatEntry(StxUnityClient.C.ClientID, chatInput.text);

            Hashtable data = new Hashtable();
            data.Add(handles, ce);

            StxUnityClient.F.BroadcastInRoomAsync(data, (state, e) =>
            {
                chatInput.text = "";
                chatInput.interactable = true;
                chatSendButton.interactable = true;
            }, false);*/
        }

    }

    public enum StxChatHandle
    {
        Nothing,
        RoomChat
    }

    public class StxChatPanelItem : MonoBehaviour
    {
        public Text senderText;
        public Text messageText;
    }
}
