using UnityEngine;

[CreateAssetMenu(menuName = "RPG/PlayerSkills")]
public class PlayerSkillData : ScriptableObject
{
    public string weaponType;
    public PlayerSkillBase[] weaponSkillBase;
    
}