#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
    [TaskCategory("Basic/Rigidbody2D")]
    [TaskDescription("Stores the fixed angle value of the Rigidbody2D. Returns Success.")]
    public class GetFixedAngle : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The fixed angle value of the Rigidbody2D")]
        [RequiredField]
        public SharedBool storeValue;

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

            storeValue.Value = targetRigidbody2D.fixedAngle;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = false;
        }
    }
}
#endif