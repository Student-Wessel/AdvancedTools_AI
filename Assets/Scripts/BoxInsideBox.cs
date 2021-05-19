using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInsideBox : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private List<BoxCollider> spawnAreas;

    private GameObject insideBox = null;

    private void Awake()
    {
        spawnAreas = new List<BoxCollider>();

        foreach (Transform child in transform)
        {
            BoxCollider childBox = child.GetComponent<BoxCollider>();
            if (childBox != null)
                spawnAreas.Add(childBox);
        }

        SpawnRandomBox();
    }


    private void Update()
    {

            SpawnRandomBox();
        
    }

    private void SpawnRandomBox()
    {
        if (insideBox != null)
            Destroy(insideBox);

        BoxCollider spawnArea = spawnAreas[Random.Range(0, spawnAreas.Count)];
        GameObject newBox = Instantiate(prefab, null);
        newBox.transform.SetParent(spawnArea.transform);

        float xScale = newBox.transform.localScale.x;
        float zScale = newBox.transform.localScale.z;

        float randomX = Random.Range(-0.5f + xScale * 0.5f , 0.5f - xScale * 0.5f );
        float randomZ = Random.Range(-0.5f + zScale * 0.5f , 0.5f - zScale * 0.5f );

        newBox.transform.localPosition = new Vector3(randomX, 0, randomZ);

        //newBox.transform.localPosition = new Vector3(randomX, 0, randomZ);
    }
}
