using System;

[System.Serializable]
public class Item
{
    public ItemData data;
    public int addCount;                                //해당 데이터는 테스트용으로 사용중임. 절대 이를 활용하지 말것

    public Action OnTest;
    //public int currentCount, maxCount;

}