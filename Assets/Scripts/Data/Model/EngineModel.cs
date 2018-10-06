using AW.Data;
using System.Collections.Generic;

namespace AW.Data {

	/// <summary>
	/// Engine model. 所有数据层的东西都必须在AW.Data下面
	/// </summary>
	[Modle(type = DataSource.FromNetwork)]
	public class EngineModel : IModelNetwork {

		public List<Server> allServer = null;
		public Server ChosenServer;

		/// 
		/// token 一共有两种，一个是快速登陆另一个是第三方平台登陆
		/// 
		public string token;
		public string platToken;
		public string platId;

		public EngineModel() {
			allServer = new List<Server>();
			ChosenServer = null;
		}

		public void fullfillByNetwork (BaseHttpRequest request, BaseResponse response) {
			ConsoleEx.DebugLog("Engine Model received http response successfully");

			GetPartitionServerResponse parSer = response as GetPartitionServerResponse;
			if(parSer != null) {
				PartitionServer data = parSer.data;
				if(data != null && data.sv != null && data.sv.Length > 0) {
					allServer.AddRange(parSer.data.sv);

					foreach(Server s in allServer) {
						if(s.sid == data.last) {
							ChosenServer = s;
							break;
						}
					}
				}

				if(data != null) {
					token     = data.token;
					platToken = data.platToken;
					platId    = data.platId;
				}

			}

		}

		public RequestType[] getFavoriteRequest() {
			RequestType[] favoriate = new RequestType[] {
				RequestType.THIRD_GET_SERVER,
			};
			return favoriate;
		}
	
	}
}

