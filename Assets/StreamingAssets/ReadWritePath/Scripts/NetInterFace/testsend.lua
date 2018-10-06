local stub  = require("Account")

function iCall(result)
    Test0:LOG("Just you code Here!!")
    _print(result)
end

--玩家登录操作
function AccountLogin()
  --帐号登录
  local isSend  = stub.userLogin(1, 'madongfang', iCall)
  _print(isSend)
end

---###############################################
--[[
  C#异步返回回调的地方入口
--]]
local Tags         = require("hprose.tags")
local ResultMode   = require("hprose.result_mode")

local Reader       = require("hprose.reader")
local InputStream  = require("hprose.input_stream")

---解析返回结果
function readResult(data, args, errorFunc)
    --反序列化返回数据
    local stream = InputStream:new(data)
    local reader = Reader:new(stream)
    local result = nil
    local tag    = stream:getc()
    
    local resultMode   = ResultMode.Normal
    
    while tag ~= Tags.End do
        if tag == Tags.Result then                          --读取返回结果
            if resultMode == ResultMode.Serialized then
                result = reader:readRaw()
            else
                reader:reset()
                result = reader:unserialize()
            end
        elseif tag == Tags.Argument then                    --读取参数
            reader:reset()
            local arguments = reader:readList()
            for i = 1, #arguments do
                args[i] = arguments[i]
            end
        elseif tag == Tags.Error then                       --读取异常
            reader:reset()
            errorFunc(reader:readString())
        else
            errorFunc("Wrong Response: \r\n" .. data)
        end
        tag = stream:getc()
    end
    
    return result
end

---异步回调函数
function sendCallBack(recData, func, errorFunc)
    Test0:LOG("sendCallBack" .. recData)
    
    local args = {}
    local result =  readResult(recData, args, errorFunc)
    
    _print(result)
    func(result, args)
end