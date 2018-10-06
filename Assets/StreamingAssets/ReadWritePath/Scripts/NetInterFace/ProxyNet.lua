local hprose   = require("NetInterFace.hprose")

local ProxyNet = {
	uri    = "http://101.251.236.157/Dota/index.php",

  new = function (self, name)
  	local o = {
  		modelName = name,
  		client = hprose.HttpCSClient:new(self.uri .. "?m=" .. name),
  		--client = hprose.HttpClient:new(self.uri .. "?m=" .. name),
  	}
  
    setmetatable(o, self)    
    return o
  end,
  
  --通过魔法函数，实现动态代理
  __index = function (self, name)
        return function (...)
          --Debug.Log(name);
          --Debug.Log(self.client.uri);
          return self.client:invoke(self.modelName .. "_" .. name, {...}, false, hprose.ResultMode.Normal, false)
        end
  end
}

return ProxyNet;