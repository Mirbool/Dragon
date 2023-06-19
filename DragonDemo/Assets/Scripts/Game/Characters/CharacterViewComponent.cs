//using Cinemachine;
using Cinemachine;
using CMF;
using Config;
using Koakuma.Game.ECS;
using Koakuma.Game.Inputs;
//using Koakuma.Game.FX;
//using Koakuma.Game.GameCamera;
//using Koakuma.Game.Inputs;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame;
using TGame.ECS;
using TGame.Message;
using UnityEngine;

namespace Koakuma.Game.Characters
{
    public class CharacterViewComponent : ECSComponent
    {
        public int configID;
        public Rigidbody rigidbody;
        public AdvancedWalkerController advancedWalkerController;
        public PlayerMoveInput playerMoveInput;
        public CharacterViewMono characterViewMono;
        public Collider collider;
        public float size;

        public Dictionary<FXBindBone, Transform> boneMapping;
        public CinemachineImpulseSource impulseSource;
        public Renderer[] meshRenderers;
        public Material[] materials;

        public GameObject characterModel;
        //  public RagdollConfig ragDollConfig;
    }

    public class CharacterViewComponentAwakeSystem : AwakeSystem<CharacterViewComponent, CharacterViewData>
    {
        public override void Awake(CharacterViewComponent c, CharacterViewData p2)
        {
            CharacterConfig characterConfig = CharacterConfig.ByID(p2.configID);

            c.configID = p2.configID;
            c.size = characterConfig.Size;

            GameObjectComponent gameObjectComponent = c.Entity.GetComponent<GameObjectComponent>();
            c.impulseSource = gameObjectComponent.gameObject.GetComponent<CinemachineImpulseSource>();
            c.rigidbody = gameObjectComponent.gameObject.GetComponent<Rigidbody>();
            c.advancedWalkerController = gameObjectComponent.gameObject.GetComponent<AdvancedWalkerController>();

            c.collider = gameObjectComponent.gameObject.GetComponent<Collider>();
            c.collider.enabled = true;

            if (p2.team == CharacterTeam.Player)
            {
                c.playerMoveInput = gameObjectComponent.gameObject.AddComponent<PlayerMoveInput>();
                c.advancedWalkerController.SetCharacterInput(c.playerMoveInput);
            }

            c.characterViewMono = gameObjectComponent.gameObject.AddComponent<CharacterViewMono>();
            c.characterViewMono.entityID = c.EntityID;

            c.characterModel = GameManager.Asset.LoadGameObject(p2.model);
            c.characterModel.transform.SetParentAndResetAll(gameObjectComponent.gameObject.transform);
            c.characterModel.SetActive(true);

            AnimatorComponent animatorComponent = c.Entity.GetComponent<AnimatorComponent>();
            if (animatorComponent != null)
            {
                //c.ragDollConfig = c.characterModel.GetComponent<RagdollConfig>();
                //if (c.ragDollConfig != null)
                //{
                //    c.ragDollConfig.animator = animatorComponent.animator;
                //}

                AvatarConfig avatarConfig = c.characterModel.GetComponent<AvatarConfig>();
                if (avatarConfig != null)
                {
                    animatorComponent.animator.avatar = avatarConfig.avatar;
                    animatorComponent.animator.enabled = true;
                }
            }

            c.meshRenderers = c.characterModel.GetComponentsInChildren<Renderer>();

            int layer = p2.team == CharacterTeam.Player ? LayerMaskUtility.PLAYER_LAYER : LayerMaskUtility.MONSTER_LAYER;
            c.characterModel.layer = layer;
            foreach (var mr in c.meshRenderers)
            {
                mr.gameObject.layer = layer;
            }

            InitBones(c, c.characterModel);

             GameManager.ECS.SendMessageToEntity(c.EntityID, new EntityMessageType.CharacterViewCreated() { characterViewComponent = c }).Coroutine();

            gameObjectComponent.gameObject.SetActive(true);
        }

        private void InitBones(CharacterViewComponent c, GameObject gameObject)
        {
            c.boneMapping = DictionaryPool<FXBindBone, Transform>.Obtain();
            c.boneMapping.Add(FXBindBone.Root, gameObject.transform);
            c.boneMapping.Add(FXBindBone.WeaponL, gameObject.transform.FindChild<Transform>("WeaponL", 0));
            c.boneMapping.Add(FXBindBone.WeaponR, gameObject.transform.FindChild<Transform>("WeaponR", 0));
            c.boneMapping.Add(FXBindBone.HandL, gameObject.transform.FindChild<Transform>("HandL", 0));
            c.boneMapping.Add(FXBindBone.HandR, gameObject.transform.FindChild<Transform>("HandR", 0));
            c.boneMapping.Add(FXBindBone.FootL, gameObject.transform.FindChild<Transform>("FootL", 0));
            c.boneMapping.Add(FXBindBone.FootR, gameObject.transform.FindChild<Transform>("FootR", 0));
            c.boneMapping.Add(FXBindBone.Chest, gameObject.transform.FindChild<Transform>("Chest", 0));
            c.boneMapping.Add(FXBindBone.Head, gameObject.transform.FindChild<Transform>("Head", 0));
            c.boneMapping.Add(FXBindBone.TopHead, gameObject.transform.FindChild<Transform>("TopHead", 0));
            c.boneMapping.Add(FXBindBone.BulletFire, gameObject.transform.FindChild<Transform>("BulletFire", 0));
        }
    }

    public class CharacterViewComponentDestroySystem : DestroySystem<CharacterViewComponent>
    {
        public override void Destroy(CharacterViewComponent c)
        {
            c.configID = default;

            c.size = default;

            c.impulseSource = default;

            c.rigidbody = default;
            c.meshRenderers = default;
            c.advancedWalkerController = default;

            Object.Destroy(c.playerMoveInput);
            c.playerMoveInput = default;

            DictionaryPool<FXBindBone, Transform>.Release(c.boneMapping);
            c.boneMapping = default;

            Object.Destroy(c.characterViewMono);
            c.characterViewMono = default;

            c.collider = default;

            GameManager.Asset.UnloadGameObject(c.characterModel);
            c.characterModel = null;

            /// c.ragDollConfig = null;
        }
    }

    //public class CharacterGenerateVibrationMessageHandler : EntityMessageHandler<EntityMessageType.GenerateVibration>
    //{
    //    public override async Task HandleMessage(ECSEntity entity, EntityMessageType.GenerateVibration message)
    //    {
    //        CharacterViewComponent characterViewComponent = entity.GetComponent<CharacterViewComponent>();
    //        if (characterViewComponent == null)
    //            return;

    //        if (characterViewComponent.impulseSource == null)
    //            return;

    //        Vector3 velocity = Vector3.one;
    //        switch (message.strength)
    //        {
    //            case CameraVibrationStrength.Weak:
    //                velocity *= 0.7f;
    //                break;
    //            case CameraVibrationStrength.Middle:
    //                velocity *= 1;
    //                break;
    //            case CameraVibrationStrength.Strong:
    //                velocity *= 1.3f;
    //                break;
    //        }
    //        characterViewComponent.impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = 0.3f;
    //        characterViewComponent.impulseSource.GenerateImpulse(velocity);

    //        UnityLog.Info("generate vibration");
    //        await Task.Yield();
    //    }
    //}

    public class CharacterViewMoveStateChangedMessageHandler : EntityMessageHandler<EntityMessageType.MoveStateChanged>
    {
        public override async Task HandleMessage(ECSEntity entity, EntityMessageType.MoveStateChanged message)
        {
            CharacterViewComponent characterViewComponent = entity.GetComponent<CharacterViewComponent>();
            if (characterViewComponent == null)
                return;

            if (message.currentMoveState == MoveState.StartMove)
            {
                characterViewComponent.advancedWalkerController.enabled = true; ;
            }
            else if (message.currentMoveState == MoveState.Idle)
            {
                characterViewComponent.advancedWalkerController.enabled = false;
            }

            await Task.Yield();
        }
    }

    //public class CharacterViewComponent_LevelUpMessageHandler : MessageHandler<MessageType.CharacterAttributeChanged>
    //{
    //    public override async Task HandleMessage(MessageType.CharacterAttributeChanged arg)
    //    {
    //        if (arg.type != CharacterAttributeType.Level)
    //            return;

    //        ECSEntity entity = GameManager.ECS.FindEntity(arg.entityID);
    //        if (entity == null)
    //            return;

    //        FXHelper.PlayFX(new PlayFXData()
    //        {
    //            configID = 80001001,
    //            targetEntityID = entity.InstanceID,
    //            parentEntityID = entity.InstanceID,
    //        });

    //        await Task.Yield();
    //    }
    //}
}
