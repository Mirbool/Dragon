using Config;
using Koakuma.Game.ECS;
//using Koakuma.Game.Weapons;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame;
using TGame.ECS;

namespace Koakuma.Game.Characters
{
    public class CharacterChangeHQModelMessageHandler : EntityMessageHandler<EntityMessageType.ChangeHQModel>
    {
        public override async Task HandleMessage(ECSEntity entity, EntityMessageType.ChangeHQModel message)
        {
            entity.RemoveComponent<CharacterViewComponent>();
            entity.RemoveComponent<AnimatorComponent>();

            CharacterComponent characterComponent = entity.GetComponent<CharacterComponent>();

            CharacterConfig characterConfig = CharacterConfig.ByID(characterComponent.configID);

            if (!message.useHQ)
            {
                string animator = message.useHQ ? characterConfig.AnimatorHQ : characterConfig.Animator;
               entity.AddComponent<AnimatorComponent, string>(animator);
            }

            string model = message.useHQ ? characterConfig.ModelHQ : characterConfig.Model;
            entity.AddComponent<CharacterViewComponent, CharacterViewData>(new CharacterViewData()
            {
                configID = characterComponent.configID,
                team = characterComponent.team,
                model = model,
            });

            //List<WeaponEntity> weaponEnties = ListPool<WeaponEntity>.Obtain();
            //entity.FindChildren(weaponEnties);
            //foreach (var weapon in weaponEnties)
            //{
            //    GameManager.ECS.SendMessageToEntity(entity.InstanceID, new EntityMessageType.AttachWeapon() { weaponEntity = weapon }).Coroutine();
            //}
            //ListPool<WeaponEntity>.Release(weaponEnties);

            await Task.Yield();
        }
    }
}
