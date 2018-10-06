LuaTools = luanet.import_type("LuaTools");
Tools = require 'Tools'

function Start()
  Debug.Log("Login Start");
end


function CreateLoginUI()
   --Debug.LogError( UnityEngine.Application.loadedLevelName );
   LuaLinker = GameObject.Find("UI Root"):GetComponent("LuaLinker");
   Debug.Log("开始创建LoginUI...");
   InitLoginUI();
end

--初始化登陆界面
function InitLoginUI()
   local node = LuaLinker.Node[0];
    loginUI = Instantiate(Resources.Load("UnPack/Login/LoginUI"));
    loginUI.transform.parent = node.transform;  
    loginUI.transform.localPosition = Vector3.zero;
    loginUI.transform.localScale = Vector3.one;   
    loginUI.name = "LoginUI";
end



--事件接收
function OnEvent(p_event , p_param)
	if p_event == "LoginUI_CreateLoginUI" then
		--创建LoginUI界面
		CreateLoginUI();

	elseif p_event == "LoginUI_OnClick" then
	     OnBtnClick(p_param[0]);
	end
end



function OnBtnClick(btn)
	if btn.name == "Btn_EnterWar" then   
    --进入战斗场景
       --Application.LoadLevel("War");
       --LuaTools._this:EnterWar();
       Tools:LoadLevelUseLoading("War");
    elseif btn.name == "Btn_EnterUI" then
    --进入主场景
       Application.LoadLevel("GameUI");
    end
end






