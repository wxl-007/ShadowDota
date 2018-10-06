using UnityEngine;
using System.Collections;
using LuaInterface;
using System.Collections.Generic;
using System.IO;
public class BaseLua : MonoBehaviour {

	protected LuaScriptMgr lua = null;
	protected LuaManager luaManager = null;

	//当前脚本的名字  <---> (将与lua脚本相对应)
	public string LuaName{get;set;}
	//是否是一个LUA控制器(决定了是否向LUA通信)
	protected bool isLuaController = false;

	protected virtual void RunBeforeAwake(){}

	protected Dictionary<string,LuaFunction> Dic_Functions = new Dictionary<string, LuaFunction>();

	//----------------------------------------------------------------------------MonoBehaviour Function-------------------------------------------------------------------------------------------------------
	protected void Awake()
	{
		RunBeforeAwake();
		if(isLuaController)
		{
			InitLua();
			Call("Awake");
		}
	}

	protected void Start()
	{
		if(isLuaController)
		Call("Start");
	}
	
	protected void Update()
	{
//		if(lua != null)
//			lua.Update();

//		if(isLuaController)
//		Call("Update");
	}

	protected void OnEnable()
	{
//		if(isLuaController)
//		Call("OnEnable");
	}

	protected void OnDisable()
	{
		if(isLuaController)
		Call("OnDisable");
	}


	public virtual void OnEvent(string p_e, params object[] p_param)
	{
		if(isLuaController)
		Call("OnEvent",new object[]{p_e,p_param});
	}

	public virtual void OnEvent(string p_e)
	{
		if(isLuaController)
			Call("OnEvent",new object[]{p_e});
	}

	protected void LateUpdate()
	{
//		if(isLuaController)
//			Call("LateUpdate");
	}

	protected void FixedUpdate()
	{
//		if(isLuaController)
//			Call("FixedUpdate");
	}

	protected void Reset()
	{
		if(isLuaController)
			Call("Reset");
	}

	protected void OnMouseEnter()
	{
		if(isLuaController)
			Call("OnMouseEnter");
	}

	protected void OnMouseOver()
	{
		if(isLuaController)
			Call("OnMouseOver");
	}

	protected void OnMouseExit()
	{
		if(isLuaController)
			Call("OnMouseExit");
	}

	protected void OnMouseDown()
	{
		if(isLuaController)
			Call("OnMouseDown");
	}

	protected void OnMouseUp()
	{
		if(isLuaController)
			Call("OnMouseUp");
	}

	protected void OnMouseUpAsButton()
	{
		if(isLuaController)
			Call("OnMouseUpAsButton");
	}

	protected void OnMouseDrag()
	{
		if(isLuaController)
			Call("OnMouseDrag");
	}

	protected void OnTriggerEnter(Collider other)
	{
		if(isLuaController)
			Call("OnTriggerEnter",new object[]{other});
	}

	protected void OnTriggerExit(Collider other)
	{
		if(isLuaController)
			Call("OnTriggerExit",new object[]{other});
	}

	protected void OnTriggerStay(Collider other)
	{
		if(isLuaController)
			Call("OnTriggerStay",new object[]{other});
	}

	protected void OnCollisionEnter(Collision collisionInfo)
	{
		if(isLuaController)
			Call("OnCollisionEnter",new object[]{collisionInfo});
	}

	protected void OnCollisionExit(Collision collisionInfo)
	{
		if(isLuaController)
			Call("OnCollisionExit",new object[]{collisionInfo});
	}

	protected void OnCollisionStay(Collision collisionInfo)
	{
		if(isLuaController)
			Call("OnCollisionStay",new object[]{collisionInfo});
	}

	protected void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(isLuaController)
			Call("OnControllerColliderHit",new object[]{hit});
	}

	protected void OnJointBreak(float breakForce)
	{
		if(isLuaController)
			Call("OnJointBreak",new object[]{breakForce});
	}

	protected void OnParticleCollision(GameObject other)
	{
		if(isLuaController)
			Call("OnParticleCollision",new object[]{other});
	}

	protected void OnBecameVisible()
	{
		if(isLuaController)
			Call("OnBecameVisible");
	}


	protected void OnBecameInvisible()
	{
		if(isLuaController)
			Call("OnBecameInvisible");
	}

	protected void OnLevelWasLoaded(int level)
	{
//		if(isLuaController)
//			Call("OnLevelWasLoaded");
	}

	protected void OnPreCull()
	{
		if(isLuaController)
			Call("OnPreCull");
	}

	protected void OnPreRender()
	{
		if(isLuaController)
			Call("OnPreRender");
	}

	protected void OnPostRender()
	{
		if(isLuaController)
			Call("OnPostRender");
	}
	protected void OnRenderObject()
	{
//		if(isLuaController)
//			Call("OnRenderObject");
	}

	protected void OnWillRenderObject()
	{
		if(isLuaController)
			Call("OnWillRenderObject");
	}

	protected void OnGUI()
	{
//		if(isLuaController)
//			Call("OnGUI");
	}
	
	protected void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if(isLuaController)
			Call("OnRenderImage",new object[]{src,dest});
	}

	protected void OnDrawGizmosSelected()
	{
//		if(isLuaController)
//			Call("OnDrawGizmosSelected");
	}

	protected void OnDrawGizmos()
	{
//		if(isLuaController)
//			Call("OnDrawGizmos");
	}


	protected void OnApplicationPause()
	{
		if(isLuaController)
			Call("OnApplicationPause");
	}

	protected void OnApplicationFocus()
	{
//		if(isLuaController)
//			Call("OnApplicationFocus");
	}

	protected void OnApplicationQuit()
	{
		if(isLuaController)
			Call("OnApplicationQuit");
	}

	protected void OnPlayerConnected(NetworkPlayer player)
	{
		if(isLuaController)
			Call("OnPlayerConnected");
	}

	protected void OnServerInitialized()
	{
		if(isLuaController)
			Call("OnServerInitialized");
	}

	protected void OnConnectedToServer()
	{
		if(isLuaController)
			Call("OnConnectedToServer");
	}

	protected void OnPlayerDisconnected(NetworkPlayer player)
	{
		if(isLuaController)
			Call("OnPlayerDisconnected",new object[]{player});
	}

	protected void OnDisconnectedFromServer(NetworkDisconnection mode)
	{
		if(isLuaController)
			Call("OnDisconnectedFromServer",new object[]{mode});
	}

	protected void OnFailedToConnect(NetworkConnectionError error)
	{
		if(isLuaController)
			Call("OnFailedToConnect",new object[]{error});
	}

	protected void OnFailedToConnectToMasterServer(NetworkConnectionError error)
	{
		if(isLuaController)
			Call("OnFailedToConnectToMasterServer",new object[]{error});
	}
                                            
	protected void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(isLuaController)
			Call("OnMasterServerEvent",new object[]{msEvent});
	}

	protected void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if(isLuaController)
			Call("OnNetworkInstantiate",new object[]{info});
	}

	protected void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if(isLuaController)
			Call("OnNetworkInstantiate",new object[]{stream,info});
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------




	protected void InitLua()
	{
		luaManager = LuaManager.Instance;

		if(string.IsNullOrEmpty(LuaName))
		{
			LuaName = gameObject.name;
			gameObject.name = "Listener<"+LuaName+">";
			gameObject.transform.parent = luaManager.transform.parent;
		}

		lua =  NewLua(LuaName);

		if(luaManager != null) 
			luaManager.AddLua(LuaName,lua);
		InitData();
	}

	private void InitData()
	{
		lua.lua["this"] = this;
		lua.lua["transform"] = transform;
		lua.lua["gameObject"] = gameObject;
		lua.lua["LuaName"] = LuaName;
	}

	private LuaScriptMgr NewLua(params string[] LuaNames)
	{
		LuaScriptMgr lua = new LuaScriptMgr();
		lua.DoFile("define");
		foreach(string luaName in LuaNames)
		{
			//直接读文件二进制码，后续加密在DoFile里面
			lua.DoFile(luaName);
		}
		return lua;
	}

//	private LuaScriptMgr LoadLua(ref LuaScriptMgr lua, string luaPath)
//	{
//		try
//		{
//			if(!System.IO.File.Exists(luaPath))
//			{
//				GUI.color = Color.red;
//				print("Not found:"+luaPath);
//				return lua;
//			}
//			FileStream aFile = new FileStream(luaPath,FileMode.Open);
//			StreamReader sr = new StreamReader(aFile);
//			string luaString = sr.ReadToEnd();	
//			if(!string.IsNullOrEmpty(luaString))
//				lua.DoString(luaString);
//			else
//				print("lua:"+luaPath+" load fail!");
//			sr.Close();
//		}
//		catch (IOException ex)
//		{
//			Debug.LogError(ex.ToString());
//		}
//		return lua;
//	}

	protected object[] Call(string functionName , params object[] args)
	{
		//Debug.LogError(functionName);
		if(lua == null) return null;
//		if( lua.GetLuaFunction(functionName) == null)
//			return null;
		return lua.CallLuaFunction(functionName,args);
	}

	protected void OnDestroy()
	{
		if(isLuaController)
			Call("OnDestroy");
		if(isLuaController)
		{
			if(luaManager != null)
				luaManager.RemoveLua(LuaName);
		}
	}

	protected void print(string str)
	{
		#if UNITY_EDITOR
		Debug.LogError("Not found:"+str);
		#else
		NGUIDebug.Log("Not found:"+str);
        #endif
	}


}
