using KKSG;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XUtliPoolLib;

public class RolesMgr : XSingleton<RolesMgr>
{
    public List<RoleBriefInfo> PlayerBriefInfo;
    public RolesMgr.XPlayerCharacterInfo XPlayerCharacters
    {
        get
        {
            return this._playerCharacterInfo;
        }
    }
    public bool ProcessAccountData(LoadAccountData roAccountData)
    {
        bool flag = roAccountData == null;
        bool result;
        if (flag)
        {
            result = false;
        }
        else
        {
            XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo.Clear();
            this.ParseRoleBriefInfo(XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo, roAccountData.role1);
            this.ParseRoleBriefInfo(XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo, roAccountData.role2);
            this.ParseRoleBriefInfo(XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo, roAccountData.role3);
            this.ParseRoleBriefInfo(XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo, roAccountData.role4);
            this.ParseRoleBriefInfo(XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo, roAccountData.role5);
            this.ParseRoleBriefInfo(XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo, roAccountData.role6);
            this.ParseRoleBriefInfo(XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo, roAccountData.role7);
            this.ParseRoleBriefInfo(XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo, roAccountData.role8);
            this.ParseRoleBriefInfo(XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo, roAccountData.role9);
            bool flag2 = roAccountData.selectSlot >= /*(uint)XGame.RoleCount*/8;
            //if (flag2)
            //{
            //    XSingleton<RolesMgr>.singleton.XPlayerCharacters.SelectedSlot = 0;
            //}
            //else
            //{
            //    XSingleton<RolesMgr>.singleton.XPlayerCharacters.SelectedSlot = (int)(roAccountData.selectSlot + 1U);
            //}
            result = true;
        }
        return result;
    }
    private void ParseRoleBriefInfo(List<RoleBriefInfo> list, byte[] data)
    {
        bool flag = list.Count >= /*XGame.RoleCount*/8;
        if (!flag)
        {
            bool flag2 = data == null || data.Length == 0;
            if (flag2)
            {
                list.Add(null);
            }
            else
            {
                MemoryStream source = new MemoryStream(data);
                RoleBriefInfo item = Serializer.Deserialize<RoleBriefInfo>(source);
                list.Add(item);
            }
        }
    }
    private XPlayerCharacterInfo _playerCharacterInfo = new XPlayerCharacterInfo();

    // Token: 0x020019F3 RID: 6643
    public class XPlayerCharacterInfo
    {
        // Token: 0x040080B6 RID: 32950
        public List<RoleBriefInfo> PlayerBriefInfo = new List<RoleBriefInfo>();

        // Token: 0x040080B7 RID: 32951
        public int SelectedSlot = 0;
    }
}
