using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyFramework))]
[CanEditMultipleObjects]
public class MyFrameworkEditor : Editor
{
    SerializedProperty meshMeterial;
    SerializedProperty parentGameObject;
    private MyFramework _myFramework;

    void OnEnable()
    {
        _myFramework = target as MyFramework;
        meshMeterial = serializedObject.FindProperty("meshMeterial");
        parentGameObject = serializedObject.FindProperty("parentMesh");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(meshMeterial);
        EditorGUILayout.PropertyField(parentGameObject);
        serializedObject.ApplyModifiedProperties();

        if(GUILayout.Button("Build") == true)
        {
            _myFramework.Build(false);
        }

        if(GUILayout.Button("Clear") == true)
        {
            _myFramework.Clear();
        }
    }
}