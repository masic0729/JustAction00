using UnityEngine;
public class Enemy : Character
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void Init()
    {
        base.Init();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "PlayerAttack")
        {
            //기본적으로 피해를 받는다
            TakeDamage(other.GetComponent<Sword>().GetDamage());
        }
    }
}
