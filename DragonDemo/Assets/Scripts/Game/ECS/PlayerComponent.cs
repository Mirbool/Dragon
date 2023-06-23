using KKSG;
using Koakuma.Proto;
using System.Collections.Generic;
using TGame.ECS;

namespace Koakuma.Game.ECS
{
    public class PlayerComponent : ECSComponent
    {
        /// <summary>
        /// 玩家实体id，创建出玩家实体后，把id存到这
        /// </summary>
        public long playerEntityID;

        /// <summary>
        /// 登录完成后，玩家游戏数据，存到这。
        /// </summary>
        public GameInfo gameInfo;

        ///// <summary>
        ///// 所有角色数据
        ///// </summary>

        public List<Role> roles=new List<Role>();

<<<<<<< HEAD
        /*/// <summary>
        /// 当前选择玩家AllData
        /// </summary>
        public RoleAllInfo role;

        /// <summary>
        /// 当前选择玩家下标
        /// </summary>
=======
        public RoleAllInfo selectRoleData;

        public Role crtSelectRole;

>>>>>>> 9783df7ccaaa6d66b300a2282af8eaa34b7e1e5b

        public int index;*/
    }
}
