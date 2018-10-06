using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
    [TaskCategory("Basic/Transform")]
    [TaskDescription("Stores the rotation of the Transform. Returns Success.")]
    public class GetRotation : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The rotation of the Transform")]
        [RequiredField]
        public SharedQuaternion storeValue;

        private Transform targetTransform;

        public override void OnStart()
        {
            targetTransform = GetDefaultGameObject(targetGameObject.Value).GetComponent<Transform>();
        }

        public override TaskStatus OnUpdate()
        {
            if (targetTransform == null) {
                Debug.LogWarning("Transform is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = targetTransform.rotation;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = Quaternion.identity;
        }
    }
}