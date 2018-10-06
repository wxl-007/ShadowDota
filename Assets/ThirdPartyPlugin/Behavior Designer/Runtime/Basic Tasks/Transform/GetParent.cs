using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
    [TaskCategory("Basic/Transform")]
    [TaskDescription("Stores the parent of the Transform. Returns Success.")]
    public class GetParent : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The parent of the Transform")]
        [RequiredField]
        public SharedTransform storeValue;

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

            storeValue.Value = targetTransform.parent;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = null;
        }
    }
}