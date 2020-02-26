using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterWave : MonoBehaviour
{
    [HideInInspector]
    public List<Unit> genericTypes;
    [HideInInspector]
    public List<int> genericTypesChances;
    [HideInInspector]
    public int genericTotalMax = 5;
    [HideInInspector]
    public int genericTotalMin = 3;

    [HideInInspector]
    public List<Unit> definedUnits;

    [Header("Current state")]
    public List<Unit> spawnedUnits;

    public bool isActive = false;

    [Header("Settings")]
    public WaveModes mode;

    public List<Transform> spawnPoints;

    [Range(0,5)]
    public float spawnOverTime = 0;

    void Update()
    {
        
    }

    public void Activate()
    {
        isActive = true;
        if (mode == WaveModes.generic)
            ActivateGeneric();
        else
            ActivateDefined();
    }

    private void ActivateGeneric()
    {

    }

    private void ActivateDefined()
    {
        StartCoroutine(SpawnDefined());
    }

    private IEnumerator SpawnDefined()
    {
        float interval = 0f;
        if (spawnOverTime > 1)
            interval = spawnOverTime / definedUnits.Count;

        Transform excludedPoint = null; 

        foreach (var unit in definedUnits)
        {
            // Исключаем точку спавна с предыдущей итерации
            var spawnPoints = new List<Transform>(this.spawnPoints);
            if (excludedPoint != null)
                spawnPoints.Remove(excludedPoint);

            var index = Random.Range(0, spawnPoints.Count);

            unit.transform.position = spawnPoints[index].position;
            unit.gameObject.SetActive(true);
            spawnedUnits.Add(unit);

            // Задаем новую исключенную точку спавна
            if (this.spawnPoints.Count > 0)
                excludedPoint = spawnPoints[index];

            if (spawnOverTime > 1)
                yield return new WaitForSeconds(interval);
            else
                yield return null;
        }
    }

    public enum WaveModes { generic, defined }
}
