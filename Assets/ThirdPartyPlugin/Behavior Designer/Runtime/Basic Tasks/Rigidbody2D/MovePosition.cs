#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4)
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
    [TaskCategory("Basic/Rigidbody")]
    [TaskDescription("Moves the Rigidbody to the specified position. Returns Success.")]
    public class MovePosition : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The new position of the Rigidbody")]
        public SharedVector2 position;

        // cache the rigidbody component
        private UnityEngine.Rigidbody2D targetRigidbody;

        public override void OnStart()
        {
            targetRigidbody = GetDefaultGameObject(targetGameObject.Value).GetComponent<UnityEngine.Rigidbody2D>();
        }

        public override TaskStatus OnUpdate()
        {
            if (targetRigidbody == null) {
                Debug.LogWarning("Rigidbody is null");
                return TaskStatus.Failure;
            }

            targetRigidbody.MovePosition(position.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            position = Vector2.zero;
        }
    }
}
#endif