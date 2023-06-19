using MySkill;
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public static class DataHelper
{
    private static string configPath;

    static DataHelper()
    {
        configPath = Application.dataPath + "/config/SkillConfig";
    }

    public static SkillData GetSkillDataByName(string skillName)
    {
        string path = configPath + "/" + skillName + ".txt";
        if (File.Exists(path))
        {
            string jsonStr = File.ReadAllText(path);
            string[] tmp = jsonStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            SkillData res = new SkillData();

            res.lifeTime = float.Parse(tmp[0]);

            for (int i = 1; i < tmp.Length; i += 2)
            {
                string typeStr = tmp[i];
                string json = tmp[i + 1];
                SkillActComponentType type = (SkillActComponentType)int.Parse(typeStr);
                SkillActComponentDataBase actData = null;
                switch (type)
                {
                    case SkillActComponentType.Animation:
                        actData = Newtonsoft.Json.JsonConvert.DeserializeObject<AnimationActData>(json);
                        break;
                    case SkillActComponentType.Auido:
                        actData = Newtonsoft.Json.JsonConvert.DeserializeObject<AudioActData>(json);
                        break;
                    case SkillActComponentType.ActorEffect:
                        actData = Newtonsoft.Json.JsonConvert.DeserializeObject<ActorEffectActData>(json);
                        break;
                    default:
                        break;
                }
                if (actData != null)
                {
                    res.actDatas.Add(actData);

                }
            }
            return res;
        }
        return null;
    }

    public static void Save(SkillData currentSkillData, string skillname)
    {
        string path = configPath + "/" + skillname + ".txt";

        if (File.Exists(path))
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(currentSkillData.lifeTime + "\r\n");
            foreach (var item in currentSkillData.actDatas)
            {
                sb.Append(((int)item.skillType).ToString() + "\r\n");
                string tmp = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                sb.Append(tmp + "\r\n");
            }
            File.WriteAllText(path, sb.ToString());
        }
    }

    public static void SaveNewSkillData(string skillName)
    {
        var skillData = new SkillData();
        StringBuilder sb = new StringBuilder();
        sb.Append(skillData.lifeTime + "\r\n");

        string path = configPath + "/" + skillName + ".txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, sb.ToString());
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("已经存在相同技能名 = " + skillName + "   " + path);
        }

    }
}

