using System;
using System.Collections.Generic;
using AW.Message;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 同时Condition也在使用这个
	/// </summary>
	public class DelayedSkData {
		public ServerNPC caster;
		public IEnumerable<ServerNPC> chosen;
		public ICastEffect castor;
		public EffectConfigData cfg;
		public Action<MsgParam> Report;
		public RtSkData rtsk;
	}
}
