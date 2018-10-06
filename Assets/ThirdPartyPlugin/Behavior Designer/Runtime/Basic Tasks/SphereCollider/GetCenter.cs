using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnitySphereCollider
{
    [TaskCategory("Basic/SphereCollider")]
    [TaskDescription("Stores the center of the SphereCollider. Returns Success.")]
    public class GetCenter : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The center of the SphereCollider")]
        [RequiredField]
        public SharedVector3 storeValue;

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

            storeValue.Value = sphereCollider.center;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            storeValue = Vector3.zero;
        }
    }
}