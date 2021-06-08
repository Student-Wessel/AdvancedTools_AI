using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevation : MonoBehaviour
{
    [SerializeField]
    private Transform goal = null;

    private bool hasGoal = false;

    [SerializeField]
    [Range(6,10)]
    private float minScale = 6, maxScale = 12;

    float xScale = 0;
    float zScale = 0;

    private void Awake()
    {
        float xScale = transform.localScale.x / 2;
        float zScale = transform.localScale.z / 2;

        if (goal != null)
            hasGoal = true;
    }

    public void RandomScale()
    {
        Vector3 newRandomScale = new Vector3(Random.Range(minScale, maxScale), 3, Random.Range(minScale, maxScale));
        transform.localScale = newRandomScale;
        xScale = transform.localScale.x / 2;
        zScale = transform.localScale.z / 2;
    }

    public void goalRandomPosition()
    {
        if (hasGoal)
        {
            Vector3 randomAdd = new Vector3(Random.Range(-xScale + 1f, xScale - 1f), 0, Random.Range(-zScale + 1f, zScale - 1f));
            goal.position = transform.position + new Vector3(0, 2, 0);
            goal.position += randomAdd;
        }
    }
}
