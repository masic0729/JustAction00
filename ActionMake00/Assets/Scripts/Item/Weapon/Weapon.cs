using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    private Collider col;
    protected int damage = 1;
    protected string target;
    protected int hitLevel = -1;

    private void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"{name} Weapon에 Collider가 없음");
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

    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == target)
        {
            other.GetComponent<Character>().TakeDamage(damage, hitLevel);
        }
    }

    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;
}
