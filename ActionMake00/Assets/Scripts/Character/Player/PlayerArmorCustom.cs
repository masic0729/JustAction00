using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerArmorCustom : MonoBehaviour
{
    /*
    플레이어 방어구의 외적 요소는 이러하다.
    참조하여 교체될 수 있는 구조이다.
    장비 타입에 따라 1개에서 여러 파츠를 복수로

    단, 무기는 단독으로 교체되는 방식이기에, 존재하지 않는다
    모자 - 모자
    상의 - 상의, 팔, 벨트
    신발 - 다리, 발

     */

    /*[SerializeField] GameObject[] heads;
    [SerializeField] GameObject[] chests;
    [SerializeField] GameObject[] arms;
    [SerializeField] GameObject[] belts;
    [SerializeField] GameObject[] legs;
    [SerializeField] GameObject[] feets;*/
    List<SkinnedMeshRenderer[]> parts = new List<SkinnedMeshRenderer[]>();

    enum PLAYERARMORS
    {
        HEADS = 0,
        CHESTS,
        ARMS,
        BELTS,
        LEGS,
        FEET
    }
    PLAYERARMORS armors;

    // Start is called before the first frame update
    void Start()
    {
        int partsLength = System.Enum.GetValues(typeof(PLAYERARMORS)).Length;

        for (int i = 0; i < partsLength; i++)
        {
            PLAYERARMORS armorType = (PLAYERARMORS)i;

            GameObject part = this.gameObject.transform.Find(armorType.ToString()).gameObject;
            parts.Add(part.transform.GetComponentsInChildren<SkinnedMeshRenderer>());

        }
        Debug.Log(parts.Count + "히히");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 장비 착용/해제 및 교체 시 플레이어 내 이미 존재하는
    /// 장비들을 활성화 및 비활성화하는 방식으로 구동된다.
    /// 인덱스는 0~2까지 존재하고
    /// 장비가 없는 경우는 인덱스가 0이다. 
    /// </summary>
    /// <param name="equipType"></param>
    /// <param name="index"></param>
    public void SetPlayerArmorVisual(EquipmentType equipType, int index)
    {
        List<Transform> parts = new List<Transform>();
        this.gameObject.transform.Find("");
        switch(equipType)
        {
            case EquipmentType.None:
                Debug.Log("잘못된 접근. 확인 요망");
                return;

            case EquipmentType.Head:
                parts.Add(this.gameObject.transform.Find("HEADS"));
                break;
            case EquipmentType.Top:
                parts.Add(this.gameObject.transform.Find("CHESTS"));
                parts.Add(this.gameObject.transform.Find("ARMS"));
                parts.Add(this.gameObject.transform.Find("BELTS"));
                break;
            case EquipmentType.Bottom:
                parts.Add(this.gameObject.transform.Find("LEGS"));
                parts.Add(this.gameObject.transform.Find("FEET"));
                break;

            default:
                Debug.Log("기본적인 파라미터 오류");
                return;
        }

        TransArmorVisuals(parts, index);
    }

    void TransArmorVisuals(List<Transform> parts, int index)
    {
        if(index < 0 || index > 2)
        {
            Debug.Log("인덱스 값이 잘못들어감. 확인 요망");
            return;
        }
        SkinnedMeshRenderer[] data = GetComponentsInChildren<SkinnedMeshRenderer>();

        for(int i = 0; i < parts.Count; i++)
        {
            if(i == index)
            {
                parts[i].gameObject.SetActive(true);
            }
            else
            {
                parts[i].gameObject.SetActive(false);
            }
        }
    }
    
}
