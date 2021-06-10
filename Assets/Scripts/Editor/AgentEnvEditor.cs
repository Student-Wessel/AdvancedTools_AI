using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnviornemntSetGenerator))]
public class AgentEnvSetEditor : Editor
{
    private int width;
    private int height;
    private float margin;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnviornemntSetGenerator setGenerator = (EnviornemntSetGenerator)target;

        if (GUILayout.Button("Generate set"))
        {
            setGenerator.GenerateSets();
        }
    }
}
