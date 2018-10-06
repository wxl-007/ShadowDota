using System;

namespace AW.Data {
	/// <summary>
	/// 所有网络过来的数据都应该存储在以IModelNetwork为基类的类中。
	/// </summary>
	public interface IModelNetwork  {

		/// <summary>
		/// 网络消息的回调
		/// </summary>
		void fullfillByNetwork(BaseHttpRequest request, BaseResponse response);

		/// <summary>
		/// 返回自己感兴趣的HTTP消息,只有自己感兴趣的消息类型，才会回调到fullfillByNetwork
		/// </summary>
		RequestType[] getFavoriteRequest();
	}
}

