using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(USP_Field))]
public class USP_Field_Editor_Script : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Initialize"))
        {
            USP_Field field = target as USP_Field;
            field.Initialize();
        }
    }

    
}
#endif
