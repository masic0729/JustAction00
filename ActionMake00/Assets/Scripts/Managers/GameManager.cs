using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject gameClearView;
    [SerializeField] GameObject gameOverView;

    //테스트를 위한 처리값. 현재는 맵생성에 사용하고 있다.
    [SerializeField] bool isTest = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
    }

    public bool GetIsTest() => isTest;

    /// <summary>
    /// 플레이어가 사망하면 게임 오버창 활성화한다
    /// </summary>
    public void GameOver()
    {
        gameOverView.SetActive(true);

        GUI_PlayerInput.instance.ShowEndUI(gameOverView);
    }

    /// <summary>
    /// 보스 몬스터를 처치하면 게임 클리어
    /// </summary>
    public void GameClear()
    {
        gameClearView.SetActive(true);

        GUI_PlayerInput.instance.ShowEndUI(gameClearView);

    }
}
