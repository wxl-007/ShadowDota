--存放所有的装备
Tools = require 'Tools'
EquipScreeningPanel = require 'EquipScreeningPanel'
StrengthenUpgradePanel = require 'StrengthenUpgradePanel'

--创件的装备物件(<装备数量,滚屏)
List_EquipmentItem = {}
--装备信息数据列表
List_EquipmentItemData = {}
--显示装备数据列表
List_ShowEquip = {}



--最近一次选中的Item
lastSelectedItem = nil
lastSelectedItem_DataIndex = -1
--滚屏
UIWrapContent = nil;


ShowRightPanel = 
{
   Stars = nil;
   Lab_Name = nil;
   Lab_Lv = nil;
   Lab_price = nil;
}


function OnEvent(p_event , p_param)
  
    if string.sub( p_event,1,20) == "EquipScreeningPanel_" then
       EquipScreeningPanel:OnEvent(p_event,p_param)
    elseif string.sub( p_event,1,23) == "StrengthenUpgradePanel_" then
       StrengthenUpgradePanel:OnEvent(p_event,p_param)   
    elseif string.sub( p_event,1,15) == "EquipPromptBox_" then
      EquipPromptBox:OnEvent(p_event,p_param)
    end




    if string.sub( p_event,1,12) ~= "EquipmentUI_" then
       return
    end  


    if p_event == "EquipmentUI_OnClick" then
       OnBtnClick(p_param);
    elseif p_event == "EquipmentUI_EquipmentItem_OnClick" then
       OnEquipmentItemBtnClick(p_param)
    elseif p_event == "EquipmentUI_Create" then
       CreateEquipmentUI()
    elseif p_event == "EquipmentUI_Back" then
       EquipmentUI_Back()
    end

end

--创建装备界面
function CreateEquipmentUI()
    InitEquipmentUI()
end


function InitEquipmentUI()
    --隐藏主界面
    EventSender.SendEvent("MainUI_SetActive",false);
    --显示Title回退按钮
    EventSender.SendEvent("TitleUI_BtnBackVisable",true);
    --注册一个退出回调(EquipmentUI_Back)
    Tools:Registered_TittleUICallBack(EquipmentUI_Back)

    --创建装备界面
    LoadingSceneUI = Instantiate(Resources.Load("UI/JC/EquipmentUI"));
    local LuaLinker = GameObject.Find("GameUI"):GetComponent("LuaLinker");
    BottomRoot = LuaLinker.Node[0];
    TopRoot = LuaLinker.Node[1];
    LoadingSceneUI.transform.parent = BottomRoot.transform;
    LoadingSceneUI.transform.localPosition = Vector3.zero;
    LoadingSceneUI.transform.localScale = Vector3.one;

    local UIModule = LoadingSceneUI:GetComponent("UIModule");
    EquipPanel_Grid = UIModule.List_Object[0];


    ShowRightPanel.Stars = UIModule.List_Object[1]:GetComponent("UIStars");
    ShowRightPanel.Lab_Name = UIModule.List_Label[0];
    ShowRightPanel.Lab_Lv = UIModule.List_Label[1];
    ShowRightPanel.Lab_price = UIModule.List_Label[2];
    ShowRightPanel.Spr_Equipment = UIModule.List_Sprite[0];

    UIWrapContent = EquipPanel_Grid:GetComponent("UIWrapContent");

    --初始化EquipmentItem数据
    InitEquipmentItemData(EquipPanel_Grid);
    --创建EquipmentItem
    CreateEquipmentItem(EquipPanel_Grid);
  
end


function OnInitializeItem(go,wrapIndex,realIndex)
    --print("go="..tostring(go).." wrapIndex="..tostring(wrapIndex).." realIndex="..tostring(realIndex))

    --Lua下标从1开始--
    realIndex = System.Math.Abs(realIndex)+1;
    wrapIndex = wrapIndex+1;
    -----------------

    local ItemName = "Item"..tostring(wrapIndex);
    
    local Item = List_EquipmentItem[ItemName];
    
    local ItemData = List_ShowEquip[realIndex];


    --print("wrapIndex="..tostring(wrapIndex).."   realIndex="..tostring(realIndex))
    --print(tostring(Item).."   "..tostring(ItemData));
    

    if Item and ItemData then
      Item.DataIndex = System.Math.Abs(realIndex);
      ShowSingleItem(Item,ItemData);
    end

end


--显示单个装备Item
function ShowSingleItem(Item,ItemData)       
        --Item.gameObject.transform.localScale = Vector3.one;
    if ItemData then 
      Item.gameObject:SetActive(true);
      Item.Lab_Name.text = ItemData.EquipName;

      Item.Lab_Lv.text = "等级: "..tostring(ItemData.Lv);

      Item.UIStars:SetStarts(ItemData.Star);

      Item.Spr_selected.enabled = Item.DataIndex == lastSelectedItem_DataIndex;
      Item.Spr_Equipment.spriteName = ItemData.EquipIcon;
    else
      Item.gameObject:SetActive(false);
    end
end



--EquipmentItem数据初始化(假数据)
function InitEquipmentItemData(Gird)

    List_EquipmentItemData = {}
    
    
    --假设有15件装备
    local EquipCount = 100;

    UIWrapContent.minIndex = -EquipCount+1;
    UIWrapContent.maxIndex = 0;
    
    for i=1,EquipCount,1 do
       local ItemData = {}

       -----装备属性------
       ItemData.Lv = i;
       ItemData.Star = i % 6;
       ItemData.Price = 300+i;
       ItemData.EquipName = "Equip_"..tostring(i);
       ItemData.Part = i % 7 + 1;
       ItemData.Color = i % 5 + 1;
       ItemData.isEquip = i%2 + 1;
       ------------------


       if i-1 >99 then
        ItemData.EquipIcon = "zhuangbei_0"..tostring(i-1);
       elseif i-1 > 9 then
        ItemData.EquipIcon = "zhuangbei_00"..tostring(i-1);      
       else
        ItemData.EquipIcon = "zhuangbei_000"..tostring(i-1);
       end

       List_EquipmentItemData[i] = ItemData;
    end

    --显示第一个装备的详细属性
    RrefreshRightPanel(List_EquipmentItemData[1]);



    List_ShowEquip = List_EquipmentItemData;
end



function CreateEquipmentItem(Gird)
    lastSelectedItem = nil;
    lastSelectedItem_DataIndex = -1;
    List_EquipmentItem = {}

    for i= 1,8,1 do 

       local Item_Object = Instantiate(Resources.Load("UI/JC/EquipmentItem"));
        Item_Object.transform.parent = Gird.transform;
        Item_Object.transform.localPosition = Vector3.zero;
        Item_Object.transform.localScale = Vector3.one;
        Item_Object.name = "Item"..tostring(i);

        local EquipmentItem = Item_Object:GetComponent("UIModule");
        local Item = {}
        Item.Lab_Name = EquipmentItem.List_Label[0];
        Item.Lab_Lv = EquipmentItem.List_Label[1];
        Item.UIStars = EquipmentItem.List_Object[0]:GetComponent("UIStars");
        Item.EquipHero = EquipmentItem.List_Object[1];
        Item.Spr_hero = EquipmentItem.List_Sprite[0];
        Item.Spr_selected = EquipmentItem.List_Sprite[1];
        Item.Spr_Equipment = EquipmentItem.List_Sprite[2];
        Item.DataIndex = i;
        Item.gameObject = Item_Object;
  
        List_EquipmentItem[Item_Object.name] = Item;
    end

    UIWrapContent:AddLuaDelegate(OnInitializeItem);
end




function OnEquipmentItemBtnClick(btn)

   local btnName = Tools:GetButtonName(btn);
   if btnName then
     local Item = List_EquipmentItem[btnName]
     if Item.Spr_selected.enabled then return end
     if Item then
        --选中当前装备
        Item.Spr_selected.enabled = true
        --取消之前选中的装备

        if lastSelectedItem and lastSelectedItem ~= Item then
           lastSelectedItem.Spr_selected.enabled = false
        end
        lastSelectedItem = Item;
        lastSelectedItem_DataIndex = Item.DataIndex;
        
        RrefreshRightPanel(List_ShowEquip[lastSelectedItem_DataIndex]);
     end
   end

end


--刷新右边的详细显示区
function RrefreshRightPanel(ItemData)
  if ItemData then
    ShowRightPanel.Lab_Name.text = ItemData.EquipName;
    ShowRightPanel.Lab_Lv.text = "等级："..tostring(ItemData.Lv);
    ShowRightPanel.Stars:SetStarts(ItemData.Star)
    ShowRightPanel.Lab_price.text = tostring(ItemData.Price);
    ShowRightPanel.Spr_Equipment.spriteName = ItemData.EquipIcon;
  end
end


function OnBtnClick(btn)
   local btnName = Tools:GetButtonName(btn);
   if btnName then
      if btnName == "Btn_Strengthening" then
         EventSender.SendEvent("StrengthenUpgradePanel_Create",0); 
      elseif btnName == "Btn_Upgrade" then
         --print("Btn_Upgrade");
         EventSender.SendEvent("StrengthenUpgradePanel_Create",1); 
      elseif btnName == "Btn_Sell" then
         SellEquipment(lastSelectedItem_DataIndex);
         --print("Btn_Sell");
      elseif btnName == "Btn_Screening" then
        --筛选
        --print("Btn_Screening");
        EventSender.SendEvent("EquipScreeningPanel_Create",EquipScreeningCallBack);        
        
      end

   end
end




--筛选结果回调
function EquipScreeningCallBack(conditions)
  -- Tools:printTable(conditions.Equip_Equiped)
  -- Tools:printTable(conditions.Equip_Parts)
  -- Tools:printTable(conditions.Equip_Color)

  --重置显示数据(显示数据为筛选结果)
  List_ShowEquip = ScreeningEquipment(conditions);

  --重置装备显示列表位置
  ResetEquipPanel();
  RefreshLeftPanel();
end

--重置装备显示列表
function ResetEquipPanel()
  UIWrapContent:SortAlphabetically()
  local UIPanel = EquipPanel_Grid.transform.parent:GetComponent("UIPanel")
  local pos = UIPanel.transform.localPosition;
  pos.y = 0;
  UIPanel.transform.localPosition = pos;
  local offset = UIPanel.clipOffset;
  offset.y = 0;
  UIPanel.clipOffset = offset;         
end



--筛选装备
function ScreeningEquipment(conditions)

  local index = 0;
  local List_ScreeningEquipment = {}
  for key,value in ipairs(List_EquipmentItemData) do

     if Tools:isTableContain(value.isEquip,conditions.Equip_Equiped) or Tools:isTableContain(value.Part,conditions.Equip_Parts) or Tools:isTableContain(value.Color,conditions.Equip_Color) then
        index = index + 1;
        List_ScreeningEquipment[index] = value;
     end
  end

  return List_ScreeningEquipment;
end



--出售装备
function SellEquipment(EquipmentIndex)
    
    table.remove (List_ShowEquip,EquipmentIndex)
    -- for key, value in ipairs(List_EquipmentItemData) do      
    --    print(tostring(key).."    "..tostring(value.EquipName))
    -- end  
  
    local EquipCount = table.getn(List_ShowEquip);
    --print("EquipCount ="..tostring(EquipCount));

    UIWrapContent.minIndex = -EquipCount+1;
    UIWrapContent.maxIndex = 0;


    -- lastSelectedItem = nil;
    -- lastSelectedItem_DataIndex = -1;
    RefreshLeftPanel()

end


--刷新装备栏
function RefreshLeftPanel()

    local showEquipCount = table.getn(List_ShowEquip);
    UIWrapContent.minIndex = -showEquipCount+1;
    UIWrapContent.maxIndex = 0;

    local NeedMovePanel =  showEquipCount > 4;

    for key, value in pairs(List_EquipmentItem) do 
      local ItemData = List_ShowEquip[value.DataIndex];
      ShowSingleItem(value,ItemData);
    end 

    local UIPanel = EquipPanel_Grid.transform.parent:GetComponent("UIPanel")
    local pos = UIPanel.transform.localPosition;
    --小于四个就不能滑动了
    UIPanel:GetComponent("UIScrollView").enabled = NeedMovePanel;

    if NeedMovePanel  then   
       --自动向下移
       if pos.y - 122 > 0 then
        pos.y = pos.y - 122;

        UIPanel.transform.localPosition = pos;
        NeedMovePanel = true;
        local offset = UIPanel.clipOffset;
        offset.y = -pos.y;
        UIPanel.clipOffset = offset;
       end
    else
      pos.y = 0;
      UIPanel.transform.localPosition = pos;
      local offset = UIPanel.clipOffset;
      offset.y = 0;
      UIPanel.clipOffset = offset;
    end

    --自动寻找下一个选中对象
    if lastSelectedItem and (pos.y > 122  or  table.getn(List_ShowEquip)+1 == lastSelectedItem.DataIndex)  then

      local ItemID =  tonumber(string.sub(lastSelectedItem.gameObject.name,5))
      if lastSelectedItem then
        lastSelectedItem.Spr_selected.enabled = false;
      end
      
      
      if ItemID == 1 then ItemID = 9 end

      lastSelectedItem = List_EquipmentItem["Item"..tostring(ItemID-1)];
      lastSelectedItem_DataIndex = lastSelectedItem.DataIndex;

      lastSelectedItem.Spr_selected.enabled = true;

    end

    if lastSelectedItem then
      RrefreshRightPanel(List_ShowEquip[lastSelectedItem.DataIndex])
    else
      RrefreshRightPanel(List_ShowEquip[0])
    end

end



function EquipmentUI_Back()
    --隐藏主界面
    EventSender.SendEvent("MainUI_SetActive",true);
    --显示Title回退按钮
    EventSender.SendEvent("TitleUI_BtnBackVisable",false);
    --销毁界面

    if LoadingSceneUI then
       Destroy(LoadingSceneUI)
    end

end