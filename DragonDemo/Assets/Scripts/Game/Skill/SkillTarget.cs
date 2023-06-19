using Config;
using Koakuma.Game.ECS;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.Skills
{
    [Beebyte.Obfuscator.Skip]
    public struct SkillTarget
    {
        public SkillTargetType type;
        public Vector3 position;
        public long entity;

        public Vector3? GetPosition()
        {
            if (type == SkillTargetType.Position)
                return position;

            ECSEntity entity = GameManager.ECS.FindEntity(this.entity);
            GameObjectComponent gameObjectComponent = entity?.GetComponent<GameObjectComponent>();
            if (gameObjectComponent != null)
            {
                return gameObjectComponent.position;
            }
            return null;
        }
    }
}
