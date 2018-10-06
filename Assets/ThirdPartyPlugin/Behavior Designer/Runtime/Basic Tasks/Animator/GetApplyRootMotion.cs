using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
    [TaskCategory("Basic/Animator")]
    [TaskDescription("Stores if root motion is applied. Returns Success.")]
    public class GetApplyRootMotion : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Is root motion applied?")]
        [RequiredField]
        public SharedBool storeValue;

        private Animator animator;

        public override void OnStart()
        {
            animator = GetDefaultGameObject(targetGameObject.Value).GetComponent<Animator>();
        }

        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = animator.applyRootMotion;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = false;
        }
    }
}