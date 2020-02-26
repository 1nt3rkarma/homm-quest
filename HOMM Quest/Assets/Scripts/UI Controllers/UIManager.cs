using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public static UIManager singlton;

    public Settings settings;

    [Header("Составные части интерфейса")]
    public GameObject mainUI;
    public List<HeroStatsUI> squadUI = new List<HeroStatsUI>();
    [Tooltip("Интерфейс реплики персонажей")]
    public SpeechUI speechUI;
    [Tooltip("Экран победы")]
    public GameObject victoryUI;
    [Tooltip("Музыкальная тема победы")]
    public AudioClip victoryTheme;
    [Tooltip("Экран поражения")]
    public GameObject defeatUI;
    [Tooltip("Музыкальная тема поражения")]
    public AudioClip defeatTheme;
    [Tooltip("Основное музыкальное сопровождение уровня")]
    public List<AudioClip> levelTheme;

    [Header("Необходимые компоненты")]
    public AudioSource audioSource;
    public Animator animator;

    [Space]
    [Header("Мобильный интерфейс")]
    public GameObject mobileControlsUI;
    public Joystick joystick;

    void Awake()
    {
        singlton = this;
    }

    void Start()
    {
        // Мобильное управление
        mobileControlsUI.SetActive(settings.touchControls);

        // Аудио и музыка
        if (levelTheme.Count > 0)
        {
            AudioClip theme = null;
            while (theme == null)
            {
                int rand = Random.Range(0, levelTheme.Count);
                if (levelTheme[rand])
                    theme = levelTheme[rand];
                else
                    levelTheme.Remove(levelTheme[rand]);
            }
            if (theme)
                audioSource.clip = theme;

            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {

    }

    public static void PlayReplica(string message, Unit fromUnit, string name, bool isLast)
    {
        singlton.speechUI.Play(message, fromUnit, name, isLast);
    }
    public static void PlayReplica(string message, Unit fromUnit, string name)
    {
        singlton.speechUI.Play(message, fromUnit, name);
    }
    public static void PlayReplica(string message, Unit fromUnit)
    {
        singlton.speechUI.Play(message, fromUnit);
    }

    public static void DisableSpeechUI()
    {
        singlton.speechUI.Disable();
    }
    public static void EnableSpeechUI()
    {
        singlton.speechUI.Enable();
    }

    public static void DisableMainUI()
    {
        singlton.mainUI.SetActive(false);
    }
    public static void EnableMainUI()
    {
        singlton.mainUI.SetActive(true);
    }

    public static void FadeIn()
    {
        singlton.animator.SetTrigger("fadeIn");
    }

    public static void FadeOut()
    {
        singlton.animator.SetTrigger("fadeOut");

    }

    public static void OnHeroClick(HeroStatsUI heroUI)
    {
        int index = singlton.squadUI.IndexOf(heroUI);
        Player.SwitchHero(index);
    }

    public void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ButtonSquare()
    {
        Player.attackEvent = true;
    }

    public void ButtonCircle()
    {
        Player.specialEvent = true;
    }

    public void ButtonCross()
    {
        Player.jumpEvent = true;
    }

    public void ButtonTriangle()
    {

    }
}

