using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public List<EncounterWave> waves = new List<EncounterWave>();

    public bool fixCamera;

    public float areaSize = 20;
    public const float areaY = 13;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var size = new Vector3(areaSize, areaY, areaY);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
