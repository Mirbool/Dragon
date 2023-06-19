using Config;
using Koakuma.Game.Characters;
using Koakuma.Game.ECS;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.Skills
{
    public class SkillComponent : ECSComponent
    {
        public bool isCastingSkill;
        public long castingSkillEntityID;
        public int castingSkillID;           //真实的释放的技能的Id，有可能是直接释放的子技能的id,不是父技能的id
        public float castingStartTime;
        public Dictionary<int, SkillData> skillIDMap;
        public List<int> mainSkillIDList;
        public int? preinputSkill;
        public SkillTarget? preinputTarget;
        public Dictionary<int, float> cdMap;
    }

    public class SkillComponentAwakeComponent : AwakeSystem<SkillComponent>
    {
        public override void Awake(SkillComponent c)
        {
            c.skillIDMap = DictionaryPool<int, SkillData>.Obtain();
            c.mainSkillIDList = ListPool<int>.Obtain();
            c.cdMap = DictionaryPool<int, float>.Obtain();

            CharacterComponent characterComponent = c.Entity.GetComponent<CharacterComponent>();
            if (characterComponent != null)
            {
                foreach (var skillInfo in characterComponent.skills)
                {
                    SkillData skillData = LearnSkill(c, skillInfo.configID, skillInfo.level);
                    c.mainSkillIDList.Add(skillData.configID);
                }
            }

            GameManager.ECS.SendMessageToEntity(c.EntityID, new EntityMessageType.BindSkillInput()).Coroutine();
        }

        private SkillData LearnSkill(SkillComponent skillComponent, int configID, int level)
        {
            SkillConfig skillConfig = SkillConfig.ByID(configID);
            UnityLog.Info($"Learn Skill:{skillConfig.Name}");

            SkillData skillData = new SkillData()
            {
                configID = configID,
                level = level,
            };
            skillComponent.skillIDMap.Add(skillData.configID, skillData);

            skillData.children = new SkillData[skillConfig.GroupSkills.Length];
            for (int subskillIndex = 0; subskillIndex < skillConfig.GroupSkills.Length; subskillIndex++)
            {
                int subskillID = skillConfig.GroupSkills[subskillIndex];
                SkillData subskill = LearnSkill(skillComponent, subskillID, level);
                if (subskill != null)
                {
                    skillData.children[subskillIndex] = subskill;
                }
            }

            return skillData;
        }
    }

    public class SkillComponentDestroyComponent : DestroySystem<SkillComponent>
    {
        public override void Destroy(SkillComponent c)
        {
            c.isCastingSkill = default;
            c.castingSkillEntityID = default;
            c.castingSkillID = default;
            c.castingStartTime = default;

            DictionaryPool<int, SkillData>.Release(c.skillIDMap);
            c.skillIDMap = null;

            ListPool<int>.Release(c.mainSkillIDList);
            c.mainSkillIDList = null;

            DictionaryPool<int, float>.Release(c.cdMap);
            c.cdMap = null;

            c.preinputSkill = null;
            c.preinputTarget = null;
        }
    }

    public class CastSkillRpcHandler : EntityRpcHandler<EntityMessageType.CastSkillRequest, EntityMessageType.CastSkillResponse>
    {
        public enum PreinputState
        {
            CanNot,
            Can,
            Immediately
        }

        public override async Task<EntityMessageType.CastSkillResponse> HandleRpc(ECSEntity entity, EntityMessageType.CastSkillRequest request)
        {
            SkillComponent skillComponent = entity.GetComponent<SkillComponent>();
            if (skillComponent == null)
                return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = "Without ability to cast skill" };

            if (!skillComponent.skillIDMap.TryGetValue(request.configID, out SkillData skillData))
                return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = $"Cast skill fail, skill not found:{request.configID}" };

            CharacterComponent characterComponent = entity.GetComponent<CharacterComponent>();
            if (characterComponent != null)
            {
                if (characterComponent.state == CharacterState.Dead)
                    return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = $"Cast skill fail, character dead:{entity.InstanceID}" };

                if (characterComponent.state == CharacterState.Unvitalized)
                    return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = $"Cast skill fail, character is unvitalized:{entity.InstanceID}" };

                if (characterComponent.interruptState != InterruptStateType.None)
                    return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = $"Cast skill fail, character interrupted:{entity.InstanceID}, state:{characterComponent.interruptState}" };
            }

            // cd
            float time = Time.time;
            if (skillComponent.cdMap.TryGetValue(request.configID, out float cdFinishTime))
            {
                if (time < cdFinishTime)
                    return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = $"Cast skill fail, skill is in CD:{request.configID}, remain time:{cdFinishTime - time}" };
            }

            SkillData realSkillData;
            if (skillComponent.isCastingSkill && !request.isCombo)
            {
                SkillData castingSkill = skillComponent.skillIDMap[skillComponent.castingSkillID];
                PreinputState preinputState = CanPreinput(skillComponent, castingSkill);///看释放的时机是否在当前释放技能的combo起始和combo结束时间内
                if (preinputState != PreinputState.CanNot)
                {
                    skillComponent.preinputSkill = request.configID;
                    skillComponent.preinputTarget = request.target;
                    return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = $"Preinputed", comboImmediately = preinputState == PreinputState.Immediately };
                }

                return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = $"Cast skill fail, casting skill now:{skillComponent.castingSkillID}" };
            }
            else
            {
                ///拿到真正要释放的技能的数据，有可能是取到正在释放某一个包含子技能数据的技能，中的下一个子技能
                realSkillData = GetCastableSkill(skillComponent, skillData);
            }

            // input target
            SkillConfig realSkillConfig = SkillConfig.ByID(realSkillData.configID);
            if (realSkillConfig.InputTarget != SkillTargetType.None &&
               (realSkillConfig.InputTarget & request.target.GetValueOrDefault().type) == 0)
            {
                return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = $"Cast skill fail, input error input target{request.target.GetValueOrDefault().type}, expect:{realSkillConfig.InputTarget}" };
            }

            if (request.target.HasValue && request.target.Value.type == SkillTargetType.Unit)
            {
                EntityMessageType.SkillInputTargetSatisfiedResponse skillTargetSatisfied = await GameManager.ECS.SendRpcToEntity<EntityMessageType.SkillInputTargetSatisfiedRequest, EntityMessageType.SkillInputTargetSatisfiedResponse>(entity.InstanceID, new EntityMessageType.SkillInputTargetSatisfiedRequest()
                {
                    targetID = request.target.Value.entity,
                    skillID = skillData.configID,
                });

                if (skillTargetSatisfied.Error)
                {
                    return new EntityMessageType.CastSkillResponse() { Error = true, ErrorMessage = $"Cast skill fail, unmatched target" };
                }
            }


            // 若是combo则将当前技能中断
            if (skillComponent.isCastingSkill && request.isCombo)
            {
                GameSkillEntity gameSkillEntity = GameManager.ECS.FindEntity<GameSkillEntity>(skillComponent.castingSkillEntityID);
                gameSkillEntity?.Dispose();
            }

            UnityLog.Info($"Request cast skill, id:{realSkillData.configID}");
            CastSkillData castSkillData = new CastSkillData()
            {
                configID = realSkillData.configID,
                casterID = entity.InstanceID,
                target = request.target,
            };

            GameSkillComponent gameSkillComponent = SkillFactory.CreateSkillEntity(castSkillData, realSkillData);
            entity.AddChild(gameSkillComponent.Entity);
            skillComponent.castingSkillEntityID = gameSkillComponent.EntityID;
            skillComponent.castingSkillID = realSkillData.configID;
            skillComponent.castingStartTime = time;

            // add skill cd
            SkillConfig skillConfig = SkillConfig.ByID(skillData.configID);
            if (skillConfig.CDGroup != 0)
            {
                SkillCDConfig skillCDConfig = SkillCDConfig.ByID(skillConfig.CDGroup);
                skillComponent.cdMap[skillData.configID] = time + skillCDConfig.CD;
            }

            OnBeginCastSkill(skillComponent, request.target);

            await Task.Yield();
            return new EntityMessageType.CastSkillResponse() { Error = false };
        }

        private bool IsSubSkillOf(SkillData parent, int childID)
        {
            if (parent == null)
                return false;

            foreach (var child in parent.children)
            {
                if (child.configID == childID)
                    return true;

                if (IsSubSkillOf(child, childID))
                    return true;
            }

            return false;
        }

        private PreinputState CanPreinput(SkillComponent skillComponent, SkillData castingSkill)
        {
            float currentTime = Time.time;
            float lastUseTimeElapsed = currentTime - skillComponent.castingStartTime;

            SkillData currentSkill;
            if (castingSkill.children.Length > 0)
            {
                currentSkill = castingSkill.children[castingSkill.currentChildrenIndex];
            }
            else
            {
                currentSkill = castingSkill;
            }

            SkillConfig currentSkillConfig = SkillConfig.ByID(currentSkill.configID);
            float comboBegin = 0;
            float comboEnd = 0;
            if (currentSkillConfig.SkillCombo.Length == 1)
            {
                comboEnd = currentSkillConfig.SkillCombo[0];
            }
            else if (currentSkillConfig.SkillCombo.Length == 2)
            {
                comboBegin = currentSkillConfig.SkillCombo[0];
                comboEnd = currentSkillConfig.SkillCombo[1];
            }

            if (lastUseTimeElapsed < comboBegin)
                return PreinputState.CanNot;

            if (lastUseTimeElapsed < comboEnd)
                return PreinputState.Can;

            return PreinputState.Immediately;
        }

        private SkillData GetCastableSkill(SkillComponent skillComponent, SkillData skillData)
        {
            if (skillData.children.Length > 0)
            {
                if (!skillComponent.isCastingSkill)
                {
                    skillData.currentChildrenIndex = 0;
                }
                else
                {
                    skillData.currentChildrenIndex = (skillData.currentChildrenIndex + 1) % skillData.children.Length;
                }
                return skillData.children[skillData.currentChildrenIndex];

            }
            else
            {
                return skillData;
            }
        }

        private void OnBeginCastSkill(SkillComponent skillComponent, SkillTarget? target)
        {
            UnityLog.Info($"Begin skill casting:{skillComponent.castingSkillID}");

            ECSEntity skillEntity = GameManager.ECS.FindEntity(skillComponent.castingSkillEntityID);
            GameSkillComponent gameSkillComponent = skillEntity?.GetComponent<GameSkillComponent>();
            if (gameSkillComponent == null)
                return;

            skillComponent.isCastingSkill = true;
            gameSkillComponent.startCastingTime = Time.time;

            SkillConfig skillConfig = SkillConfig.ByID(skillComponent.castingSkillID);

            GameManager.ECS.SendMessageToEntity(skillComponent.EntityID, new EntityMessageType.UpdateMoveableState()).Coroutine();

            // animation
            AnimatorComponent animatorComponent = skillComponent.Entity.GetComponent<AnimatorComponent>();
            if (animatorComponent != null)
            {
                animatorComponent.animator.CrossFadeInFixedTime(skillConfig.Animation, 0.1f, 0, 0);
            }

            // rotation
            if (target.HasValue)
            {
                Vector3? targetPosition = null;
                if (target.Value.type == SkillTargetType.Position)
                {
                    targetPosition = target.Value.position;
                }
                else if (target.Value.type == SkillTargetType.Unit)
                {
                    ECSEntity entity = GameManager.ECS.FindEntity(target.Value.entity);
                    GameObjectComponent gameObjectComponent = entity?.GetComponent<GameObjectComponent>();
                    if (gameObjectComponent != null)
                    {
                        targetPosition = gameObjectComponent.position;
                    }
                }

                if (targetPosition.HasValue)
                {
                    GameObjectComponent gameObjectComponent = skillComponent.Entity.GetComponent<GameObjectComponent>();
                    if (gameObjectComponent != null)
                    {
                        gameObjectComponent.rotation = Quaternion.LookRotation((targetPosition.Value - gameObjectComponent.position).ToXZPlane(), Vector3.up);
                    }
                }
            }
        }
    }

    public class EndSkillCastingMessageHandler : EntityMessageHandler<EntityMessageType.EndSkillCasting>
    {
        public override async Task HandleMessage(ECSEntity entity, EntityMessageType.EndSkillCasting message)
        {
            SkillComponent skillComponent = entity.GetComponent<SkillComponent>();
            if (skillComponent == null)
                return;

            if (!skillComponent.isCastingSkill)
                return;

            UnityLog.Info($"End skill casting:{skillComponent.castingSkillID}");
            skillComponent.isCastingSkill = false;
            skillComponent.castingSkillID = 0;

            // remove skill entity
            ECSEntity skillEntity = GameManager.ECS.FindEntity(skillComponent.castingSkillEntityID);
            skillEntity?.Dispose();

            skillComponent.castingSkillEntityID = 0;
            skillComponent.castingStartTime = 0;

            GameManager.ECS.SendMessageToEntity(entity.InstanceID, new EntityMessageType.UpdateMoveableState()).Coroutine();

            AnimatorComponent animatorComponent = entity.GetComponent<AnimatorComponent>();
            if (animatorComponent != null)
            {
                if (message.isCancel)
                {
                    animatorComponent.animator.Play(CharacterAnimationHash.FREE);
                }
                else
                {
                    animatorComponent.animator.CrossFade(CharacterAnimationHash.FREE, 0.1f);
                }
            }

            await GameManager.Message.Post(new MessageType.SkillCastFinished()
            {
                entityID = entity.InstanceID,
                isCancel = message.isCancel,
            });
        }
    }

    public class SkillReachComboTimeMessageHandler : EntityMessageHandler<EntityMessageType.SkillReachComboTime>
    {
        public override async Task HandleMessage(ECSEntity entity, EntityMessageType.SkillReachComboTime message)
        {
            SkillComponent skillComponent = entity.GetComponent<SkillComponent>();
            if (skillComponent == null)
                return;

            if (!skillComponent.preinputSkill.HasValue)
                return;

            int preinputSkill = skillComponent.preinputSkill.Value;
            SkillTarget? preinputTarget = skillComponent.preinputTarget;

            skillComponent.preinputSkill = null;
            skillComponent.preinputTarget = null;

            await GameManager.ECS.SendRpcToEntity<EntityMessageType.CastSkillRequest, EntityMessageType.CastSkillResponse>(skillComponent.EntityID, new EntityMessageType.CastSkillRequest()
            {
                configID = preinputSkill,
                target = preinputTarget,
                isCombo = true,
            });
        }
    }

    public class SkillInputTargetSatisfiedRpcHandler : EntityRpcHandler<EntityMessageType.SkillInputTargetSatisfiedRequest, EntityMessageType.SkillInputTargetSatisfiedResponse>
    {
        public override async Task<EntityMessageType.SkillInputTargetSatisfiedResponse> HandleRpc(ECSEntity entity, EntityMessageType.SkillInputTargetSatisfiedRequest request)
        {
            await Task.Yield();

            SkillComponent skillComponent = entity.GetComponent<SkillComponent>();
            if (skillComponent == null)
                return new EntityMessageType.SkillInputTargetSatisfiedResponse() { Error = true, ErrorMessage = $"Request skill satisfied failed, Without ability to cast skill" };

            ECSEntity targetEntity = GameManager.ECS.FindEntity(request.targetID);
            if (targetEntity == null)
                return new EntityMessageType.SkillInputTargetSatisfiedResponse() { Error = true, ErrorMessage = $"Request skill satisfied failed, target entity not found:{request.targetID}" };

            CharacterState characterState = CharacterHelper.GetCharacterState(request.targetID);

            if (characterState == CharacterState.Dead)
                return new EntityMessageType.SkillInputTargetSatisfiedResponse() { Error = true, ErrorMessage = $"Request skill satisfied failed, target entity dead" };

            if (characterState == CharacterState.Unvitalized)
                return new EntityMessageType.SkillInputTargetSatisfiedResponse() { Error = true, ErrorMessage = $"Request skill satisfied failed, target entity is unvitalized" };

            SkillConfig skillConfig = SkillConfig.ByID(request.skillID);
            if (skillConfig.InputTargetRelation != CharacterRelationType.None)
            {
                CharacterRelationType relation = CharacterHelper.GetRelation(entity as CharacterEntity, targetEntity as CharacterEntity);
                if ((relation & skillConfig.InputTargetRelation) == 0)
                    return new EntityMessageType.SkillInputTargetSatisfiedResponse() { Error = true, ErrorMessage = $"Request skill satisfied failed, relation unmatch:{relation}" };
            }

            CharacterViewComponent characterViewComponent1 = entity.GetComponent<CharacterViewComponent>();
            CharacterViewComponent characterViewComponent2 = targetEntity.GetComponent<CharacterViewComponent>();
            if (characterViewComponent1 == null || characterViewComponent2 == null)
                return new EntityMessageType.SkillInputTargetSatisfiedResponse() { Error = true, ErrorMessage = $"Request skill satisfied failed, character view component not found" };

            //float dist = (characterViewComponent1.gameObject.transform.position - characterViewComponent2.gameObject.transform.position).ToXZPlane().magnitude - characterViewComponent1.size - characterViewComponent2.size;
            //if (dist > skillConfig.CastLimit.Distance && skillConfig.CastLimit.Distance != -1)
            //    return new EntityMessageType.SkillInputTargetSatisfiedResponse() { Error = true, ErrorMessage = $"Request skill satisfied failed, distance unmatch" };

            return new EntityMessageType.SkillInputTargetSatisfiedResponse();
        }
    }
}
