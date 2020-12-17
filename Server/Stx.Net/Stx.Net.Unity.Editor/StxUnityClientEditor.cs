using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Stx.Net.Unity.Editor
{
    [CustomEditor(typeof(StxUnityClient))]
    public class StxUnityClientEditor : UnityEditor.Editor
    {
        private bool showInfo = false;

        private StxUnityClient Target
        {
            get
            {
                return (StxUnityClient)target;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            /*GUILayout.Label("Connection");
            EditorGUI.indentLevel++;
            GUILayout.Label("Stx Version " + StxNet.CLIENT_STX_VERSION);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Host:");
            StxUnityClient.Instance.host = EditorGUILayout.TextField(StxUnityClient.Instance.host);
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel--;*/

            showInfo = EditorGUILayout.Foldout(showInfo, "Runtime Info");
            if (showInfo)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(true);

                if (StxUnityClient.C != null)
                {
                    EditorGUILayout.LabelField("Remote", StxUnityClient.C.Remote.ToString());
                    EditorGUILayout.LabelField("Network ID", StxUnityClient.C.NetworkID);
                    EditorGUILayout.LabelField("Auth Token", StxUnityClient.C.ClientAuthorisationToken);
                    EditorGUILayout.LabelField("App Name", StxUnityClient.C.ApplicationName);
                    EditorGUILayout.LabelField("App Version", StxUnityClient.C.ApplicationVersion);
                    EditorGUILayout.Toggle("Connected", StxUnityClient.C.Connected);
                    EditorGUILayout.LabelField("Data Handlers", StxUnityClient.C.DataReceiver.handlers.Count.ToString());
                }
                else
                {
                    EditorGUILayout.HelpBox("Not available.", MessageType.Warning);
                }

                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
        }
    }
}
