using UnityEngine;
using AW.War;
using BehaviorDesigner.Runtime.Tasks;


[TaskDescription("英雄是否是队长")]
[TaskCategory("Hero")]
public class IsHeroCaptain : Conditional
{
	public SharedLifeNPC hero;
	public bool self;
    private ServerLifeNpc npc;
    private WarServerCharactor chaPool;

	public override void OnStart()
	{
		if (self)
            npc = GetComponent<ServerLifeNpc> ();
		else
			npc = hero.Value;

        chaPool = WarServerManager.Instance.realServer.monitor.CharactorPool;
	}

	public override TaskStatus OnUpdate()
	{
        if (chaPool.IsHeroActive(npc))
			return TaskStatus.Success;
		return TaskStatus.Failure;
	}

	public override void OnEnd()
	{

	}
}