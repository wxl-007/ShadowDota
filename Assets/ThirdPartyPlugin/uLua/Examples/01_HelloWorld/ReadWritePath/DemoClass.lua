--类的写法
local moduleName = ...

--定义一个类(表)
DemoClass = {
	x=0,
	y=0
}  

--这句是重定义元表的索引，必须要有，  
DemoClass.__index = DemoClass  
_G[moduleName] = DemoClass
package.loaded[moduleName] = DemoClass
  

--模拟构造体，一般名称为new()  
function DemoClass:new()  
        local self = {}     
        setmetatable(self, DemoClass)   --必须要有  
        return self    
end  
  
function DemoClass:test()  
    print("DemoClass Print  "..self.x.."   "..self.y)  
end  

