--luanet.load_assembly("Assembly-CSharp")

--NGUITools = luanet.import_type("NGUITools");


---宏定义---
DEBUG_LUA = LuaManager.DEBUG_LUA

--基础类型定义--
Debug			= UnityEngine.Debug;
Object		    = UnityEngine.Object;
GameObject		= UnityEngine.GameObject;
Transfrom		= UnityEngine.Transfrom;
Vector2			= UnityEngine.Vector2;
Vector3			= UnityEngine.Vector3;
Time			= UnityEngine.Time;
GUI				= UnityEngine.GUI;
Rect			= UnityEngine.Rect;
Resources		= UnityEngine.Resources;
Font			= UnityEngine.Font;
Application     = UnityEngine.Application;
Instantiate     = GameObject.Instantiate;
Destroy         = GameObject.Destroy;
DontDestroyOnLoad = GameObject.DontDestroyOnLoad;

-- --消息中心--
--EventSender = luanet.UIEventCenter.EventSender;
EventSender = UIEventCenter.EventSender;
UIEventManager = UIEventCenter.UIEventManager;



--暂时反射
--LuaTools = luanet.LuaTools



typeof = function (_Object)
	local TypeArray = System.Type.GetTypeArray({_Object});
    if TypeArray ~= nil and TypeArray.Length > 0 then
    	return  tostring( TypeArray[0]:ToString() );
    end
    return "nil";
end



function StartCoroutine (_func,...)
     local co = coroutine.create(_func)
     coroutine.resume(co,...) 
end




