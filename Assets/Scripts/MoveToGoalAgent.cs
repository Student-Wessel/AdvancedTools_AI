using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    [Range(0.1f,4)]
    private float moveSpeed = 1f;

    private bool foundGoal = false;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
        target.localPosition = new Vector3(Random.Range(-6, -10), 0, Random.Range(-3, 3));

        foundGoal = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float moveX = vectorAction[0];
        float moveZ = vectorAction[1];

        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Horizontal");
        actionsOut[1] = -Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            SetReward(1f);
            EndEpisode();
        } 
        else if (other.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
}
