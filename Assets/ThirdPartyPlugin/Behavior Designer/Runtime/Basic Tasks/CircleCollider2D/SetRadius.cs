#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCircleCollider2D
{
    [TaskCategory("Basic/CircleCollider2D")]
    [TaskDescription("Sets the radius of the CircleCollider2D. Returns Success.")]
    public class SetRadius : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The radius of the CircleCollider2D")]
        public SharedFloat radius;

        private CircleCollider2D circleCollider2D;

        public override void OnStart()
        {
            circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        }

        public override TaskStatus OnUpdate()
        {
            if (circleCollider2D == null) {
                Debug.LogWarning("CircleCollider2D is null");
                return TaskStatus.Failure;
            }

            circleCollider2D.radius = radius.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            radius = 0;
        }
    }
}
#endif