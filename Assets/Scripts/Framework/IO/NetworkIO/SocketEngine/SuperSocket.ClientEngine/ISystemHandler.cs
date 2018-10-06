/*
 * 由SharpDevelop创建。
 * 用户： siena
 * 日期: 2014/3/4
 * 时间: 12:17
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace SuperSocket.ClientEngine
{
	/// <summary>
	/// 实现该接口
	/// </summary>
	public interface ISystemHandler
	{
		// 创建连接
		bool OnConnect(NonBlockingConnection conn);
		
		// 接收数据
		bool OnData(NonBlockingConnection conn);
		
		// 断开连接
		bool OnDisconnect(NonBlockingConnection conn);
		
		// 发生异常
		bool OnException(NonBlockingConnection conn, Exception e);
	}
}
