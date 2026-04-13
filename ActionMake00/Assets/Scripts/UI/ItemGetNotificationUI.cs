using UnityEngine;

public class ItemGetNotificationUI : MonoBehaviour
{
    public static ItemGetNotificationUI instance;

    // 슬롯 프리팹 및 슬롯들이 붙을 부모 트랜스폼
    [SerializeField] ItemNotifySlot slotPrefab;
    [SerializeField] Transform slotRoot;

    // 슬롯이 화면에 유지되는 시간 (초)
    [SerializeField] float slotLifetime = 2.5f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // 아이템 획득 시 외부에서 호출하는 진입점
    // ItemObject.AddItemInInventory 에서 호출됨
    public void ShowNotification(ItemData data)
    {
        if (data == null || data.icon == null)
        {
            Debug.LogWarning("[ItemGetNotificationUI] data 또는 icon이 null, 알림 스킵");
            return;
        }

        // 획득할 때마다 무조건 신규 슬롯 생성
        ItemNotifySlot slot = Instantiate(slotPrefab, slotRoot);
        slot.transform.SetAsFirstSibling();
        slot.Init(data.icon, slotLifetime);
    }
}