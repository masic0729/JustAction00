using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelUp : MonoBehaviour
{
    Player player;
    [SerializeField] string LevelUpPS;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    /// <summary>
    /// 레벨 구간에 따라 플레이어에게 이로운 혜택이 다르다.
    /// 2레벨의 경우 스킬 해금,
    /// 3레벨의 경우 모든 능력치 소폭상승한다.
    /// 해당 함수에 레벨업에 따른 능력 획득 및 이펙트가 발현된다.
    /// </summary>
    public void LevelUp()
    {
        int level = player.GetLevel();

        switch(level)
        {
            case 2:
                //player.LevelUpForSkillOpen();
                //player.LevelUpForStatUp();
                break;

            case 3:
                player.LevelUpForSkillOpen();
                break;

            default:
                break;
        }
        GameObject instance = PoolManager.instance.Spawn(LevelUpPS, transform.position);
        instance.transform.Translate(0, 1f, 0);
        instance.transform.parent = this.gameObject.transform;
    }
}