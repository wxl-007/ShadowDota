using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
    [TaskCategory("Basic/Rigidbody")]
    [TaskDescription("Sets the constraints of the Rigidbody. Returns Success.")]
    public class SetConstraints : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The constraints of the Rigidbody")]
        public RigidbodyConstraints constraints = RigidbodyConstraints.None;

        // cache the rigidbody component
        private Rigidbody targetRigidbody;

        public override void OnStart()
        {
            targetRigidbody = GetDefaultGameObject(targetGameObject.Value).GetComponent<Rigidbody>();
        }

        public override TaskStatus OnUpdate()
        {
            if (targetRigidbody == null) {
                Debug.LogWarning("Rigidbody is null");
                return TaskStatus.Failure;
            }

            targetRigidbody.constraints = constraints;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            constraints = RigidbodyConstraints.None;
        }
    }
}