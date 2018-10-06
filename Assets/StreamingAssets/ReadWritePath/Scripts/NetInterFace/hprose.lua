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
 * hprose.lua                                             *
 *                                                        *
 * hprose for Lua                                         *
 *                                                        *
 * LastModified: May 14, 2014                             *
 * Author: Ma Bingyao <andot@hprose.com>                  *
 *                                                        *
\**********************************************************/
--]]

--[[
  Lua端发送请求使用
--]]
local hprose = {
    Tags         = require("NetInterFace.hprose.tags"),
    ResultMode   = require("NetInterFace.hprose.result_mode"),

    InputStream  = require("NetInterFace.hprose.input_stream"),
    OutputStream = require("NetInterFace.hprose.output_stream"),
    
    Reader       = require("NetInterFace.hprose.reader"),
    Writer       = require("NetInterFace.hprose.writer"),
    
    Formatter    = require("NetInterFace.hprose.formatter"),
    ClassManager = require("NetInterFace.hprose.class_manager"),    

    Client       = require("NetInterFace.hprose.client"),    
    --HttpClient   = require("NetInterFace.hprose.http_client"),
    HttpCSClient = require("NetInterFace.hprose.http_csclient")
}

return hprose




