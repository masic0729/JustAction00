using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject slot;
    Transform inventoryTransform;
    const int slotCount = 40;

    private void Awake()
    {
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Init()
    {
        inventoryTransform = this.gameObject.transform.GetComponentInChildren<GridLayoutGroup>().transform;
        for (int i = 0; i < slotCount; i++)
        {
            GameObject instance = Instantiate(slot);
            instance.transform.parent = inventoryTransform;
        }
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
