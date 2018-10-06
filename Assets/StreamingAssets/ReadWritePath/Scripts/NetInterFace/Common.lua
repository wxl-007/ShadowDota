local ProxyNet = require("NetInterFace.ProxyNet")
local callBack = require("NetInterFace.hprose_callback")
local Common   = ProxyNet:new("Common")

--[[
  通用接口方法说明
--]]
Common.api = {
    --检查版本
	 checkUpdate = {
	  	--请求参数列表
		param  = {
		  'version',      -- int      当前版本
		  'md5'           -- string   当前文件MD5
		},
		
		--接口返回
		result = {
		  '_type',        -- 返回类型  List<UpdateBean>

		  UpdateBean = {
			'downUrl',      -- string   下载地址
		  	'md5',          -- string   资源MD5
		  	'foceUpdat',    -- boolea   是否强制更新
		  	'sourceSize',   -- int      资源大小
		  	'version'       -- int      资源版本
		  }
		}
	}
}

--[[
local demo = function()
	print(Common:checkUpdate('version', 'md5'))
	print(Common:checkUpdate('version', 'md5'))	
end
--]]

return Common;							