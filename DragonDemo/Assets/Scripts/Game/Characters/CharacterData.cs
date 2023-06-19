using KKSG;
using UnityEngine;

namespace Koakuma.Game.Characters
{
    public class CharacterData
    {
        public Vector3 position;
        public Quaternion rotation;
        public long scene; ///³¡¾°ÊµÌåid

        public Proto.CharacterInfo characterInfo;

        public bool useHQ;

        //public RoleBriefInfo roleBriefInfo;
    }
}
