using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    

    public void PlayParticle(ParticleSystem ps, Transform tr)
    {
        ParticleSystem instance = Instantiate(ps.gameObject, tr.position, tr.rotation).GetComponent<ParticleSystem>();
        instance.Play();
        Destroy(instance, instance.main.duration);
    }
}
