using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class Missile : MonoBehaviour
{
    [Tooltip("Урон данного снаряда")]
    public int hitDamage = 3;

    [Tooltip("Радиус поражения по оси Z (заметно только в режиме 3D)")]
    public float collisionRadius = 0.25f;

    [Tooltip("Звук (или варианты звука) попадания снаряда")]
    public List<AudioClip> hitSounds;

    [HideInInspector]
    public Team ownerTeam = Team.player;

    // Время, по истечении которого снаряд будет уничтожен
    float expirationTime = 5;

    void Start()
    {
        Destroy(gameObject, expirationTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Unit target = other.GetComponent<Unit>();
        if (target != null && target.isAlive)
        {
            if (Mathf.Abs(this.transform.position.z - target.transform.position.z) <= collisionRadius)
            {
                // Если это Персонаж, проверяем, является ли он врагом
                if (ownerTeam != target.team)
                {
                    target.TakeDamage(hitDamage);
                    if (hitSounds.Count > 0)
                    {
                        int random = Random.Range(0, hitSounds.Count);
                        if (hitSounds[random] != null)
                            target.PlaySound(hitSounds[random]);
                        else
                            Debug.Log("Отсутствует звук удара");
                    }
                    Destroy(gameObject);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (isActiveAndEnabled)
        {
            // Отрисовка области поражения
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, collisionRadius);
        }
    }
}

