using System;

namespace AW.War {
	/// <summary>
	/// 存活于客户端的，监控其他客户端的情况
	/// 
	/// 目的：在进入战斗场景之前，了解所有客户端信息
	/// 
	/// 被动的接受服务器端推送过来的信息
	/// </summary>
	public class SimpleMonitorClient : BaseMonitor {

		public void PullClientInfo(VirCli[] syncInfo) {
			if(syncInfo != null) {

				int len = syncInfo.Length;
				if(len <= 0) return;

				for(int i = 0; i < len; ++ i) {
					VirCli sync = syncInfo[i];

					VirtualClient client = null;
					ClientStatus status = (ClientStatus) Enum.ToObject(typeof(ClientStatus), sync.curStatus);

					bool found = ClientPool.TryGetValue(sync.ClientID, out client);
					if(found) {
						client.curStatus = status;
					} else {
						ClientPool[sync.ClientID] = new VirtualClient() {
							ClientID  = sync.ClientID,
							curStatus = status,
						};
					}
				}

			}
		}
	}
}