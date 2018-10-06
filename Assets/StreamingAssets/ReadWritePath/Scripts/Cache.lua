module (..., package.seeall)

SharedData = luanet.import_type("SharedData");
SharedListData = luanet.import_type("SharedListData");
SharedDataMgr = luanet.import_type("SharedDataMgr");
local t={}




--取出一个table
function t:get(className)
   local pData = SharedDataMgr.getSharedData(className);
   return pData;
end

--缓存一个table
function t:save(className,class)  
    --创建一个类
    local classData =  SharedDataMgr.newClass(className);

    if classData then
      for key, value in pairs(class) do
        if type(value) == 'table' then
        	childClass = SharedData();
        	--递归子类  
          t:DealChildClass(childClass,value);
        	classData[key] = childClass;
        else
     	    classData[key] = value;
     	  end
      end
    else
      Debug.LogError("lua's cache new "..className.." fail!");
    end
end  

--递归子类
function t:DealChildClass(child,childTable)
	for key, value in pairs(childTable) do 
      if type(value) == 'table' then
      	childClass = SharedData();
      	t:DealChildClass(childClass,value);
        child[key] = childClass;
      else
   	    child[key] = value;
   	  end
	end
end




function t:getList(ListName)  
  return SharedDataMgr.getList(ListName);
end


function t:saveList(ListName,_table)  
  local list = SharedDataMgr.newList(ListName);
  for key,value in ipairs(_table) do
    list:Add(value)
  end
end


return t
  

