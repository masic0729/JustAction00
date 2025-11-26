using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSameTarget : MonoBehaviour
{
    public TestCube test1;
    public TestCube test2;
    // Start is called before the first frame update
    void Start()
    {
        if(test1 == test2)
        {
            Debug.Log("유니티는 똑같이 취급함");
        }
        else
        {
            Debug.Log("Not Same Object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
