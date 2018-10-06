using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;


namespace UIEventCenter
{
	public  class EventListener : BaseLua
	{
		public bool isEnable{get;set;}
		protected override void RunBeforeAwake ()
		{
			isLuaController = true;
			isEnable = true;
			//EventSender.onEvent += OnEvent;
			EventSender.Registere(this);
		}

		void OnDestroy()
		{
			EventSender.Remove(this);
			//EventSender.onEvent -= OnEvent;
			base.OnDestroy();
		}
	}
}