--装备筛选类
module (..., package.seeall)

Cache = require 'Cache'
Tools = require 'Tools'

local EquipPromptBox={}



function EquipPromptBox:EquipPromptBox_Create(str_content)

   str_content = "是否花费[ffff00]10000[-]|升级该装备？|1级   27级"

print(str_content);

   Box = Instantiate(Resources.Load("UI/JC/EquipPromptBox"));
   Box.transform.parent = TopRoot.transform;
   Box.transform.localPosition = Vector3.zero;
   Box.transform.localScale = Vector3.one;
   local UIModule = Box:GetComponent("UIModule");

end


function EquipPromptBox:OnBtnClick(btn)
   local btnName = Tools:GetButtonName(btn);
   if btnName then
      if btnName == "Btn_Cancel" then
         print("Btn_Cancel")

      elseif btnName == "Btn_Sure" then
        print("Btn_Sure")

      end      
   end

end


function EquipPromptBox:OnEvent(p_event , p_param)

print(p_event)
    -- if p_event == "EquipPromptBox_Create" then  
    --   print("EquipPromptBox_Create")
    --    --EquipPromptBox:EquipPromptBox_Create(p_param[0]);
    -- elseif p_event== "EquipPromptBox_OnClick" then
    --    EquipPromptBox:OnBtnClick(p_param);
    -- end

end


function EquipPromptBox:OnClose()
  Destroy(Box);
end





return EquipPromptBox
  

