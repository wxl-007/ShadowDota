#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
    [TaskCategory("Basic/Rigidbody2D")]
    [TaskDescription("Returns Success if the Rigidbody2D is kinematic, otherwise Failure.")]
    public class IsKinematic : Conditional
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

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

            return targetRigidbody2D.isKinematic ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}
#endif