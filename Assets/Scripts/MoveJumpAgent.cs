using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class MoveJumpAgent : Agent
{
    [SerializeField]
    [Range(1.0f, 5.0f)]
    private float movmentSpeed = 1f;

    [SerializeField]
    private Transform goal;

    [SerializeField]
    private GameObject elevation,floorToColor;

    [SerializeField]
    private Material winMat, loseMat;

    [SerializeField]
    private Spawner agentSpawner, goalSpawner;

    private Rigidbody rb;
    private MeshRenderer floorMeshRenderer;


    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        floorMeshRenderer = floorToColor.GetComponent<MeshRenderer>();
        movmentSpeed *= 2f;
    }

    public override void OnEpisodeBegin()
    {
        agentSpawner.SpawnRandomPosition();
        goalSpawner.SpawnRandomPosition();

        rb.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goal.localPosition);

        //sensor.AddObservation(transform.InverseTransformDirection(rb.velocity));

        //float angle = getAngleBetweenTargetAndForward();

        //if (float.IsNaN(angle) == false)
        //    sensor.AddObservation(angle);
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
            EndEpisode();
            floorMeshRenderer.material = winMat;
        }
        else if (other.CompareTag("Wall"))
        {
            SetReward(-1f);
            EndEpisode();
            floorMeshRenderer.material = loseMat;
        }
    }

    private float getAngleBetweenTargetAndForward()
    {
        Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 targetPosition2D = new Vector2(goal.transform.position.x, goal.transform.position.z).normalized;

        float dot = Vector2.Dot(forward2D, targetPosition2D);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        return angle;
    }
}
