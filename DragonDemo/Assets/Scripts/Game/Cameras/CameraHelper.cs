using Koakuma.Game.ECS;
using System.Threading.Tasks;
using TGame.ECS;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

namespace Koakuma.Game.Cameras
{
    public static class CameraHelper
    {
        public static long? RaycastCharacterEntity(Vector3 screenPosition)
        {
            CameraComponent cameraComponent = GameManager.ECS.World.GameScene.GetComponent<CameraComponent>();
            if (cameraComponent == null)
                return null;

            Ray ray = cameraComponent.brain.OutputCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMaskUtility.PLAYER_LAYER | 1 << LayerMaskUtility.MONSTER_LAYER))
            {
                CharacterViewMono characterViewMono = hit.transform.gameObject.GetComponent<CharacterViewMono>();
                if (characterViewMono != null)
                {
                    return characterViewMono.entityID;
                }
            }
            return null;
        }

        public static Vector3? RaycastToXZPlane(Vector3 screenPosition, bool snapToCharacter = true)
        {
            CameraComponent cameraComponent = GameManager.ECS.World.GameScene.GetComponent<CameraComponent>();
            if (cameraComponent == null)
                return null;

            Vector3? position = null;
            Ray ray = cameraComponent.brain.OutputCamera.ScreenPointToRay(screenPosition);
            if (snapToCharacter)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMaskUtility.PLAYER_LAYER | 1 << LayerMaskUtility.MONSTER_LAYER))
                {
                    CharacterViewMono characterViewMono = hit.transform.gameObject.GetComponent<CharacterViewMono>();
                    if (characterViewMono != null)
                    {
                        ECSEntity ecsEntity = GameManager.ECS.FindEntity(characterViewMono.entityID);
                        GameObjectComponent gameObjectComponent = ecsEntity?.GetComponent<GameObjectComponent>();
                        if (gameObjectComponent != null)
                        {
                            position = gameObjectComponent.position;
                        }
                    }
                }
            }

            if (!position.HasValue)
            {
                //float angle = Vector3.Angle(ray.direction, Vector3.down);
                //float length = ray.origin.y / Mathf.Cos(Mathf.Deg2Rad * angle);
                //position = ray.origin + ray.direction * length;
                if (Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMaskUtility.TERRAIN_LAYER))
                {
                    position = hit.point;
                }
            }

            return position;
        }

        //public static async Task TweenColor(Color to, float duration)
        //{
        //    CameraComponent cameraComponent = GameManager.ECS.World.GameScene.GetComponent<CameraComponent>();
        //    if (cameraComponent != null)
        //    {
        //        PostProcessVolume volumn = cameraComponent.brain.GetComponent<PostProcessVolume>();
        //        ColorGrading colorGrading = volumn.profile.GetSetting<ColorGrading>();
        //        Color fromColor = colorGrading.colorFilter.value;
        //        LinearApproximatorF cameraColorApproximator = new LinearApproximatorF(1 / duration);
        //        cameraColorApproximator.Destination = 1;
        //        while (cameraColorApproximator.Current != 1)
        //        {
        //            UnityLog.Info("value:" + cameraColorApproximator.Current);
        //            colorGrading.colorFilter.Interp(fromColor, to, cameraColorApproximator.Current);
        //            cameraColorApproximator.Update(Time.deltaTime);
        //            await new WaitForUpdate();
        //        }
        //        colorGrading.colorFilter.Interp(colorGrading.colorFilter.value, to, cameraColorApproximator.Current);
        //    }
        //}
    }
}
