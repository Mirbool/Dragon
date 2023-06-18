using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Demo))]
public class DemoCustomEditor : Editor
{
    private SerializedProperty allGos;
    private SerializedProperty otherValue;

    private void OnEnable()
    {
        allGos = this.serializedObject.FindProperty("gameObjects");
        otherValue  = this.serializedObject.FindProperty("otherValue");
        otherValue.intValue = 0;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        EditorGUILayout.PropertyField(allGos);
        //EditorGUILayout.PropertyField(otherValue);
        Demo t = this.target as Demo;

       /// t.otherValue= (int)EditorGUILayout.Slider(t.otherValue, 0, 100);
         otherValue.intValue = (int)EditorGUILayout.Slider(otherValue.intValue, 0, 100);
        otherValue.serializedObject.ApplyModifiedProperties();


        if (GUILayout.Button("搜集所有游戏对象"))
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag("monster");
            foreach (GameObject go in gos)
            {
                allGos.InsertArrayElementAtIndex(0);
                allGos.GetArrayElementAtIndex(0).objectReferenceValue = go;
                allGos.serializedObject.ApplyModifiedProperties();

                otherValue.intValue = 100;
                otherValue.serializedObject.ApplyModifiedProperties();
            }
        }

        if (GUILayout.Button("清除"))
        {
            allGos.ClearArray();
            allGos.serializedObject.ApplyModifiedProperties();

            otherValue.intValue = 0;
            otherValue.serializedObject.ApplyModifiedProperties();


        }
    }
}
