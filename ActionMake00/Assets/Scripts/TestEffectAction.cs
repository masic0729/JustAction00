using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffectAction : MonoBehaviour
{
    public GameObject testEffect;
    public List<GameObject> testList = new List<GameObject>();
    GameObject ins;

    // Start is called before the first frame update
    void Start()
    {
        ins = Instantiate(testEffect);
        testList.Add(ins);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            testList.Remove(ins);
            Destroy(ins);
        }
    }
}
