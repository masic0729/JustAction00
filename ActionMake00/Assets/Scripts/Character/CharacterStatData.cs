using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatConfig", menuName = "GameData/CharacterStatData")]

public class CharacterStatData : ScriptableObject
{
    [SerializeField] float hp;                                          //체력
    [SerializeField] float damage;                                      //공격력
    [SerializeField] float moveSpeed;                                   //이동속도
    [SerializeField] float def;                                         //방어력


    public void SetDamage(float value) { damage = value; }

    public float GetDamage() { return damage; }

    public void SetHp(float value) { hp = value; }

    public float GetHp() { return hp; }

    public void SetMoveSpeed(float value) { moveSpeed = value; }

    public float GetMoveSpeed() { return moveSpeed; }

    public void SetDef(float value) { def = value; }

    public float GetDef() { return def; }

}
