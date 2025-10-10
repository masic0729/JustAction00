using UnityEngine;

public class Weapon : Attacker, IAttacker
{


    protected override void Start()
    {
        base.Start();
        //Init();
    }
    protected override void Init()
    {
        base.Init();
        owner = GetComponentInParent<Character>();
        damage = owner.GetResultDamage();
    }


    

}
