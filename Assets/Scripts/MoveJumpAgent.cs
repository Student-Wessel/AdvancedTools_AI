using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


[RequireComponent(typeof(Rigidbody))]
public class MoveJumpAgent : Agent
{
    [SerializeField]
    [Range(1.0f, 20.0f)]
    private float movmentSpeed = 2f;

    [SerializeField]
    private Transform goal;

    private Rigidbody rb;
    private AgentEnvironment agentEnvironment;

    public delegate void EpisodeBeginHandler(object source, System.EventArgs args);
    public event EpisodeBeginHandler episodeBeginHandler;

    public bool WasPreviousEpisodeSuccess = false;

    public void Awake()
    {
        agentEnvironment = GetComponentInParent<AgentEnvironment>();
        if (agentEnvironment == null)
        {
            gameObject.SetActive(false);
            Debug.LogError("NO PARENT ENVIRONMENT FOUND");
            return;
        }

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void OnEpisodeBegin()
    {
        rb.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (episodeBeginHandler != null)
            episodeBeginHandler(this, System.EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goal.localPosition);
    }

    public override void OnActionReceived(float[] act)
    {
        AddReward(-1f / MaxStep);
        MoveAgent(act);
    }

    public void MoveAgent(float[] act)
    {
        float moveX = act[0];
        float moveZ = act[1];

        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized * Time.deltaTime * movmentSpeed;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * 10f, 5.5f);

        transform.localPosition += direction;
        if (newDirection != Vector3.zero)
            transform.localRotation = Quaternion.LookRotation(newDirection);
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
            WasPreviousEpisodeSuccess = true;
            EndEpisode();
        }
        else if (other.CompareTag("Wall"))
        {
            SetReward(-1f);
            WasPreviousEpisodeSuccess = false;
            EndEpisode();
        }
    }
}
