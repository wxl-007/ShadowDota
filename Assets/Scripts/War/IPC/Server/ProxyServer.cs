using System;
using System.Collections.Generic;

namespace AW.War {

	using NetMQ;

	public class ProxyServer : IServer {
		/// <summary>
		/// 加入战斗
		/// </summary>
		/// <param name="ClientID">Client ID</param>
		public void Join(string ClientInfo) {
			NetMQMessage msg = new NetMQMessage();
			msg.Append(WarMsgConsts.JOINReq);
			msg.Append(ClientInfo);
			reqCli.send(msg);
		}

		/// <summary>
		/// 战斗准备
		/// </summary>
		/// <param name="info">Info.</param>
		public void Ready(string info) {
			NetMQMessage msg = new NetMQMessage();
			msg.Append(WarMsgConsts.ReadyReq);
			msg.Append(info);
			reqCli.send(msg);
		}

		/// <summary>
		/// 取消战斗准备
		/// </summary>
		/// <param name="info">Info.</param>
		public void NotReady(string info) {
			NetMQMessage msg = new NetMQMessage();
			msg.Append(WarMsgConsts.NotReadyReq);
			msg.Append(info);
			reqCli.send(msg);
		}

		/// <summary>
		/// UI准备好了
		/// </summary>
		/// <param name="info">Info.</param>
		public void UIReady(string info) {
			NetMQMessage msg = new NetMQMessage();
			msg.Append(WarMsgConsts.UIReadyReq);
			msg.Append(info);
			reqCli.send(msg);
		}

		/// <summary>
		/// 退出战斗, ClientInfo 必须和Join的一致
		/// </summary>
		/// <param name="ClientInfo">Client info.</param>
		public void Quit(string ClientInfo) {
			NetMQMessage msg = new NetMQMessage();
			msg.Append(WarMsgConsts.QuitReq);
			msg.Append(ClientInfo);
			reqCli.send(msg);
		}

		/// <summary>
		/// 释放技能
		/// </summary>
		/// <param name="pos">Position.</param>
		public void CastSkill(string CastInfo) {
			NetMQMessage msg = new NetMQMessage();
			msg.Append(WarMsgConsts.CastSkReq);
			msg.Append(CastInfo);
			reqCli.send(msg);
		}

        /// <summary>
        /// 普通攻击
        /// </summary>
        /// <param name="AttackInfo">Attack info.</param>
        public void Attack(string AttackInfo)
        {
            NetMQMessage msg = new NetMQMessage();
            msg.Append(WarMsgConsts.AttackReq);
            msg.Append(AttackInfo);
            reqCli.send(msg);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="MoveInfo">Move info.</param>
        public void Move(string MoveInfo)
		{
			NetMQMessage msg = new NetMQMessage ();
			msg.Append (WarMsgConsts.MoveReq);
            msg.Append (MoveInfo);
			reqCli.send (msg);
		}

        /// <summary>
        /// 移动结束
        /// </summary>
        public void MoveStop(string MoveStopInfo)
        {
            NetMQMessage msg = new NetMQMessage ();
            msg.Append (WarMsgConsts.MoveStopReq);
            msg.Append (MoveStopInfo);
            reqCli.send (msg);
        }

		/// <summary>
		/// 切换激活状态的英雄
		/// </summary>
		/// <param name="active">Active.</param>
		public void Switch(string activeInfo) {
			NetMQMessage msg = new NetMQMessage ();
			msg.Append (WarMsgConsts.SwitchReq);
			msg.Append (activeInfo);
			reqCli.send (msg);
		}

		/// <summary>
		/// 切换手动和自动
		/// </summary>
		/// <param name="autoInfo">Auto info.</param>
		public void ManualAuto(string autoInfo) {
			NetMQMessage msg = new NetMQMessage ();
			msg.Append (WarMsgConsts.ManualOrAutoReq);
			msg.Append (autoInfo);
			reqCli.send (msg);
		}

		/// <summary>
		/// 返回成功或者失败的 回调
		/// </summary>
		private Dictionary<string, Action<string>> FromRep;
		public readonly RequestClient reqCli = null;

		public ProxyServer(WarInfo war, Action connected = null) {
			reqCli = new RequestClient(war, HandleMQMsg, connected);
			FromRep = new Dictionary<string, Action<string>>();
		}

		public void registerRep(string Rep, Action<string> back) {
			FromRep[Rep] = back;
		}

		void HandleMQMsg(NetMQMessage msg) {
			if(msg != null) {
				//回来的消息，有命令和数据

				string cmd = msg[0].ConvertToString();
				string arg = cmd;
				if(msg.FrameCount == 2)
					arg = msg[1].ConvertToString();
				FromRep[cmd](arg);
			}
		}

		public void Quit() {
			if(reqCli != null) {
				reqCli.Quit();
			}
		}

	}
}