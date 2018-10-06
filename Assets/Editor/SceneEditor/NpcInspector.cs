using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EditorNpc))]
public class NpcInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(Event.current.type == EventType.DragExited)
        {
            EditorNpc ec = target as EditorNpc;
            if(ec != null)
            {
                ec.ChangeModel();
            }
        }
    }

}
