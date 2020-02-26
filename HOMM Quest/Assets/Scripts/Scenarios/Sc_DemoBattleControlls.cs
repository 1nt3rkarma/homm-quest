using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_DemoBattleControlls : Scenario
{
    [Header("Variables")]
    public Unit knight;
    public Unit archer;
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
        Player.DisableControll();

        knight.Stop();
        archer.Stop();

        yield return PlayReplicaAndWait("Oh, you approaching me? How about fighting some skeletons first!", evilguy, "The Lich");

        yield return PlayReplicaAndWait("Sure they will persuade you to join the undead forces!", evilguy, "The Lich");

        yield return PlayReplicaAndWait("Unless you use J or Square to melee attack...", evilguy, "The Lich");

        yield return PlayReplicaAndWait("...and L or Circle to use your special ability.", evilguy, "The Lich");

        yield return PlayReplicaAndWait("Why do I say those things?", evilguy, "The Lich");

        archer.unlimitedAmmo = true;
        archer.Shoot();
        yield return PlayReplicaAndWait("My special attack - is shooting arrows.", archer, "Diana");
        archer.unlimitedAmmo = false;

        knight.Attack();
        yield return PlayReplicaAndWait("And mine is to block attacks with my shield!", knight, "Lisandor");

        yield return PlayReplicaAndWait("Be careful - enemies can stun you if they interrupt your attack.", knight, "Lisandor");

        yield return PlayReplicaAndWait("But you can do the same thing.", knight, "Lisandor");


        UIManager.DisableSpeechUI();

        Player.EnableControll();

        yield return null;
    }
}
