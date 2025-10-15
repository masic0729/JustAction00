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

    protected virtual void OnEnable()
    {
        objectCol.enabled = true;
        Invoke("ResetColiderDisable", Time.deltaTime * 20f);
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

        objectCol.enabled = true;
        Invoke("ResetColiderDisable", Time.deltaTime * 20f);

    }

    protected void OnParticleSystemStopped()
    {

        PoolManager.instance.skillPrefabs[this.gameObject.name].Release(this.gameObject);
    }

}
