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
 * hprose/client.lua                                      *
 *                                                        *
 * hprose Client for Lua                                  *
 *                                                        *
 * LastModified: May 14, 2014                             *
 * Author: Ma Bingyao <andot@hprose.com>                  *
 *                                                        *
\**********************************************************/
--]]

local Tags         = require("NetInterFace.hprose.tags")
local ResultMode   = require("NetInterFace.hprose.result_mode")
local OutputStream = require("NetInterFace.hprose.output_stream")
local InputStream  = require("NetInterFace.hprose.input_stream")
local Writer       = require("NetInterFace.hprose.writer")
local Reader       = require("NetInterFace.hprose.reader")

local error        = error
local tostring     = tostring
local setmetatable = setmetatable
local remove       = table.remove

--动态代理类
local dynamicProxy = {
    client = nil,
    new = function (self, client)
        local o = {}
        setmetatable(o, self)
        o.client = client
        return o
    end,
    
    --通过魔法函数，实现动态代理
    __index = function (self, name)
        return function (...)
            return self.client:invoke(name, {...}, false, ResultMode.Normal, false)
        end
    end
}

--客户端调用类
local Client = {}

function Client:new(uri)
    local o = {}
    setmetatable(o, self)
    self.__index = self
    o.uri = uri
    o.filters = {}
    o.simple = false
    return o
end

function Client:useService(uri)
    if uri ~= nil then 
      self.uri = uri 
    end
    
    return dynamicProxy:new(self)
end

--[[
  name  函数名
  args  参数列表
--]]
function Client:invoke(name, args, byRef, resultMode, simple)
    if args == nil then 
        args = {} 
    end
    
    if simple == nil then 
        simple = self.simple 
    end
    
    --开始序列化
    local stream = OutputStream:new()
    
    --调用标识 C
    stream:write(Tags.Call)
    
    --写入方法名
    local writer = Writer:new(stream, simple)
    writer:writeString(name)
    
    --回调函数
    local callBack;        
    local errorCallBack;    
    local count = args.n or #args    
    for i = 1, count do
        if type(args[i]) == "function" then     --第一个函数为回调函数，第二个为错误处理函数
           if callBack == nil then
              callBack = remove(args, i)           
              errorCallBack = remove(args, i)
           end
        end
    end
    
    --写入参数
    if #args > 0 or byRef then
        writer:reset()
        writer:writeList(args)
        
        if byRef then 
            writer:writeBoolean(true) 
        end
    end
    
    --写入协议结尾
    stream:write(Tags.End)
    
    --执行输出过滤链
    local data = tostring(stream)
    local count = #self.filters
    for i = 1, count do
        data = self.filters[i].outputFilter(data, self)
    end
    
    --发送协议
    data = self:sendAndReceive(data, callBack, errorCallBack)    
    
    --异步调用，立即返回
    if data == nil then
        return true
    end
    
    --执行输入过滤链
    for i = count, 1, -1 do
        data = self.filters[i].inputFilter(data, self)
    end
    
    if resultMode == ResultMode.RawWithEndTag then
        return data
    end    
    if resultMode == ResultMode.Raw then
        return data:sub(1, -2)
    end
    
    --反序列化返回数据
    stream = InputStream:new(data)
    local reader = Reader:new(stream)
    local result = nil
    local tag    = stream:getc()
    
    while tag ~= Tags.End do
        if tag == Tags.Result then                  --读取返回结果
            if resultMode == ResultMode.Serialized then
                result = reader:readRaw()
            else
                reader:reset()
                result = reader:unserialize()
            end
        elseif tag == Tags.Argument then            --读取参数
            reader:reset()
            local arguments = reader:readList()
            for i = 1, #arguments do
                args[i] = arguments[i]
            end
        elseif tag == Tags.Error then               --读取异常
            reader:reset()
            error(reader:readString())
        else
            error("Wrong Response: \r\n" .. data)
        end
        tag = stream:getc()
    end
    
    return result
end

function Client:getFilter()
    if #self.filters == 0 then
        return nil
    else
        return self.filters[1]
    end
end

function Client:setFilter(filter)
    if filter == nil then
        self.filters = {}
    else
        self.filters = {filter}
    end
end

function Client:addFilter(filter)
    self.filters[#self.filters + 1] = filters
end

function Client:removeFilter(filter)
    for i = 1, #self.filters do
        if self.filters[i] == filter then
            remove(self.filters, i)
            return true
        end
    end
    
    return false
end

return Client