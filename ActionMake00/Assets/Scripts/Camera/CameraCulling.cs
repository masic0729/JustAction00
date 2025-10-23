using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[System.Serializable]
public struct LayerData
{
    public string layerName;
    public float distance;
}

public class CameraCulling : MonoBehaviour
{
    Camera camera;
    [SerializeField] LayerData[] layerDatas;


    void Start()
    {
        camera = GetComponent<Camera>();
        if (camera == null)
            return;

        //각 레이어 간의 식별 거리를 적용하기 위해 만든 배열
        //해당 배열이 있어야 레이어를 정확한 인덱스에 적용할 수 있다
        float[] distanceDatas = new float[32];

        //camera.layerCullDistances[3] = 50f;

        for(int i = 0; i < layerDatas.Length; i++)
        {
            int index = LayerMask.NameToLayer(layerDatas[i].layerName);
            distanceDatas[index] = layerDatas[i].distance;
        }

        camera.layerCullDistances = distanceDatas;
    }
}