using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using AW.War;
using AW.Data;

#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
#endif

namespace BehaviorDesigner.Samples
{
    // Move towards the target specified using Unity's nav mesh
    [TaskCategory("Common")]
    public class Seek : Action
    {
		[Tooltip("移动速度")]
        public SharedFloat moveSpeed;
		[Tooltip("旋转速度")]
        public SharedFloat rotationSpeed;
        [Tooltip("true if the nav agent's rotation should end with the same rotation as the target")]
        public bool rotateToTarget;
        [Tooltip("avoid running into objects who are on defense. Note that this probabily should be made into a tag instead but to prevent having to update project files " +
                 "with these demos we are doing it this way")]
        public bool avoidDefeneUnits;
        [Tooltip("The position that the agent is seeking")]
        public SharedVector3 targetPosition;
        [Tooltip("The target that the agent is seeking")]
		public SharedLifeNPC target;

        // remember the magnitude within the previous frame so we know if the target respawns and we no longer need to seek the target
//        private float prevMagnitude = Mathf.Infinity;
        // true if the target was obtained from the targets position
        private bool staticPosition = false;
        // true if the nav agent is currently on an alternate path to avoid the defensive object
        private bool alternatePath = false;

        private AIPath navMeshAgent;
        private ServerLifeNpc myHero;

		private WarMsgParam param;
	

		RaycastHit hit;
		Vector3 hitPoint;

		public  Vector3 tgtPos
		{
			get
			{
				Vector3 pos = target.Value.transform.position;
				Vector3 dir = Vector3.Normalize (pos - this.transform.position);
				Vector3 radius = dir * target.Value.data.configData.radius;
				Vector3 finalPos = (pos - radius);
//				finalPos.y = 1;
				return finalPos;
			}
		}


        public override void OnAwake()
        {
            myHero = GetComponent<ServerLifeNpc>();

			if (myHero.data.configData.moveable == Moveable.Movable)
			{
				// cache for quick lookup
//				navMeshAgent = gameObject.GetComponent<NavMeshAgent> ();
                navMeshAgent = myHero.pathFinding;

				// set the speed and angular speed
//				navMeshAgent.angularSpeed = rotationSpeed.Value;
//				navMeshAgent.speed = myHero.data.configData.speed;

				param = new WarMsgParam ();
			}
        }

        public override void OnStart()
        {
			if (myHero.data.configData.moveable == Moveable.BeStatic)
				return;

			navMeshAgent.enabled = true;

            // use the position if it is not zero
            if (targetPosition.Value != Vector3.zero) {
                staticPosition = true;
                navMeshAgent.destination = targetPosition.Value;
            }

            // set the destination if it hasn't already been set with a static position
            if (staticPosition == false) 
			{
				navMeshAgent.destination = tgtPos;
            }
        }

        // Move towards the destination. Return success once we have reached the destination. Return failure if the destination has respawned and we no longer should be seeking it.
        // Will return running if we are currently seeking
        public override TaskStatus OnUpdate()
        {
			myHero.data.btData.btStatus = NPCBattle_Status.Seeking;

			if (myHero.data.configData.moveable == Moveable.BeStatic)
				return TaskStatus.Success;

			if (!target.Value.IsAlive)
				return TaskStatus.Failure;

			if (!navMeshAgent.enabled)
				navMeshAgent.enabled = true;
				
            if (myHero.mAnimState.canMove && myHero.mAnimState.STATE != NpcAnimState.Run)
			{
//				param.cmdType = WarMsg_Type.Running;
//				param.Sender = myHero.UniqueID;
//				param.Receiver = myHero.UniqueID;
//				myHero.SendMsg (myHero.UniqueID, param);

                myHero.SendAnimMsg(WarMsg_Type.Running);
			}

            // use the nav agent's destination position if we are on an alternate path or the target is null. We are using an alternate path if the previous path would have collided with
            // an object on defense. target will be null when we are seeking a position specified by the position variable
			var targetPosition = (alternatePath || target.Value == null ? navMeshAgent.destination : tgtPos);
			targetPosition.y = 1;// ignore y

            // we can only arrive if the path isn't pending

                var thisPosition = transform.position;
                thisPosition.y = targetPosition.y;
                // If the magnitude is less than the arrive magnitude then we have arrived at the destination


				if (AITools.IsInRange(thisPosition, myHero.ATKRange + myHero.data.configData.radius, navMeshAgent.destination)) 
				{

                    // If we arrived from an alternate path then switch back to the regular path
                    if (alternatePath) {
                        alternatePath = false;
						targetPosition = tgtPos;
					} else {

                        // return success if we don't need to rotate to the target or we are already at the target's rotation
//						if (!rotateToTarget || transform.rotation == target.Value.transform.rotation) {
//							return TaskStatus.Success;
//                        }
					
						return TaskStatus.Success;
                        // not done yet. still need to rotate
//						transform.rotation = Quaternion.RotateTowards(transform.rotation, target.Value.transform.rotation, rotationSpeed.Value * Time.deltaTime);
                    }
                }

                // fail if the target moved too quickly in one frame. This happens after the target has been caught and respawns
//                float distance;
//                if (prevMagnitude * 2 < (distance = Vector3.SqrMagnitude(thisPosition - targetPosition))) {
//					return TaskStatus.Failure;
//                }
//                prevMagnitude = distance;


            // try not to head directly for a defensive object
			if (avoidDefeneUnits && (rayCollision(transform.position - transform.right * 1, targetPosition, out hit) ||
				rayCollision(transform.position + transform.right * 1, targetPosition, out hit))) {
                // looks like an object is within the path. Avoid the object by setting a new destination towards the right of the object that we would have hit
				hitPoint = hit.point + transform.right * 3;

//				hitPoint.y = transform.position.y;
				hitPoint.y = 1;

                if (myHero.mAnimState.canMove)
				{
					navMeshAgent.destination = hitPoint;
				}

                // The avoid object may still be in the way even though we moved to the right of the object. If this is the case then move to the left and hope that works
                if (rayCollision(transform.position, navMeshAgent.destination, out hit)) {
					hitPoint = hit.point - transform.right * 4;
//					hitPoint.y = transform.position.y;
					hitPoint.y = 1;
                    if (myHero.mAnimState.canMove)
					{
						navMeshAgent.destination = hitPoint;
					}
                }

                // remember that we are taking an alternate path to prevent the agent from jittering back and forth
                alternatePath = true;
//                var thisPosition = transform.position;
//                thisPosition.y = hitPoint.y;
//                prevMagnitude = Vector3.SqrMagnitude(thisPosition - hitPoint);
            } else if (navMeshAgent.destination != targetPosition) {
                // the target position has changed since we last set the destination. Update the destination

//				if (myHero.playHero.CanMove)
                if (myHero.mAnimState.canMove)
				{
					navMeshAgent.destination = targetPosition;
				}
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            // reset the variables and disable the nav mesh agent when the task ends
            alternatePath = false;
//            prevMagnitude = Mathf.Infinity;

			myHero.data.btData.btStatus = NPCBattle_Status.None;

			if(navMeshAgent != null)
            	navMeshAgent.enabled = false;
        }

        // cast a ray between startPosition and targetPosition. Return true if a defensive object was hit
        private bool rayCollision(Vector3 startPosition, Vector3 targetPosition, out RaycastHit hit)
        {
			startPosition.y = 1;
			targetPosition.y = 1;

			float dis = AITools.GetSqrDis (this.transform.position, target.Value.transform.position);
			if(dis <= 0)
				dis = 1.0f;

			if (avoidDefeneUnits && Physics.Raycast(startPosition, targetPosition - startPosition, out hit, dis)) 
			{
                ServerLifeNpc npc = null;
                if ((npc = hit.collider.GetComponent<ServerLifeNpc>()) != null) 
				{
					return (npc.Camp == myHero.Camp) 
						&& (npc.UniqueID != myHero.UniqueID) 
						&& (npc.ATKRange == myHero.ATKRange);
                }
            }
	        hit = new RaycastHit();
            return false;
        }


		public override void OnDrawGizmos()
		{
//			#if UNITY_EDITOR && DEBUG
//			var oldColor = UnityEditor.Handles.color;
//			UnityEditor.Handles.color = Color.green;
//
//			if(target.Value != null)
//			{
//				if(navMeshAgent != null) {
//					UnityEditor.Handles.DrawLine(this.transform.position, navMeshAgent.destination);
//				} 
//				UnityEditor.Handles.DrawWireDisc(target.Value.transform.position, transform.up, target.Value.data.configData.radius + myHero.ATKRange);
//			}
//
//			UnityEditor.Handles.color = oldColor;
//			#endif
		}
    }
}