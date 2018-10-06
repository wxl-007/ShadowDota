require 'Cache'

function Start()
  DontDestroyOnLoad(gameObject);
  CreateLoadingSceneUI()
  StartCoroutine(JumpToTargetScene);
end
  

LoadingSceneUI = 
{
    LoadSceneProgress = nil
}


--创建Loading界面
function  CreateLoadingSceneUI()

   local camera_object = this.Node[0].transform;
   local source = Resources.Load("UI/LoadingSceneUI");  
   local LoadingSceneUI_Object = Instantiate(source);
   LoadingSceneUI_Object.transform.parent = camera_object;
   LoadingSceneUI_Object.transform.localPosition = Vector3.zero;
   LoadingSceneUI_Object.transform.localScale = Vector3.one; 
   local UIModule = LoadingSceneUI_Object:GetComponent("UIModule");
    
   LoadingSceneUI.LoadSceneProgress = UIModule.List_Slider[0];

  
 end  

--跳转到指定界面
 function JumpToTargetScene()
 	 --yield.WaitForSeconds(0.5)
     Jumper.LoadSceneAsyncWithLoading("War",Jump_complete,Jump_progress)
 end

 function Jump_complete()
 	--Debug.LogError("Jump_complete")
 	Destroy(gameObject)
 end

 function Jump_progress(progress)
 	if LoadingSceneUI.LoadSceneProgress then
 	   --Debug.LogError(progress)
 	   LoadingSceneUI.LoadSceneProgress.value = progress;
    end
 end