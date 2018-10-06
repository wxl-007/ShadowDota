using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnitySphereCollider
{
    [TaskCategory("Basic/SphereCollider")]
    [TaskDescription("Sets the center of the SphereCollider. Returns Success.")]
    public class SetCenter : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The center of the SphereCollider")]
        public SharedVector3 center;

        private SphereCollider sphereCollider;

        public override void OnStart()
        {
            sphereCollider = GetDefaultGameObject(targetGameObject.Value).GetComponent<SphereCollider>();
        }

        public override TaskStatus OnUpdate()
        {
            if (sphereCollider == null) {
                Debug.LogWarning("SphereCollider is null");
                return TaskStatus.Failure;
            }

            sphereCollider.center = center.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            center = Vector3.zero;
        }
    }
}