using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
//监听器注册器
namespace UIEventCenter
{
	public class UIEventManager
	{
		public static Dictionary<string,EventListener> listeners = new Dictionary<string, EventListener>(); 

		public static void Registe(string listenerName)
	    {
			if(listeners.ContainsKey(listenerName)) 
				return;
			GameObject o = new GameObject();
			o.name = listenerName;
			EventListener listener = o.AddComponent<EventListener>();
			if(listener != null)
			{
				listeners.Add(listenerName,listener);
			}
	    }

		//设置监听器是否可用
		public static void SetEnable(string listenerName, bool enable)
		{
			if(listeners.ContainsKey(listenerName))
				listeners[listenerName].isEnable = enable;
		}

		public static void RemoveListener(string listenerName)
		{
			if(listeners.ContainsKey(listenerName))
			{
				EventListener listener = listeners[listenerName];
				listeners.Remove(listenerName);
				GameObject.Destroy(listener.gameObject);
			}
		}

	 }
}