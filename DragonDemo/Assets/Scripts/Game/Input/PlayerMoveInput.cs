using CMF;
using Koakuma.Game.Characters;
using UnityEngine;

namespace Koakuma.Game.Inputs
{
    public class PlayerMoveInput : CharacterInput
    {
        private int lastUpdateMovementFrame;

        private Vector3 movement;
        private Vector3 Movement
        {
            get
            {
                if (lastUpdateMovementFrame != Time.frameCount)
                {
                    lastUpdateMovementFrame = Time.frameCount;

                    CharacterEntity playerCharacter = CharacterHelper.GetPlayerEntity();
                    CharacterMoveComponent characterMoveComponent = playerCharacter?.GetComponent<CharacterMoveComponent>();
                    if (characterMoveComponent != null)
                    {
                        movement = characterMoveComponent.localDirection;
                    }
                }
                return movement;
            }
        }

        public override float GetHorizontalMovementInput()
        {
            return Movement.x;
        }

        public override float GetVerticalMovementInput()
        {
            return Movement.z;
        }

        public override bool IsJumpKeyPressed()
        {
            return false;
        }
    }
}
