using Stx.Net.RoomBased;
using UnityEngine;
using UnityEngine.UI;

namespace Stx.Net.Unity.UI
{
    [RequireComponent(typeof(Button))]
    public class StxReadyUpButton : MonoBehaviour
    {
        public string readyText = "<color=#FF2200>Click if ready.</color>";
        public string notReadyText = "<color=#33FF11>You're ready.</color>";

        private Button button;
        private Text buttonText;

        private bool mode = false;

        void Start()
        {
            button = GetComponent<Button>();
            buttonText = GetComponentInChildren<Text>();

            buttonText.text = readyText;
        }

        public void ReadyUp()
        {
            mode = !mode;
            button.interactable = false;

            ClientRoomStatus newStatus = mode ? ClientRoomStatus.Ready : ClientRoomStatus.NotReady;

            StxUnityClient.F.ChangeInRoomStatusAsync(newStatus, (state, e) =>
            {
                if (state == PacketResponseStatus.Responded)
                {
                    if (!mode)
                        buttonText.text = readyText;
                    else
                        buttonText.text = notReadyText;

                    button.interactable = true;
                }
            });
        }
    }
}
