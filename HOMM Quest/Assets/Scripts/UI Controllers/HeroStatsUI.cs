using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatsUI : MonoBehaviour
{
    [Tooltip("Герой, для которого отображаются характеристики")]
    public Unit hero;

    public Image portrait;
    public Text nameLabel;

    public Slider healthBar;

    public Image ammoIcon;
    public Text ammoCountLabel;

    public Outline selectionOutline;

    void Start()
    {
        if (hero)
        {
            portrait.sprite = hero.graphics.portriat;
            nameLabel.text = hero.name;
        }
    }

    void Update()
    {
        if (hero)
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
                nameLabel.text = hero.name;
                portrait.sprite = hero.graphics.portriat;
            }

            healthBar.maxValue = hero.HPMax;
            healthBar.value = hero.HP;

            if (hero.canShoot)
            {
                if (!ammoIcon.gameObject.activeInHierarchy)
                    ammoIcon.gameObject.SetActive(true);

                ammoCountLabel.text = hero.ammo.ToString();

                if (hero.ammo == 0)
                    ammoCountLabel.color = Color.red;
                else
                    ammoCountLabel.color = Color.white;
            }
            else
            {
                if (ammoIcon.gameObject.activeInHierarchy)
                    ammoIcon.gameObject.SetActive(false);
            }

            if (Player.singlton.hero == hero)
                selectionOutline.enabled = true;
            else
                selectionOutline.enabled = false;
        }
        else
            if (gameObject.activeInHierarchy)
                gameObject.SetActive(false);

    }

    public void OnClick()
    {
        UIManager.OnHeroClick(this);
    }
}
