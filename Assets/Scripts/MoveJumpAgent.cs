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
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * 10f, 0.0f);

        transform.localPosition += direction;
        transform.localRotation = Quaternion.LookRotation(newDirection);

        //var dirToGo = Vector3.zero;
        //var rotateDir = Vector3.zero;

        //var action = act[0];
        //switch (action)
        //{
        //    case 1:
        //        dirToGo = transform.forward * 1f;
        //        break;
        //    case 2:
        //        dirToGo = transform.forward * -1f;
        //        break;
        //    case 3:
        //        rotateDir = transform.up * 1f;
        //        break;
        //    case 4:
        //        rotateDir = transform.up * -1f;
        //        break;
        //}
        //transform.Rotate(rotateDir, Time.deltaTime * 200f);
        //rb.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Horizontal");
        actionsOut[1] = -Input.GetAxis("Vertical");

        //actionsOut[0] = 0;
        //if (Input.GetKey(KeyCode.D))
        //{
        //    actionsOut[0] = 3;
        //}
        //else if (Input.GetKey(KeyCode.W))
        //{
        //    actionsOut[0] = 1;
        //}
        //else if (Input.GetKey(KeyCode.A))
        //{
        //    actionsOut[0] = 4;
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    actionsOut[0] = 2;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            SetReward(2f);
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
