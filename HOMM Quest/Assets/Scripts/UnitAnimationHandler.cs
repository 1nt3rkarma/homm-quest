using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationHandler : MonoBehaviour
{
    public Unit character;
    public Animator animator;

    public float timerView;
    public bool blockCounter;

    [Range(1, 5)]
    public int idleClipCount = 1;

    private void Update()
    {
        timerView = character.counterTimer;
        blockCounter = character.blockCounter;
    }

    public void CastDamage()
    {
        if (character.isAlive)
            character.CastDamage();
    }

    public void CastMissile()
    {
        if (character.isAlive)
            character.CreateMissile();
    }

    public void EnableControll()
    {
        if (character.isAlive)
            character.controllEnabled = true;
    }

    public void DisableControll()
    {
        if (character.isAlive)
            character.controllEnabled = false;
    }

    public void AllowCounterAttack()
    {
        if (character.isAlive)
            character.blockCounter = false;
    }

    public void BlockCounterAttack()
    {
        if (character.isAlive)
            character.blockCounter = true;
    }

    public void GenerateRandomIdle()
    {
        if (idleClipCount > 1)
            animator.SetInteger("index", Random.Range(0, idleClipCount));
        else
            animator.SetInteger("index", 0);
    }
}
