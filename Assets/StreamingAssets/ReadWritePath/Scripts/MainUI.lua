require 'TitleUI'
Cache = require 'Cache';
Tools = require 'Tools'

local titleUI = TitleUI:GetInstance();
local UIMainModule;
local UIModuleType = LuaTools.GetUIModule();
--local UIMain3DModule;

local GameUI = 
{
   _BottomRoot = nil;
   _TopRoot = nil;
   _Main_3DRoot = nil;
   
}

--边栏3
local Sidebar3 = 
{
     Btn_OpenOrClose = nil;
     DuiWu = nil;
     Setting = nil;
     Lab_Fix = nil;
     List_TweenPostion = {};
     List_TweenScale = {};
     List_TweenAlpha = {}
}

--需要用到的Asset资源
local MainUIAssets = 
{
  Asset_MainUI = nil;
  Asset_TitleUI = nil;
}

local MainModule = 
{
	_MainModule = nil;
}
local MainModule3D = {
  _MainModule3D = nil;
}


--主界面入口函数
local isVisable = false;
function CreateMainUI()
     --注册一个装备系统监听器
     --UIEventManager.Registe("EquipmentUI");

     isVisable = false;
     LuaLinker = GameObject.Find("GameUI"):GetComponent("LuaLinker");
     GameUI._BottomRoot = LuaLinker.Node[0];
     GameUI._TopRoot = LuaLinker.Node[1];
     GameUI._Main_3DRoot = LuaLinker.Node[2];
     LoadNeedResource();
end

function InitTopUI(source)
   --Resources.Load("UI/LS/pbPlayerInfo") 
   TopU = Instantiate(source);
   TopU.transform.parent = GameUI._BottomRoot.transform;
   TopU.transform.localPosition = Vector3.zero;
   TopU.transform.localScale = Vector3.one;   
   UIModule = TopU:GetComponent("UIModule");
   titleUI:InitData(UIModule); 
   
   --主界面没有回退按钮
   titleUI:BtnBackVisible(false);  
end


--初始化主界面
function InitMainUI(source)
   GameMain  =  Instantiate(source);
   GameMain.transform.parent = GameUI._BottomRoot.transform;
   GameMain.transform.localPosition = Vector3.zero;
   GameMain.transform.localScale = Vector3.one;   
   UIMainModule = GameMain:GetComponent("UIModule");
   MainModule._MainModule = UIMainModule;
   Cache:save("MainModule", MainModule);
   InitData(UIMainModule); 
end

--加载需求的资源(主界面资源和头部UI,两块UI编到一起了)     改成   resources load 
function LoadNeedResource()
if DEBUG_LUA then
     --从Cache中取出Asset缓存      
     local UIAsset = Cache:get("MainUIAssets");
     if UIAssetbundles then
        MainUIAssets.Asset_MainUI = UIAsset:Get("Asset_MainUI");
        MainUIAssets.Asset_TitleUI = UIAsset:Get("Asset_TitleUI");
     end

     --是否需要加载主界面Asset    
     if MainUIAssets.Asset_MainUI then
        --直接创建主界面
        local res = MainUIAssets.Asset_MainUI:Load("MainUI",UIModuleType);
        InitMainUI(res.gameobject);

     else
        LuaTools.LoadResource(LuaName,"MainUI","LoadResourceFinished"); 
     end

     --是否需要加载顶部UI界面Asset
     if MainUIAssets.Asset_TitleUI then
        --直接创建顶部UI界面
        InitTopUI(MainUIAssets.Asset_TitleUI:Load("pbPlayerInfo"));
     else
        LuaTools.LoadResource(LuaName,"pbPlayerInfo","LoadResourceFinished"); 
     end
else
    local tMainUI = Resources.Load("UI/MainUI");
      if tMainUI then
        InitMainUI(tMainUI);
      end

    local tTitle = Resources.Load("UI/LS/pbPlayerInfo");
      if tTitle then
        InitTopUI(tTitle);
      end
end
    --加载3D卡牌
    local tMain3d = Resources.Load("UI/LS/pbMainUI3D");
    if tMain3d then
        InitMain3DUI(tMain3d);
    end
    
end

-- 初始化 主3d ui
function InitMain3DUI(source)
  GameMain3DUI = GameObject.Instantiate(source);
  GameMain3DUI.transform.parent = GameUI._Main_3DRoot.transform;
  GameMain3DUI.transform.localPosition = Vector3.zero;
  GameMain3DUI.transform.localScale = Vector3.one;   
  UIMain3DModule = GameMain3DUI:GetComponent("UIModule");
  MainModule3D._MainModule = UIMain3DModule;
  Cache:save("MainModule3D", MainModule3D);
   -- MainModule._MainModule = UIMainModule;
   -- Cache:save("MainModule", MainModule);
   -- InitData(UIMainModule); 
end

--加载需求资源完成
function LoadResourceFinished(assetBundle,loadname)

  if loadname == "MainUI" then
     MainUIAssets.Asset_MainUI = assetBundle;
     local res = assetBundle:Load(loadname,UIModuleType)
     InitMainUI(res.gameobject);

  elseif loadname == "pbPlayerInfo" then
     MainUIAssets.Asset_TitleUI = assetBundle;
     InitTopUI(assetBundle:Load(loadname));
  end

  if MainUIAssets.Asset_MainUI and MainUIAssets.Asset_TitleUI then
    Cache:save("MainUIAssets",MainUIAssets);
  end

end 


--事件接收
function OnEvent(p_event , p_param)
  -- print(tostring(p_event).."    "..tostring(p_param))
  -- if p_event == nil then return end

     -- 如果这个一个顶部UI的事件
     if string.sub( p_event,1,8) == "TitleUI_"  then 
        titleUI:OnEvent(p_event,p_param);
     elseif p_event == "MainUI_OnClick" then
        OnBtnClick(p_param[0]); 
     elseif p_event == "MainUI_Create" then
        CreateMainUI();    
     elseif p_event == "MainUI_SetActive" then
        MainUI_SetActive(p_param[0])
     end    
end


local CanClickNow = true;
function OnBtnClick(btn)

    if not CanClickNow then return end
 
    local btnName = Tools:GetButtonName(btn);

     if btnName == "Btn_OpenOrClose" then   
         isVisable = not isVisable;        
         OpenOrCloseSidebar(isVisable);         
     elseif btnName == "Btn_Pub" then 
      --点击了酒馆
         Debug.Log("Btn_Pub");        
     elseif btnName == "Btn_War" then
      --点击了战役
         On3DUIClick(btnName);
     elseif btnName == "Btn_Battle" then 
      --点击了对战
         On3DUIClick(btnName);     
     elseif btnName == "Btn_Arena" then 
      --点击了竞技场
         Debug.Log("Btn_Arena");
         
     elseif btnName == "Btn_Guild" then 
      --点击了公会 
         Debug.Log("Btn_Guild");
         
      elseif btnName == "Btn_Head0" then 
      --点击了头像0
         Debug.Log("Btn_Head0");
         
      elseif btnName == "Btn_Head1" then 
      --点击了头像1
         Debug.Log("Btn_Head1");
         
      elseif btnName == "Btn_Head2" then 
      --点击了头像2
         Debug.Log("Btn_Head2");
         
      elseif btnName == "Btn_Signin" then 
      --点击了签到
         Debug.Log("Btn_Signin");
         
      elseif btnName == "Btn_Task" then 
      --点击了任务
         Debug.Log("Btn_Task");
         
      elseif btnName == "Btn_Activity" then 
      --点击了活动
         Debug.Log("Btn_Activity");
         
      elseif btnName == "Btn_Holiday" then 
      --点击了节日
         Debug.Log("Btn_Holiday");
         
       elseif btnName == "Btn_Role" then 
      --点击了(角色)英雄
         Debug.Log("Btn_Role");
         
       elseif btnName == "Btn_Equipment" then 
      --点击了装备
         EventSender.SendEvent("EquipmentUI_Create");
         --Debug.Log("Btn_Equipment");
         
       elseif btnName == "Btn_Rune" then 
      --点击了符文
         Debug.Log("Btn_Rune");
         
       elseif btnName == "Btn_Inheritance" then 
      --点击了继承
         Debug.Log("Btn_Inheritance");
         
       elseif btnName == "Btn_Chuangong" then 
      --点击了传功
         Debug.Log("Btn_Chuangong"); 
         
      elseif btnName == "Btn_Setting" then 
      --点击了设置
         Debug.Log("Btn_Setting"); 
         Application.LoadLevel("Login");

      elseif btnName == "Btn_Friend" then 
      --点击了好友
         Debug.Log("Btn_Friend"); 
         
         EventSender.SendEvent("AssetBundleUnLoad", {"Font_SimHei",true} );

      elseif btnName == "Btn_Mail" then 
      --点击了邮件
         Debug.Log("Btn_Mail"); 

      elseif btnName == "OnShowMain3DUI" then
          OnShowMain3DUI(true);
      end
      --Debug.Log(btn.name);
end

function InitData(uiModule)
--第3边侧栏
    Sidebar3.DuiWu = uiModule.List_Element[0].m_ObjectList[0];
    Sidebar3.Setting = uiModule.List_Element[0].m_ObjectList[1];
    Sidebar3.Btn_OpenOrClose = uiModule.List_Element[0].m_ButtonList[0]; 
    local tweenposCount = uiModule.List_Element[0].m_tweenPosition.Count;
    for i = 1,tweenposCount,1 do
      Sidebar3.List_TweenPostion[i] = uiModule.List_Element[0].m_tweenPosition[i-1];
      Sidebar3.List_TweenAlpha[i] = uiModule.List_Element[0].m_tweenAlpha[i-1];
    end

    for i = 1,3,1 do
      Sidebar3.List_TweenScale[i] = uiModule.List_Element[0].m_tweenScale[i-1];
      --Sidebar3.List_TweenAlpha[i] = uiModule.List_Element[0].m_tweenAlpha[i-1];
    end
    
    --Sidebar3.Vertical = uiModule.List_Element[0].m_tweenScale[3];
    Sidebar3.Lab_Fix = uiModule.List_Element[0].m_LabelList[0];

end

function OpenOrCloseSidebar(OpenOrClose)
     if OpenOrClose then
     --展开
		     Sidebar3.Btn_OpenOrClose.normalSprite = "main-041";
      PlaySildbar3Animation(true)
     else
     --合起来
        Sidebar3.Btn_OpenOrClose.normalSprite = "main-018";
      PlaySildbar3Animation(false)
     end

end


--动画播完的回调
function OnSildbar3AnimationFinished(ForwardOrReverse)
  CanClickNow = false
 
  if ForwardOrReverse then
    Sidebar3.Lab_Fix.gameObject:SetActive(false)
  end

  yield.WaitForSeconds(0.6)
  --print("OnSildbar3AnimationFinished.."..tostring(ForwardOrReverse))

  if not ForwardOrReverse then
    Sidebar3.Lab_Fix.gameObject:SetActive(true)
  end


  CanClickNow = true
end


function PlaySildbar3Animation(ForwardOrReverse)

  local delay = 0.06
  local anim_delay = delay + 0.1
  local duration = 0.1

  local teamdelay = 0
  if not ForwardOrReverse then
     anim_delay = delay * 7
     teamdelay = anim_delay --+ duration

  end

  

  for i = 1,3,1 do
    local TweenScale = Sidebar3.List_TweenScale[i];
    --local TweenAlpha = Sidebar3.List_TweenAlpha[i];
    TweenScale.delay = teamdelay;
    TweenScale.duration = 0.15
    --TweenAlpha.delay = teamdelay;

    if ForwardOrReverse then
      TweenScale:PlayForward();
      --TweenAlpha:PlayForward();
    else
      TweenScale:PlayReverse();
      --TweenAlpha:PlayReverse();
    end
  end

  for i = 1,10,1 do

    local TweenPos = Sidebar3.List_TweenPostion[i];
    local TweenAlpha = Sidebar3.List_TweenAlpha[i];
    if i< 8 then    
      TweenPos.delay = anim_delay
      TweenPos.duration = duration
      TweenAlpha.delay = anim_delay
      TweenAlpha.duration = duration
    else
      TweenPos.delay = 0
      TweenPos.duration = 0.2
      TweenAlpha.delay = 0
      TweenAlpha.duration = 0.2
    end

    if ForwardOrReverse then
      TweenPos:PlayForward()
      TweenAlpha:PlayForward()
      anim_delay = anim_delay + delay
    else
      TweenPos:PlayReverse()
      TweenAlpha:PlayReverse()
      anim_delay = anim_delay - delay
    end

  end

  StartCoroutine(OnSildbar3AnimationFinished,ForwardOrReverse);

end



function On3DUIClick(btnName)

  local  tObj = nil;
  local tUIModule = UIMain3DModule:GetComponent("UIModule");
  if tUIModule then
    local tGrid = tUIModule.List_Grid[0];
    if tGrid then
      local tCenter =  tGrid.gameobject:GetComponent("CenterOnChildForMain");
      if tCenter then
         tObj = tCenter.centeredObject;
      end
    end
  end

  if tObj and tObj.name == btnName then

    if btnName == "Btn_War" then
      if 5 >  math.abs(tObj.transform.localPosition.x)  then
        UIMainModule.gameobject:SetActive(false);
        titleUI:BtnBackVisible(true);  
        EventSender.SendEvent("WarUI_Create");
        OnShowMain3DUI(false);
      end
    elseif btnName == "Btn_Battle" then
      EventSender.SendEvent("BattleUI_Create");
    end

  end
end

function OnShowMain3DUI(isShow)
   UIMain3DModule.gameObject:SetActive(isShow);
end

function MainUI_SetActive(value)
   GameMain:SetActive(value);
   GameMain3DUI:SetActive(value);
end













