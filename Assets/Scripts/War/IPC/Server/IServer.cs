using System;

namespace AW.War {
	public interface IServer {
		/// <summary>
		/// 加入战斗
		/// </summary>
		/// <param name="ClientID">Client ID</param>
		void Join(string ClientInfo);

		/// <summary>
		/// 战斗准备
		/// </summary>
		/// <param name="info">Info.</param>
		void Ready(string info);

		/// <summary>
		/// 取消战斗准备
		/// </summary>
		/// <param name="info">Info.</param>
		void NotReady(string info);

		/// <summary>
		/// 退出战斗
		/// </summary>
		/// <param name="ClientInfo">Client info.</param>
		void Quit(string ClientInfo);

		/// <summary>
		/// 释放技能
		/// </summary>
		/// <param name="pos">Position.</param>
		void CastSkill(string CastInfo);
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="MoveInfo">Move info.</param>
        void Move(string MoveInfo);
        /// <summary>
        /// 移动结束
        /// </summary>
        /// <param name="MoveStopInfo">Move stop info.</param>
        void MoveStop(string MoveStopInfo);

		/// <summary>
		/// 切换英雄
		/// </summary>
		/// <param name="SwitchInfo">Switch info.</param>
		void Switch(string SwitchInfo);
		/// <summary>
		/// 切换自动和手动
		/// </summary>
		/// <param name="autoInfo">Auto info.</param>
		void ManualAuto(string autoInfo);
	}
}
