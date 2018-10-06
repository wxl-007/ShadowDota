using UnityEngine;
using System.Collections;
using LuaInterface;
using AW.Message;
using AW.Entity;
public class Jumper : MonoBehaviour {

	private static Jumper _instance = null;
	private LuaManager luaManager = null;
	public static Jumper _this
	{
		get
		{
			if(_instance == null)
			{
				GameObject o = new GameObject();
				o.name = "Jumper";
				_instance = o.AddComponent<Jumper>();
				o.transform.parent = LuaManager.Instance.transform;
				_instance.luaManager = LuaManager.Instance;
			}
			return _instance;
		}
	}
	
	class JumperData
	{
		public string levelName;
		public LuaFunction  func_complete;
		public LuaFunction func_progress;
	}
	static JumperData jumperData;
	
	public static void LoadSceneAsyncWithLoading(string levelName,LuaFunction  func_complete,LuaFunction func_progress = null)
	{
		if(jumperData == null) jumperData = new JumperData();
		jumperData.levelName = levelName;
		jumperData.func_complete = func_complete;
		jumperData.func_progress = func_progress;
		SendEnterWarMsg();
	}

	public static void  SendEnterWarMsg()
	{
		WarStartParam param = new WarStartParam();
		Core.EntityMgr.sendMessage(LogicalType.Anonymous, LogicalType.War, param, true, MsgRecType.MakeSure);
	}
	
	//进入战斗场景数据初始化完成
	public static void EnterWarDataInitFinished()
	{
		Debug.Log("EnterWarDataInitFinished");
		//开始跳转场景
		if(jumperData != null)
			_this.StartCoroutine(_this.loadScene(jumperData.levelName,jumperData.func_complete,jumperData.func_progress));
		jumperData = null;
	}
	
	IEnumerator loadScene(string levelName,LuaFunction  func_complete,LuaFunction func_progress = null)
	{
		AsyncOperation operation=Application.LoadLevelAsync (levelName);
		//AsyncOperation operation=Application.LoadLevelAdditiveAsync (levelName);
		while (!operation.isDone) 
		{
			if(func_progress != null)
				func_progress.Call(new object[]{operation.progress});
			//yield return new WaitForEndOfFrame();
			yield return 0;
		}
		
		if(func_progress != null)
			func_progress.Call(new object[]{1f});
		operation = null;
		
		//yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(0.2f);
		func_complete.Call();
	}





}
