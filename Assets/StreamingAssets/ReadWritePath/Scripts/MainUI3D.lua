LuaTools   = luanet.import_type("LuaTools");
NGUIDebug  = luanet.import_type("NGUIDebug");
UnityUtils = luanet.import_type("UnityUtils");
Core = luanet.import_type("Core");
SceneName = luanet.import_type("SceneName");


require 'MainUI';
Cache = require 'Cache';

local UIMainModule;
local UIModuleType = LuaTools.GetUIModule();


function OnEvent(p_event , p_param)
	-- Debug.Log(p_event);
	-- Debug.Log(p_param);
-- 如果这个一个顶部UI的事件
     -- if string.sub( p_event,1,8) == "MainUI3D_"  then 
     --    titleUI:OnEvent(p_event,p_param);
-- 如果是主界面的按钮点击事件
   if p_event == "MainUI3D_OnClick" then
           OnBtnClick(p_param[0]);        
    end    
end


function  OnBtnClick( btn )
 	if btn	~=  nil then
 		if btn.name ==  "Btn_War" then
 			EventSender.SendEvent("MainUI_OnClick","Btn_War" );
 		elseif btn.name == "Btn_Battle" then
 			EventSender.SendEvent("MainUI_OnClick","Btn_Battle" );
 		elseif btn.name == "Btn_Pub"	then
 			Debug.Log("Btn_Pub");
		elseif btn.name == "Btn_Arena" then
			Debug.Log("Btn_Arena");
		elseif btn.name == "Btn_Guild" then
			Debug.Log("Btn_Guild");
		end
	end
end
-- local TitleUI = require 'TitleUI';
-- require 'MainUI';
-- Cache = require 'Cache';

-- local GameUI;
-- local UIGrid;
-- local WarUI = nil;
-- local UIModuleArry = {};
-- local UICellChoose = nil;

-- local CellClass = 
-- {
-- 	Title_lab = nil;
-- 	Title_Circle = nil;
-- 	Title_Sprite = nil;
-- 	index = 0;
-- }

-- function CreatWarUI()
-- 	GameUI = GameObject.Find("GameUI"):GetComponent("LuaLinker");
-- 	WarUI = GameObject.Instantiate(Resources.Load("UI/LS/pbWarUI"));
-- 	WarUI.transform.parent = GameUI.Node[0].transform;
--     WarUI.transform.localPosition = Vector3.zero;
--     WarUI.transform.localScale = Vector3.one;
-- 	if WarUI ~= nil then
-- 		local UIModule = WarUI:GetComponent("UIModule");
-- 	    InitUI(UIModule);
-- 	    Createcell(UIModule);
--     end
--     	TitleUI:BtnBackCallback("WarUI_Back");
-- end

-- function Start()

	   
-- end

-- function InitUI(module)
	
-- end

-- function WarBackBtn()
-- 	local MainModule = Cache:get("MainModule")
-- 	MainModule._MainModule.gameobject:SetActive(true);
-- 	TitleUI:BtnBackVisible(false);
-- 	WarUI.gameObject:SetActive(false);
-- 	GameObject.Destroy(WarUI.gameObject);
-- end

-- function Createcell(module)
-- 	UIGrid = module.List_Grid[0];
-- 	for n=0, 5 do
-- 		WarUICell = GameObject.Instantiate(Resources.Load("UI/LS/pbWarCell"));
-- 		WarUICell.gameObject.name = WarUICell.gameObject.name..n;
		
-- 		WarUICell.gameObject.name = "pbWarCell"..n;
-- 		WarUICell.transform.parent = UIGrid.gameObject.transform;
--     	WarUICell.transform.localPosition = Vector3.zero;
--     	WarUICell.transform.localScale = Vector3.one;
    	
--     	UIModuleElement = WarUICell:GetComponent("UIModuleElement");
		
-- 		UIModuleElement.m_LabelList[0].gameObject.name  = UIModuleElement.m_LabelList[0].gameObject.name..n;
-- 		UIModuleElement.m_ObjectList[0].gameObject.name = UIModuleElement.m_ObjectList[0].gameObject.name..n;
-- 		UIModuleElement.m_SpriteList[0].gameObject.name = UIModuleElement.m_SpriteList[0].gameObject.name..n;
		
-- 		CellClass.Title_lab = UIModuleElement.m_LabelList[0];
-- 		CellClass.Title_Circle = UIModuleElement.m_ObjectList[0];
-- 		CellClass.Title_Sprite = UIModuleElement.m_SpriteList[0];
-- 		CellClass.index = n;
		
-- 		UIModuleArry[n] = CellClass;
-- 		InitCellUI(UIModuleArry[n], n);
-- 	end
-- 	UICellChoose = UIModuleArry[1];
-- 	UIGrid:Reposition();
-- end

-- function InitCellUI(CellClass, n)
-- 	if n~=0 then
-- 		CellClass.Title_Circle:SetActive(false);
-- 		CellClass.Title_Sprite.color = UnityEngine.Color.black;
-- 	else
-- 		CellClass.Title_Sprite.color = UnityEngine.Color.white;
-- 	end
-- end

-- function ChooseCell(_index)
-- 	if UICellChoose ~= nil then
-- 		UICellChoose.Title_Circle:SetActive(false);
-- 	end
-- 	UICellChoose = UIModuleArry[_index];
-- 	UICellChoose.Title_Circle:SetActive(true);
-- end

-- function OnEvent(p_event, p_param)
-- 	if p_event == "WarUI_Create" then
-- 		CreatWarUI();
-- 	elseif p_event == "WarUI_OnClick" then
-- 		Click(p_param);
-- 	elseif p_event == "WarUI_Back" then
-- 		WarBackBtn();
-- 	end
-- end

-- function Click(btn)
-- 	if btn.name == "WarUI_Begin_Btn" then
-- 		UnityUtils.JumpToScene(Core.GameFSM, SceneName.BattleScene);
-- 	else
-- 		mystring = btn.name;
-- 		start = string.find(mystring, "pbWarCell");
-- 		if start ~= nil then
-- 			_index = string.sub(mystring, 9);
-- 			ChooseCell(_index);
-- 		end
-- 	end	
-- end
