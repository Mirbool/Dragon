using KKSG;
using Koakuma.Proto;
using System.Collections.Generic;
using TGame.ECS;

namespace Koakuma.Game.ECS
{
    public class PlayerComponent : ECSComponent
    {
        /// <summary>
        /// ���ʵ��id�����������ʵ��󣬰�id�浽��
        /// </summary>
        public long playerEntityID;

        /// <summary>
        /// ��¼��ɺ������Ϸ���ݣ��浽�⡣
        /// </summary>
        public GameInfo gameInfo;

        ///// <summary>
        ///// ���н�ɫ����
        ///// </summary>

        public List<Role> roles=new List<Role>();

<<<<<<< HEAD
        /*/// <summary>
        /// ��ǰѡ�����AllData
        /// </summary>
        public RoleAllInfo role;

        /// <summary>
        /// ��ǰѡ������±�
        /// </summary>
=======
        public RoleAllInfo selectRoleData;

        public Role crtSelectRole;

>>>>>>> 9783df7ccaaa6d66b300a2282af8eaa34b7e1e5b

        public int index;*/
    }
}
