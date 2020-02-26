using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitAI : MonoBehaviour
{
    const float primaryTargetFactor = 3f;
    const int maxEnemies = 5;

    [Tooltip("Персонаж, которым управляет данный ИИ")]
    public Unit character;

    [Tooltip("Радиус поиска цели")][Range(1.5f, 10)]
    public float targetingRange = 5;
    [Tooltip("Радиус преследования")][Range(3, 10)]
    public float followRange = 5;

    [Tooltip("Должен ли данный персонаж патрулировать?")]
    public bool isPatrolling = false;
    [Tooltip("Радиус области патрулирования")][Range(1.5f, 10)]
    public float patrolRange = 4;
    Vector3 patrolPoint;

    [HideInInspector] public Unit primaryTarget = null;
    [HideInInspector] public List<Unit> enemiesSpotted = new List<Unit>();

    void Start()
    {
        patrolPoint = transform.position;
    }

    void Update()
    {
        if (character.isAlive && character.controllEnabled)
            AIAlgorythm();
    }

    #region Основные методы

    /// <summary> Основной алгоритм Искусственного Интеллекта врагов </summary>
    void AIAlgorythm()
    {
        UpdateEnemies();

        if (primaryTarget == null)
        {
            if (enemiesSpotted.Count > 0)
                primaryTarget = PickEnemy();
            else if (isPatrolling)
            {
                Vector3 position = transform.position;

                // Определяем направление взгляда персонажа
                int direction = character.GetCharacterFacing();

                float delta = position.x - patrolPoint.x;
                if (Mathf.Abs(delta) > patrolRange)
                {
                    if (delta < -patrolRange)
                        direction = -1;
                    else if (delta > patrolRange)
                        direction = 1;
                }

                Vector3 destination = new Vector3(patrolPoint.x + direction * patrolRange, patrolPoint.y, 0);
                character.MoveTowards(destination);
            }
        }
        else
        {
            if (primaryTarget.isAlive)
            {
                // Определяем расстояние до цели
                Vector3 charPos = transform.position;
                Vector3 targetPos = primaryTarget.transform.position;
                float distance = Mathf.Abs(targetPos.x - charPos.x);

                // Определяем направление к цели и разворачиваем персонажа, если нужно
                int directionX = character.GetXDirectionTo(primaryTarget);
                if (directionX != character.GetCharacterFacing())
                    character.Flip();

                float hitMaxRange = character.attackRange + character.hitArea;
                float hitMinRange = character.attackRange - character.hitArea;
                if (distance < hitMaxRange)
                {
                    if (distance <= hitMinRange)
                    {
                        Vector3 destination = charPos;
                        destination.x -= directionX;

                        character.MoveTowards(destination);
                    }
                    else
                    {
                        if (Mathf.Abs(targetPos.y - charPos.y) < 0.2f)
                        {
                            character.Stop();
                            character.Attack();
                        }
                        else
                        {
                            int directionY = character.GetYDirectionTo(primaryTarget);

                            Vector3 destination = charPos;
                            destination.y += directionY;

                            character.MoveTowards(destination);
                        }
                    }
                }
                // Если можем стрелять - стреляем
                else if (character.canShoot && character.ammo > 0)
                {
                    if (Mathf.Abs(targetPos.y - charPos.y) < 0.2f)
                    {
                        character.Stop();
                        character.Shoot();
                    }
                    else
                    {
                        int directionVertical = 0;
                        if (targetPos.y > charPos.y)
                            directionVertical = 1;
                        else if (targetPos.y < charPos.y)
                            directionVertical = -1;

                        Vector3 direction = charPos;
                        direction.y += directionVertical;

                        character.MoveTowards(direction);
                    }
                }
                // Если не можем стрелять - догоняем
                else
                {
                    if (primaryTarget == PickEnemy())
                        character.MoveTowards(targetPos);
                    else
                        primaryTarget = PickEnemy();
                }
            }
            else
                primaryTarget = null;
        }
    }

    /// <summary> Выбор наиболее подходящего противника из списка обнаруженных </summary>
    Unit PickEnemy()
    {
        int count = enemiesSpotted.Count;
        float[] weights = new float[enemiesSpotted.Count];
        for (int i = 0; i < count; i++)
        {
            weights[i] = 1 - Mathf.Abs(character.GetXDeltaTo(enemiesSpotted[i]));
            if (enemiesSpotted[i] == primaryTarget)
                weights[i] += primaryTargetFactor;
        }
        int index = 0;
        float maxWeight = weights[0];
        for (int i = 1; i < count; i++)
        {
            if (weights[i] > maxWeight)
            {
                maxWeight = weights[i];
                index = i;
            }
        }
        return enemiesSpotted[index];
    }

    /// <summary> Обновление списка обнаруженных противников </summary>
    void UpdateEnemies()
    {
        List<Unit> enemies = new List<Unit>(enemiesSpotted);
        foreach (var enemy in enemies)
        {
            float distance = Mathf.Abs(character.GetXDeltaTo(enemy));
            // Если расстояние оказалось больше, чем радиус преследования
            // ИИ "теряет" цель"
            // !ВАЖНО - следите, чтобы followRange был больше targetingRange
            // иначе ИИ будет всегда терять цель
            if (distance > followRange || !enemy.isAlive)
            {
                enemiesSpotted.Remove(enemy);
                if (enemy == primaryTarget)
                    primaryTarget = null;
            }
        }

        if (enemiesSpotted.Count < maxEnemies)
            ScoutEnemies();
    }

    /// <summary> Поиск противников вокруг персонажа </summary>
    void ScoutEnemies()
    {
        // Осматриваем объекты вокруг
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, targetingRange);
        foreach (var other in colliders)
        {
            // Пытаемся понять, является ли то, что мы нашли Персонажем
            Unit target = other.GetComponent<Unit>();
            if (target != null)
            {
                // Если это Персонаж, проверяем, является ли он врагом
                if (character != target && character.team != target.team && target.isAlive)
                {
                    if (!enemiesSpotted.Contains(target))
                        enemiesSpotted.Add(target);
                }
            }
        }
    }
    #endregion

    #region Дополнительный интерфейс в режиме Сцены

    void OnDrawGizmosSelected()
    {
        if (isActiveAndEnabled)
        {
            // Отрисовка области поиска цели
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, targetingRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, followRange);

            // Отрисовка обнаруженных противников
            if (primaryTarget)
            {
                Vector3 charPos = transform.position; charPos.z = 0;
                Vector3 targetPos = primaryTarget.transform.position; targetPos.z = 0;
                Debug.DrawRay(charPos, targetPos, Color.red, 0.1f);
            }
            foreach (var enemy in enemiesSpotted)
                if (enemy != primaryTarget)
                {
                    Vector3 charPos = transform.position; charPos.z = 0;
                    Vector3 targetPos = enemy.transform.position; targetPos.z = 0;
                    Debug.DrawRay(charPos, targetPos, Color.yellow, 0.1f);
                }
            

            // Отрисовка области патрулирования
            if (isPatrolling)
            {
                Vector3 point1 = transform.position; point1.x += patrolRange;
                Vector3 point2 = transform.position; point2.x -= patrolRange;
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(point1, 0.2f);
                Gizmos.DrawSphere(point2, 0.2f);
            }
        }
    }

    #endregion
}
