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

        if (level == 3)
            player.LevelUpForSkillOpen();


        switch(level)
        {
            case 2:
                player.LevelUpForStatUp();
                break;

            case 3:
                player.LevelUpForSkillOpen();
                break;

            default:
                Debug.Log("레벨업 예외 발생");
                break;
        }

        //레벨업 이펙트 출력
        GameObject instance = PoolManager.instance.Spawn(LevelUpPS, transform.position);
        instance.transform.Translate(0, 1f, 0);
        instance.transform.parent = this.gameObject.transform;
    }
}