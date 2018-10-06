#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityBoxCollider2D
{
    [TaskCategory("Basic/BoxCollider2D")]
    [TaskDescription("Stores the center of the BoxCollider2D. Returns Success.")]
    public class GetCenter : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The center of the BoxCollider2D")]
        [RequiredField]
        public SharedVector2 storeValue;

        private BoxCollider2D boxCollider2D;

        public override void OnStart()
        {
            boxCollider2D = GetDefaultGameObject(targetGameObject.Value).GetComponent<BoxCollider2D>();
        }

        public override TaskStatus OnUpdate()
        {
            if (boxCollider2D == null) {
                Debug.LogWarning("BoxCollider2D is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = boxCollider2D.center;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = Vector2.zero;
        }
    }
}
#endif