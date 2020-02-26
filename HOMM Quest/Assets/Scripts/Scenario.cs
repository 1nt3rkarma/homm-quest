using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Базовый класс пользовательских сценариев
// которые могут быть активированы по различным причинам
public class Scenario : MonoBehaviour
{

    [Header("Common settings")]
    // Режим срабатывания сценария
    public TriggerModes triggerMode;

    // Режим повторения сценария
    public RepeatModes repeatMode;

    // Задержка перед срабатыванием сценария после его активации
    public float delay = 0;
    private float delayTimer;

    // Для повторяющихся сценариев
    [HideInInspector]
    public int repeats = 2;
    private int repeatCounter;

    [HideInInspector] // Активирован ли в данный момент этот сценарий?
    public bool isAсtive = false;
    [HideInInspector] // Играется ли в данный момент этот сценарий?
    public bool isPlaying = false;

    // Ссылка на объект-сопрограмму, реализующую сценарий
    public Coroutine coroutine;

    // Эти два метода придется копировать
    // для каждого потомка
    void Start()
    {
        OnStart();
    }

    void Update()
    {
        OnUpdate();
    }

    protected void OnStart()
    {
        repeatCounter = repeats;

        if (triggerMode == TriggerModes.start)
            Activate();
    }

    protected void OnUpdate()
    {
        // Таймер обратного отсчета
        if (isAсtive && !isPlaying)
            if (delayTimer > 0)
            {
                delayTimer -= Time.deltaTime;
                if (delayTimer - Time.deltaTime <= 0)
                    Play();
            }
    }

    public void Activate()
    {
        if (!isAсtive && !isPlaying && this.isActiveAndEnabled)
        {
            //Debug.Log("Сценарий активирован");
            isAсtive = true;

            if (delay > 0)
                delayTimer = delay;
            else
                Play();
        }
    }
    protected void Play()
    {
        //Debug.Log("Сценарий начал исполняться");

        coroutine = StartCoroutine(MainRoutine());
        isPlaying = true;
    }
    public void End()
    {
        //Debug.Log("Сценарий закончил исполнение");

        isAсtive = false;
        isPlaying = false;

        switch (repeatMode)
        {
            case RepeatModes.single:
                Disable();
                break;
            case RepeatModes.limited:
                repeatCounter--;
                if (repeatCounter == 0)
                    Disable();
                break;
        }
    }
    public void Stop()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
            isPlaying = false;
            isAсtive = false;
        }
    }
    public void Disable()
    {
        this.enabled = false;
    }
    public void Enable()
    {
        this.enabled = true;
    }

    IEnumerator MainRoutine()
    {
        yield return ScenarioRoutine();

        End();
    }

    // Сопрограмма, реализующая сценарий
    // переопределяется в классах-потомках
    protected virtual IEnumerator ScenarioRoutine()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("This is an example scenario playing");
        yield return new WaitForSeconds(5);
    }

    protected IEnumerator PlayReplicaAndWait(string message, Unit actor, string named, bool isLast)
    {
        UIManager.PlayReplica(message, actor, named, isLast);
        while (!UIManager.singlton.speechUI.isExectued)
            yield return null;
    }

    protected IEnumerator PlayReplicaAndWait(string message, Unit actor, string named)
    {
        yield return PlayReplicaAndWait(message, actor, named, false);
    }


    public enum TriggerModes { start, condition, custom }
    public enum RepeatModes { single, limited, infinit }
}
