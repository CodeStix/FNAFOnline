using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Stx.Net.Unity.Editor
{
    public class PacketAnalyser : EditorWindow
    {
        public bool analyseRequestPackets = false;
        public string status = "Waiting for editor to play...";

        private float updateTime = 0.2f;

        [MenuItem("Window/Packet Analyser")]
        public static void ShowWindow()
        {
            GetWindow<PacketAnalyser>();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Status: " + status);

            if (!analyseRequestPackets && GUILayout.Button("Analyse request packets [START]"))
            {
                if (Application.isPlaying)
                {
                    analyseRequestPackets = true;
                    status = "Analysing requests...";
                }
            }

            if (analyseRequestPackets && GUILayout.Button("Analyse request packets [STOP]"))
            {
                analyseRequestPackets = false;
                status = "Not analysing.";
            }

            if (analyseRequestPackets)
            {
                StringBuilder sb = new StringBuilder();

                foreach (string s in RequestPacket.requests.Keys)
                {
                    sb.AppendLine($"Pending { RequestPacket.requestItems[s] }[{ s }] calls { RequestPacket.requests[s].Method.Name }.");
                }

                GUILayout.Box(sb.ToString());
            }
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                analyseRequestPackets = false;
                status = "Waiting for editor to play...";
            }
            else
            {
                updateTime -= Time.deltaTime;

                if (updateTime <= 0f)
                {
                    updateTime = 0.2f;

                    Repaint();
                }
            }
        }
    }
}
