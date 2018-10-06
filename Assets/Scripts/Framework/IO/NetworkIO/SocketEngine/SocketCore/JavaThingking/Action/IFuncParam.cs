/*
 * 由SharpDevelop创建。
 * 用户： siena
 * 日期: 2014/3/6
 * 时间: 13:35
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;

namespace xClient.Action
{
	/// <summary>
	/// Description of Interface1.
	/// </summary>
	public interface IFuncParam 
	{
		Dictionary<string, object> ToDictionay();
		
		void FromDictionay(Dictionary<string, object> dic);
		
		IFuncParam clone();

	}
}
