using Config;
//using FMOD.Studio;
using Koakuma.Game.ECS;
using Koakuma.Game.Effects;
using Koakuma.Game.FX;
//using Koakuma.Game.Effects;
//using Koakuma.Game.FX;
using Koakuma.Game.Timer;
using Koakuma.Game.Times;
using System.Collections.Generic;
using TGame.ECS;

namespace Koakuma.Game.Skills
{
    public class GameSkillComponent : ECSComponent
    {
        public int configID;
        public long casterID;
        public float startCastingTime;  ///开始时间
        public SkillConfig skillConfig;
        public ConfigSkillData configSkillData;
        public SkillTarget? inputTarget;
       // public List<EventInstance> sfxList;

        
    }

    public class GameSkillComponentAwakeSystem : AwakeSystem<GameSkillComponent, CastSkillData, SkillData>
    {
        public override void Awake(GameSkillComponent c, CastSkillData p1, SkillData p2)
        {
            c.configID = p1.configID;
            c.casterID = p1.casterID;
            c.skillConfig = SkillConfig.ByID(c.configID);
            c.inputTarget = p1.target;
        //    c.sfxList = ListPool<EventInstance>.Obtain();

            float comboTime = 0;
            if (c.skillConfig.SkillCombo.Length > 1)
            {
                comboTime = c.skillConfig.SkillCombo[1];
            }
            else if (c.skillConfig.SkillCombo.Length > 0)
            {
                comboTime = c.skillConfig.SkillCombo[0];
            }
            c.configSkillData = SkillConfig.FindConfigSkillData(c.configID);

            // fx
            if (c.configSkillData.FXData != null)
            {
                foreach (var fxData in c.configSkillData.FXData)
                {
                    TimerHelper.CreateCountdown(c.EntityID, new CountdownTimerData()
                    {
                        delay = fxData.Delay,
                        callback = OnPlayFX(c, fxData.ID),
                    });
                }
            }

            // sfx
            if (c.configSkillData.SFXData != null)
            {
                foreach (var sfxData in c.configSkillData.SFXData)
                {
                    TimerHelper.CreateCountdown(c.EntityID, new CountdownTimerData()
                    {
                        delay = sfxData.Delay,
                        callback = OnPlaySfx(c, sfxData.ID),
                    });
                }
            }

            // hit
            for (int i = 0; i < c.configSkillData.DetectHitData.Length; i++)
            {
                ConfigSkillDetectHitData hitData = c.configSkillData.DetectHitData[i];
                SkillHitData configHitData = c.skillConfig.DetectHits[i];
                if (configHitData.Time == 0 && configHitData.DetectType == SkillDetectType.None)
                    continue;

                TimerHelper.CreateCountdown(c.EntityID, new CountdownTimerData()
                {
                    delay = configHitData.Time,
                    callback = OnHitDetect(c, hitData, configHitData, i)
                });
            }

            // combo time
            TimerHelper.CreateCountdown(c.EntityID, new CountdownTimerData()
            {
                delay = comboTime,
                callback = (canceled) =>
                {
                    if (!canceled)
                    {
                        GameManager.ECS.SendMessageToEntity(c.casterID, new EntityMessageType.SkillReachComboTime()).Coroutine();
                    }
                },
            });

            // duration
            TimerHelper.CreateCountdown(c.EntityID, new CountdownTimerData()
            {
                delay = c.skillConfig.Duration,
                callback = (canceled) =>
                {
                    if (!canceled)
                    {
                        GameManager.ECS.SendMessageToEntity(c.casterID, new EntityMessageType.EndSkillCasting()).Coroutine();
                    }
                },
            });
        }

        private CountdownTimerComponent.CountdownEventArgs OnHitDetect(GameSkillComponent gameSkillComponent, ConfigSkillDetectHitData hitData, SkillHitData configHitData, int index)
        {
            return async (canceled) =>
            {
                if (!canceled)
                {
                    List<SkillTarget> targetList = ListPool<SkillTarget>.Obtain();
                    EntityMessageType.SkillSelectTargetsResponse skillSelectTargetsResponse = await GameManager.ECS.SendRpcToEntity<EntityMessageType.SkillSelectTargetsRequest, EntityMessageType.SkillSelectTargetsResponse>(gameSkillComponent.casterID, new EntityMessageType.SkillSelectTargetsRequest()
                    {
                        inputTarget = gameSkillComponent.inputTarget,
                        detectType = configHitData.DetectType,
                        detectArgs = configHitData.DetectArgs,
                        detectCenterType = configHitData.DetectCenterType,
                        detectCenterOffset = configHitData.CenterOffset,
                        relation = configHitData.TargetRelation,
                        targetList = targetList,
                    });

                    if (!skillSelectTargetsResponse.Error)
                    {
                        foreach (var target in targetList)
                        {
                            foreach (var configEffectData in hitData.EffectData)
                            {
                                if (configEffectData == null)
                                    continue;

                                GameEffectData gameEffectData = new GameEffectData()
                                {
                                    casterEntityID = gameSkillComponent.casterID,
                                    sourceEntityID = gameSkillComponent.EntityID,
                                    effectName = configEffectData.Name,
                                    effectRevert = configEffectData.Revert,
                                    effectArgs = configEffectData.Args[0],
                                    inputTarget = gameSkillComponent.inputTarget,
                                    target = target,
                                };
                                GameEffectFactory.CreateEffect(gameEffectData);
                            }
                        }
                    }

                    ListPool<SkillTarget>.Release(targetList);
                }
            };
        }

        private CountdownTimerComponent.CountdownEventArgs OnPlayFX(GameSkillComponent gameSkillComponent, int fxID)
        {
            return (canceled) =>
            {
                if (!canceled)
                {
                    FXConfig fXConfig = FXConfig.ByID(fxID);
                    FXHelper.PlayFX(new PlayFXData()
                    {
                        configID = fxID,
                        targetEntityID = gameSkillComponent.casterID,
                        parentEntityID = fXConfig.AutoRecycle ? gameSkillComponent.casterID : gameSkillComponent.EntityID,
                    });
                }
            };
        }

        private CountdownTimerComponent.CountdownEventArgs OnPlaySfx(GameSkillComponent gameSkillComponent, int sfxID)
        {
            return (canceled) =>
            {
                if (!canceled)
                {
                    ECSEntity casterEntity = GameManager.ECS.FindEntity(gameSkillComponent.casterID);
                    GameObjectComponent gameObjectComponent = casterEntity?.GetComponent<GameObjectComponent>();

                    //EventInstance? sfx = GameManager.Audio.PlaySFX(sfxID, gameObjectComponent?.rigidbody);
                    //if (sfx.HasValue)
                    //{
                    //    gameSkillComponent.sfxList.Add(sfx.Value);
                    //}
                }
            };
        }
    }

    public class GameSkillComponentDestroySystem : DestroySystem<GameSkillComponent>
    {
        public override void Destroy(GameSkillComponent c)
        {
            UnityLog.Info($"Destroy GameSkillComponent:{c.configID}");
            c.configID = default;
            c.casterID = default;
            c.startCastingTime = default;
            c.skillConfig = default;
            c.configSkillData = default;
            c.inputTarget = null;

            //foreach (var eventInstance in c.sfxList)
            //{
            //    eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            //}
            //ListPool<EventInstance>.Release(c.sfxList);
            //c.sfxList = null;
        }
    }
}
