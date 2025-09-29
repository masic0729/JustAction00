using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolReleaser : MonoBehaviour
{
    ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
            return;

        ps.Play();
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnParticleSystemStopped()
    {
        PoolReleaser();
    }

    public void PoolReleaser()
    {
        PoolManager.instance.skillPrefabs[this.gameObject.name].Release(this.gameObject);

    }
}
