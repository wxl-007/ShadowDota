--类的写法
local moduleName = ...

local Cache = require 'Cache';
--定义一个类(表)
local TitleUI = {
	UI = {
    this = nil;
    UIModule = nil;
    Lab_Energy = nil;
    Lab_Power = nil;
    Lab_Coin = nil;
    Lab_Stone = nil;
    Lab_VipLevel = nil;
    Btn_Back = nil;
    Functionname = nil;
    }
}  

--这句是重定义元表的索引，必须要有
TitleUI.__index = TitleUI  
_G[moduleName] = TitleUI
package.loaded[moduleName] = TitleUI
  

--模拟构造体，一般名称为new()  
function TitleUI:new()  
        local self = {}     
        setmetatable(self, TitleUI)   --必须要有  
        return self    
end  
  
  
 function TitleUI.GetInstance()
     if TitleUI.this == nil then
     TitleUI.this = TitleUI.new();
     end
     return TitleUI.this;
 end
 

function TitleUI:InitData(uiModule)

     TitleUI.UI.UIModule = uiModule;
     TitleUI.UI.Lab_Energy = uiModule.List_Label[0];
     TitleUI.UI.Lab_Power = uiModule.List_Label[1];
	 TitleUI.UI.Lab_Coin = uiModule.List_Label[2];
	 TitleUI.UI.Lab_Stone = uiModule.List_Label[3];
	 TitleUI.UI.Lab_VipLevel = uiModule.List_Label[4];
	 TitleUI.UI.Btn_Back = uiModule.List_Button[0];
	 Cache:save("TitleUI", TitleUI.UI);
end



function TitleUI:OnEvent(p_event , p_param)
       if p_event == "TitleUI_OnClick" then
            TitleUI:OnBtnClick(p_param);    
       elseif p_event == "TitleUI_Refresh" then
            TitleUI:Refresh();
       elseif p_event == "TitleUI_BtnBackVisable" then
            TitleUI:BtnBackVisible(p_param[0]);
       end
end

--TitleUI上的按钮被点击了
function TitleUI:OnBtnClick(btn)
    
    local btnName = btn[0]
    if type(btn[0]) ~= 'string' then
        if typeof(btn[0]) == "UnityEngine.GameObject" then
          btnName = btn[0].name
        else
          --第一个参数即不是GameObject也不是String
          return;
        end
    end

     if(btnName == "Add_Energy") then
        Debug.Log("Add_Energy");  

     elseif btnName == "Add_Power" then
        Debug.Log("Add_Power");    

     elseif btnName == "Add_Coin" then
        Debug.Log("Add_Coin");    

     elseif btnName == "Add_Stone" then    
        Debug.Log("Add_Stone");   
        EventSender.SendEvent("TitleUI_OnClick", "Add_Coin" );

     elseif btnName == "Btn_Back" then
     	 local BackDelegate = Cache:getList("TitleUIBackDelegate")

     	 if BackDelegate then
             TitleUI:DropAndCallBackLastPage(BackDelegate)
         end
     end
end



function TitleUI:DropAndCallBackLastPage(List_Delegate)
    local lastIndex = List_Delegate.Count - 1;
    List_Delegate:CallFunction(lastIndex);
    List_Delegate:RemoveAt(lastIndex);
end


function TitleUI:BtnBackVisible(isVisible)
	local TitleUI = Cache:get("TitleUI");

    if TitleUI then
       TitleUI.Btn_Back.gameObject:SetActive(isVisible);
    end
end


--刷新界面
function TitleUI:Refresh()
    TitleUI.Lab_Energy.text = "a";
    TitleUI.Lab_Power.text = "b";
    TitleUI.Lab_Coin.text = "c";
    TitleUI.Lab_Stone.text = "d";
    TitleUI.Lab_VipLevel.text = "e";
end




