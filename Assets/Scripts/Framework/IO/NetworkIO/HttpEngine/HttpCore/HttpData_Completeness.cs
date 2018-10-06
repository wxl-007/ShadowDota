using System;
using System.Collections.Generic;
using AW.IO;
using AW.FSM;

//for storage , this class is element
public class HttpDataNoElement
{
    public int no;
    public int act;

    public HttpDataNoElement() { }

    public HttpDataNoElement(int No, int Act)
    {
        no = No;
        act = Act;
    }
}
// 
public class HttpData_No : DataObject
{
    public HttpDataNoElement[] httpNo;

    public HttpData_No() { }

    public static void load(LocalIOManager persist, out Dictionary<int, int> HttpDataNo)
    {
        HttpDataNo = new Dictionary<int, int>();

		HttpData_No db = persist.ReadFromLocalFileSystem(DataType.HTTP_COMPLETE, DataObject.Relevant.AccountRelevant, AES_ENCRYPT) as HttpData_No;

        if (db != null)
        {
            foreach (HttpDataNoElement element in db.httpNo)
            {
                HttpDataNo.Add(element.act, element.no);
            }
        }
    }


    public void save(LocalIOManager persist, HttpDataNoElement[] data)
    {
        if (data != null)
        {
            httpNo = data;
            mType = DataType.HTTP_COMPLETE;
			KindOfRelevant = Relevant.AccountRelevant;
            persist.WriteToLocalFileSystem(this, AES_ENCRYPT);
        }
    }
}

// we should keep the singleton
public class HttpData_Completeness : IGameState
{
    //------------ const vars -----------------
    private const int MAX_NO = 9999;
    // Becasue we need to read local file, wo must keep one LocalIOManager instance.
    private LocalIOManager persistManager;
        
    private Dictionary<int, int> HttpDataNo;

    public HttpData_Completeness(LocalIOManager persist) {
        persistManager = persist;
        HttpData_No.load(persistManager, out HttpDataNo);
    }

	public int getHttpRequestNo(HttpRequest request)
    {
        int No = 0;
        if (request != null)
        {
            if (HttpDataNo.TryGetValue(request.Act, out No))
            {
                //
            }
            else
            {
                No = 1;
                HttpDataNo.Add(request.Act, No);
            }
        }

        return No;
    }

	public void incHttpRequestNo(HttpRequest request)
    {
        int No = 0;
        if (request != null)
        {
            if (HttpDataNo.TryGetValue(request.Act, out No))
            {
                No = (++No) % MAX_NO;
                HttpDataNo[request.Act] = No;
            }
            else
            {
                No = 1;
                HttpDataNo.Add(request.Act, No);
            }
        }
    }

	/// 
	/// ************************ 接口实现 ******************************
	/// 

	#region IGameState implementation

	public void OnLogin (StateParam<GameState> obj) {

	}

	public void OnUnregister (StateParam<GameState> obj) {
		if(HttpDataNo != null) HttpDataNo.Clear();
	}

	public void OnDayChanged (StateParam<GameState> obj) {
		// TODO : ignore it
	}

	public void OnLevelChanged (StateParam<GameState> obj) {
		// TODO : ignore it
	}

	#endregion

}
