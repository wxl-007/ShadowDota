using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
//默认的Mono函数调用封装在LuaBase,继承以后可以在此写自定义的通信方式
public class HelloWorld : BaseLua 
{


	void Start()
	{
		base.Start();
		//NGUIDebug.Log( DeviceInfo.PersistRootPath );
	}
}
