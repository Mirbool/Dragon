using KKSG;
using Koakuma.Game;
using XUtliPoolLib;

namespace XMainClient
{
    // Token: 0x02001147 RID: 4423
    internal class Process_RpcC2M_SelectRoleNew
	{
		// Token: 0x0600D9E7 RID: 55783 RVA: 0x0032BCC0 File Offset: 0x00329EC0
		public static void OnReply(SelectRoleNewArg oArg, SelectRoleNewRes oRes)
		{
			bool flag = oRes.result == ErrorCode.ERR_SUCCESS;
			if (flag)
			{
				//发送进如大厅消息
                RpcC2G_DoEnterScene rpcC2G_DoEnterScene = new RpcC2G_DoEnterScene();
                rpcC2G_DoEnterScene.oArg.sceneid = 0;

                XSingleton<XClientNetwork>.singleton.Send(rpcC2G_DoEnterScene);
            }
			else
			{
				//登录出现错误

			}
		}

		// Token: 0x0600D9E8 RID: 55784 RVA: 0x0032BEE4 File Offset: 0x0032A0E4
		public static void OnTimeout(SelectRoleNewArg oArg)
		{
			XSingleton<XDebug>.singleton.AddLog("rpc Select Role Timeout.", null, null, null, null, null, XDebugColor.XDebug_None);
		}
	}
}
