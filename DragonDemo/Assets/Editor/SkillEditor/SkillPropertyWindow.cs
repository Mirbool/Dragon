using MySkill;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SkillPropertyWindow : EditorWindow
{
    private SkillData skillData;
    private SkillActComponentType crtAddActType;

    private Dictionary<SkillActComponentType, Type> skillActMap;

    private Dictionary<SkillActComponentDataBase, bool> foldOutValueMap;

    private EditorPlayer editorPlayer;

    private GameObject crtEffectRes;
    private AudioClip crtAudioClip;
    private void OnEnable()
    {
        skillActMap = new Dictionary<SkillActComponentType, Type>() {
            { SkillActComponentType.Animation,typeof(AnimationActData) },
            { SkillActComponentType.Auido,typeof(AudioActData) },
            { SkillActComponentType.ActorEffect,typeof(ActorEffectActData) }
        };

        foldOutValueMap = new Dictionary<SkillActComponentDataBase, bool>();

    }

    private void OnDisable()
    {

    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        ShowOpBtns();

        ShowSkillData();

        EditorGUILayout.EndVertical();
    }

    private void ShowSkillData()
    {
        this.skillData.lifeTime = EditorGUILayout.FloatField("技能总时长", this.skillData.lifeTime);

        EditorGUILayout.Space(5);
        ///展示技能数据中的每个组件的数据
        EditorGUILayout.BeginVertical();

        foreach (var item in this.skillData.actDatas)
        {
            switch (item.skillType)
            {
                case SkillActComponentType.Animation:
                    ShowAnimationActData(item);
                    break;
                case SkillActComponentType.Auido:
                    ShowAudioActData(item);
                    break;
                case SkillActComponentType.ActorEffect:
                    ShowActorEffectActData(item);
                    break;
                default:
                    break;
            }
        }
        EditorGUILayout.EndVertical();

    }

    private void ShowActorEffectActData(SkillActComponentDataBase data)
    {
        EditorGUILayout.Space(5);
        ActorEffectActData dt = data as ActorEffectActData;
        if (this.foldOutValueMap.TryGetValue(dt, out bool v))
        {
            foldOutValueMap[dt] = EditorGUILayout.Foldout(v, "特效组件数据");
            if (v)
            {
                dt.triggerTime = EditorGUILayout.FloatField("TriggerTime ", dt.triggerTime);

                if (dt.effect == null)
                {
                    dt.effect = Resources.Load<GameObject>("effect/" + dt.effectPath);
                }
                dt.effect = (GameObject)EditorGUILayout.ObjectField("特效资源", dt.effect, typeof(GameObject), false);
                if (dt.effect != null)
                {
                    dt.effectPath = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(dt.effect));
                }
                dt.bindPoint = (EffectBindPoint)EditorGUILayout.EnumPopup("特效绑点", dt.bindPoint);
            }
        }
    }

    private void ShowAudioActData(SkillActComponentDataBase data)
    {
        EditorGUILayout.Space(5);
        AudioActData audioData = data as AudioActData;
        if (this.foldOutValueMap.TryGetValue(audioData, out bool v))
        {
            foldOutValueMap[audioData] = EditorGUILayout.Foldout(v, "声音组件数据");
            if (v)
            {
                audioData.triggerTime = EditorGUILayout.FloatField("TriggerTime ", audioData.triggerTime);

                if (audioData.audioClip == null)
                {
                    audioData.audioClip = Resources.Load<AudioClip>("audios/" + audioData.audioPath);
                }
                audioData.audioClip = (AudioClip)EditorGUILayout.ObjectField("声音资源", audioData.audioClip, typeof(AudioClip), false);
                if (audioData.audioClip != null)
                {
                    audioData.audioPath = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(audioData.audioClip));
                }
                audioData.audioVolume = EditorGUILayout.Slider("声音大小", audioData.audioVolume, 0, 1);
            }
        }
    }

    private void ShowAnimationActData(SkillActComponentDataBase data)
    {
        AnimationActData aniData = data as AnimationActData;
        if (this.foldOutValueMap.TryGetValue(aniData, out bool v))
        {
            foldOutValueMap[aniData] = EditorGUILayout.Foldout(v, "动画组件数据");
            if (v)
            {
                aniData.triggerTime = EditorGUILayout.FloatField("TriggerTime ", aniData.triggerTime);
                aniData.animationName = EditorGUILayout.TextField("动画资源路径", aniData.animationName);
            }
        }
    }

    private void ShowOpBtns()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("播放"))
        {
            editorPlayer?.crtSkill?.play();
        }

        if (GUILayout.Button("停止"))
        {
            editorPlayer?.crtSkill?.Stop();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加"))
        {
            this.skillActMap.TryGetValue(crtAddActType, out Type type);
            if (type != null)
            {
                var obj = (SkillActComponentDataBase)System.Activator.CreateInstance(type);
                this.skillData.actDatas.Add(obj);
                foldOutValueMap.Add(obj, true);
            }
        }

        crtAddActType = (SkillActComponentType)EditorGUILayout.EnumPopup(crtAddActType);


        EditorGUILayout.EndHorizontal();

    }

    public void Init(EditorPlayer p, SkillData data)
    {
        this.skillData = data;
        this.editorPlayer = p;
        foreach (var item in skillData.actDatas)
        {
            this.foldOutValueMap.Add(item, true);
        }
    }
}

