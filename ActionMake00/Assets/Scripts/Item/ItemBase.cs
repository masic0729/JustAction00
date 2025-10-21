using System;

[System.Serializable]
public class ItemBase
{
    public ItemData data;
    public int addCount;                                //해당 데이터는 테스트용으로 사용중임. 절대 이를 활용하지 말것

    //public Action<Character> OnCheckUse;                           //아이템을 사용하기 직전 상호작용이 제대로 되는 지 확인함
    public Action<Character> OnItemUse;                            //아이템을 사용할 때 발생하는 상호작용
    public Action<Character> OnItemUpdate;                         //아이템 사용 후 처리에 대한 부분. 예시로 인벤토리 삭제, 카운트 차감 등등 기본적인 상호작용 이후의 처리를 뜻한다


}