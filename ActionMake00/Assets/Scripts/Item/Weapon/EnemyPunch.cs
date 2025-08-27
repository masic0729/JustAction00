using UnityEngine;

public class EnemyPunch : EnemyWeapon
{

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        hitLevel = 0;

    }

}
