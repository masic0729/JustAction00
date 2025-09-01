using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    public GameObject test;
    // Start is called before the first frame update
    void Start()
    {
        test = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
