using System;

[System.Serializable]
public class ItemBase
{
    public ItemData data;
    public ItemSlot slotData;                                        //해당 아이템이 위치한 슬롯 정보
    public int addCount;                                             //해당 데이터는 테스트용으로 사용중임. 절대 이를 활용하지 말것
    public int currentCount;                                         //현재 아이템 슬롯에 쌓여있는 개수. (예시로 포션 같은 소비형 아이템이 여러개 쌓여있는 경우)

    public Action<Character> OnItemUse;                              //아이템을 사용할 때 발생하는 상호작용
    public Action<ItemSlot> OnItemUpdate;                            //아이템 사용 후 처리에 대한 부분. 예시로 슬롯 데이터 삭제, 카운트 및 차감 등등 기본적인 상호작용 이후의 처리를 뜻한다
}