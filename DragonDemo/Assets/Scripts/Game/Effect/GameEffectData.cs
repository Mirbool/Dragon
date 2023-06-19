using Koakuma.Game.Skills;
using System.Collections.Generic;

namespace Koakuma.Game.Effects
{
    public struct GameEffectData
    {
        public long casterEntityID;
        public long sourceEntityID;
        public string effectName;
        public bool effectRevert;
        public List<string> effectArgs;
        public SkillTarget target;
        public SkillTarget? inputTarget;
    }
}
