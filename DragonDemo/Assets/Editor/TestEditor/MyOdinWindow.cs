using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyOdinWindow : OdinEditorWindow
{
    [MenuItem("tools/TestOdinWindow")]
    public static void Open()
    {
        MyOdinWindow window = MyOdinWindow.GetWindow<MyOdinWindow>();
        window.Show();
    }


    [LabelWidth(100), HideLabel,ReadOnly]
    [OnInspectorGUI("draw")]
    [TabGroup("Scene")]
    [TabGroup("‘§¿¿")]
    public RenderTexture renderTexture;

    [TabGroup("Scene")]
    public int sceneid = 0;

    [TabGroup("Scene")]
    public int type = 0;

    [TabGroup("Scene"),ReadOnly]
    public Vector3 pos;

    [TabGroup("Scene")]
    public int juqingid = 0;

    [TableList]
    [TabGroup("NPCs"), HideLabel]
    public List<NpcData> npcsList = new List<NpcData>();


    [TabGroup("Scene")]
    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    public void AddMonster()
    {
        
    }


    [TabGroup("‘§¿¿")]
    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    public void LoadScene()
    {
        Debug.Log("º”‘ÿ≥°æ∞£¨º”‘ÿ≈‰÷√±Ì£¨");
    }

    public void draw()
    {
        if (this.renderTexture == null)
        {
            return;
        }
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(this.renderTexture);
        EditorGUILayout.EndVertical();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    private void Update()
    {
        GameObject go = GameObject.Find("Cube");
        this.pos = go.transform.position;
    }
}

public class NpcData
{
    public string name;
    public int id;
    public int juqingid;
}

 
