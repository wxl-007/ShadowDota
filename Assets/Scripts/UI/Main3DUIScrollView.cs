using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CenterOnChildForMain))]
public class Main3DUIScrollView : MonoBehaviour
{
	List<UITexture> m_TexList = new List<UITexture>();
    CenterOnChildForMain centerChild;
    public float cScaleSize = 1.1f;
    //假的目标点  周围卡牌 都面向此点
    Transform cTarget;
    public Vector3 otherScale = new Vector3(0.8f, 0.8f, 1);
    public float farEuler = 60;
    /// <summary>
    /// 卡牌最多 排列距离 350
    /// </summary>
    public readonly int maxWidth = 350;
    /// <summary>
    /// 最近卡牌距离
    /// </summary>
    public readonly int closeDis = 200;
    readonly float tweenTime = 0.3f;
    UIGrid myGrid = null;
	UIModule m_Module;
	Camera _camera;

	void Awake()
	{
		m_Module = transform.parent.GetComponent<UIModule>();
		SetTextureMaterial();
	}

    void OnEnable()
    {
        FindCenter();
    }

    void Start()
    {

        if (centerChild == null)
            this.FindCenter();

    }
	

	void SetTextureMaterial()
	{
		if(m_Module != null)
		{
			for(int i=0; i<m_Module.List_Object.Count; i++)
			{
				UITexture texture = m_Module.List_Object[i].GetComponent<UITexture>();
				if(texture != null)
				{
					Shader shader = m_Module.List_ShaderObject[0];
					Material m = new Material(shader);
					texture.material = m;
					texture.material.renderQueue = 1000;
					m_TexList.Add(texture);
				}
			}
		}
	}

    void FindCenter()
    {
        if (centerChild == null)
        {
            centerChild = GetComponent<CenterOnChildForMain>();
        }
        if (centerChild == null)
        {
            Debug.LogWarning(" UICenterOnChild  is Not Find ");
            this.enabled = false;
        }

    }

    void FindGrid()
    {
        if (myGrid == null)
            myGrid = GetComponent<UIGrid>();

        if (myGrid == null)
        {
            Debug.LogWarning("grid is not find ");
            this.enabled = false;
        }
    }

    //居中状态
    public void ShowCenterEffect(Transform tCenter)
    {

        if (tCenter != null)
        {
            Vector3 tlocalScale = Vector3.one * cScaleSize;

            if (tCenter.GetComponent<TweenRotation>() != null && tCenter.GetComponent<TweenRotation>().enabled)
            {
                tCenter.GetComponent<TweenRotation>().enabled = false;
                tCenter.localRotation = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                tCenter.localRotation = Quaternion.Euler(Vector3.zero);
            }



            TweenScale.Begin(tCenter.gameObject, tweenTime, tlocalScale).method = UITweener.Method.EaseOut;
            float tDistance = Mathf.Abs(tCenter.localPosition.x);
           
            float tTime = tDistance > closeDis ? tweenTime :  tweenTime / closeDis * tDistance  ;
            if( Mathf.Abs(tCenter.localPosition.x) > 3)
                TweenPosition.Begin(tCenter.gameObject,tTime,Vector3.zero).method = UITweener.Method.EaseOut ;
            else 
                tCenter.transform.localPosition = Vector3.zero;

            this.ChangeDepth(tCenter, 1000);
			List<Transform> tram = myGrid.GetChildList();
			UITexture texture1 = tCenter.GetComponentInChildren<UITexture>();
			if(texture1 != null)
			{
				if(texture1.material == null)
				{
					SetTextureMaterial();
					return;
				}
				texture1.material.SetFloat("_BlurValur",0.0f);
				texture1.material.SetFloat("_AlphaValur",1.0f);
				Transform ss = texture1.transform.parent.FindChild("background");
				if(ss != null)
				{
					UISprite bg = ss.GetComponent<UISprite>();
					if(bg != null)bg.alpha = 1.0f;
				}

			}
			foreach(Transform tr in tram)
			{
				if(tr.name != tCenter.name)
				{
					UITexture texture = tr.GetComponentInChildren<UITexture>();
					if(texture != null)
					{
						texture.material.SetFloat("_BlurValur",0.01f);
						texture.material.SetFloat("_AlphaValur",0.5f);
						Transform ss = texture.transform.parent.FindChild("background");
						if(ss != null)
						{
							UISprite bg = ss.GetComponent<UISprite>();
							if(bg != null)bg.alpha = 0.7f;
						}
					}
				}
			}

        }
    }

    //整体 depth 提高
    void ChangeDepth(Transform tCenter, int offset, bool isSet = false)
    {
        if (tCenter != null)
        {
            if (!isSet)
            {
                foreach (UISprite tS in tCenter.GetComponentsInChildren<UISprite>())
                {
                    tS.depth += offset;
                }
				foreach (UITexture tx in tCenter.GetComponentsInChildren<UITexture>())
				{
					tx.depth += offset;
				}
			} else
            {
                foreach (UISprite tS in tCenter.GetComponentsInChildren<UISprite>())
                {
                    int tD = tS.depth % 100;
                    tS.depth = tD + offset;
                }
				foreach (UITexture tx in tCenter.GetComponentsInChildren<UITexture>())
				{
					int tD = tx.depth % 100;
					tx.depth = tD + offset;
				}
            }
        }
    }

    void OtherRotate(Transform tOther, int dir,int midIndex)
    {
        if (tOther != null)
        {
            Vector3 targEuler = Vector3.zero;
            TweenScale.Begin(tOther.gameObject, tweenTime, otherScale).method =  UITweener.Method.EaseOut;
//            Vector3 fromEuler = Vector3.zero;
            if (myGrid != null)
            {
                if (midIndex - myGrid.GetIndex(tOther)  >= 1)
                {
                    targEuler = Vector3.down * farEuler;
                }
            }
            if (dir < 0)
            {
                TweenRotation tR = tOther.GetComponent<TweenRotation>();
                if (tR != null)
                {
                    tR.from = targEuler;
                    targEuler = Vector3.down * farEuler;
                    tR.to = targEuler;
                }
                else
                {
                    tR = tOther.gameObject.AddComponent<TweenRotation>();
                    tR.from = targEuler;
                    targEuler = Vector3.down * farEuler;
                    tR.to = targEuler;
                }
                tR.duration = tweenTime;
                tR.method = UITweener.Method.EaseOut;
                tR.ResetToBeginning();
                tR.Play();
            } else
            {
                targEuler = Vector3.up * farEuler;
                Quaternion targetQ =  Quaternion.Euler(targEuler);
                TweenRotation.Begin(tOther.gameObject, tweenTime, targetQ).method =  UITweener.Method.EaseOut;
            }
        }

      
    }

    void OtherPosition(Transform tOther, int dir, int index)
    {
        if (tOther != null)
        {
            //index 是center 的序号
            if (myGrid == null)
                myGrid = GetComponent<UIGrid>();
            int cellCount = myGrid.GetChildList().Count;
            Vector3 moveOffset = Vector3.zero;
            if (myGrid != null)
            {
                int curInd = myGrid.GetIndex(tOther);
                Vector3 posCach =Vector3.zero; //myGrid.GetChild(index).transform.localPosition;
                if (cellCount <= 5)
                {
                    //左
                    if (dir < 0)
                    {
                        //  * 最近一个 
                        if (curInd == index - 1)
                        {
                            moveOffset = Vector3.left * closeDis + posCach;
                        } else
                        {
                            if (index != 1)
                            {
                                float tDis = (maxWidth - closeDis) / (index - 1);
                                moveOffset = Vector3.left * (closeDis + tDis * (index - curInd - 1)) + posCach;

                            } else
                            {
                                Debug.LogWarning(" logical  is wrong  : index = " + index + " name = " + tOther.name);
                            }
                        }

                        ChangeDepth(tOther, 100 * curInd, true);
                    } else
                    {
                        //右
                        if (curInd == index + 1)
                        {
                            moveOffset = Vector3.right * closeDis + posCach;
                        } else
                        {
                            if (index != cellCount - 1)
                            {
                                float tDis = (maxWidth - closeDis) / (cellCount - 2 - index);
                                moveOffset = Vector3.right * (closeDis + tDis * (curInd - index - 1)) + posCach;

                            } else
                            {
                                Debug.LogWarning(" logical  is wrong  : index = " + index + " name = " + tOther.name);
                            }
                        }

                        ChangeDepth(tOther, 100 * (cellCount - curInd), true);
                    }
                }
                TweenPosition.Begin(tOther.gameObject, 0.2f, moveOffset).method =  UITweener.Method.EaseIn;;
            }
        }
    }

    public void OtherTransform(Transform tOther, int dir, int index)
    {
        this.OtherPosition(tOther, dir, index);
        this.OtherRotate(tOther, dir, index);
    }

    //时时计算  旋转
    public void OnUpdateCalculate(Vector3 moveOffset)
    {
        GameObject tObj = centerChild.centeredObject;
        if (tObj != null)
        {
            if(myGrid == null)
                myGrid = GetComponent<UIGrid>();
            if (myGrid != null)
            {
                int midInd = myGrid.GetIndex(tObj.transform);
                int maxInd = myGrid.GetChildList().Count;

                if(moveOffset.x >0 && midInd ==0 ) 
                    return;
                if(moveOffset.x < 0 && midInd == maxInd -1 )
                    return;

                foreach (Transform tR  in myGrid.GetChildList())
                {
                    int curInd = myGrid.GetIndex(tR);
                    if (tR.gameObject == tObj)
                    {
                        OnCalculate(moveOffset, tR, true);
                    } else
                    {
                        //<0 
                        if (Mathf.Abs(midInd - curInd) == 1)
                        {
                            if (curInd > midInd)
                            {
                                if (moveOffset.x > 0)
                                    OnCalculate(moveOffset, tR, false);
                                else
                                    OnCalculate(moveOffset, tR, true);
                            } else
                            {
                                if (moveOffset.x > 0)
                                    OnCalculate(moveOffset, tR, true);
                                else
                                    OnCalculate(moveOffset, tR, false);
                            }
                        } else
                        {
                            OnCalculate(moveOffset, tR, false);
                        }
                    }
                }   
            }
        }
    }

    void OnCalculate(Vector3 moveOffset, Transform tTarget, bool isRotate = false)
    {
        if (centerChild.centeredObject != null)
        {
            GameObject tObj = centerChild.centeredObject;
            float offset = Mathf.Abs(moveOffset.x);
            float tFactor = offset / closeDis;

            if (tTarget.gameObject == tObj)
            {
                float dis = offset >= closeDis ? closeDis : offset;
                // dis  0-200    rot  0 - 60
                if (isRotate)
                {
                    float tRotation = farEuler / closeDis * dis;
                    Quaternion targetQ = moveOffset.x >= 0 ? Quaternion.Euler(new Vector3(0, tRotation, 0)) : Quaternion.Euler(new Vector3(0, -tRotation, 0));
                    tObj.transform.localRotation = targetQ;
                }
                tObj.transform.localPosition = moveOffset.x >= 0 ? Vector3.right * dis : Vector3.left * dis;
                tObj.transform.localScale = Vector3.one * Mathf.Lerp(1.1f, 0.9f, tFactor);

            } else
            {
                //  rotation  0 - 60
                if (isRotate)
                {
                    Vector3 tRot = tTarget.transform.localRotation.eulerAngles;
                    if (moveOffset.x > 0)
                        tRot = new Vector3(0, Mathf.Lerp(-farEuler, 0, tFactor), 0);
                    else if (moveOffset.x < 0)
                        tRot = new Vector3(0, Mathf.Lerp(farEuler, 0, tFactor), 0);
                    tTarget.localRotation = Quaternion.Euler(tRot);
                    tTarget.localScale = Vector3.one * Mathf.Lerp(0.9f, 1.1f, tFactor);
                }

                //  position
                float dis = maxWidth - closeDis;
                int curInd = myGrid.GetIndex(tTarget);
                int midInd = myGrid.GetIndex(tObj.transform);
                int maxCount = myGrid.GetChildList().Count;
                Vector3 xOffset = Vector3.zero;

                if (moveOffset.x > 0)
                {
                    if (curInd < midInd)
                    {
                        if (curInd != midInd - 1)
                        {
                            float curInterval = midInd == 1 ? closeDis : dis / (midInd - 1);
                            float nextInterval = 0;
                            float tTo =0;

                            if (midInd > 2)
                            {
                                nextInterval = dis / (midInd - 2);
                                tTo  =  curInd == 0 ? -maxWidth : -maxWidth + nextInterval * curInd ;
                            } else
                            {
                                nextInterval = dis;
                                tTo = midInd ==1 ? 0: curInd ==0? -closeDis: 0;
                            }
                            float tFrom = midInd == 1 ? -closeDis : -maxWidth + curInterval * curInd;
                            xOffset =  Vector3.right * Mathf.Lerp(tFrom, tTo, tFactor);
                        } else
                        {
                            xOffset = Vector3.right * Mathf.Lerp(-closeDis, 0, tFactor);
                        }
                        
                    } else
                    {
                        // not = 
                        if (maxCount - midInd > 2)
                        {
                            float curInterval = dis / (maxCount - midInd - 2);
                            float nextInterval = dis / (maxCount - midInd - 1);
                            float tFrom = closeDis + curInterval * (curInd - midInd - 1);
                            float tTo =  closeDis + nextInterval * (curInd - midInd );
                            xOffset = Vector3.right * Mathf.Lerp(tFrom, tTo, tFactor);
                        } else
                        {
                            //maxcout - midint < 2 
                            if (maxCount == midInd + 2)
                                xOffset = Vector3.right * Mathf.Lerp(closeDis, maxWidth, tFactor);
                        }

                    }
                } else
                {
                    if (curInd < midInd)
                    {
                        float curInterval = midInd == 1 ? closeDis : dis / (midInd - 1);
                        float nextInterval = 0;
                        if (midInd >= 2)
                        {
                            nextInterval = dis / midInd;
                        } else
                        {
                            nextInterval = dis;
                        }

                        float tFrom = midInd == 1 ? -closeDis : -maxWidth + curInterval * curInd;
                        float tTo = midInd == 1 ? -maxWidth : -maxWidth + nextInterval * curInd; 
                        xOffset = Vector3.right * Mathf.Lerp(tFrom, tTo, tFactor);

                    } else
                    {
                        // != 
                        if (maxCount - midInd > 3)
                        {
                            float curInterval = dis / (maxCount - midInd - 2);
                            float nextInterval = dis / (maxCount - midInd - 3);
                            float tFrom = isRotate ?  closeDis  : closeDis + curInterval * (curInd - midInd - 1);
                            float tTo = isRotate ?  0 :  closeDis + nextInterval * (curInd - midInd - 2);
                            xOffset = Vector3.right * Mathf.Lerp(tFrom, tTo, tFactor);
                        } else
                        {
                            xOffset = isRotate ? Vector3.right * Mathf.Lerp(closeDis, 0, tFactor) : Vector3.right * Mathf.Lerp(maxWidth, closeDis, tFactor);    
                        }
                    }
                }
                tTarget.localPosition = xOffset;
            }
        }
    }


}
