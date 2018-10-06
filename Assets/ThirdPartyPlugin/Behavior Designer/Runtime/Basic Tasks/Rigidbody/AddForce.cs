using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
    [RequiredComponent(typeof(Rigidbody))]
    [TaskCategory("Basic/Rigidbody")]
    [TaskDescription("Applies a force to the rigidbody. Returns Success.")]
    public class AddForce : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The amount of force to apply")]
        public SharedVector3 force;
        [Tooltip("The type of force")]
        public ForceMode forceMode = ForceMode.Force;

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

            targetRigidbody.AddForce(force.Value, forceMode);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            if (force != null) {
                force.Value = Vector3.zero;
            }
            forceMode = ForceMode.Force;
        }
    }
}