using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UIEventCenter
{
	//说明因为toLua无法包装代理方式，这里换写法实现代理
	public class EventSender 
	{
//		public delegate void OnEvent(string p_event,  params object[] p_param);
//		public static OnEvent onEvent;

//		public delegate void OnEventNull(string p_event,  params object[] p_param);
//		public static OnEvent onEventNull;
				
		public static List<BaseLua> listeners = new List<BaseLua>();

		public static void SendEvent(string p_event, params object[] p_param)
	    {
			foreach(BaseLua listener in listeners )
			{
				listener.OnEvent(p_event,p_param);
			}
	    }

		public static void SendEvent(string p_event)
		{
			foreach(BaseLua listener in listeners )
			{
				listener.OnEvent(p_event);
			}
		}
			
			

		public static bool Registere(BaseLua listener)
		{
			bool result = listeners.Contains(listener);
			if(result) return result;
			listeners.Add(listener);
			return result;
		}

		public static bool Remove(BaseLua listener)
		{
			return listeners.Remove(listener);
		}



	}
}
