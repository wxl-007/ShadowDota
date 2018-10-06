function Start()
--  注册一个监听器 MainUI.lua,使MainUI具有OnEvent功能
	 UIEventManager.Registe("MainUI");
	 UIEventManager.Registe("WarUI");
	 UIEventManager.Registe("MainUI3D");
	 UIEventManager.Registe("WarSecond");
	 EventSender.SendEvent("MainUI_Create");
end












