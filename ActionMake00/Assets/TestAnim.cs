using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    DOTweenAnimation anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<DOTweenAnimation>();
        anim.DOPlay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
