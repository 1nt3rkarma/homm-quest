using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    public Transform followTraget;

    public Vector2 shift = new Vector3(0, 5);
    public bool followY = false;

    void Update()
    {
        if (followTraget)
        {
            Vector3 position;
            if (followY)
                position = new Vector3(followTraget.position.x + shift.x,
                    followTraget.position.y + shift.y, transform.position.z);
            else
                position = new Vector3(followTraget.position.x + shift.x, shift.y, -10);
            transform.position = position;
        }
    }
}
