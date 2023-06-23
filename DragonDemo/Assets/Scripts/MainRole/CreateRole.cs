using Config;
using Koakuma.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRole : MonoBehaviour
{
    private SelectRoleComponent sm;
    public GameObject root;
    public GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        sm = GameManager.ECS.World.GetComponent<SelectRoleComponent>();
        for (int i = 0; i < RoleInfoConfig.Count; i++)
        {
            if(i == sm.index)
            {
                Role role = new Role();

                RoleInfoConfig data = RoleInfoConfig.ByIndex(sm.index);
                role.Init(i, data.Prefabs, data.PartConfigData, data.Animation, data.WeaponPoint);
                role.RoleGo.transform.position = root.transform.position;

                role.Init(i, data.Prefabs, data.PartConfigData, data.Animation, data.WeaponPoint);
                role.RoleGo.transform.position = go.transform.position;
                role.RoleGo.transform.eulerAngles = new Vector3(0, 180, 0);

                //role.ChangeAnimation("Idle", data.Animation + "idle_new");
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
