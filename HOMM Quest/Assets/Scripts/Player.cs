using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public static Player singlton;

    [Tooltip("Персонаж, которым управляет данный Игрок")]
    public Unit hero;
    [Tooltip("Персонажи игрока, между которыми можно переключаться")]
    public List<Unit> squad = new List<Unit>();


    [Tooltip("Камера игрока")]
    public CameraController2D cameraController;

    [Tooltip("Компонент, управляющий интерфейсом игрока")]
    public UIManager UI;

    [Tooltip("Файл настроек игры")]
    public Settings settings;

    public bool isActive = true;

    static public bool attackEvent = false;
    static public bool specialEvent = false;
    static public bool jumpEvent = false;

    private void Awake()
    {
        singlton = this;

        Physics2D.IgnoreLayerCollision(8, 8, true);

        #region Проверки ссылок
        if (hero == null)
        {
            Debug.LogError("Отсутствует ссылка на персонажа игрока!");
            Debug.Break();
            return;
        }

        if (cameraController == null)
        {
            Debug.LogError("Отсутствует ссылка на камеру!");
            Debug.Break();
            return;
        }

        if (settings == null)
        {
            Debug.LogError("Отсутствует ссылка на файл настроек!");
            Debug.Break();
            return;
        }

        #endregion
    }

    void Start()
    {
        if (!squad.Contains(hero))
            squad.Add(hero);
        SwitchHero(hero);

        foreach (var character in squad)
            character.AI.isPatrolling = false;

        for (int i = 0; i < UI.squadUI.Count && i < squad.Count; i++)
            UI.squadUI[i].hero = squad[i];

    }

    void Update()
    {
        #region Обновление параметров героя

        List<Unit> toRemove = new List<Unit>();
        foreach (var character in squad)
        {
            if (character == null || character.isAlive == false)
                toRemove.Add(character);
            else
            {
                if (character != hero)
                    if (character.AI.primaryTarget == null)
                        if (Mathf.Abs(character.GetXDeltaTo(hero)) > 3)
                        {
                            Vector3 point = hero.transform.position + Vector3.right * hero.GetXDirectionTo(character);
                            character.MoveTowards(point);
                        }
                        else
                            character.Stop();
            }
        }
        foreach (var character in toRemove)
            squad.Remove(character);

        for (int i = 0; i < UI.squadUI.Count && i < squad.Count; i++)
            UI.squadUI[i].hero = squad[i];

        if (isActive)
        {
            if (hero.isAlive == false)
            {
                if (squad.Count > 0)
                    SwitchHero(squad[0]);
                else
                    Defeat();
            }

            #region Пользовательское управление

            #region Переключение между персонажами
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (squad.Count > 1)
                {
                    int i = squad.IndexOf(hero);

                    if (i == squad.Count - 1)
                        i = 0;
                    else
                        i++;
                    if (squad[i] && squad[i].isAlive)
                        SwitchHero(squad[i]);
                }
            }
            #endregion

            if (!hero.controllEnabled)
                return;

            #region Контроль движения персонажа
            int directionHorizontal = 0;
            int directionVertical = 0;

            if (settings.touchControls)
            {
                if (UI.joystick.Horizontal > 0)
                    directionHorizontal = 1;
                if (UI.joystick.Horizontal < 0)
                    directionHorizontal = -1;

                if (UI.joystick.Vertical > 0)
                    directionVertical = 1;
                if (UI.joystick.Vertical < 0)
                    directionVertical = -1;
            }
            else
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                {
                    if (Input.GetKey(KeyCode.A))
                        directionHorizontal += -1;
                    if (Input.GetKey(KeyCode.D))
                        directionHorizontal += 1;

                    if (Input.GetKey(KeyCode.W))
                        directionVertical += 1;
                    if (Input.GetKey(KeyCode.S))
                        directionVertical += -1;
                }
                else
                    hero.Stop();
            }

            if (directionHorizontal != 0 || directionVertical != 0)
            {
                Vector3 direction = hero.transform.position;
                direction.x += directionHorizontal;
                direction.y += directionVertical;

                hero.MoveTowards(direction);
            }
            else
                hero.Stop();
            #endregion

            #region Контроль атаки

            if (Input.GetKeyDown(KeyCode.J))
                Player.attackEvent = true;
            else if (Input.GetKeyDown(KeyCode.L))
                Player.specialEvent = true;

            if (Player.attackEvent)
                hero.Attack();
            else if (Player.specialEvent && hero.canShoot)
                hero.Shoot();

            #endregion

            #endregion
        }
        #endregion

        attackEvent = false;
        specialEvent = false;
        jumpEvent = false;
    }

    public void Victory()
    {
        singlton.UI.victoryUI.SetActive(true);
        singlton.UI.audioSource.Stop();
        if (singlton.UI.victoryTheme)
            singlton.UI.audioSource.PlayOneShot(singlton.UI.victoryTheme);
        singlton.hero.Stop();
        DisableControll();
    }

    public void Defeat()
    {
        singlton.UI.defeatUI.SetActive(true);
        singlton.UI.audioSource.Stop();
        if (singlton.UI.defeatTheme)
            singlton.UI.audioSource.PlayOneShot(singlton.UI.defeatTheme);
        DisableControll();
    }

    public static void SwitchHero(int index)
    {
        if (index < 0 || index >= singlton.squad.Count)
            return;

        singlton.SwitchHero(singlton.squad[index]);
    }

    public void SwitchHero(Unit character)
    {
        if (hero != character)
        {
            hero.canUseItems = false;
            hero.AI.enabled = true;

            hero = character;
        }

        hero.canUseItems = true;
        hero.AI.enabled = false;
        hero.AI.enemiesSpotted.Clear();
        hero.AI.primaryTarget = null;

        cameraController.followTraget = hero.transform;
    }

    public static void EnableControll()
    {
        singlton.isActive = true;
    }

    public static void DisableControll()
    {
        singlton.isActive = false;
    }
}
