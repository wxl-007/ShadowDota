using UnityEngine;
using System.Collections;
using AW.War;

public interface ISkillEffect
{
    void EmitEffect(ClientNPC _from, ClientNPC _target, bool _needUpdate);
    void LifeTime(float lifeTime);
}

public abstract class SkillEffectBase : MonoBehaviour, ISkillEffect {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract void EmitEffect(ClientNPC _from, ClientNPC _target, bool _needUpdate);
    public abstract void LifeTime(float lifeTime);
}
