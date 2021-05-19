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
    private float maxRayDown = 0.01f, jumpCooldown = 0.2f;

    private float jumpCountDown;

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
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        agentSpawner.SpawnRandomPosition();
        goalSpawner.SpawnRandomPosition();

        transform.localRotation = Quaternion.identity * Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // 3
        sensor.AddObservation(goal.localPosition); // 3
        sensor.AddObservation(transform.InverseTransformDirection(rb.velocity)); // 3
        sensor.AddObservation(transform.localRotation); // 4

        float angle = getAngleBetweenTargetAndForward();
        sensor.AddObservation(angle); // 1
    }

    public override void OnActionReceived(float[] act)
    {
        MoveAgent(act);
        GiveReward();
    }

    private void GiveReward()
    {
        float reward;
        reward = -1f / MaxStep;

        AddReward(reward);
    }

    public void MoveAgent(float[] act)
    {
        bool grounded = DoGroundCheck();

        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var dirToGoForwardAction = act[0];
        var rotateDirAction = act[1];
        var dirToGoSideAction = act[2];
        var jumpAction = act[3];

        if (dirToGoForwardAction == 1)
            dirToGo = (grounded ? 1f : 0.5f) * 1f * transform.forward;
        else if (dirToGoForwardAction == 2)
            dirToGo = (grounded ? 1f : 0.5f) * -1f * transform.forward;
        if (rotateDirAction == 1)
            rotateDir = transform.up * -1f;
        else if (rotateDirAction == 2)
            rotateDir = transform.up * 1f;
        if (dirToGoSideAction == 1)
            dirToGo = (grounded ? 1f : 0.5f) * -0.6f * transform.right;
        else if (dirToGoSideAction == 2)
            dirToGo = (grounded ? 1f : 0.5f) * 0.6f * transform.right;
        if (jumpAction == 1)
            if (grounded && jumpCountDown <= 0)
            {
                jumpCountDown = jumpCooldown;
            }

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);
        rb.AddForce(dirToGo * movmentSpeed,ForceMode.VelocityChange);

        jumpCountDown -= Time.deltaTime;
        jumpCountDown = Mathf.Clamp(jumpCountDown, 0f, jumpCountDown);
    }

    private bool DoGroundCheck()
    {
        int layer = LayerMask.GetMask("ground");

        bool grounded = false;

        if (Physics.Raycast(transform.position, Vector3.down, maxRayDown, layer))
            grounded = true;

        return grounded;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
        actionsOut[1] = 0;
        actionsOut[2] = 0;

        if (Input.GetKey(KeyCode.D))
        {
            actionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            actionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = 2;
        }
        actionsOut[3] = Input.GetKey(KeyCode.Space) ? 1 : 0;
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
        Vector2 targetPosition2D = new Vector2(goal.transform.position.x, goal.transform.position.z);
        targetPosition2D.Normalize();

        return Vector2.Angle(forward2D, targetPosition2D);
    }

    private float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
