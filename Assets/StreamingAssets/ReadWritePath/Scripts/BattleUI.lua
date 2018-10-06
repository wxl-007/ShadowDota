Tools = require 'Tools'
BattleHallUI = require 'BattleHallUI'

function OnEvent(p_event , p_param)
    if string.sub( p_event,1,13) == "BattleHallUI_" then 
       BattleHallUI:OnEvent(p_event,p_param)
       return
    end  

    if string.sub( p_event,1,9) ~= "BattleUI_" then     
       return
    end  

    if p_event == "BattleUI_OnClick" then
       OnBtnClick(p_param);
    elseif p_event == "BattleUI_Create" then
       CreateBattleUI()
    elseif p_event == "BattleUI_SetActive" then
       SetActive(p_param[0])
    end
end


--创建装备界面
function CreateBattleUI()

    --隐藏主界面
    EventSender.SendEvent("MainUI_SetActive",false);
    --显示Title回退按钮
    EventSender.SendEvent("TitleUI_BtnBackVisable",true);
    --注册一个退出回调(EquipmentUI_Back)
    Tools:Registered_TittleUICallBack(BattleUI_Back)


    BattleUIPanel = Instantiate(Resources.Load("UI/JC/BattleUIPanel"));
    local LuaLinker = GameObject.Find("GameUI"):GetComponent("LuaLinker");
    local BottomRoot = LuaLinker.Node[0];

    BattleUIPanel.transform.parent = BottomRoot.transform;
    BattleUIPanel.transform.localPosition = Vector3.zero;
    BattleUIPanel.transform.localScale = Vector3.one;

end


function OnBtnClick(btn)
    local btnName = Tools:GetButtonName(btn);
     if btnName == "Btn_War" then
        print("Btn_War")
     elseif btnName == "Btn_Solo" then
        print("Btn_Solo")
     elseif btnName == "Btn_Rune" then
        print("Btn_Rune")
     elseif btnName == "Btn_Wifi" then
        EventSender.SendEvent("BattleHallUI_Create");
     end
end



function SetActive(value)
   BattleUIPanel:SetActive(value)
end


function BattleUI_Back()
    --显示主界面
    EventSender.SendEvent("MainUI_SetActive",true);
    --隐藏Title回退按钮
    EventSender.SendEvent("TitleUI_BtnBackVisable",false);
    --销毁界面

    if BattleUIPanel then
       Destroy(BattleUIPanel)
    end
end
