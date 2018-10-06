using System;

namespace AW.War {
	/// <summary>
	/// 承受效果
	/// </summary>
	public static class EffectSufferShared  {
		private static EffectSelector efSelector = null;
		//shared in suffer
		public static EffectSelector get(WarServerNpcMgr npcMgr) {
			if(efSelector == null) efSelector = new EffectSelector(npcMgr);
			return efSelector;
		}
	}
}
