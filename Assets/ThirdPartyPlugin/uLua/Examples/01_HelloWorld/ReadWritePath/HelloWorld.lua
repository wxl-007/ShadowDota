LuaTools = luanet.import_type("LuaTools");
NGUIDebug = luanet.import_type("NGUIDebug");


require "DemoClass"
require "Tools"

function Awake()
   Debug.Log("Awake: "..this.name);
end

function Start()
	 Debug.Log("Start: "..this.name);	 
	 ShowDemos();	 
end

function Update()
	 --Debug.Log("Update:"..this.name);
end


function ShowDemos()
--面向对象的使用
	 UseDemoClass();	 
--使用一个工具类 
	 UseToosClass();
--使用unity上实现的工具类
	 UseCTools();
	 UseCTools2();
end


--面向对象的使用
 function UseDemoClass()
    demo = DemoClass:new()  
    demo.x = 100;
    demo.y = 200;
	demo:test()  
	print("Call Class DemoClass function test()  "..demo.x.."   "..demo.y)     
end

--使用一个工具类 
 function UseToosClass()
    Tools.Tool1();
end

----使用unity上实现的工具类(invoke)
 function UseCTools()
	 local ff = {};
	 ff.name2 = 10;
	 ff.name1 = "30";
	 ff.name3 = "jcpkwudi";
	 LuaTools.Invoke(LuaName,"InvokeFinished",1,ff);
end

function InvokeFinished(pp,b,k)
    local c = tonumber(b) + pp;
    Debug.Log("1秒后运行: ".. tostring(c) .."   "..tostring(pp).."  "..k );
end



 function UseCTools2()
     LuaTools.LoadResource(LuaName,"bag1","LoadResourceFinished");
     --LuaTools.LoadResource(LuaName,"bag2","LoadResourceFinished");
end

function LoadResourceFinished(assetBundle)
     uiRoot  =  GameObject.Instantiate(assetBundle:Load("UI Root"));
     transform.parent = uiRoot.transform;
     transform.localPosition = Vector3.Zero;
     transform.localScale = Vector3.One;
     Button  =  GameObject.Instantiate(assetBundle:Load("Button"));
     Button.transform.parent = transform;
     Button.transform.localPosition = Vector3.Zero;
     Button.transform.localScale = Vector3.One;
     Button.name = "Btn1";
     buttonMsg = Button:AddComponent("UIButtonMessage");
     buttonMsg.target = gameObject;
     buttonMsg.functionName = "OnBtnClick";
end



function OnBtnClick(button)
     if button.name == "Btn1" then
        NGUIDebug.Log(this.name);
	 end
end










