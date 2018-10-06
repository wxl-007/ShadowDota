/*
 * 由SharpDevelop创建。
 * 用户： siena
 * 日期: 2014/3/4
 * 时间: 18:11
 * 
 * 函数派发器
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using SuperSocket.ClientEngine;
using fastJSON;
using xClient.Common;

namespace xClient.Action
{
	/// <summary>
	/// Description of Dispatch.
	/// </summary>
	public class Dispatch
	{
		//key is action ID.
		private static Dictionary<int, ActionBean> actions = new Dictionary<int, ActionBean>();
		
		[ThreadStatic]
        private static Dispatch _instance;

		public static Dispatch Instance {
            get { return _instance ?? (_instance = new Dispatch()); }
        }
        
		private Dispatch()
		{
			// 加载所有的Action
			try{
				AllActClass aac = new AllActClass();
				foreach(IAction act in aac.dic.Values)
				{
					MethodInfo[] ms = act.GetType().GetMethods();
					foreach(MethodInfo m in ms)
					{
						ActionBean bean = ActionBean.newInstance(act, m);
						if((bean != null) && (!actions.ContainsKey(bean.actId))) {
							actions.Add(bean.actId, bean);
						}
					}
				}
			} catch(Exception e){
				ConsoleEx.DebugLog(e.ToString());
			}
			
			// 初始化线程池
			ThreadPool.LoadConfig();
		}
		
		public void Init(){}
		
		public void DispatchAct(Protocol protocol, NonBlockingConnection conn) {
			ActionBean bean = null;

			if(actions.TryGetValue(protocol.act, out bean)) {
				// 分配一个线程执行
				Task.Dispatch(conn, bean, protocol);
			}
		}
		
		// 执行线程
	    internal sealed class Task : Runnable
	    {
	        private ActionBean bean;
	        private Protocol protocol;
			private NonBlockingConnection conn;
			
	        private Task(int priority, ActionBean bean, NonBlockingConnection conn, Protocol protocol) : base(priority)
	        {
	            this.bean = bean;
	            this.conn = conn;
	            this.protocol = protocol;
	        }
	
	        public override void run()
	        {
	            try
	            {
	            	// TODO 可以做一些权限上的限制
					bean.invoke(protocol, conn);                
				} catch (Exception e) {
	                ConsoleEx.DebugLog(e.StackTrace);
	                ConsoleEx.DebugLog(e.Message);
	            }
	        }
	
			internal static void Dispatch (NonBlockingConnection conn, ActionBean bean, Protocol protocol)
	        {
	        	int priority = 1;
	            Task task = new Task(priority, bean, conn, protocol);
	            ThreadPool.AddTask(task);
	        }
	    }		
		
		// 协议和Act映射类
		internal class ActionBean
		{
			public int actId { get; private set; }
			private MethodInfo mi;
			private IAction action;
			
			// 通过反射存储的一个参数格式
			private IFuncParam param;
			
			public ActionBean(IAction action, MethodInfo mi)
			{
				this.action = action;
				this.mi = mi;
				this.actId = getActionId();
				param = getFuncParam();
			}
			
			public void invoke(Protocol proto, NonBlockingConnection conn)
			{
				if(proto.act != this.actId) return;
				if(proto.data == null) return;
				
				ConsoleEx.DebugLog("Receive Protocol :: " + proto.ToString());
				
				IFuncParam newParam = getParamObje(proto.data);
				try{
					mi.Invoke(action, new Object[]{ newParam, conn });
				} catch(Exception e) {
					ConsoleEx.DebugLog(e.ToString());
				}
			}
			
			private int getActionId()
			{
				string mName = mi.Name;
				string[] ma = mName.Split(Convert.ToChar("_"));
				if(ma.Length != 3)
				{
					return -1;
				}
				return Convert.ToInt32(ma[2]);
			}

			// create a new IFuncParam
			private IFuncParam getFuncParam()
			{
				ParameterInfo[] pi = mi.GetParameters();
				if(pi.Length < 1)
				{
					return null;
				}
				//
				Type pt = pi[0].ParameterType;
				ConstructorInfo ci = pt.GetConstructor(Type.EmptyTypes);
				object o = ci.Invoke(Type.EmptyTypes);
				return (IFuncParam)o;
			}

			//fullfill the new IFuncParam
			public IFuncParam getParamObje(Dictionary<string, Object> dic)
			{
				IFuncParam newOne = param.clone();
				newOne.FromDictionay(dic);
				return newOne;
			}
			
			public static ActionBean newInstance(IAction act, MethodInfo m){
				string name = m.Name;
				if(!name.StartsWith("_"))
				{
					return null;
				}
				return new ActionBean(act, m);
			}
		}
	}
}
