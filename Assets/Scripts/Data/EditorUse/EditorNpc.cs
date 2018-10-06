using UnityEngine;
using System.Collections;

public enum NpcCamp
{
    Enemy,
    Neutral,
    Allies
}

[ExecuteInEditMode]
public class EditorNpc : MonoBehaviour {
    private BoxCollider box;
    private GameObject currentObj;

    public GameObject model;

    public NpcCamp camp;

	// Use this for initialization
	void Start () {
        box = GetComponent<BoxCollider>();
        //if(model != null && !Application.isPlaying)
        //{
        //    currentObj = Instantiate(model, transform.position, Quaternion.identity) as GameObject;
        //}
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void ChangeModel()
    {
        DestroyImmediate(currentObj);
        currentObj = Instantiate(model, transform.position, Quaternion.identity) as GameObject;
        currentObj.transform.parent = transform;
    }

    public void LoadModel(string name)
    {
        DestroyImmediate(currentObj);
        model = Resources.Load<GameObject>("Heroes/" + name);
        currentObj = Instantiate(model, transform.position, transform.rotation) as GameObject;
        currentObj.transform.parent = transform;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, box.size);
        Gizmos.DrawIcon(transform.position + Vector3.forward, "Hero.jpg", true);
    }
}
