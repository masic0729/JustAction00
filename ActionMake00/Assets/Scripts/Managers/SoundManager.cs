using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource BGM_Player;
    [SerializeField] AudioClip BGM_TitleClip;

    [SerializeField] AudioSource UI_Player;
    [SerializeField] AudioClip UI_InteractionClip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    void Start()
    {
        UI_Player.clip = UI_InteractionClip;

        PlayBGM(BGM_TitleClip);
    }

    
    /// <summary>
    /// BGM 사운드를 변경한다
    /// </summary>
    /// <param name="clip_BGM"></param>
    public void PlayBGM(AudioClip clip_BGM)
    {
        BGM_Player.clip = clip_BGM;
        BGM_Player.Play();
    }

    public void PlayUI()
    {
        UI_Player.Play();
    }
}
