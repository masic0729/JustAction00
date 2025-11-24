using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 체력, 경험치, 스킬 및 버프 등 플레이어의 전투와 관련된 데이터를 시각화하는 스크립트
/// </summary>
public class PlayerStatUI : MonoBehaviour
{
    Player player;
    public Slider HpSlider;
    public Slider ExpSlider;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        player.hitAction += PlayerUpdateHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 플레이어 체력 상태를 업데이트 한다.
    /// </summary>
    public void PlayerUpdateHp()
    {
        HpSlider.value = (float)player.GetHp()/player.GetResultMaxHp();
    }

    public void UpdateExp()
    {
        ExpSlider.value = (float)player.GetCurrentExp() / player.GetNeedExp();
    }
}
