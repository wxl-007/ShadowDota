/*
 * 由SharpDevelop创建。
 * 用户： siena
 * 日期: 2014/3/4
 * 时间: 18:28
 * 
 * 协议定义
 */
using System;
using fastJSON;
using System.Collections.Generic;
using SuperSocket.ClientEngine;

namespace xClient.Action
{
	public class Protocol {
		private static readonly String AppVersion = "1.0";
		
		// 协议头
		// /////////////////////
		// AppVersion
		public String v{get; set;}
	
		// crc
		public String crc{get;set;}
	
		// rv[]
		public int[] rv;
	
		// rpc的时候使用
		public String no {get;set; }
	
		// upf
		public String upf { get;set;}
	
		// act
		public int act{get;set;}
	
		// 协议正文 （JSON格式表示）
		// /////////////////////
		public Dictionary<string, object> data{ get;set; }
		
		public Protocol()
		{
			this.v = AppVersion;
			data = new Dictionary<string, object>();
		}
		
		public static Protocol Ctreat(int actId, IFuncParam param)
		{
			Protocol p = new Protocol();
			p.act = actId;
			p.data = param.ToDictionay();
			return p;
		}
		
		public override string ToString()
		{
			return "The ActId = " + act + ", The Data Size = " + data.Count;
		}
		
	}
}
