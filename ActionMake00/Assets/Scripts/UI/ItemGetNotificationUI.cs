using System.Collections.Generic;
using UnityEngine;

public class ItemGetNotificationUI : MonoBehaviour
{
    public static ItemGetNotificationUI instance;

    // 슬롯 프리팹 및 슬롯들이 붙을 부모 트랜스폼
    // 부모에는 Vertical Layout Group 컴포넌트 부착 필요
    [SerializeField] ItemNotifySlot slotPrefab;
    [SerializeField] Transform slotRoot;

    // 슬롯이 화면에 유지되는 시간 (초)
    [SerializeField] float slotLifetime = 2.5f;

    // 현재 활성화된 슬롯을 itemName 키로 관리 (동시 획득 스택 처리용)
    Dictionary<string, ItemNotifySlot> activeSlots = new Dictionary<string, ItemNotifySlot>();

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

        string key = data.itemName;

        // 같은 아이템이 이미 화면에 노출 중이면 수량만 갱신
        if (activeSlots.TryGetValue(key, out ItemNotifySlot existing) && existing != null)
        {
            //existing.AddCount();
            existing.RefreshLifetime(slotLifetime);
            return;
        }

        // 신규 슬롯 생성 및 등록
        ItemNotifySlot slot = Instantiate(slotPrefab, slotRoot);
        slot.transform.SetAsFirstSibling();

        slot.Init(data.icon, slotLifetime, () => activeSlots.Remove(key));
        activeSlots[key] = slot;
    }
}