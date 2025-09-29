using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGolem : BossEnemyBT
{
    GameObject pattenEffect;
    Transform spawnProjectileTransform;
    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Init()
    {
        base.Init();

        weaponDic["PunchWeapon"] = FindTransformAtChild("PunchWeapon").GetComponent<Weapon>();
        weaponDic["HookWeapon"] = FindTransformAtChild("HookWeapon").GetComponent<Weapon>();
        
        playerFindDistance = 10f;
        activityAllowValue = 20f;
        attackReadyDistance = 3f;
        punchDistance = 2.5f;
        
    }

    public override void Dead(float animationTime)
    {
        base.Dead(animationTime);
        if(pattenEffect != null)
        {
            //보스가 사망할 때 바로 삭제해버릴 것
            pattenEffect.GetComponent<ParticlePoolReleaser>().PoolReleaser();
        }
    }


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public void CastWarning(int pattenIndex)
    {
        spawnProjectileTransform = FindTransformAtChild("CastShooter").GetComponent<Transform>();
        if(pattenIndex != 0)
        {
            pattenEffect = PoolManager.instance.Spawn("pCastWarning", spawnProjectileTransform.position, spawnProjectileTransform.rotation);
            pattenEffect.transform.parent = spawnProjectileTransform;
        }
        
    }

    public void SpawnStone(int pattenIndex)
    {
        Instantiate(skillProjectiles[0], spawnProjectileTransform.position, spawnProjectileTransform.rotation);
        Debug.Log("소환됨");
        if(pattenEffect != null)
        {
            Destroy(pattenEffect);
            pattenEffect = null;

            float rotateValue = 25f;
            GameObject instance = Instantiate(skillProjectiles[0], spawnProjectileTransform.position, spawnProjectileTransform.rotation);
            instance.transform.Rotate(0, rotateValue, 0);
            instance = Instantiate(skillProjectiles[0], spawnProjectileTransform.position, spawnProjectileTransform.rotation);
            instance.transform.Rotate(0, -rotateValue, 0);
        }

        spawnProjectileTransform = null;


    }

    public void TurnOff()
    {
        isCanTurn = false;
    }
}
