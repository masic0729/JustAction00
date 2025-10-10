using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolReleaser : MonoBehaviour
{
    ParticleSystem ps;
    float releaseTime = 10f;
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
        releaseTime -= Time.deltaTime;
        if(releaseTime <=0)
        {
            ParticleSystem[] psDatas = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach(ParticleSystem ps in psDatas)
            {
                ps.Stop();
            }

        }
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
