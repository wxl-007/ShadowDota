LuaTools = luanet.import_type("LuaTools");
NGUIDebug = luanet.import_type("NGUIDebug");


function Start()
	--创建HelloWorld模块(以后可能就是副本，商城，活动等模块)
    CreateManager("HelloWorld");	
end

function CreateManager(scriptName)
    res = GameObject.Instantiate(Resources.Load("GameObject"));
    script = res:AddComponent(scriptName);
    res.name = scriptName;
    return script;
end