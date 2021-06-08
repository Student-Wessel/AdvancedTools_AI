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
    private float jumpForce = 5f, gravityMultiplier = 2f;

    [SerializeField]
    private Transform goal;

    private Rigidbody rb;
    private AgentEnvironment agentEnvironment;

    public delegate void EpisodeBeginHandler(object source, System.EventArgs args);
    public event EpisodeBeginHandler episodeBeginHandler;

    public bool WasPreviousEpisodeSuccess = false;

    [SerializeField]
    private float groundCheckHeight = 0.01f;

    [SerializeField]
    private float cooldownAfterJump = 0.05f;
    private float jumpCooldown = 0f;

    private bool isGrounded = true;
    private int groundLayer;

    public ForceMode jumpForceMode = ForceMode.Acceleration;
    public ForceMode gravityForceMode = ForceMode.Acceleration;

    float halfScale;

    public void Awake()
    {
        agentEnvironment = GetComponentInParent<AgentEnvironment>();
        if (agentEnvironment == null)
        {
            gameObject.SetActive(false);
            Debug.LogError("NO PARENT ENVIRONMENT FOUND");
            return;
        }

        groundLayer = LayerMask.GetMask("Ground");
        halfScale = transform.localScale.magnitude / 2;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
        sensor.AddObservation(transform.localPosition); // 3
        sensor.AddObservation(goal.localPosition); // 3
        sensor.AddObservation(rb.velocity); // 3
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
        float moveJumpInput = act[2];

        isGrounded = IsGrounded();

        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized * Time.deltaTime * movmentSpeed;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * 10f, 5.5f);

        if (newDirection != Vector3.zero)
            transform.localRotation = Quaternion.LookRotation(newDirection);

        // Jump
        if (jumpCooldown <= 0 && isGrounded && moveJumpInput > 0.5f)
        {
            rb.AddForce(Vector3.up * jumpForce, jumpForceMode);
            jumpCooldown = cooldownAfterJump;
        }
        else if (jumpCooldown > 0)
            jumpCooldown -= Time.deltaTime;

        // Grounded movment speed
        if (isGrounded)
        {
            //transform.localPosition += direction;
            rb.AddForce(newDirection * movmentSpeed, ForceMode.Impulse);
        }// Air movment speed
        if (!isGrounded)
        {
            rb.AddForce((newDirection * movmentSpeed)*0.05f, ForceMode.Impulse);
            rb.AddForce(-Vector3.up * gravityMultiplier, gravityForceMode);
        }


    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Horizontal");
        actionsOut[1] = -Input.GetAxis("Vertical");
        actionsOut[2] = (Input.GetKey(KeyCode.Space) ? 1 : 0);
    }

    private bool IsGrounded()
    {
        if (Physics.Raycast(transform.position + new Vector3(halfScale, 0, halfScale), -Vector3.up, groundCheckHeight,groundLayer))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + new Vector3(halfScale, 0, -halfScale), -Vector3.up, groundCheckHeight, groundLayer))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + new Vector3(-halfScale, 0, halfScale), -Vector3.up, groundCheckHeight, groundLayer))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + new Vector3(-halfScale, 0, -halfScale), -Vector3.up, groundCheckHeight, groundLayer))
        {
            return true;
        }

        return false;
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
