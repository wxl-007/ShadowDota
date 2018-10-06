using System;
using AW.Framework;

public class GetUniqueIDFactory  {

	public IGetUniqueID createInstance()  {
		IGetUniqueID login = null;
		#if UNITY_EDITOR
		login = new QuickLogin();
		#else
		login = new QuickLogin();
		#endif
		return login;
	}

}
