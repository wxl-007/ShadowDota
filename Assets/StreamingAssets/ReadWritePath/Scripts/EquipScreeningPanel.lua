--装备筛选类
module (..., package.seeall)

Cache = require "Cache"


local EquipScreeningPanel={}

local Callback = nil; 



function EquipScreeningPanel:OnEvent(p_event , p_param)
   if p_event == "EquipScreeningPanel_Create" then
      EquipScreeningPanel:EquipScreeningPanel_Create(p_param)
   elseif p_event == "EquipScreeningPanel_OnClick" then
      EquipScreeningPanel:OnClick(p_param)
   end
end



function EquipScreeningPanel:EquipScreeningPanel_Create(p_param)
  
    Callback = p_param[0];
    EquipScreening = Instantiate(Resources.Load("UI/JC/EquipScreeningPanel"));
    EquipScreening.transform.parent = BottomRoot.transform;
    EquipScreening.transform.localPosition = Vector3.zero;
    EquipScreening.transform.localScale = Vector3.one;

    UIModule = EquipScreening:GetComponent("UIModule");
    local TogglesCount = UIModule.List_Toggle.Count;
    

    List_Toggles = {};
    for i = 1, TogglesCount,1 do 
        List_Toggles[i] = UIModule.List_Toggle[i-1];
    end


    -----之前的选项设置-----
    local Setting = Cache:get("EquipScreeningPanel_Setting");
    local Equip_Equiped_index = 0
    local Equip_Parts_index = 0
    local Equip_Color_index = 0   
    local ToggleIndex = 0;
    if Setting then     

       for i = 1, TogglesCount,1 do 
           UIModule.List_Toggle[i-1].value = false;
       end

       for i = 1, Setting.Equip_Equiped.Count ,1 do
           UIModule.List_Toggle[Setting.Equip_Equiped[i]-1].value = true;
       end

       for i = 1,Setting.Equip_Parts.Count,1 do
            UIModule.List_Toggle[Setting.Equip_Parts[i]-1 + 2 ].value = true;
       end
        
       for i = 1,Setting.Equip_Color.Count,1 do
            UIModule.List_Toggle[Setting.Equip_Color[i]-1 + 7].value = true;
       end
    end
    ---------------------
end




local isEquiped = false;

local Equip_Part = 
{
   Weapon = 0;
   Helmet = 1;
   Armor = 2;
   Shoes = 3;
   Decoration = 4;
}

local Equip_Color = 
{
  white = 0,
  green = 1,
  blue = 2,
  purple = 3,
  golden = 4,
  orange = 5,
}


function EquipScreeningPanel.Close()

    local Equip_Equiped = {}
    local Equip_Parts = {}
    local Equip_Color = {}

    local Equip_Equiped_index = 0
    local Equip_Parts_index = 0
    local Equip_Color_index = 0   

    local TogglesCount = UIModule.List_Toggle.Count;
    for i = 1, TogglesCount,1 do 

      local Toggle = UIModule.List_Toggle[i - 1];
      if Toggle.value then
        if i < 3 then
          Equip_Equiped_index = Equip_Equiped_index + 1;
          Equip_Equiped[Equip_Equiped_index] = i;
        elseif i < 8 then
           Equip_Parts_index = Equip_Parts_index + 1;
           Equip_Parts[Equip_Parts_index] = i - 2;
        else
          Equip_Color_index = Equip_Color_index + 1;
          Equip_Color[Equip_Color_index] = i - 7;
        end
      end
    end


    --回调面板操作结果(筛选条件)
    local conditions = {}
    conditions.Equip_Equiped = Equip_Equiped;
    conditions.Equip_Parts = Equip_Parts;
    conditions.Equip_Color = Equip_Color;

    

    if EquipScreeningPanel:isOperationChange(conditions) then
      Callback(conditions);
      --保存用户操作
      Cache:save("EquipScreeningPanel_Setting", conditions);
    end
       
    --删除界面
    Destroy(EquipScreening);
end

function EquipScreeningPanel:OnClick(btn)
    local btnName = Tools:GetButtonName(btn);
    if btnName == "Btn_Sure" then
        LuaTools.Invoke(EquipScreeningPanel.Close,0.5);
    end
end


--用户操作是否发生改变
function EquipScreeningPanel:isOperationChange(conditions)

    local Setting = Cache:get("EquipScreeningPanel_Setting");
    local Count = table.getn(conditions.Equip_Equiped) + table.getn(conditions.Equip_Parts) + table.getn(conditions.Equip_Color);
    if not Setting then     
       if UIModule.List_Toggle.Count == Count then
          return false
       else
          return true
       end
    end
       
    if Setting.Equip_Equiped.Count ~= table.getn(conditions.Equip_Equiped) then
       return true
    end

    if Setting.Equip_Parts.Count ~= table.getn(conditions.Equip_Parts) then
       return true
    end

    if Setting.Equip_Color.Count ~= table.getn(conditions.Equip_Color) then
       return true
    end

    for i = 1, Setting.Equip_Equiped.Count, 1 do 
      if Setting.Equip_Equiped[i] ~= conditions.Equip_Equiped[i] then
        return true
      end
    end

    for i = 1, Setting.Equip_Equiped.Count, 1 do 
      if Setting.Equip_Parts[i] ~= conditions.Equip_Parts[i] then
        return true
      end
    end

    for i = 1, Setting.Equip_Color.Count, 1 do 
      if Setting.Equip_Color[i] ~= conditions.Equip_Color[i] then
        return true
      end
    end

    return false;

end




return EquipScreeningPanel
  

