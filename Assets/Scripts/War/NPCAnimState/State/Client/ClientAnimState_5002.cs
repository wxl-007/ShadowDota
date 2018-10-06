using UnityEngine;
using System.Collections;
using AW.Resources;

namespace AW.War
{
    public class ClientAnimState_5002 : ClientNpcAnimState {

        public GameObject child;

    	// Use this for initialization
        public override void Start () {
            base.Start();

    	}
    	
    	// Update is called once per frame
        public override void Update () {

    	}

        public override void On_Destroy(WarMsgParam param)
        {
            if(param.cmdType == WarMsg_Type.Destroy)
            {
                if(child != null)
                {
                    child = Instantiate(child, transform.position, transform.rotation) as GameObject;
                    Destroy(child, 2f);
                }
                Destroy(gameObject);
            }
        }
    }
}
