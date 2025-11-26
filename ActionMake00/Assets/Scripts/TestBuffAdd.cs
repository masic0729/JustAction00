using UnityEngine;
using UnityEngine.UI;

public class TestBuffAdd : MonoBehaviour
{
    public RectTransform parentTarget;
    public Image testBuffIcon;
    public string iconTestPath;

    // Start is called before the first frame update
    void Start()
    {
        iconTestPath = "Icons/Buffs/DamageUp";
        AddBuff();

    }


    void AddBuff()
    {
        RectTransform instance = Instantiate(testBuffIcon.gameObject).GetComponent<RectTransform>();
        instance.transform.SetParent(parentTarget, false);
        instance.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconTestPath);
        Debug.Log(Resources.Load<Sprite>(iconTestPath));
    }
}