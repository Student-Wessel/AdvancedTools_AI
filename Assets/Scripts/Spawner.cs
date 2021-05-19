using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToSpawn;

    private Transform objectOgParent;

    private List<SpawnArea> spawnAreas;
    private void Awake()
    {
        if (objectToSpawn == null)
            gameObject.SetActive(false);

        objectOgParent = objectToSpawn.transform.parent;

        spawnAreas = new List<SpawnArea>();

        foreach (Transform child in transform)
        {
            SpawnArea childBox = child.GetComponent<SpawnArea>();
            if (childBox != null)
                spawnAreas.Add(childBox);
        }
    }

    public void SpawnRandomPosition()
    {
        SpawnArea spawnArea = spawnAreas[Random.Range(0, spawnAreas.Count)];
        objectToSpawn.transform.SetParent(spawnArea.transform);

        float xScale = objectToSpawn.transform.localScale.x;
        float zScale = objectToSpawn.transform.localScale.z;

        float randomX = Random.Range(-0.5f + xScale * 0.5f, 0.5f - xScale * 0.5f);
        float randomZ = Random.Range(-0.5f + zScale * 0.5f, 0.5f - zScale * 0.5f);

        objectToSpawn.transform.localPosition = new Vector3(randomX, 0, randomZ);
        objectToSpawn.transform.SetParent(objectOgParent);
    }
}
