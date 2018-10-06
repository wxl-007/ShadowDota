using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCapsuleCollider
{
    [TaskCategory("Basic/CapsuleCollider")]
    [TaskDescription("Sets the center of the CapsuleCollider. Returns Success.")]
    public class SetCenter : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The center of the CapsuleCollider")]
        public SharedVector3 center;

        private CapsuleCollider capsuleCollider;

        public override void OnStart()
        {
            capsuleCollider = GetDefaultGameObject(targetGameObject.Value).GetComponent<CapsuleCollider>();
        }

        public override TaskStatus OnUpdate()
        {
            if (capsuleCollider == null) {
                Debug.LogWarning("CapsuleCollider is null");
                return TaskStatus.Failure;
            }

            capsuleCollider.center = center.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            center = Vector3.zero;
        }
    }
}