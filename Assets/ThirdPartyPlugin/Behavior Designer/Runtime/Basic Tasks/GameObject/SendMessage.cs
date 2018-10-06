using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
    [TaskCategory("Basic/GameObject")]
    [TaskDescription("Sends a message to the target GameObject. Returns Success.")]
    public class SendMessage : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The message to send")]
        public SharedString message;

        public override TaskStatus OnUpdate()
        {
            GetDefaultGameObject(targetGameObject.Value).SendMessage(message.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            message = "";
        }
    }
}