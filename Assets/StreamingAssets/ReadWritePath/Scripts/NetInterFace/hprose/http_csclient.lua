--[[
/**********************************************************\
|                                                          |
|                          hprose                          |
|                                                          |
| Official WebSite: http://www.hprose.com/                 |
|                   http://www.hprose.org/                 |
|                                                          |
\**********************************************************/

/**********************************************************\
 *                                                        *
 * hprose/http_client.lua                                 *
 *                                                        *
 * hprose HTTP Client for Lua                             *
 *                                                        *
 * LastModified: May 14, 2014                             *
 * Author: Ma Bingyao <andot@hprose.com>                  *
 *                                                        *
\**********************************************************/
--]]

--[[
  基于LuaInterface的通信组件
--]]
local pairs  = pairs
local unpack = unpack

local Client = require("NetInterFace.hprose.client")
local HttpCSClient = Client:new()

--创建HttpClient
--加载 C# HttpLuaClient 类请求，使用CS2Lua时，则不需要显示调用
local _client = luanet.import_type('HttpLuaClient')

---发送数据
function HttpCSClient:sendAndReceive(data, callback, errorCallBack)
    --创建发送类
    local c = _client.Create(LuaName, self.uri)      
    
    --同步发送
    if callback == nil then
        local recData = c:send(data)        
        return recData
    end
    
    --异步发送
    if errorCallBack == nil then
        c:send(data, callback)
    else
        c:send(data, callback, errorCallBack)    
    end
    
    return nil         
end

return HttpCSClient