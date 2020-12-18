using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Stx.Net.Unity.UI
{
    [RequireComponent(typeof(Text))]
    public class StxInfoText : MonoBehaviour
    {
        public StxInfoTextSource info = StxInfoTextSource.None;
        [Multiline]
        public string textFormat = "{0}";

        private Text text;

        void Start()
        {
            text = GetComponent<Text>();
        }

        void Update()
        {
            if (info == StxInfoTextSource.None)
                return;

            switch(info)
            {
                case StxInfoTextSource.Ping:
                    SetText(RequestPacket.LastSendReceiveTime.TotalMilliseconds.ToString());
                    break;

                case StxInfoTextSource.YourID:
                    SetText(StxUnityClient.C.NetworkID);
                    break;

                case StxInfoTextSource.YourName:
                    SetText(StxUnityClient.C.You.DisplayName);
                    break;


                case StxInfoTextSource.CurrentRoomPlayerCount:
                    SetText(Infos.currentRoomPlayerCount.ToString());
                    break;

                case StxInfoTextSource.CurrentRoomName:
                    SetText(Infos.currentRoomName);
                    break;

                case StxInfoTextSource.CurrentRoomCode:
                    SetText(Infos.currentRoomCode);
                    break;
            }
        }

        public void SetText(string info)
        {
            text.text = string.Format(textFormat, info);
        }

        public static class Infos
        {
            public static string currentRoomName = null;
            public static string currentRoomCode = null;
            public static int currentRoomPlayerCount = 0;
        }
    }

    public enum StxInfoTextSource
    {
        None,
        YourName,
        YourID,
        CurrentRoomName,
        CurrentRoomCode,
        CurrentRoomPlayerCount,
        Ping
    }
}
