--用户登录
local ProxyNet = require("NetInterFace.ProxyNet")
local Account  = ProxyNet:new("Account")

--加入回调全局函数
local callBack = require("NetInterFace.hprose_callback")

Account.api = {
	_login = {
		param  = {},
		result = {}
	}
}	

return Account;							