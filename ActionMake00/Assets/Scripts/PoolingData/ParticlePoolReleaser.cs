using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolReleaser : MonoBehaviour
{
    ParticleSystem ps;
    float releaseTime = 10f;
    float resetReleaseTime = 0f;

    private void Awake()
    {
        resetReleaseTime = releaseTime;

    }

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

    private void OnEnable()
    {
        releaseTime = resetReleaseTime;
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
        PoolRelease();
    }

    public void PoolRelease()
    {
        if (this.transform.parent != null)
            this.transform.parent = null;
        PoolManager.instance.skillPrefabs[this.gameObject.name].Release(this.gameObject);
    }

    public void SetReleaseTime(float time)
    {
        releaseTime = time;
    }
}
