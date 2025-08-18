using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    private Collider col;
    private int damage = 1;


    
    protected virtual void Init()
    {
        col = GetComponent<Collider>();
        if (col == null)
            Debug.LogError($"{name} Weapon에 Collider가 없음!");
        else
            col.enabled = false; // 시작은 꺼두기
    }

    public void ColliderEnable(int state)
    {
        if (col == null) return;
        col.enabled = (state == 1);
    }

    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;
}
