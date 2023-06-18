using MySkill;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SelectRoleWindow : EditorWindow
{
    [MenuItem("tools/SkillEditor")]
    public static void Open()
    {
        if (Application.isPlaying)
        {
            var window = SelectRoleWindow.GetWindow<SelectRoleWindow>("���ܱ༭��");
            window.minSize = new Vector2(400, 600);
            window.Show();

        }
    }

    private string[] roles = null;
    private string[] skillConfigNames = null;

    private int crtRoleIndex = -1;

    private EditorPlayer editorPlayer;
    private GameObject crtPlayerGo;


    SkillPropertyWindow skillPropertyWindow;

    private string CrtInputSKillName;
    private string currentSkillName;

    private SkillData currentSkillData;

    private void OnEnable()
    {
        editorPlayer = new EditorPlayer();
        initData();

    }

    private void initData()
    {
        ///��ɫ��������
        roles = loadPlayerData();

    }

    private string[] loadPlayerData()
    {
        string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Role" });
        return (from guid in guids select GetNameByGUID(guid)).ToArray();
    }

    private string GetNameByGUID(string guid)
    {
        var path = AssetDatabase.GUIDToAssetPath(guid);
        return Path.GetFileNameWithoutExtension(path);
    }

    private void OnDisable()
    {
        if (skillPropertyWindow != null)
        {
            skillPropertyWindow.Close();
            skillPropertyWindow = null;
        }
        EditorApplication.isPlaying = false;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        //ѡ��ɫ
        GUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        this.crtRoleIndex = EditorGUILayout.Popup("��ѡ���ɫ", crtRoleIndex, this.roles);
        if (EditorGUI.EndChangeCheck())
        {
            ChangePlayer();
            ChangeSkillInfo();
        }

        GUILayout.EndHorizontal();

        ShowOpBtns();

        ShowSkillInfo();

        GUILayout.EndVertical();
    }

    private void ShowOpBtns()
    {
        EditorGUILayout.Space(10);
        GUILayout.BeginHorizontal();

        this.CrtInputSKillName = EditorGUILayout.TextField(this.CrtInputSKillName);
        if (GUILayout.Button("New"))
        {
            if (!string.IsNullOrEmpty(this.CrtInputSKillName))
            {
                DataHelper.SaveNewSkillData(this.CrtInputSKillName);
                ChangeSkillInfo();
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("����"))
        {
            if (currentSkillData != null && !string.IsNullOrEmpty(this.currentSkillName))
            {
                DataHelper.Save(this.currentSkillData, currentSkillName);
            }

        }

        if (GUILayout.Button("ɾ������"))
        {

        }

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// ��ʾ��ҵ����м�������
    /// </summary>
    private void ShowSkillInfo()
    {
        ///�����б�
        GUILayout.BeginVertical();
        if (skillConfigNames?.Length > 0)
        {
            EditorGUILayout.LabelField("��Ҽ����б�");
            EditorGUILayout.BeginVertical();
            foreach (var item in skillConfigNames)
            {
                if (GUILayout.Button(item))
                {
                    this.currentSkillData = DataHelper.GetSkillDataByName(item);
                    this.currentSkillName = item;
                    if (currentSkillData != null)
                    {
                        ///ѡ��ͬ���ܣ���������ʵ�壬
                        this.editorPlayer.crtSkill = new SkillEntity();
                        this.editorPlayer.player.SkillEntity = this.editorPlayer.crtSkill;
                        this.editorPlayer.crtSkill.Init(this.editorPlayer.player, this.currentSkillData);

                        ///�򿪼������Ա༭����
                        showSkillPropertyWindow(this.editorPlayer, currentSkillData);
                    }
                    else
                    {
                        Debug.Log("���������ļ��쳣  �ļ� = " + item);
                    }

                }
            }
            EditorGUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    private void showSkillPropertyWindow(EditorPlayer editorPlayer, SkillData skilldata)
    {
        skillPropertyWindow = SkillPropertyWindow.GetWindow<SkillPropertyWindow>("���ܱ༭����");
        skillPropertyWindow.minSize = new Vector2(400, 600);
        skillPropertyWindow.Show();
        skillPropertyWindow.Init(editorPlayer,skilldata);
    }

    private void ChangeSkillInfo()
    {
        string[] guids = AssetDatabase.FindAssets("t:textAsset", new string[] { "Assets/config/SkillConfig" });

        skillConfigNames = (from guid in guids where GetNameByGUID(guid).StartsWith(this.roles[this.crtRoleIndex]) select GetNameByGUID(guid)).ToArray();
    }

    /// <summary>
    /// �л�������ģ�ͣ�
    /// </summary>
    private void ChangePlayer()
    {
        ///�л����
        if (crtPlayerGo != null)
        {
            GameObject.DestroyImmediate(crtPlayerGo);
            crtPlayerGo = null;
        }
        crtPlayerGo = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Role/" + this.roles[this.crtRoleIndex]));
        PlayerEntity p = crtPlayerGo.GetComponent<PlayerEntity>();
        if (p == null)
        {
            p = crtPlayerGo.AddComponent<PlayerEntity>();
        }
        editorPlayer.player = p;

    }

}

/// <summary>
/// ��������Ϸ����ʱ�������
/// </summary>
public class EditorPlayer
{
    public SkillEntity crtSkill;
    public int crtSpeed;
    public PlayerEntity player;
}
