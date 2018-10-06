--装备筛选类
module (..., package.seeall)

Cache = require "Cache"


local BattleHallUI={}

function BattleHallUI:OnEvent(p_event , p_param)

    if p_event == "BattleHallUI_OnClick" then
       OnBtnClick(p_param);
    elseif p_event == "BattleHallUI_Create" then
        print("p_event="..tostring(p_event))
       BattleHallUI:BattleHallUI_Create()
    end

end



function BattleHallUI:BattleHallUI_Create()
    print("BattleHallUI:BattleHallUI_Create")
    --隐藏主界面
    EventSender.SendEvent("BattleUI_SetActive",false);
    --注册一个退出回调
    Tools:Registered_TittleUICallBack(BattleHallUI.BattleHallUI_Back)


    BattleHall = Instantiate(Resources.Load("UI/JC/BattleHallUI"));
    local LuaLinker = GameObject.Find("GameUI"):GetComponent("LuaLinker");
    local BottomRoot = LuaLinker.Node[0];

    BattleHall.transform.parent = BottomRoot.transform;
    BattleHall.transform.localPosition = Vector3.zero;
    BattleHall.transform.localScale = Vector3.one;
end


function BattleHallUI:BattleHallUI_Back()
    --显示对战主界面
    EventSender.SendEvent("BattleUI_SetActive",true);
    --销毁界面
    if BattleHall then
       Destroy(BattleHall)
    end
end




function BattleHallUI:OnBtnClick(btn)

end




return BattleHallUI
  

