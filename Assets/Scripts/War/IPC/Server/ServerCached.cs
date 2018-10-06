using System;

namespace AW.War {
	public class ServerCached {
		public static ServerCached Instance {
			get {
				return GenericSingleton<ServerCached>.Instance;
			}
		}

		public ServerInfo curServer;

		public void clear() {
			curServer = null;
		}
	}
}