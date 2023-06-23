using Config;
using Koakuma.Game;
using Koakuma.Game.ECS;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame.Procedure;
using UnityEngine;
using UnityEngine.UIElements;

public class InitProcedure : BaseProcedure
{
    public override async Task OnEnterProcedure(object value)
    {
        UnityLog.Info("enter Init Procedure");

        ///附加游戏 的各个模块的组件，
        PlayerComponent pm=  GameManager.ECS.World.AddNewComponent<PlayerComponent>();
        SelectRoleComponent sm =  GameManager.ECS.World.AddNewComponent<SelectRoleComponent>();
        
        GameManager.ECS.World.AddNewComponent<ServerCommponent>();

      
        XMainClient.XEquipDocument.instance.init();
        for (int i = 0; i < RoleInfoConfig.Count; i++)
        {
            Role role = new Role();

            RoleInfoConfig data = RoleInfoConfig.ByIndex(i);
            role.Init(i, data.Prefabs, data.PartConfigData,data.Animation,data.WeaponPoint);
            role.RoleGo.transform.position = new Vector3(data.ShowPosX, data.ShowPosY, data.ShowPosZ);
            role.ChangeAnimation("Idle",data.Animation+"idle_new");
            pm.roles.Add(role);
        }

        
        ///打开登录UI
        await GameManager.UI.OpenUIAsync(UIViewID.LoginUI);
        await Task.Yield();
    }
}

