--用户登录
local Account = require 'NetInterFace.Account';

Debug = {
  Log = print
}

function _print(t)
    if(type(t) == 'table')  then
        for k, v in pairs(t) do
            _print(k);
            _print(v);
        end
    else
      print(t);   
    end
end


local result = Account.userLogin(1, "madongfang");
_print(result)

