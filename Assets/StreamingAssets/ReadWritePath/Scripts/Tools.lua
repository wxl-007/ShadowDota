--常用工具类
module (..., package.seeall)
Cache = require "Cache"
local tools={}


function tools:StringSplit(str, split_char)
    local sub_str_tab = {};
    local i = 0;
    local j = 0;
    while true do
        j = string.find(str, split_char,i+1);    --从目标串str第i+1个字符开始搜索指定串

        if j == nil then
            table.insert(sub_str_tab,string.sub(str,i+1));
            break;
        end
        --print (tostring(i+1)..tostring(j-1));

        table.insert(sub_str_tab,string.sub(str,i+1,j-1));
        i = j;
    end
    return sub_str_tab;
end



function tools:LoadLevelUseLoading(levelName)
   Application.LoadLevel("Loading");
end


--按钮点击事件传过来的是GameObject也可能是String，这个函数会直接返回按钮名称
function tools:GetButtonName(btnParam)
     local  Param = btnParam
     if LuaTools.GetType(btnParam):ToString()=="System.Object[]" then
        Param = btnParam[0]
     end


     if LuaTools.GetType(Param):ToString() == "UnityEngine.GameObject" then
        return Param.name;
     elseif LuaTools.GetType(Param):ToString() == "System.String" then
        return Param;
     end
     return nil;
end

--表中是否包含
function tools:isTableContain(_value,_table)
   for key,value in pairs(_table) do
     if _value == value then return true end
   end
   return false
end


--DeBug Table
function tools:printTable(_table)
   print("--------------"..tostring(table).."--------------")
   for key, value in pairs(_table) do 
     print(tostring(key).."="..tostring(value))
   end
   print("---------------------------------------------")
end




  -- white = 0,
  -- green = 1,
  -- blue = 2,
  -- purple = 3,
  -- golden = 4,
  -- orange = 5,
function tools:SetLabelColor(UILabel,color)
    print(type(color))
end


--注册一个页面回调
function tools:Registered_TittleUICallBack(Page_CallBack)
     local BackDelegate = Cache:getList("TitleUIBackDelegate")
print(tostring(BackDelegate))
     
     if BackDelegate == nil then
          local delegate = {Page_CallBack}
          Cache:saveList("TitleUIBackDelegate",delegate)
     else
      
        BackDelegate:Add(Page_CallBack)
     end
end




return tools
  

