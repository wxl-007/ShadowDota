--LuaTools   = luanet.import_type("LuaTools");
--NGUIDebug  = luanet.import_type("NGUIDebug");
--UnityUtils = luanet.import_type("UnityUtils");
--Core = luanet.import_type("Core");
--SceneName = luanet.import_type("SceneName");

local TitleUI = require 'TitleUI';
--require 'MainUI';
Cache = require 'Cache';
Tools = require 'Tools'



local GameUI;
local UIGrid;
local WarSecond = nil;
local UIModuleWarArry = {};
local UICellChoose = nil;

local WarSecondCellClass = 
{
	Title_Bg = nil;
	Title_Sprite = nil;
	Title_Circle = nil;
	Title_Lock = nil;
	Title_Star1 = nil;
	Title_Star2 = nil;
	Title_Star3 = nil;
	Title_Name = nil;
	index = 0;
}

function CreatWarUI()
	GameUI = GameObject.Find("GameUI"):GetComponent("LuaLinker");
	WarSecond = GameObject.Instantiate(Resources.Load("UI/LS/pbWarSecond"));
	WarSecond.transform.parent = GameUI.Node[0].transform;
    WarSecond.transform.localPosition = Vector3.zero;
    WarSecond.transform.localScale = Vector3.one;
	if WarSecond ~= nil then
		local UIModule = WarSecond:GetComponent("UIModule");
	    Createcell(UIModule);
    end

    --注册一个退出回调(EquipmentUI_Back)
    Tools:Registered_TittleUICallBack(WarSecondUI_Back)

end

function OnEvent(p_event, p_param)
	if p_event == "WarSecond_Create" then
		CreatWarUI();
	elseif p_event == "WarSecond_OnClick" then
		Click(p_param);
	end
end

function Click(btn)
	if btn[0].name == "WarSecond_Btn3" then
		Tools:LoadLevelUseLoading("War");
		--Application.LoadLevel("Loading");
		--EventSender.SendEvent("TitleUI_BtnBackCallback","WarSecond_Back");
	end	
end

function Createcell(module)
	UITable = module.List_Object[0]:GetComponent("UITable");
	for n=1, 3 do
		WarSecondCell = GameObject.Instantiate(Resources.Load("UI/LS/pbWarSecondCell"));
		WarSecondCell.gameObject.name = WarSecondCell.gameObject.name..n;
		
		WarSecondCell.gameObject.name = "pbWarCell"..n;
		WarSecondCell.transform.parent = UITable.gameObject.transform;
    	WarSecondCell.transform.localPosition = Vector3.zero;
    	WarSecondCell.transform.localScale = Vector3.one;
    	
    	UIModuleElement = WarSecondCell:GetComponent("UIModuleElement");
		
		WarSecondCellClass.Title_Bg = UIModuleElement.m_SpriteList[0];
		WarSecondCellClass.Title_Sprite = UIModuleElement.m_SpriteList[1];
		WarSecondCellClass.Title_Circle = UIModuleElement.m_SpriteList[2];
		WarSecondCellClass.Title_Lock = UIModuleElement.m_SpriteList[3];
		WarSecondCellClass.Title_Star1 = UIModuleElement.m_SpriteList[4];
		WarSecondCellClass.Title_Star2 = UIModuleElement.m_SpriteList[5];
		WarSecondCellClass.Title_Star3 = UIModuleElement.m_SpriteList[6];
		WarSecondCellClass.Title_Name = UIModuleElement.m_LabelList[0];
		WarSecondCellClass.Title_Name.text = "关卡"..n;
		WarSecondCellClass.Title_Sprite.spriteName = "Checkpoint00"..n;
		WarSecondCellClass.index = n;
		
		UIModuleWarArry[n] = WarSecondCellClass;
		InitCellUI(UIModuleWarArry[n], n);
	end
	UICellChoose = UIModuleWarArry[1];
	UITable:Reposition();
end

function InitCellUI(CellClass, n)
	--Debug.Log("n="..n);
	if n~=1 then
		CellClass.Title_Star1.color = UnityEngine.Color.black;
		CellClass.Title_Star2.color = UnityEngine.Color.black;
		CellClass.Title_Star3.color = UnityEngine.Color.black;
		CellClass.Title_Circle.gameObject:SetActive(false);
		CellClass.Title_Sprite.color = UnityEngine.Color.black;
		CellClass.Title_Bg.color = UnityEngine.Color.black;
		CellClass.Title_Lock.gameObject:SetActive(false);

	else
		CellClass.Title_Star1.color = UnityEngine.Color.white;
		CellClass.Title_Star2.color = UnityEngine.Color.white;
		CellClass.Title_Star3.color = UnityEngine.Color.white;
		CellClass.Title_Circle.gameObject:SetActive(true);
		CellClass.Title_Sprite.color = UnityEngine.Color.white;
		CellClass.Title_Bg.color = UnityEngine.Color.white;
		CellClass.Title_Lock.gameObject:SetActive(false);

	end
end

function WarSecondUI_Back()
	local WarUIModule = Cache:get("WarUIModule")
	WarUIModule._WarUIModule.gameobject:SetActive(true);
	TitleUI:BtnBackVisible(true);
	EventSender.SendEvent("TitleUI_BtnBackCallback","WarUI_Back");
	WarSecond.gameObject:SetActive(false);
	GameObject.Destroy(WarSecond.gameObject);
end
