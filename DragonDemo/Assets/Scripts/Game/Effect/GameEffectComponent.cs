using Config;
using Koakuma.Game.Characters;
using Koakuma.Game.ECS;
using Koakuma.Game.Skills;
using Koakuma.Utility;
//using Koakuma.Utility;
using System.Threading.Tasks;
using TGame;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.Effects
{
    public abstract class GameEffectComponent : ECSComponent
    {
        public abstract bool InstantEffect { get; }
    }

    public class SkillSelectTargetRpcHandler : EntityRpcHandler<EntityMessageType.SkillSelectTargetsRequest, EntityMessageType.SkillSelectTargetsResponse>
    {
        public override async Task<EntityMessageType.SkillSelectTargetsResponse> HandleRpc(ECSEntity entity, EntityMessageType.SkillSelectTargetsRequest request)
        {
            //GameSkillEntity skillEntity = entity.FindChild<GameSkillEntity>(request.skillEntityID);
            //if (skillEntity == null)
            //    return new EntityMessageType.SkillSelectTargetsResponse() { Error = true, ErrorMessage = $"Skill select target fail, game skill not found, id:{request.skillEntityID}" };

            //GameSkillComponent gameSkillComponent = skillEntity.GetComponent<GameSkillComponent>();
            //if (gameSkillComponent == null)
            //    return new EntityMessageType.SkillSelectTargetsResponse() { Error = true, ErrorMessage = $"Skill select target fail, game skill component not found, id:{request.skillEntityID}" };

            switch (request.detectType)
            {
                case SkillDetectType.InputArgs:
                    await DetectInputArgs(entity, request.inputTarget, request);
                    break;
                case SkillDetectType.InputPosition:
                    await DetectInputPosition(entity, request.inputTarget, request);
                    break;
                case SkillDetectType.Caster:
                    await DetectInputCaster(entity, request.inputTarget, request);
                    break;
                case SkillDetectType.TriggerUnit:
                    await DetectInputTriggerUnit(entity, request.inputTarget, request);
                    break;
                case SkillDetectType.Circle:
                    await DetectInputCircle(entity, request.inputTarget, request);
                    break;
                case SkillDetectType.Sector:
                    await DetectInputSector(entity, request.inputTarget, request);
                    break;
                case SkillDetectType.Rectangle:
                    await DetectInputRectangle(entity, request.inputTarget, request);
                    break;
                default:
                    return new EntityMessageType.SkillSelectTargetsResponse() { Error = true };
            }

            for (int i = request.targetList.Count - 1; i >= 0; i--)
            {
                SkillTarget target = request.targetList[i];
                if (target.type == SkillTargetType.Unit)
                {
                    ECSEntity targetEntity = GameManager.ECS.FindEntity(target.entity);

                    // 存活状态
                    CharacterState characterState = CharacterHelper.GetCharacterState(target.entity);
                    if (characterState == CharacterState.Unvitalized ||
                        characterState == CharacterState.Dead)
                    {
                        request.targetList.RemoveAt(i);
                        continue;
                    }

                    // 关系
                    CharacterRelationType relation = CharacterHelper.GetRelation(entity as CharacterEntity, targetEntity as CharacterEntity);
                    if ((relation & request.relation) == 0)
                    {
                        request.targetList.RemoveAt(i);
                        continue;
                    }
                }
            }

            return new EntityMessageType.SkillSelectTargetsResponse()
            {
                targets = request.targetList,
            };
        }

        private async Task DetectInputArgs(ECSEntity entity, SkillTarget? inputTarget, EntityMessageType.SkillSelectTargetsRequest request)
        {
            if (inputTarget.HasValue && inputTarget.Value.type == SkillTargetType.Unit)
            {
                request.targetList.Add(inputTarget.Value);
            }
            await Task.Yield();
        }

        private async Task DetectInputPosition(ECSEntity entity, SkillTarget? inputTarget, EntityMessageType.SkillSelectTargetsRequest request)
        {
            if (inputTarget.HasValue && inputTarget.Value.type == SkillTargetType.Position)
            {
                request.targetList.Add(inputTarget.Value);
            }
            else if (inputTarget.HasValue && inputTarget.Value.type == SkillTargetType.Unit)
            {
                ECSEntity targetEntity = GameManager.ECS.FindEntity(inputTarget.Value.entity);
                GameObjectComponent gameObjectComponent = targetEntity?.GetComponent<GameObjectComponent>();
                if (gameObjectComponent != null)
                {
                    request.targetList.Add(new SkillTarget()
                    {
                        type = SkillTargetType.Position,
                        position = gameObjectComponent.position,
                    });
                }
            }
            await Task.Yield();
        }

        private async Task DetectInputCaster(ECSEntity entity, SkillTarget? inputTarget, EntityMessageType.SkillSelectTargetsRequest request)
        {
            request.targetList.Add(new SkillTarget()
            {
                type = SkillTargetType.Unit,
                entity = entity.InstanceID,
            });
            await Task.Yield();
        }

        private async Task DetectInputTriggerUnit(ECSEntity entity, SkillTarget? inputTarget, EntityMessageType.SkillSelectTargetsRequest request)
        {
            await Task.Yield();
        }

        private async Task DetectInputCircle(ECSEntity entity, SkillTarget? inputTarget, EntityMessageType.SkillSelectTargetsRequest request)
        {
            GameObjectComponent gameObjectComponent = entity.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
                return;

            Vector3? detectCenter = GetDetectCenter(gameObjectComponent, inputTarget, request.detectCenterType, request.detectCenterOffset);
            if (detectCenter == null)
                return;

            float radius = request.detectArgs[0];

            CircleRegion.Show(radius, detectCenter.Value);

            Collider[] colliders = Physics.OverlapSphere(detectCenter.Value, radius, 1 << LayerMaskUtility.OBJECT_LAYER | 1 << LayerMaskUtility.PLAYER_LAYER | 1 << LayerMaskUtility.MONSTER_LAYER | 1 << LayerMaskUtility.INVISIBLE_CHARACTER_LAYER);
            foreach (var collider in colliders)
            {
                //if (collider.CompareTag(TagUtility.OBJECT) && request.includeTargetType.HasFlag(SkillTargetType.Object))
                //{
                //    DestructableObject destructableObject = collider.GetComponent<DestructableObject>();
                //    if (destructableObject == null)
                //    {
                //        UnityLog.Error($"Destructable Object找不到脚本:{destructableObject.name}");
                //    }
                //    else
                //    {
                //        targets.Add(new SkillTarget() { Type = SkillTargetType.Object, Object = destructableObject, });
                //    }
                //    continue;
                //}
                if (collider.CompareTag(TagUtility.Character))
                {
                    CharacterViewMono characterViewMono = collider.gameObject.GetComponent<CharacterViewMono>();
                    if (characterViewMono == null)
                        continue;

                    request.targetList.Add(new SkillTarget() { type = SkillTargetType.Unit, entity = characterViewMono.entityID, });
                }
            }
            await Task.Yield();
        }

        private async Task DetectInputSector(ECSEntity entity, SkillTarget? inputTarget, EntityMessageType.SkillSelectTargetsRequest request)
        {
            GameObjectComponent gameObjectComponent = entity.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
                return;

            Vector3? detectCenter = GetDetectCenter(gameObjectComponent, inputTarget, request.detectCenterType, request.detectCenterOffset);
            if (detectCenter == null)
                return;

            float radius = request.detectArgs[0];
            float angle = request.detectArgs[1];
            Vector3 forward = gameObjectComponent.rotation * Vector3.forward;

            SectorRegion.Show(detectCenter.Value, radius, angle, forward);

            Collider[] colliders = Physics.OverlapSphere(detectCenter.Value, radius, 1 << LayerMaskUtility.OBJECT_LAYER | 1 << LayerMaskUtility.PLAYER_LAYER | 1 << LayerMaskUtility.MONSTER_LAYER | 1 << LayerMaskUtility.INVISIBLE_CHARACTER_LAYER);
            foreach (var collider in colliders)
            {
                SkillTarget? target = null;
                //if (collider.CompareTag(TagUtility.OBJECT))
                //{
                //    SceneObjectMono sceneObjectMono = collider.gameObject.GetComponent<SceneObjectMono>();
                //    if (sceneObjectMono != null)
                //    {
                //        target = new SkillTarget() { type = SkillTargetType.Object, entity = sceneObjectMono.entityID };
                //    }
                //}
                if (collider.CompareTag(TagUtility.Character))
                {
                    CharacterViewMono characterViewMono = collider.gameObject.GetComponent<CharacterViewMono>();
                    if (characterViewMono == null)
                        continue;

                    target = new SkillTarget() { type = SkillTargetType.Unit, entity = characterViewMono.entityID, };
                }

                if (target.HasValue)
                {
                    Vector3 toCollider = (collider.transform.position - gameObjectComponent.position).ToXZPlane();
                    float angleToCollider = Vector3.Angle(forward, toCollider) * 2;
                    if (angleToCollider > angle)
                    {
                        // 角度不在区间内时，如果范围重合 也可以
                        float targetSize = 0.5f;
                        if (target.Value.type == SkillTargetType.Unit)
                        {
                            ECSEntity targetEntity = GameManager.ECS.FindEntity(target.Value.entity);
                            CharacterViewComponent targetCharacterViewComponent = targetEntity?.GetComponent<CharacterViewComponent>();
                            if (targetCharacterViewComponent != null)
                            {
                                targetSize = targetCharacterViewComponent.size;
                            }
                        }

                        Vector3? targetPositon = target.Value.GetPosition();
                        if (!targetPositon.HasValue)
                            continue;

                        if (!GraphUtility.CircleCollide2D(detectCenter.Value, 0.5f, targetPositon.Value, targetSize))
                            continue;
                    }

                    request.targetList.Add(target.Value);
                }
            }

            await Task.Yield();
        }

        private async Task DetectInputRectangle(ECSEntity entity, SkillTarget? inputTarget, EntityMessageType.SkillSelectTargetsRequest request)
        {
            GameObjectComponent gameObjectComponent = entity.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
                return;

            Vector3? detectCenter = GetDetectCenter(gameObjectComponent, inputTarget, request.detectCenterType, request.detectCenterOffset);
            if (detectCenter == null)
                return;

            float width = request.detectArgs[0];
            float height = request.detectArgs[1];

            //foreach (var target in GameManager.Data.AllCharacters.Values)
            //{
            //    if (!GraphUtility.RectangleCircleCollide2D(detectCenter.Value, caster.Rotation, width, height, target.Position, target.Size))
            //        continue;

            //    targets.Add(new SkillTarget() { Type = SkillTargetType.Unit, Unit = target, });
            //}

            RectangleRegion.Show(detectCenter.Value, width, height, gameObjectComponent.rotation * Vector3.forward);

            Vector3 rectCenter = detectCenter.Value + gameObjectComponent.rotation * Vector3.forward * height / 2f;
            //GameObject rectObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //rectObject.transform.position = rectCenter;
            //rectObject.transform.localScale = new Vector3(width, 1, height);
            //rectObject.transform.rotation = caster.Rotation;

            Collider[] colliders = Physics.OverlapBox(rectCenter, new Vector3(width / 2f, 10, height / 2f), gameObjectComponent.rotation, 1 << LayerMaskUtility.OBJECT_LAYER | 1 << LayerMaskUtility.PLAYER_LAYER | 1 << LayerMaskUtility.MONSTER_LAYER | 1 << LayerMaskUtility.INVISIBLE_CHARACTER_LAYER);
            foreach (var collider in colliders)
            {
                //if (collider.CompareTag(TagUtility.OBJECT) && includeTargetType.HasFlag(SkillTargetType.Object))
                //{
                //    DestructableObject destructableObject = collider.GetComponent<DestructableObject>();
                //    if (destructableObject == null)
                //    {
                //        UnityLog.Info($"找不到DestructableObject脚本:{destructableObject.name}");
                //    }
                //    else
                //    {
                //        targets.Add(new SkillTarget() { Type = SkillTargetType.Object, Object = destructableObject, });
                //    }
                //    continue;
                //}
                if (collider.CompareTag(TagUtility.Character))
                {
                    CharacterViewMono characterViewMono = collider.gameObject.GetComponent<CharacterViewMono>();
                    if (characterViewMono == null)
                        continue;

                    request.targetList.Add(new SkillTarget() { type = SkillTargetType.Unit, entity = characterViewMono.entityID, });
                    continue;
                }
            }

            await Task.Yield();
        }

        private Vector3? GetDetectCenter(GameObjectComponent gameObjectComponent, SkillTarget? inputTarget, SkillDetectCenterType detectCenterType, float detectCenterOffset)
        {
            Vector3 center = Vector3.zero;
            switch (detectCenterType)
            {
                case SkillDetectCenterType.None:
                    return null;
                case SkillDetectCenterType.CasterPosition:
                    center = gameObjectComponent.position;
                    break;
                case SkillDetectCenterType.InputPosition:
                    if (inputTarget == null || inputTarget.Value.type != SkillTargetType.Position)
                    {
                        UnityLog.Error($"Get detect center fail, target type: {inputTarget.Value.type}, expect type: position");
                        return null;
                    }
                    center = inputTarget.Value.position;
                    break;
                case SkillDetectCenterType.InputUnit:
                    if (inputTarget == null || inputTarget.Value.type != SkillTargetType.Unit)
                    {
                        UnityLog.Error($"Get detect center fail, target type: {inputTarget.Value.type}, expect type: unit");
                        return null;
                    }
                    ECSEntity inputTargetEntity = GameManager.ECS.FindEntity(inputTarget.Value.entity);
                    GameObjectComponent inputTargetGameObjectComponent = inputTargetEntity?.GetComponent<GameObjectComponent>();
                    if (inputTargetGameObjectComponent != null)
                    {
                        center = inputTargetGameObjectComponent.position;
                    }
                    break;
                default:
                    UnityLog.Error($"Get detect center fail, unsupported detect center type");
                    return null;
            }

            if (detectCenterType == SkillDetectCenterType.CasterPosition)
            {
                center += gameObjectComponent.rotation * Vector3.forward * detectCenterOffset;
            }
            else
            {
                Vector3 casterToCenter = (center - gameObjectComponent.position).ToXZPlane().normalized;
                center += casterToCenter * detectCenterOffset;
            }
            return center;
        }
    }
}
