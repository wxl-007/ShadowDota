using System;
using UObj = UnityEngine.Object;

//TODO: 这个类要全部重写
public class AssetTask : BaseTaskAbstract {

    //load type
    public enum loadType {
        Only_Download,
        Only_loadlocal,
        Both_Download_loadlocal,
    }

    public string PrefabName;
    public Type UType;
    public Action<AssetTask> LoadFinished;

    //报告下载进度
    public Action<float> reportProgress;

    //加载出错（可能是网络错误，可能是本没有文件）
    public Action<string> LoadError;

    //加载Mode
    public loadType LoadType;
    //pass some arguments 
    public int arg1;

    public string AssetBundleName {
        get {
            return AW.Resources.ResourceSetting.ConvertToAssetBundleName(PrefabName);
        } 
    }

    //Result 结果
    public UObj Obj;

    public AssetTask (string pbName, Type utype, Action<AssetTask> end) {
        PrefabName = pbName;
        UType = utype;
        LoadFinished = end;

        this.type = TaskType.ResourceTask;
        this.threadType = ThreadType.MainThread;
        this.respType = TaskResponse.Default_Response;
    }

    public override void DispatchToRealHandler() {
		/*
		Obj = Core.ResEng.GetResource(PrefabName);

        if(null == Obj) {
            Core.ResEng.LoadAsync(this);
        } else {
            LoadFinished(this);
        }*/

    }

    public void AppendCommonParam (int id, UObj loaded = null, loadType download = loadType.Both_Download_loadlocal) {
        arg1 = id;
        Obj = loaded;
        LoadType = download;
    }

}
