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


    }
}
