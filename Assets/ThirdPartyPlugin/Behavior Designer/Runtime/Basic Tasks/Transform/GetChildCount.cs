using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
    [TaskCategory("Basic/Transform")]
    [TaskDescription("Stores the number of children a Transform has. Returns Success.")]
    public class GetChildCount : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The number of children")]
        [RequiredField]
        public SharedInt storeValue;

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

            storeValue.Value = targetTransform.childCount;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = 0;
        }
    }
}