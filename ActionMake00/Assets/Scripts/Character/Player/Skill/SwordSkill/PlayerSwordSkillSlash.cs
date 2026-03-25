using DG.Tweening;
using UnityEngine;


public class PlayerSwordSkillSlash : PlayerWeaponSkill
{
    PlayerController playerController;
    protected override void Start()
    {
        base.Start();
    }

    public override void SkillUse()
    { 
        base.SkillUse();
        playerController = player.GetComponent<PlayerController>();
        playerController.mainCamera.transform.DOShakePosition(
            duration: 0.15f,
            strength: new Vector3(0f, 0.12f, 0f), // 상하만 (검격용)
            vibrato: 12,
            randomness: 8f,
            snapping: false,
            fadeOut: true
        );

        DOTween.Complete("HARDZOOM");
        DOTween.Restart("NORMALZOOM");

        PoolManager.instance.Spawn(skillPrefab.name, player.weaponDic["Sword"].transform.position , player.weaponDic["Sword"].transform.rotation, player);
    }

    
}
