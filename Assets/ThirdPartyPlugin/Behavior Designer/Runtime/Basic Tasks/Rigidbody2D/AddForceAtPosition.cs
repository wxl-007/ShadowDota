#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
    [TaskCategory("Basic/Rigidbody2D")]
    [TaskDescription("Applies a force at the specified position to the Rigidbody2D. Returns Success.")]
    public class AddForceAtPosition : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The amount of force to apply")]
        public SharedVector2 force;
        [Tooltip("The position of the force")]
        public SharedVector2 position;

        private UnityEngine.Rigidbody2D targetRigidbody2D;

        public override void OnStart()
        {
            targetRigidbody2D = GetDefaultGameObject(targetGameObject.Value).GetComponent<UnityEngine.Rigidbody2D>();
        }

        public override TaskStatus OnUpdate()
        {
            if (targetRigidbody2D == null) {
                Debug.LogWarning("Rigidbody2D is null");
                return TaskStatus.Failure;
            }

            targetRigidbody2D.AddForceAtPosition(force.Value, position.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            force = Vector2.zero;
            position = Vector2.zero;
        }
    }
}
#endif