using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using AW.Cache;
/// -----------------------------------------------
/// ------- 重要：已经不再使用，仅供参考-------------
/// -----------------------------------------------
namespace AW.Resources {

	using UObj = UnityEngine.Object;

    public enum ManagerType {
        ModelManager = 0x0,
    }

    public class ResourceManager {
        // 已解压的Asset列表 [prefabPath = asset bundle name, asset] asset bundle name 包含路径的信息
        private Dictionary<string, UObj> dicAsset = null;
        // "正在"加载的资源列表 [prefabPath, www]
        private Dictionary<string, WWW> dicLoadingReq = null;
        // 同时加载的最大允许值
        private const int MAX_WWW_COUNT = 1;

        private Queue<AssetTask> cachedWork;

		private Dictionary<ManagerType, RefManager<int>> AMgr = null;

        public ResourceManager () {
            dicAsset = new Dictionary<string, UObj>();
            dicLoadingReq = new Dictionary<string, WWW>();
			AMgr = new Dictionary<ManagerType, RefManager<int>>();

            cachedWork = new Queue<AssetTask>();
        }

        //不包含路径的预设体名,
        public UObj GetResource(string PrefabName)
        {
            UObj obj = null;

            string ABname = ResourceSetting.ConvertToAssetBundleName(PrefabName);

            if (dicAsset.TryGetValue(ABname, out obj) == false)
            {
                if (dicLoadingReq.ContainsKey(ABname)) {
                    ConsoleEx.DebugLog("<GetResource Failed> Res is still loading, res.Name = " + ABname);
                } else {
                   // ConsoleEx.DebugLog("<GetResource Failed> Res not exist, res.Name = " + ABname);
                }
				obj = null;
            }
			else {
                // 添加引用
                RefAsset(ABname);
            }
            return obj;
        }

		public void RegisterAM(ManagerType mType, RefManager<int> mgr) {
            if(!AMgr.ContainsKey(mType)) {
                AMgr.Add(mType, mgr);
            }
        }
       
        // name表示 不包含路径的预设体名,
        public void LoadAsync(string name) {
            LoadAsync( new AssetTask(name, typeof(UObj), null));
        }

        // name表示 不包含路径的预设体名,
        public void LoadAsync(AssetTask task) {
            if(task == null)
                return;
            // 如果已经下载，则返回
            if (dicAsset.ContainsKey(task.AssetBundleName))
                return;

            // 如果正在下载，则返回
            if (dicLoadingReq.ContainsKey(task.AssetBundleName))
                return;

            if(dicLoadingReq.Count < MAX_WWW_COUNT) {
				if(Core.DevFSM.rtPlatform == RuntimePlatform.OSXEditor || Core.DevFSM.rtPlatform  == RuntimePlatform.OSXPlayer) {
                    //CoroutineProvider.Instance().StartCoroutine(AsyncLoadLoacl(task));
					CoroutineProvider.Instance().StartCoroutine(AsyncLoadCoroutine(task));
                } else {
                    // 如果没下载，则开始下载
                    CoroutineProvider.Instance().StartCoroutine(AsyncLoadCoroutine(task));
                }

            } else {
                cachedWork.Enqueue(task);
            }
        }

        /// <summary>
        /// According to LoadType, it will download or load local sync
        /// 把AssetBundle载入内存，之后再异步Load出来，之后再次删除Assetbundle内存镜像.
        /// </summary>
        /// <returns>The load coroutine.</returns>
        /// <param name="task">Task.</param>
        private IEnumerator AsyncLoadCoroutine(AssetTask task) {
            bool errorOcurred = false;

            string assetBundleName = task.AssetBundleName;


            string url = ResourceSetting.ConverToFtpPath(assetBundleName);
            int verNum = 1;//Core.Data.sourceManager.getNewNum(assetBundleName + ".unity3d");

            // 添加引用
            RefAsset(assetBundleName);

            if (Caching.IsVersionCached(url, verNum) == false) {
                //ConsoleEx.DebugLog("Version Is not Cached, which will download from net!");
            }

            WWW www = null;
            try {
               www = WWW.LoadFromCacheOrDownload(url,verNum);
            } catch(Exception ex) {
                errorOcurred = true;
               // ConsoleEx.DebugLog("LoadFromCacheOrDownload Error : " + ex.ToString());
                if(task.LoadError != null) 
					task.LoadError("["+assetBundleName+"]:"+ex.ToString());
            } 

            if(!errorOcurred) {
                dicLoadingReq.Add(assetBundleName, www);
                while (www.isDone == false) {
                    if( (task.LoadType == AssetTask.loadType.Only_Download || task.LoadType == AssetTask.loadType.Both_Download_loadlocal )
                       && task.reportProgress != null) {
                        task.reportProgress(www.progress);
                    }
                    yield return null;
                }

                // Print the error to the console
                if (!String.IsNullOrEmpty(www.error)) {
					//ConsoleEx.DebugLog("["+assetBundleName+"]"+www.error);
					Debug.LogError("["+assetBundleName+"]"+www.error);

					if (task.LoadError != null ) {
						task.LoadError (www.error);
					} 
                    dicLoadingReq.Remove(assetBundleName);
                    errorOcurred = true;
                }

                bool TaskEnd = false;

                if(task.LoadType == AssetTask.loadType.Only_loadlocal || task.LoadType == AssetTask.loadType.Both_Download_loadlocal) {
                    if(!errorOcurred) {
                        AssetBundleRequest req = www.assetBundle.LoadAsync(task.PrefabName, task.UType);
                        while (req.isDone == false)
                            yield return null;
                        
                        dicAsset.Add(assetBundleName, req.asset);
                        task.Obj = req.asset;
                        dicLoadingReq.Remove(assetBundleName);
                        www.assetBundle.Unload(false);
                        
                        //--- load local finished ---
                        TaskEnd = true;
                       
                    }
                } else {
                    dicLoadingReq.Remove(assetBundleName);
                  
                    //--- Downloading finished ---
                    TaskEnd = true;
                }

                //release memeory
                if(www != null) {
                    www.Dispose();
                    www = null;
                }

                //start to callback
                if(TaskEnd) {
                    LoadEnd();
                    if(task.LoadFinished != null) task.LoadFinished(task);
                }
            }
            
        }

        /// <summary>
        /// 异步加载位于本机的AssetBundle
        /// </summary>
        /// <returns>The load loacl.</returns>
        /// <param name="task">Task.</param>
        private IEnumerator AsyncLoadLoacl(AssetTask task) {
            string assetBundleName = task.AssetBundleName;
            // 添加引用
            RefAsset(assetBundleName);

            string fullPath = Path.Combine(DeviceInfo.PersistRootPath, GetPlatformName());
            fullPath = Path.Combine(fullPath, assetBundleName);

            WWW www = new WWW ("file:///" + fullPath);
            www.threadPriority = ThreadPriority.High;
            yield return www;
            if (www.error != null) {
                ConsoleEx.DebugLog("WWW download:" + www.error);
            } else {
                AssetBundleRequest req = www.assetBundle.LoadAsync(task.PrefabName, task.UType);
                yield return req;

                dicAsset.Add(assetBundleName, req.asset);
                task.Obj = req.asset;
                dicLoadingReq.Remove(assetBundleName);
                www.assetBundle.Unload(false);
                www.Dispose();
                www = null;

                //本地加载完成
                LoadEnd();
                if(task.LoadFinished != null) task.LoadFinished(task);
            }

        }

        /// <summary>
        /// 同步加载位于本机的资源
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="task">Task.</param>
        public IEnumerator SyncLoad(AssetTask task) {
            string assetBundleName = task.AssetBundleName;
            // 添加引用
            RefAsset(assetBundleName);

            string fullPath = Path.Combine(DeviceInfo.PersistRootPath, GetPlatformName());
            fullPath = Path.Combine(fullPath, assetBundleName);

            WWW www = new WWW ("file:///" + fullPath);
            www.threadPriority = ThreadPriority.High;
            yield return www;
            if (www.error != null) {
                ConsoleEx.DebugLog("WWW download:" + www.error);
            } else {
                UObj obj = www.assetBundle.Load(task.PrefabName, task.UType);

                dicAsset.Add(assetBundleName, obj);
                task.Obj = obj;
                dicLoadingReq.Remove(assetBundleName);
                www.assetBundle.Unload(false);
                www.Dispose();
                www = null;

                //本地加载完成
                LoadEnd();
                if(task.LoadFinished != null) task.LoadFinished(task);
            }
        }

        #region 测试目的的使用 -- 不会添加引用等信息

        public IEnumerator AsyncLoadAll(AssetTask task) {
            string assetBundleName = task.AssetBundleName;
            string fullPath = Path.Combine(DeviceInfo.PersistRootPath, GetPlatformName());
            fullPath = Path.Combine(fullPath, assetBundleName);

            WWW www = new WWW ("file:///" + fullPath);
            www.threadPriority = ThreadPriority.High;
            yield return www;
            if (www.error != null) {
                ConsoleEx.DebugLog("WWW download:" + www.error);
            } else {
                StringBuilder sb = new StringBuilder();

                UObj[] objs = www.assetBundle.LoadAll();
                foreach(UObj obj in objs) {
                    sb.Append("name = " + obj.name + "\t type = " + obj.GetType().ToString()).Append("\n");
                }
                ConsoleEx.DebugLog(sb.ToString());

                www.assetBundle.Unload(true);
                www.Dispose();
                www = null;
                //本地加载完成
                LoadEnd();
                if(task.LoadFinished != null) task.LoadFinished(task);
            }
        }

        #endregion


        private void LoadEnd() {

            if(cachedWork.Count > 0) {
                AssetTask task = cachedWork.Dequeue();

                if(task != null) {
                    if(dicLoadingReq.Count < MAX_WWW_COUNT) {
                        // 如果没下载，则开始下载
                        CoroutineProvider.Instance().StartCoroutine(AsyncLoadCoroutine(task));
                    } else {
                        cachedWork.Enqueue(task);
                    }
                }
            }
        }

        public bool IsResLoading(string pbName) {
            string AssetBundleName = ResourceSetting.ConvertToAssetBundleName(pbName);
            return dicLoadingReq.ContainsKey(AssetBundleName);
        }

        public bool IsResLoaded(string pbName) {
            string AssetBundleName = ResourceSetting.ConvertToAssetBundleName(pbName);
            return dicAsset.ContainsKey(AssetBundleName);
        }

        public WWW GetLoadingWWW(string pbName) {
            string AssetBundleName = ResourceSetting.ConvertToAssetBundleName(pbName);
            WWW www = null;
			dicLoadingReq.TryGetValue(AssetBundleName, out www);
            return www;
        }

        // 移除Asset资源的引用，name表示prefab name
        public void UnrefAsset(string AssetName) {
            if(dicAsset.ContainsKey(AssetName)) {
                dicAsset.Remove(AssetName);
                ConsoleEx.DebugLog("<Res is removed ref> name = " + AssetName);
            }
        }

        // 根据资源路径添加资源引用，每个管理器管理自己的引用
        private void RefAsset(string AssetName)
        {
            // 模型之类的
            /*if (AssetName.Contains(ResourceSetting.FULL_PATH)) {
                RefManager mgr = null;
                if(AMgr.TryGetValue(ManagerType.ModelManager, out mgr)) {
                    mgr.RefAsset(AssetName);
                }
            }
			else
			{
                //ConsoleEx.DebugLog("<Res not ref> name = " + AssetName);
			}*/
        }

        /*
        public void UnloadUnusedAsset()
        {
            // bool effectNeedUnload = GameApp.GetEffectManager().UnloadAsset();
            // bool worldNeedUnload = GameApp.GetWorldManager().UnloadAsset();
            // bool sceneNeedUnload = GameApp.GetSceneManager().UnloadAsset();
            if (effectNeedUnload || worldNeedUnload || sceneNeedUnload)
            {
                Resources.UnloadUnusedAssets();
            }
        }
        */

        private string GetPlatformName() {
            string platform = string.Empty;
            #if UNITY_IOS
            platform = "AssetBundles/IOS";
            #elif UNITY_STANDALONE_WIN
            platform = "AssetBundles/Windows32";
            #elif UNITY_EDITOR
            platform = "AssetBundles/Mac";
            #elif UNITY_STANDALONE_OSX
            platform = "AssetBundles/Mac";
            #elif UNITY_WP8
            platform = "AssetBundles/WP";
            #elif UNITY_ANDROID
            platform = "AssetBundles/Android";
            #elif UNITY_WEBPLAYER
            platform = "AssetBundles/WebPlayer";
            #endif
            return platform;
        }
    }
}

