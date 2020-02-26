using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemHeal : Item
{
    [Tooltip("Объем восстанавливаемого здоровья")]
    [Range(1, 10)]
    public int heal = 5;

    public override void Use(Unit user)
    {
        if (user.HP < user.HPMax)
            base.Use(user);

        user.AddHP(heal);
    }
}
