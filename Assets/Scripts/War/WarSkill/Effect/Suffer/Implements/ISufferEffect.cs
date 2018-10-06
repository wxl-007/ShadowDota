using System;
using System.Collections.Generic;
using AW.Message;

namespace AW.War {

	/// <summary>
	/// 释放器有一个ICastEffect的接口，
	/// 承受器也有一个接口，就是ISufferEffect
	/// </summary>
	public interface ISufferEffect {
		void Suffer (ServerNPC caster, ServerNPC sufferer, SelfDescribed des, WarServerNpcMgr npcMgr);
	}
}
