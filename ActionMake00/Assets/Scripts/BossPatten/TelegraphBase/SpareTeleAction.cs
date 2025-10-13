using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 임시용 스크립트. 패턴이 다양화함에 따라 다양항 텔레그래피를 요구하면 다시 설계해야할 것
/// </summary>
public class SpareTeleAction : MonoBehaviour
{
    [SerializeField] GameObject ob;
    float targetScale;
    float currentScale = 0f;
    // Start is called before the first frame update
    void Start()
    {
        targetScale = ob.transform.localScale.x;
        ob.transform.localScale = new Vector3(0, ob.transform.localScale.y, 0);
    }


    // Update is called once per frame
    void Update()
    {
        currentScale += Time.deltaTime * 3f;
        if (currentScale >= targetScale)
            currentScale = targetScale;

        ob.transform.localScale = new Vector3(currentScale,ob.transform.localScale.y, currentScale);
    }
}
