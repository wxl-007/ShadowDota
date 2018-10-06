-- scp UniLua-master.zip vpn@192.168.1.110:/www/static/lua/
-- ssh vpn@192.168.1.110
-- cd /www/static/lua
-- chmod +r xxxx.zip

luanet.load_assembly('UnityEngine')
local GUI  = luanet.import_type('UnityEngine.GUI')
local Rect = luanet.import_type('UnityEngine.Rect')

--帐号相关
local Account = require 'NetInterFace.Account';

local Cache   = require 'Cache';
SharedDataMgr = luanet.import_type("SharedDataMgr");

function _print(t)
    if(type(t) == 'table')  then
        for k, v in pairs(t) do
            _print(k);
            _print(v);
        end
    else
      Debug.Log(t);   
    end
end

function Start()
  
end


function callb(result)
  _print(result);
end

function OnGUI()
   if (GUI.Button(Rect(10,10,120,50), "同步调用")) then        
       local result = Account.userLogin(1, "madongfang");
       print(result.userId);
       print(result.userName);
   end
    
   if (GUI.Button(Rect(150,10,120,50), "异步调用1")) then        
       local result = Account.userLogin(10, "madongfang", callb);        
        _print(result)
    end
    
   if (GUI.Button(Rect(290,10,120,50), "异步调用2")) then        
       local result = Account.userLogin(1, "madongfang",          
          --回调处理 
          function(recData)
            _print(recData)
          end,
          
          --错误处理
          function(error)
            _print(error)
          end          
        );
        
        _print(result)
    end    
end