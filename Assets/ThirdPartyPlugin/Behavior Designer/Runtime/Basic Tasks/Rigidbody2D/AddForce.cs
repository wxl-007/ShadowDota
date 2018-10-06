#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Rigidbody2D
{
    [TaskCategory("Basic/Rigidbody2D")]
    [TaskDescription("Applies a force to the Rigidbody2D. Returns Success.")]
    public class AddForce : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The amount of force to apply")]
        public SharedVector2 force;

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

            targetRigidbody2D.AddForce(force.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            force = Vector2.zero;
        }
    }
}
#endif