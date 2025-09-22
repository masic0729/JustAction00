using UnityEngine;

[CreateAssetMenu(menuName = "RPG/PlayerSkills")]
public class PlayerSkillData : ScriptableObject
{
    public string weaponType;                   //무기 타입
    /*public Sprite icon;                         //스킬의 아이콘
    public string skillName;                    //스킬명

    public string description;                  //스킬 설명
    public int coolTime;                        //스킬 쿨
    public string triggerName;                  //애니메이션 및 함수 트리거명*/
    public PlayerSkillBase[] weaponSkillBase; 
    
}

[System.Serializable]
public class PlayerSkillBase
{
    //public string weaponType;                   //무기 타입
    public Sprite icon;                         //스킬의 아이콘
    public string skillName;                    //스킬명

    public string description;                  //스킬 설명
    public int coolTime;                        //스킬 쿨
    public string triggerName;                  //애니메이션 및 함수 트리거명
}