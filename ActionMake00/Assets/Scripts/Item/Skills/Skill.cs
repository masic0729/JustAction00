using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : Attacker
{
    ParticleSystem ps;
    protected override void Start()
    {
        base.Start();
    }

    protected override void Init()
    {
        base.Init();
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
            return;

        ps.Play();
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    protected void OnParticleSystemStopped()
    {
        Debug.Log("³­ µÆÀ½" + this.gameObject.name);

        PoolManager.instance.skillPrefabs[this.gameObject.name].Release(this.gameObject);
    }

}
