using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_DemoIntro : Scenario
{
    [Header("Variables")]
    public Unit hero1;
    public Unit hero2;
    public Unit evilguy;

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

    // Этот метод нужно переопределять в потомках
    protected override IEnumerator ScenarioRoutine()
    {
        UIManager.FadeIn();

        UIManager.DisableMainUI();

        Player.DisableControll();

        yield return new WaitForSeconds(1.5f);

        yield return PlayReplicaAndWait("Damn it! We got in a trap!", hero1, "Lisandor");

        yield return PlayReplicaAndWait("There is should be a way out...", hero2, "Diana");

        yield return PlayReplicaAndWait("Fools... You stay here forever!", evilguy, "The Lich");

        yield return PlayReplicaAndWait("Unless you use W,A,S,D or Joystick to move around.", evilguy, "The Lich");

        yield return PlayReplicaAndWait("To switch character press Tab or click on his portrait.", hero1, "Lisandor", true);

        UIManager.DisableSpeechUI();

        Player.EnableControll();

        UIManager.EnableMainUI();


        yield return null;
    }
}
