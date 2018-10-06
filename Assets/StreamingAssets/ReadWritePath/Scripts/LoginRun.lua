-- scp UniLua-master.zip vpn@192.168.1.110:/www/static/lua/
-- ssh vpn@192.168.1.110
-- cd /www/static/lua
-- chmod +r xxxx.zip


local Common = require 'NetInterFace.Common';
local Cache = require 'Cache';
local Tools = require 'Tools';



--定义一个类
local DownLoadUI = 
{
  gameObject = nil;
  DownLoadProgress = nil;
  gameObject = nil;
}


--下载资源存放的根目录
local DownLoadRootPath = DeviceInfo.PersistRootPath;
--下载地址
local DownLoadHttpPath = nil;
local DownLoadZipMD5 = nil;
local DownLoadZipPath = DownLoadRootPath.."/ReadWritePath.zip";
local version = 1.0


 function Start() 
  --如果使用了DEBUG_LUA宏
  if DEBUG_LUA then
      local zipMD5 = "nil"
      if System.IO.File.Exists(DownLoadZipPath) then
        zipMD5 = string.lower(LuaTools.GetMD5(DownLoadZipPath)) 
      end
      --询问服务器是否需要下载更新
      Common.checkUpdate(version,zipMD5,function( UpdateBean )
           --如果要更新
           if UpdateBean then
             DownLoadHttpPath = UpdateBean.downUrl;
             DownLoadZipMD5 = string.upper( UpdateBean.md5);
             CreateDownLoadUI();
           else
            NGUIDebug.Log("当前已经是最新版本")
            --直接初始化游戏
            InitGame();
           end
      end);      
  else
    InitGame();
  end
 end


--创建下载界面
function CreateDownLoadUI()
    LuaLinker = GameObject.Find("UI Root"):GetComponent("LuaLinker");
    if LuaLinker then
      --创建下载界面
      DownLoadUI.gameObject = Instantiate( Resources.Load("UI/DownLoadUI") );
      DownLoadUI.gameObject.transform.parent = LuaLinker.Node[0].transform;
      DownLoadUI.gameObject.transform.localPosition = Vector3.zero;
      DownLoadUI.gameObject.transform.localScale = Vector3.one;
      DownLoadUI.gameObject.name = "DownLoadUI";
      local UIModule = DownLoadUI.gameObject:GetComponent("UIModule");
      if UIModule then
        DownLoadUI.DownLoadProgress = UIModule.List_Slider[0];
        DownLoadUI.Lab_Des = UIModule.List_Label[0];
        DownLoad_UpdatePackage();
      end
    end
end


--初始化游戏 脚本\资源\配表
function InitGame()
   if DownLoadUI ~= nil and DownLoadUI.Lab_Des ~= nil then
      DownLoadUI.Lab_Des.text = "正在加载游戏资源..";
   end

  --注册一个LoginUI的监听器
    UIEventManager.Registe("LoginUI"); 

  --注册一个装备系统监听器
    UIEventManager.Registe("EquipmentUI");
  
  --注册一个对战系统监听器
    UIEventManager.Registe("BattleUI");
  

   --获取UI共享缓存
   local UIAssetbundles = Cache:get("AssetbundleSource");

   --如果没有就加载
   if UIAssetbundles == nil then
     --UI共享资源列表
     sharedNames = 
     {
        ["Font_SimHei"] = false,
        ["Atals_MainUI"] = false,
     };

     --读取游戏配表
     Core.Data:readLocalConfig(); 
     --加载共享资源
     LoadAllSharedResources();
   else
    DestroyDownLoadUI();
    --如果缓存中已经有数据了
    --直接发送创建LoginUI的消息
    EventSender.SendEvent("LoginUI_CreateLoginUI");
   end
end



--加载共享资源
function LoadAllSharedResources()
	for key, value in pairs(sharedNames) do  
		LuaTools.LoadResource(LuaName,key,"LoadResourceFinished"); 
  end 
end


--UI共享资源的assetbundles缓存
local AssetbundleSource ={}
--加载共享资源完成
function LoadResourceFinished(assetBundle,loadname)
  --保存assetBundle资源
  AssetbundleSource[loadname] = assetBundle; 
	sharedNames[loadname] = true;

  --是否所有的资源都加载完成
  local all_loaded = false;
	for key, value in pairs(sharedNames) do  
		all_loaded = value;
		if not all_loaded then break end
	end

	if all_loaded then
    --缓存共享UI的assetbundle
    Cache:save("AssetbundleSource",AssetbundleSource);

    StartCoroutine(DestroyDownLoadUI);
    --临时模拟登陆   
    this:SendOkMsg();
	end

end

--删除下载界面
function DestroyDownLoadUI()    
    if DownLoadUI ~= nil and DownLoadUI.Lab_Des ~= nil then
     yield.WaitForSeconds(0.5);
     DownLoadUI.Lab_Des.text = "游戏资源加载完毕,进入游戏..";
     yield.WaitForSeconds(0.5);
     --删除游戏下载界面
     if DownLoadUI.gameObject then
        GameObject.Destroy(DownLoadUI.gameObject);
     end
    end
end


--事件接收
function OnEvent(p_event , p_param)
    if p_event == "AssetBundleUnLoad" then
       AssetBundleUnLoad(p_param);
    end
end

function AssetBundleUnLoad(p_param)
  -- Debug.LogError(p_param[0]);
  -- Debug.LogError(p_param[1]);
   local source = AssetbundleSource[p_param[0]];
   if source then 
     source:UnLoad(p_param[1]);
   end
end


--临时模拟
function loginServerFinished()
    --发送创建LoginUI的消息
    EventSender.SendEvent("LoginUI_CreateLoginUI");
end


--下载更新资源包
function DownLoad_UpdatePackage()
   DownLoadUI.Lab_Des.text = "正在下载资源..";
   local downLoadTask = DownLoadFromWeb();
   local targetPath = DownLoadZipPath;
   downLoadTask:DownLoad(LuaName,DownLoadHttpPath,targetPath,"DownLoad_UpdatePackage_completed","DownLoad_UpdatePackage_progress");
end

--下载进度
function DownLoad_UpdatePackage_progress(BytesReceived,TotalBytesToReceive,ProgressPercentage)
  --Debug.LogError("callback_progress  "..tostring(BytesReceived).."  "..tostring(TotalBytesToReceive).."  "..tostring(ProgressPercentage));
  DownLoadUI.DownLoadProgress.value = ProgressPercentage/100.0;  
end

--下载完成
function DownLoad_UpdatePackage_completed(event)
  if CheckDownLoadZipMD5() then
     DownLoadUI.Lab_Des.text = "下载完成";
     --开启协程
     StartCoroutine(UnZip);
  else
     DownLoadUI.Lab_Des.text = "下载的ZIP不对,重新下载"
  end 
end




--检测下载的ZIP是否正确
function CheckDownLoadZipMD5()
  if not System.IO.File.Exists(DownLoadZipPath) then
    return false;
  else
    local zipMD5 = LuaTools.GetMD5(DownLoadZipPath)
    if DownLoadZipMD5 and zipMD5 and zipMD5 == DownLoadZipMD5 then
      return true;
    else
      return false;
    end
  end
end


--解压压缩包
function UnZip()
  yield.WaitForSeconds(0.5)
  DownLoadUI.Lab_Des.text = "正在解压更新包..";
  yield.WaitForSeconds(0.5)
  if LuaTools.UnZip(DownLoadZipPath,DownLoadRootPath) then
    --如果解压成功
    yield.WaitForSeconds(0.5)
    DownLoadUI.Lab_Des.text = "正在验证更新包..";
    if CheckMd5FromRead(DownLoadRootPath.."/ReadWritePath/Md5.cfg") then
      yield.WaitForSeconds(0.5)
      DownLoadUI.Lab_Des.text = "验证通过..";
      yield.WaitForSeconds(0.5)
      --初始化游戏
      InitGame();
    else
      DownLoadUI.Lab_Des.text = "验证失败..准备重新解压..";
    end
  else
     NGUIDebug.Log("解压压缩包失败");
  end
end



--检查MD5文件
function CheckMd5FromRead(md5Path)

  local file = io.open(md5Path,"r")
  local fileArray ={}
  local index = 1;
  for line in file:lines() do     
     fileArray[index] = line;
     index = index + 1;
  end
  file:close();  

  local arrayLength = table.getn(fileArray);
  --第一行是版本号,最后一行是end符
  if arrayLength > 2 then
    --最后一行不是end说明MD5文件没有问题
    if fileArray[arrayLength] ~= "end" then
       return false;
    end
  else
    --MD5文件中没有内容
    return false;
  end

  for i=2,arrayLength-1,1 do 
    --Debug.LogError(fileArray[i]); 
     local result = Tools:StringSplit(fileArray[i]," ");
     if table.getn(result) ~= 2 then
      return false;
     end
     local LocalFileMD5 = LuaTools.GetMD5(DownLoadRootPath.."/"..result[1]);
     if LocalFileMD5 ~= result[2] then
      return false;
     end
  end
  return true;

end  




