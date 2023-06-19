using MySkill;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能实体   逻辑层
/// </summary>
public class SkillEntity
{
    /// <summary>
    /// 技能的数据
    /// </summary>
    private SkillData _skillData;
    /// <summary>
    /// 技能所在的玩家
    /// </summary>
    private PlayerEntity _playerEntity;
    public PlayerEntity PlayerEntity { get => _playerEntity; }

    /// <summary>
    /// 包含技能实体里的所有动作组件
    /// </summary>
    private List<SkillActComponentBase> comMap;

    private float timeCount = 0;



    public SkillEntity()
    {
        comMap = new List<SkillActComponentBase>();
    }

    public void Init(PlayerEntity player, SkillData currentSkillData)
    {
        this._skillData = currentSkillData;
        this._playerEntity = player;

        InitComponent();
    }

    private void InitComponent()
    {
        this.comMap.Clear();
        foreach (SkillActComponentDataBase componentdata in _skillData.actDatas)
        {
            switch (componentdata.skillType)
            {
                case SkillActComponentType.Animation:
                    this.comMap.Add(new AnimationActComponent(componentdata as AnimationActData, this));
                    break;
                case SkillActComponentType.Auido:
                    this.comMap.Add(new AudioActComponent(componentdata as AudioActData, this));
                    break;
                case SkillActComponentType.ActorEffect:
                    this.comMap.Add(new ActorEffectActComponent(componentdata as ActorEffectActData, this));
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 技能实体播放，展示技能效果
    /// </summary>
    public void play()
    {
        timeCount = 0;
        foreach (var item in comMap)
        {
            item.Play();
        }
    }

    public void Stop()
    {
        foreach (var item in comMap)
        {
            item.Stop();
        }
    }

    public void Update()
    {
        if (timeCount < this._skillData.lifeTime)
        {
            foreach (var item in comMap)
            {
                item.Update();
            }

            timeCount += Time.deltaTime;
            if (timeCount >= this._skillData.lifeTime)
            {
                this.Stop();
            }

        }
    }
}

