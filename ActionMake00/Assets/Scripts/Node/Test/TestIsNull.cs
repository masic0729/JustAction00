using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIsNull : MonoBehaviour
{
    public GameObject thisGameobject;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartTest());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator StartTest()
    {
        Destroy(thisGameobject);

        yield return null; // 한 프레임 기다리기

        if (thisGameobject == null)
            Debug.Log("삭제확인1 (== null)");

        if (thisGameobject is null)
            Debug.Log("삭제확인2 (is null)");
    }
}