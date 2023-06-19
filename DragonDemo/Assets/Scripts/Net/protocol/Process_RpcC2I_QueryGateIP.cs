using System;
using System.Diagnostics;
using KKSG;
using Koakuma.Game;
using Koakuma.Game.ECS;
using TGame.ECS;
using UnityEngine;
//using XMainClient.UI;
using XUtliPoolLib;

namespace XMainClient
{
    // Token: 0x0200106F RID: 4207
    internal class Process_RpcC2I_QueryGateIP
    {
        // Token: 0x0600D674 RID: 54900 RVA: 0x0032618C File Offset: 0x0032438C
        public static void OnReply(QueryGateArg oArg, QueryGateRes oRes)
        {

            UnityEngine.Debug.Log("收到消息  处理" + Convert.ToBase64String( oRes.loginToken));

            
            bool flag = XSingleton<XClientNetwork>.singleton.OnAuthorized(oArg, oRes);
            if (flag)
            {
                MessageType.EnterServer enterServer = new MessageType.EnterServer();
                enterServer.res = oRes;
      
                GameManager.Message.Post<MessageType.EnterServer>(enterServer).Coroutine();

                //xsingleton<xlogindocument>.singleton.setannouncement(ores.notice);
                //xsingleton<xlogindocument>.singleton.onauthorized(ores.userphone);
                //xsingleton<xlogindocument>.singleton.setgateiptable(ores.servers, ores.gateconfig, ores.allservers);
                //xsingleton<xlogindocument>.singleton.setfriendserverlist(ores.platfriendservers);
                //xsingleton<xlogindocument>.singleton.setloginzoneid(ores.loginzoneid);
                //xsingleton<xlogindocument>.singleton.setfreeflow(ores.freeflow, ores.cctype);
            }
            else
            {
                UnityEngine.Debug.Log("授权失败");
                //xsingleton<xlogindocument>.singleton.onauthorizedfailed();
                //bool flag2 = ores.error == errorcode.err_plat_banacc;
                //if (flag2)
                //{
                //    string format = ores.baninfo.reason + "\n" + xsingleton<uiutility>.singleton.replacereturn(xstringdefineproxy.getstring(ores.error.tostring()));
                //    xsingleton<uiutility>.singleton.showmodaldialog(string.format(format, xsingleton<uiutility>.singleton.timeformatsince1970((int)ores.baninfo.endtime, xstringdefineproxy.getstring("idip_tips_time"), true)), xstringdefineproxy.getstring("common_ok"), null, 300);
                //}
            }
        }

        // Token: 0x0600D675 RID: 54901 RVA: 0x000FEEFC File Offset: 0x000FD0FC
        public static void OnTimeout(QueryGateArg oArg)
        {

        }
    }
}
