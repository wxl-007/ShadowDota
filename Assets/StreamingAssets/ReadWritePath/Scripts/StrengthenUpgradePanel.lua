--装备筛选类
module (..., package.seeall)

Cache = require 'Cache'
Tools = require 'Tools'

local StrengthenUpgradePanel={}



function StrengthenUpgradePanel:StrengthenUpgradePanel_Create(_type)

   StrengthenUpgradePanel.type = _type;

   StrengthenUpgrade = Instantiate(Resources.Load("UI/JC/StrengthenUpgradePanel"));
   StrengthenUpgrade.transform.parent = TopRoot.transform;
   StrengthenUpgrade.transform.localPosition = Vector3.zero;
   StrengthenUpgrade.transform.localScale = Vector3.one;
   local UIModule = StrengthenUpgrade:GetComponent("UIModule");

  if _type == 0 then
    --打开装备强化界面
    print("open Strengthen")
    local Strengthen = {}
    Strengthen.gameObject = UIModule.List_Element[0].gameObject;
    Strengthen.Lab_Book = UIModule.List_Element[0].m_LabelList[0];
    Strengthen.Lab_Coin = UIModule.List_Element[0].m_LabelList[1];
    StrengthenUpgradePanel.Strengthen = Strengthen;
    Strengthen.gameObject:SetActive(true);
  elseif _type == 1 then
    --打开装备升级界面
    print("open Upgrade")
    local Upgrade = {}
    Upgrade.gameObject = UIModule.List_Element[1].gameObject;
    Upgrade.Lab_Coin = UIModule.List_Element[1].m_LabelList[0];
    StrengthenUpgradePanel.Upgrade = Upgrade;
    Upgrade.gameObject:SetActive(true);
  end

end


function StrengthenUpgradePanel:OnBtnClick(btn)
   local btnName = Tools:GetButtonName(btn);
   if btnName then
      if btnName == "mask" then
        StrengthenUpgradePanel:OnClose()
      
      elseif btnName == "Btn_Strengthen" then
        print("Btn_Strengthen")

      elseif btnName == "Btn_Upgrade" then
        print("Btn_Upgrade")
        EventSender.SendEvent("EquipPromptBox_Create");
      elseif btnName == "Btn_UpgradeMax" then
        print("Btn_UpgradeMax")

      end      
   end

end


function StrengthenUpgradePanel:OnEvent(p_event , p_param)

    if p_event == "StrengthenUpgradePanel_Create" then  
       StrengthenUpgradePanel:StrengthenUpgradePanel_Create(p_param[0]);
    elseif p_event== "StrengthenUpgradePanel_OnClick" then
       StrengthenUpgradePanel:OnBtnClick(p_param);
    end

end


function StrengthenUpgradePanel:OnClose()
  Destroy(StrengthenUpgrade);
end





return StrengthenUpgradePanel
  

