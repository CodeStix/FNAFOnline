using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraSystem))]
public class CameraSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Camera navigation", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Previous camera"))
        {
            ((CameraSystem)target).PreviousCamera();
        }

        if (GUILayout.Button("Next camera"))
        {
            ((CameraSystem)target).NextCamera();
        }

        EditorGUILayout.EndHorizontal();
    }
}
