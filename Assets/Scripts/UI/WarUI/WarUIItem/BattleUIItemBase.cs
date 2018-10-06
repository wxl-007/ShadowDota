using UnityEngine;
using System.Collections;

namespace AW.War
{
    public abstract class BattleUIItemBase : MonoBehaviour
    {
        public WarUI rootUI;
        public abstract void SetItemController(WarUI root);
    }
}
