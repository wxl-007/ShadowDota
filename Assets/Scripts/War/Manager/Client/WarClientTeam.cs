using System;
using System.Collections.Generic;
using fastJSON;
using AW.Data;

namespace AW.War {
	/// <summary>
	/// 存活于Client端的，当前队伍信息
	/// </summary>
	public class WarClientTeam {
		//Key is the index of hero
		private Dictionary<int, ClientNPC> team;

		private RealClient realClient;

		public ClientNPC activeNpc {
			private set;
			get;
		}

		/// <summary>
		/// 自动还是手动
		/// </summary>
		/// <value><c>true</c> if is auto; otherwise, <c>false</c>.</value>
		public bool isAuto {
			private set;
			get;
		}


		public WarClientTeam(RealClient rc) {
			team = new Dictionary<int, ClientNPC>();
			realClient = rc;
		}

		public void filterNpc(CrtHero hero, ClientNPC npc) {
			if(hero.ClientID == DeviceInfo.GUID) {
				team[hero.index] = npc;

				if(hero.index == 0) activeNpc = npc;
			}
		}

		/// <summary>
		/// 切换激活状态的英雄
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="SwitchOK">Switch O.</param>
		public bool SwitchActive(int index) {
			bool validate = false;

			bool found = team.ContainsKey(index);
			if(found) {
				int wishtoID = team[index].UniqueID;
				CAMP cp = team[index].Camp;

				bool same = wishtoID == activeNpc.UniqueID;
				validate  = same ? false : true;
				if(validate) {
					WarCamp wcamp = WarCamp2Camp.toWarCamp(cp);

					SwitchInfo si = new SwitchInfo(){
						UniqueID = wishtoID,
						ClientID = DeviceInfo.GUID,
						camp     = wcamp,
					};
					string plaintext = JSON.Instance.ToJSON(si);
					realClient.proxyServer.Switch(plaintext);
				}
			}

			return validate;
		}

		/// <summary>
		/// 切换自动和手动
		/// </summary>
		/// <returns><c>true</c>, if auto or manual was switched, <c>false</c> otherwise.</returns>
		public void SwitchAutoOrManual(bool bAuto) {
			if(activeNpc != null) {

				CAMP cp = activeNpc.Camp;
				WarCamp wcamp = WarCamp2Camp.toWarCamp(cp);

				ManualOrAuto ma = new ManualOrAuto() {
					UniqueID = activeNpc.UniqueID,
					ClientID = DeviceInfo.GUID,
					camp     = wcamp,
				}; 

                ma.auto = (short)(bAuto == true ? 1 : 0);
				string plaintext = JSON.Instance.ToJSON(ma);
				realClient.proxyServer.ManualAuto(plaintext);
			}
		}

		//Index is zero-based
		public ClientNPC get(int index) {
			ClientNPC npc = null;
			bool found = team.TryGetValue(index, out npc);
			return found ? npc : null;
		}

        public void SwitchActiveHeroSuc(ClientNPC npc)
        {
            bool found = team.ContainsValue(npc);
            if(found)
            {
                activeNpc = npc;
            }
        }

		public void SwitchManualOrAutoSuc(bool auto) {
			isAuto = auto;
		}
	}
}