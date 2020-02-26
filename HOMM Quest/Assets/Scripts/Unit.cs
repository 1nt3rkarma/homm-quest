using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource), typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    [Tooltip("Графические настройки персонажа")]
    public CharacterGraphics graphics;

    [Tooltip("К какой команде принадлежит персонаж")]
    public Team team;

    [Tooltip("Может ли персонаж использовать подбираемые предметы?")]
    public bool canUseItems = false;

    #region Параметры сражения
    [Header("Параметры сражения", order = 0)]

    [Range(0.1f, 3)][Tooltip("Минимальное время между получением урона, которое должно пройти, чтобы атаку снова можно было прервать")]
    public float counterLimit = 2;
    [HideInInspector] // Таймер задержки между прерываниями атак
    public float counterTimer = 0;
    [HideInInspector] // Персонаж защищен от прерывания атак?
    public bool blockCounter = false;

    [Header("Ближний бой", order = 1)]
    [Range(0.1f, 2)][Tooltip("Размер области поражения")]
    public float hitArea = 0.5f;
    [HideInInspector] // Радиус взаимодействия
    public float attackRange { get => GetMeleeRange(); }
    [Tooltip("Урон будет нанесен одной цели или всем в зоне поражения?")]
    public bool splashDamage = false;
    [Range(1, 25)][Tooltip("Урон этого персонажа")]
    public int damage = 1;
    [Range(0.5f, 3)][Tooltip("Скорость атаки (количество ударов в секунду)")]  
    public float attackSpeed = 1;
    [HideInInspector]
    public float attackTimer;
    [Tooltip("Звук (или варианты звука) удара")]
    public List<AudioClip> hitSounds;
    [Tooltip("Звук смерти")]
    public AudioClip deathSound;

    [Header("Дальний бой")]
    [Tooltip("Может стрелять?")]
    public bool canShoot = false;
    [Tooltip("Заготовка снаряда персонажа")]
    public Missile missilePrefab;
    [Tooltip("Звук оружия (воспроизводится при анимации стрельбы)")]
    public AudioClip missileLaunchSound;
    [Tooltip("Начальная скорость вылета снаряда")]
    public float missileSpeed = 10;
    [Range(0.5f, 3)][Tooltip("Скорость стрельбы (количество выстрелов в секунду)")]
    public float fireRate = 1;
    [Tooltip("Бесконечный запас выстрелов?")]
    public bool unlimitedAmmo = false;
    [Range(1, 100)][Tooltip("Запас выстрелов")]
    public int ammoDefault;
    [HideInInspector]
    public int ammo;

    #endregion

    #region Параметры здоровья
    [Header("Параметры здоровья")]

    [Tooltip("Максимальный запас здоровья")]
    [Range(1, 25)]
    public int HPMax = 10;
    [Tooltip("Исчезает ли персонаж после смерти")]
    public bool decays = true;
    [HideInInspector] // Реальный запас здоровья
    public int HP;
    [HideInInspector] // Жив ли персонаж в данный момент?
    public bool isAlive = true;

    #endregion

    #region Параметры движения
    [Header("Параметры движения")]
    [Tooltip("Скорость движения")]
    [Range(0.2f, 10)]
    public float moveSpeed = 1;

    [HideInInspector] // Может ли персонаж двигаться в данный момент
    public bool controllEnabled = true;
    //[HideInInspector] // Может ли персонаж двигаться в данный момент
    //public bool isMoving = false;

    [HideInInspector] // Текущее направление взгляда
    public FlipModes facing= FlipModes.right;

    #endregion

    public AudioSource audioSource;
    public Rigidbody2D rbody;
    public UnitAI AI;

    void Start()
    {
        HP = HPMax;
        ammo = ammoDefault;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        if (transform.localEulerAngles.y == 180)
            facing = FlipModes.left;
        else
            facing = FlipModes.right;
    }

    void FixedUpdate()
    {
        if (attackTimer > 0)
            attackTimer -= Time.fixedDeltaTime;
        if (counterTimer > 0)
            counterTimer -= Time.fixedDeltaTime;

        graphics.sprite.sortingOrder = 150 - (int)(transform.localPosition.y * 100);
    }

    #region Методы, реализующие движение

    public void Dash()
    {
        if (controllEnabled)
            StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        controllEnabled = false;
        int direction = -GetCharacterFacing();
        float timer = 0,
              duration = 0.15f,
              distance = 2,
              speed = distance / duration;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            Vector3 position = transform.position;
            position.x += direction * speed * Time.deltaTime;
            transform.position = position;
            yield return null;
        }
        controllEnabled = true;
    }

    public void MoveTowards(Vector3 destination)
    {
        if (controllEnabled)
        {
            Vector3 position = transform.position;

            int directionHorizontal = 0;
            int directionVertical = 0;

            if (destination.x > position.x)
                directionHorizontal = 1;
            else if (destination.x < position.x)
                directionHorizontal = -1;

            if (destination.y > position.y)
                directionVertical = 1;
            else if (destination.y < position.y)
                directionVertical = -1;


            if (directionHorizontal != 0 && directionHorizontal != GetCharacterFacing())
                Flip();


            if (directionHorizontal != 0 || directionVertical != 0)
            {
                TriggerAnimation("isMoving", true);

                position.x += directionHorizontal * moveSpeed * Time.deltaTime;
                position.y += directionVertical * (moveSpeed / 2) * Time.deltaTime;

                SetPosition(position);
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        position.z = position.y;
        transform.position = position;
    }

    public void Stop()
    {
        rbody.velocity = Vector2.zero;
        TriggerAnimation("isMoving", false);
    }

    #endregion

    #region Методы, реализующие нанесение и получение урона

    public void Attack()
    {
        if (controllEnabled && attackTimer <= 0)
        {
            // Заводим таймер задержки между атаками
            attackTimer = 1 / attackSpeed;
            // Проигрывание анимации
            TriggerAnimation("attack");
        }
    }

    public float CastDamage()
    {
        float factDamage = 0;

        // В зависимости от поворота задаем направление атаки
        // Получаем список коллайдеров, которые задел персонаж при атаке
        Collider2D[] colliders = Physics2D.OverlapCircleAll(graphics.attackPoint, hitArea);
        foreach (var other in colliders)
        {
            // Пытаемся понять, является ли то, что мы ударили Персонажем
            Unit target = other.GetComponent<Unit>();
            if (target != null && target.isAlive)
            {
                // Если это Персонаж, проверяем, является ли он врагом
                if (this.team != target.team)
                {
                    if (Mathf.Abs(target.transform.position.y - transform.position.y) <= 0.3f)
                    {
                        if (hitSounds.Count > 0 && factDamage <= 0)
                        {
                            int random = Random.Range(0, hitSounds.Count);
                            if (hitSounds[random] != null)
                                PlaySound(hitSounds[random]);
                            else
                                Debug.Log("Отсутствует звук удара");
                        }
                        if (!splashDamage)
                            return target.TakeDamage(damage);
                        else
                            factDamage += target.TakeDamage(damage);
                    }
                }
            }
        }
        return factDamage;
    }

    public void Shoot()
    {
        if (controllEnabled && attackTimer <= 0)
        {
            if (ammo > 0 || unlimitedAmmo)
            {
                attackTimer = 1 / fireRate;
                // Проигрывание анимации
                TriggerAnimation("shoot");
            }
        }
    }

    public Missile CreateMissile()
    {
        if (!unlimitedAmmo)
            ammo--;

        if (missilePrefab == null)
        {
            Debug.LogError("Отсутствует префаб снаряда!");
            Debug.Break();  //ставим игру на паузу
            return null;
        }

        // Воспроизводим звук запуска снаряда
        if (missileLaunchSound)
            PlaySound(missileLaunchSound);

        // Проверяем, в какую сторону повернут персонаж
        int direction = GetCharacterFacing();

        // В зависимости от поворота задаем направление атаки
        Vector3 point = new Vector3(graphics.launchPoint.x, graphics.launchPoint.y, transform.position.z);

        // Создаем объект-снаряд
        Missile missile = Instantiate(missilePrefab, point, Quaternion.identity);
        missile.GetComponent<Rigidbody2D>().velocity = Vector2.right * direction * missileSpeed;
        missile.ownerTeam = team;

        // Разворачиваем его в сторону, куда смотрит персонаж
        if (direction == -1)
            missile.transform.Rotate(0, 180, 0);

        return missile;
    }

    public float TakeDamage(int damage)
    {
        if (isAlive)
        {
            // Создаем эффекта удара
            if (graphics.hitEffect)
            {
                GameObject hitEffect = Instantiate(graphics.hitEffect, graphics.chestPoint, Quaternion.identity);
                var renderer = hitEffect.GetComponent<SpriteRenderer>().sortingOrder = graphics.sprite.sortingOrder + 10;
                Destroy(hitEffect, 0.6f);
            }

            HP -= damage;
            if (HP <= 0)
                StartCoroutine(DieRoutine());
            else if (counterTimer <= 0 && blockCounter == false)
            {
                TriggerAnimation("hit");
                counterTimer = counterLimit;
            }
            return damage;
        }
        return 0;
    }

    IEnumerator DieRoutine()
    {
        isAlive = false;
        TriggerAnimation("die");
        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        name = name + "(погиб)";

        if (decays)
        {
            yield return new WaitForSecondsRealtime(3f);
            Destroy(gameObject);
        }
    }

    #endregion

    #region Вспомогательные методы

    public void AddHP(int amount)
    {
        if (amount > 0)
        {
            HP += amount;
            if (HP > HPMax)
                HP = HPMax;
        }
    }

    public void AddAmmo(int amount)
    {
        if (amount > 0)
        {
            ammo += amount;
            if (ammo > ammoDefault)
                ammo = ammoDefault;
        }
    }

    public float GetMeleeRange()
    {
        return Mathf.Abs(graphics._attackPoint.localPosition.x);
    }

    public int GetCharacterFacing()
    {
        return (int)facing;
    }

    public int GetXDirectionTo(Unit character)
    {
        return GetXDirectionTo(character.transform.position);
    }
    public int GetXDirectionTo(Vector3 point)
    {
        if (GetXDeltaTo(point) < 0)
            return 1;
        else
            return -1;
    }
    public int GetYDirectionTo(Unit character)
    {
        return GetYDirectionTo(character.transform.position);
    }
    public int GetYDirectionTo(Vector3 point)
    {
        if (GetYDeltaTo(point) < 0)
            return 1;
        else
            return -1;
    }

    public float GetXDeltaTo(Unit character)
    {
        return GetXDeltaTo(character.transform.position);
    }
    public float GetXDeltaTo(Vector3 point)
    {
        return transform.position.x - point.x;
    }
    public float GetYDeltaTo(Unit character)
    {
        return GetYDeltaTo(character.transform.position);
    }
    public float GetYDeltaTo(Vector3 point)
    {
        return transform.position.y - point.y;
    }

    public void Flip(FlipModes mode)
    {
        if (mode == FlipModes.left)
            transform.localEulerAngles = new Vector3(0, 180, 0);
        else
            transform.localEulerAngles = new Vector3(0, 0, 0);

        facing = mode;
    }

    public void Flip()
    {
        if (facing == FlipModes.left)
            Flip(FlipModes.right);
        else
            Flip(FlipModes.left);
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void TriggerAnimation(string animTag)
    {
        if (graphics.animator)
            graphics.animator.SetTrigger(animTag);
    }

    public void TriggerAnimation(string animTag, bool mod)
    {
        if (graphics.animator)
            graphics.animator.SetBool(animTag, mod);
    }

    bool CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        if (colliders.Length > 0)
            return true;
        else
            return false;
    }
    #endregion

    #region Дополнительный интерфейс в режиме Сцены
    void OnDrawGizmosSelected()
    {
        #region Отрисовка точки атаки и области поражения в ближнем бою

        int direction = GetCharacterFacing();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(graphics.attackPoint, hitArea);
        Gizmos.DrawWireSphere(graphics.attackPoint, 0.1f);

        if (canShoot)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(graphics.launchPoint, 0.1f);
        }

        #endregion

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(graphics.chestPoint, 0.3f);
        Gizmos.DrawWireSphere(graphics.overheadPoint, 0.3f);
        Gizmos.DrawWireSphere(graphics.originPoint, 0.3f);
    }

    #endregion

    #region СТАРОЕ

    //float GetCurrentAnimationLength()
    //{
    //    return graphics.animator.GetCurrentAnimatorStateInfo(0).length;
    //}

    //float GetAnimationLength(string animTag)
    //{
    //    RuntimeAnimatorController controller = graphics.animator.runtimeAnimatorController;
    //    foreach (var clip in controller.animationClips)
    //    {
    //        if (clip.name == animTag)
    //            return clip.length;
    //    }
    //    return 0;
    //}

    #endregion
}

public enum FlipModes { left = -1, right = 1}

public enum Team { player, enemy }

[System.Serializable]
public class CharacterGraphics : object
{
    #region Свойства-геттеры

    public Vector2 attackPoint { get => new Vector2(_attackPoint.position.x, _attackPoint.position.y); }
    public Vector2 launchPoint { get => new Vector2(_launchPoint.position.x, _launchPoint.position.y); }
    public Vector2 overheadPoint { get => new Vector2(_overheadPoint.position.x, _overheadPoint.position.y); }
    public Vector2 chestPoint { get => new Vector2(_chestPoint.position.x, _chestPoint.position.y); }
    public Vector2 originPoint { get => new Vector2(_originPoint.position.x, _originPoint.position.y); }

    #endregion

    [Tooltip("Портрет юнита, отображаемый в диалогах")]
    public Sprite portriat;
    [Tooltip("Компонент, отвечающий за отрисовку спрайта")]
    public SpriteRenderer sprite;
    [Tooltip("Компонент, управляющий анимацией этого персонажа")]
    public Animator animator;
    [Tooltip("Визуальный эффект получения персонажем урона")]
    public GameObject hitEffect;

    [Header("Ключевые точки графических эффектов")]
    public Transform _attackPoint;
    public Transform _launchPoint;
    public Transform _overheadPoint;
    public Transform _chestPoint;
    public Transform _originPoint;
}
