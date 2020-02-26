using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemAmmo : Item
{
    [Tooltip("Количество полученных боеприпасов")]
    [Range(1, 10)]
    public int ammo = 5;

    public override void Use(Unit user)
    {
        if (user.canShoot && user.ammo < user.ammoDefault)
            base.Use(user);

        user.AddAmmo(ammo);
    }
}

