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
        if (Input.GetKeyDown(KeyCode.A))
            LogAngle();

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

        float angle = Vector2.Angle(forward2D, targetPosition2D);
        float angleReward = Map(180 - angle, 0.0f, 180.0f, -1.0f, 1.0f);

        float distance = (transform.position - target.position).magnitude;
        float distanceReward = Map(20 - distance, 0.0f, 20.0f, -1.0f, 1.0f);

        Debug.Log(distanceReward);
    }

    private float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
