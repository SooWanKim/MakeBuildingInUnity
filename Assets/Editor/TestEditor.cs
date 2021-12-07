using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class TestEditor : EditorWarpper
{
    [MenuItem ("EidtorWindow/TestEditor", priority = 0)]
    static public void ShowWindow ()
    {
        TestEditor window = GetWindow<TestEditor> ();
        window.Show ();
    }
    private void OnGUI ()
    {
        if(GUILayout.Button("Test") == true)
        {
        }
    }
}
#endif