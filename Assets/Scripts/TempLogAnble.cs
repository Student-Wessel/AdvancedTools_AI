using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempLogAnble : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogAngle();
        }
    }

    private void LogAngle()
    {
        Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 targetPosition2D = new Vector2(target.transform.position.x, target.transform.position.z);

        targetPosition2D.Normalize();

        float dot = Vector2.Dot(forward2D, targetPosition2D);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        float reward = (180 - angle) * 0.01f;
        reward -= 0.9f;

        if (!float.IsNaN(reward))
            Debug.Log(reward);
    }
}
