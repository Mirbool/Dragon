using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySkill
{
    /// <summary>
    /// 一个技能的完整数据结构
    /// </summary>
    public class SkillData
    {
        /// <summary>
        /// 技能总时间
        /// </summary>
        public float lifeTime = 0;
        /// <summary>
        /// 技能整体的播放速度
        /// </summary>
        public int playSpeed;

        public List<SkillActComponentDataBase> actDatas = new List<SkillActComponentDataBase>();

    }
}



public class SkillActComponentDataBase
{
    public SkillActComponentType skillType;
    /// <summary>
    /// 每个技能动作组件的触发时间
    /// </summary>
    public float triggerTime;

}

public class AnimationActData : SkillActComponentDataBase
{
    public string animationName;
    [NonSerialized]
    public AnimationClip animationClip;
    public AnimationActData()
    {
        this.skillType = SkillActComponentType.Animation;
    }
}

/// <summary>
/// 声音动作组件的数据
/// </summary>
public class AudioActData : SkillActComponentDataBase
{
    public string audioPath;

    [NonSerialized]
    public AudioClip audioClip;

    public float audioVolume;
    public AudioActData()
    {
        this.skillType = SkillActComponentType.Auido;
    }
}

/// <summary>
/// 特效动作组件的数据
/// </summary>
public class ActorEffectActData : SkillActComponentDataBase
{
    public string effectPath;

    [NonSerialized]
    public GameObject effect;

    public EffectBindPoint bindPoint;
    public ActorEffectActData()
    {
        this.skillType = SkillActComponentType.ActorEffect;
    }
}
public enum EffectBindPoint : byte
{
    None, Body, Head, LeftHand, RightHand, Foot
}

