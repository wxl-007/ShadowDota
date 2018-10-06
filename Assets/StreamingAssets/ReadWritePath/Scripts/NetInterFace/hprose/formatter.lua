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
 * hprose/reader.lua                                      *
 *                                                        *
 * hprose Reader for Lua                                  *
 *                                                        *
 * LastModified: May 13, 2014                             *
 * Author: Ma Bingyao <andot@hprose.com>                  *
 *                                                        *
\**********************************************************/
--]]

local Writer       = require("NetInterFace.hprose.writer")
local Reader       = require("NetInterFace.hprose.reader")

local OutputStream = require("NetInterFace.hprose.output_stream")
local InputStream  = require("NetInterFace.hprose.input_stream")

local tostring     = tostring

local Formatter = {
    --序列化
    serialize = function(variable, simple)
        local stream = OutputStream:new()
        local writer = Writer:new(stream, simple)
        
        writer:serialize(variable)
        return tostring(stream)
    end,
    
    --反序列化
    unserialize = function(variable_representation, simple)
        local stream = InputStream:new(variable_representation)
        local reader = Reader:new(stream, simple)
        
        return reader:unserialize()
    end
}

return Formatter