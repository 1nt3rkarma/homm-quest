using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechUI : MonoBehaviour
{
    public Image background;

    public Image portrait;

    public Text replicaActor;

    public Text replicaText;

    public Button buttonNext;
    public Text buttonNextText;

    public bool isPlaying = false;
    public bool isFinished = false;
    public bool isExectued = false;

    bool isLast;
    string replicaTextBuffer;
    Coroutine textRoutine;

    private void Start()
    {
        Disable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            if (!isExectued)
                if (!isFinished)
                    OnClickSkip();
                else
                    OnClickNext();
    }

    public void Play(string message, Unit fromUnit, string name, bool isLast)
    {
        Enable();

        replicaTextBuffer = message;
        this.isLast = isLast;

        isFinished = false;
        isExectued = false;

        buttonNext.gameObject.SetActive(false);


        if (isLast)
            buttonNextText.text = "End";
        else
            buttonNextText.text = "Next";

        replicaActor.text = name;

        portrait.sprite = fromUnit.graphics.portriat;
        isPlaying = true;

        replicaText.text = "";

        textRoutine = StartCoroutine(TextRoutine(message));
    }

    public void Play(string message, Unit fromUnit, string name)
    {
        Play(message, fromUnit, name, false);
    }

    public void Play(string message, Unit fromUnit)
    {
        Play(message, fromUnit, fromUnit.name);
    }

    IEnumerator TextRoutine(string message)
    {
        foreach (var character in message)
        {
            replicaText.text += character;
            yield return null;
        }

        Finish();
    }

    public void Finish()
    {
        isPlaying = false;

        buttonNext.gameObject.SetActive(true);

        if (textRoutine != null)
            StopCoroutine(textRoutine);

        replicaText.text = replicaTextBuffer;
        isFinished = true;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        isPlaying = false;
        isFinished = false;
        isExectued = false;

        gameObject.SetActive(false);
    }

    public void OnClickSkip()
    {
        Finish();
    }

    public void OnClickNext()
    {
        isExectued = true;
    }
}
