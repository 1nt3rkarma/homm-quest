using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{
    [Tooltip("Звук подбирания предмета")]
    public AudioClip pickUpSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        Unit character = other.GetComponent<Unit>();
        if (character)
            if (character.canUseItems)
                Use(character);
    }

    public virtual void Use(Unit user)
    {
        if (pickUpSound)
            user.PlaySound(pickUpSound);
        Destroy(gameObject);
    }

}

