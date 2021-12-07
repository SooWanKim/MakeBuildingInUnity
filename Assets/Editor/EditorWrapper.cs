using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;


public class EditorWarpper : EditorWindow
{
    protected void GUITitleText(string title, ref string text, float width, float height, bool expand, bool useHorizontal = true)
    {
        if (useHorizontal == true)
            GUILayout.BeginHorizontal();

        GUILayout.Label(title, GUILayout.Width(100.0f), GUILayout.Height(height), GUILayout.ExpandWidth(expand));
        text = EditorGUILayout.TextArea(text, GUILayout.Width(width), GUILayout.Height(height), GUILayout.ExpandWidth(expand));

        if (useHorizontal == true)
            GUILayout.EndHorizontal();
    }

    protected void GUITitleFloat(string title, ref float value, float width, float height, bool expand, bool useHorizontal = true)
    {
        if (useHorizontal == true)
            GUILayout.BeginHorizontal();

        GUILayout.Label(title, GUILayout.Width(100.0f), GUILayout.Height(20.0f));
        value = EditorGUILayout.FloatField(value, GUILayout.Width(width), GUILayout.Height(height), GUILayout.ExpandWidth(expand));

        if (useHorizontal == true)
            GUILayout.EndHorizontal();

    }

    protected void GUITitleVector3(string title, ref Vector3 value, float width, float height, bool expand, bool useHorizontal = true)
    {
        if (useHorizontal == true)
            GUILayout.BeginHorizontal();

        value = EditorGUILayout.Vector3Field(title, value, GUILayout.Width(width), GUILayout.Height(height), GUILayout.ExpandWidth(expand));

        if (useHorizontal == true)
            GUILayout.EndHorizontal();

    }

    protected void GUITitleInt(string title, ref int value, float width, float height, bool expand, bool useHorizontal = true)
    {
        if (useHorizontal == true)
            GUILayout.BeginHorizontal();

        GUILayout.Label(title, GUILayout.Width(100.0f), GUILayout.Height(20.0f));
        value = EditorGUILayout.IntField(value, GUILayout.Width(width), GUILayout.Height(height), GUILayout.ExpandWidth(expand));

        if (useHorizontal == true)
            GUILayout.EndHorizontal();
    }

    protected void GUIToggle(string title, ref bool value, float width, float height, bool expand)
    {
        value = GUILayout.Toggle(value, title, GUILayout.Width(width), GUILayout.Height(height), GUILayout.ExpandWidth(false));
    }

    protected void DrawLine(Color color, float space = 30.0f, float lineWidth = 1000f)
    {
        Rect curRect = GUILayoutUtility.GetLastRect();
        Handles.color = color;
        GUILayout.Space(space/2.0f);
        Handles.DrawLine(new Vector3(curRect.x, curRect.y + space, 0.0f), new Vector3(lineWidth, curRect.y+ space, 0.0f));
        Handles.color = Color.white;
    }
}

#endif