using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    public UnityEvent onEnterEvents;
    public UnityEvent onStayEvents;
    public UnityEvent onExitEvents;

    private List<GameObject> registredCollisions;

    private void Awake()
    {
        registredCollisions = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!registredCollisions.Contains(collision.gameObject))
        {
            registredCollisions.Add(collision.gameObject);
            var unit = collision.GetComponent<Unit>();
            if (unit)
                if (unit == Player.singlton.hero)
                    onEnterEvents.Invoke();
        }
    }

    // ДОРАБОТАТЬ: исключить повторные срабатывания для объектов
    // с несколькими коллайдерами
    private void OnTriggerExit2D(Collider2D collision)
    {
        var unit = collision.gameObject.GetComponent<Unit>();
        if (unit)
            if (unit == Player.singlton.hero)
                onEnterEvents.Invoke();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (registredCollisions.Contains(collision.gameObject))
        {
            registredCollisions.Remove(collision.gameObject);
            var unit = collision.gameObject.GetComponent<Unit>();
            if (unit)
                if (unit == Player.singlton.hero)
                    onEnterEvents.Invoke();
        }
    }
}
