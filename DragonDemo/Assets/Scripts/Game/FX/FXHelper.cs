using Config;
using Koakuma.Game.Characters;
using Koakuma.Game.ECS;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.FX
{
    public static class FXHelper
    {
        public static void PlayFX(PlayFXData data)
        {
            if (data.configID == 0)
                return;

            FXConfig fXConfig = FXConfig.ByID(data.configID);

            ECSEntity targetEntity = GameManager.ECS.FindEntity(data.targetEntityID);
            Transform bindTransform = null;
            if (fXConfig.BindBone == FXBindBone.Root ||
                fXConfig.BindBone == FXBindBone.VFX)
            {
                GameObjectComponent gameObjectComponent = targetEntity.GetComponent<GameObjectComponent>();
                if (gameObjectComponent != null)
                {
                    if (fXConfig.BindBone == FXBindBone.Root)
                    {
                        bindTransform = gameObjectComponent.transform;
                    }
                    else if (fXConfig.BindBone == FXBindBone.VFX)
                    {
                        bindTransform = gameObjectComponent.fxBindPoint;
                    }
                }
            }
            else
            {
                // 尝试绑定Character
                CharacterViewComponent characterViewComponent = targetEntity.GetComponent<CharacterViewComponent>();
                if (characterViewComponent != null)
                {
                    characterViewComponent.boneMapping.TryGetValue(fXConfig.BindBone, out bindTransform);
                }
            }

            if (bindTransform == null)
            {
                UnityLog.Error($"Play fx failed, bind bone not found, id:{data.configID}");
                return;
            }

            ECSEntity parentEntity;
            if (data.parentEntityID == 0)
            {
                parentEntity = targetEntity;
            }
            else
            {
                parentEntity = GameManager.ECS.FindEntity(data.parentEntityID);
            }

            FXEntity fXEntity = new FXEntity();
            parentEntity.AddChild(fXEntity);
            fXEntity.AddComponent<FXComponent, FXData>(new FXData()
            {
                id = data.configID,
                bindTransform = bindTransform,
            });
        }
    }
}
