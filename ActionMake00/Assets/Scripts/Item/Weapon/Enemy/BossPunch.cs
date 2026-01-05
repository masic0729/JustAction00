using UnityEngine;

public class BossPunch : EnemyWeapon
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Init()
    {
        base.Init();
        hitLevel = 0;
        damageMultify = 2f;
    }

}
