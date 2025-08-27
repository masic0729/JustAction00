using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    private Collider col;
    private int damage = 1;


    
    protected virtual void Init()
    {
        col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"{name} Weapon에 Collider가 없음!");
            return;
        }
        col.enabled = false; // 시작은 꺼두기
    }

    public void ColliderTransEnable()
    {
        if (col == null) return;
        if (col.enabled == true)
        {
            col.enabled = false;
        }
        else
            col.enabled = true;
    }

    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;
}
