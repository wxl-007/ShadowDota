--LuaTools   = luanet.import_type("LuaTools");
--NGUIDebug  = luanet.import_type("NGUIDebug");
--UnityUtils = luanet.import_type("UnityUtils");
--Core = luanet.import_type("Core");
--SceneName = luanet.import_type("SceneName");

local TitleUI = require 'TitleUI';
require 'MainUI';
Cache = require 'Cache';

local GameUI;
local UIGrid;
local WarUI;
local WarUIModule = 
{
	_WarUIModule = nil;
}
local UIModuleArry = {};
local UICellChoose = nil;

local CellClass = 
{
	Title_lab = nil;
	Title_Circle = nil;
	Title_Sprite = nil;
	index = 0;
}



function CreatWarUI()
	GameUI = GameObject.Find("GameUI"):GetComponent("LuaLinker");
	WarUI = GameObject.Instantiate(Resources.Load("UI/LS/pbWarUI"));
	WarUI.transform.parent = GameUI.Node[0].transform;
    WarUI.transform.localPosition = Vector3.zero;
    WarUI.transform.localScale = Vector3.one;
    WarUIModule._WarUIModule = WarUI:GetComponent("UIModule");
    Cache:save("WarUIModule", WarUIModule);
	if WarUI ~= nil then
		local UIModule = WarUI:GetComponent("UIModule");
	    InitUI(UIModule);
	    Createcell(UIModule);
    end

    --注册一个退出回调(EquipmentUI_Back)
    Tools:Registered_TittleUICallBack(WarUI_Back)

end









function Start()

	   
end


function InitUI(module)
	
end


function WarUI_Back()
	local MainModule = Cache:get("MainModule")
	MainModule._MainModule.gameobject:SetActive(true);
	TitleUI:BtnBackVisible(false);
	WarUI.gameObject:SetActive(false);
	GameObject.Destroy(WarUI.gameObject);
	EventSender.SendEvent("MainUI_OnClick","OnShowMain3DUI" );
end

function Createcell(module)
	UIGrid = module.List_Grid[0];
	for n=0, 2 do
		WarUICell = GameObject.Instantiate(Resources.Load("UI/LS/pbWarCell"));
		WarUICell.gameObject.name = WarUICell.gameObject.name..n;
		
		WarUICell.gameObject.name = "pbWarCell"..n;
		WarUICell.transform.parent = UIGrid.gameObject.transform;
    	WarUICell.transform.localPosition = Vector3.zero;
    	WarUICell.transform.localScale = Vector3.one;
    	
    	UIModuleElement = WarUICell:GetComponent("UIModuleElement");
		
		UIModuleElement.m_LabelList[0].gameObject.name  = UIModuleElement.m_LabelList[0].gameObject.name..n;
		UIModuleElement.m_ObjectList[0].gameObject.name = UIModuleElement.m_ObjectList[0].gameObject.name..n;
		UIModuleElement.m_SpriteList[0].gameObject.name = UIModuleElement.m_SpriteList[0].gameObject.name..n;
		
		CellClass.Title_lab = UIModuleElement.m_LabelList[0];
		CellClass.Title_Circle = UIModuleElement.m_ObjectList[0];
		CellClass.Title_Sprite = UIModuleElement.m_SpriteList[0];
		CellClass.Title_Sprite.spriteName = "Chapter00"..(n+1);
		CellClass.index = n;
		if n==0 then
			CellClass.Title_lab.text = "启程";
		elseif n==1 then
			CellClass.Title_lab.text = "偶遇";
		elseif n==2 then
			CellClass.Title_lab.text = "盟友";
		end
		UIModuleArry[n] = CellClass;
		InitCellUI(UIModuleArry[n], n);
	end
	UICellChoose = UIModuleArry[1];
	UIGrid:Reposition();
end

function InitCellUI(CellClass, n)
	if n~=0 then
		CellClass.Title_Circle:SetActive(false);
		CellClass.Title_Sprite.color = UnityEngine.Color.black;
	else
		CellClass.Title_Sprite.color = UnityEngine.Color.white;
	end
end

function ChooseCell(_index)
	if UICellChoose ~= nil then
		UICellChoose.Title_Circle:SetActive(false);
	end
	UICellChoose = UIModuleArry[_index];
	UICellChoose.Title_Circle:SetActive(true);
end

function OnEvent(p_event, p_param)
	if p_event == "WarUI_Create" then
		CreatWarUI();
	elseif p_event == "WarUI_OnClick" then
		EventSender.SendEvent("WarSecond_Create");
		WarUI.gameObject:SetActive(false);
	end
end

function Click(btn)
	if btn[0].name == "WarUI_Begin_Btn" then
		--Application.LoadLevel("War");


		--Application.LoadLevel("Loading");
		EventSender.SendEvent("LoadingSceneUI_CreateXXXXX");

        --print("------------");

	else
		mystring = btn[0].name;
		start = string.find(mystring, "pbWarCell");
		if start ~= nil then
			_index = string.sub(mystring, 9);
			ChooseCell(_index);
		end
	end	
end
