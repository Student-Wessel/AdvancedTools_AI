using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;


[RequireComponent(typeof(Rigidbody))]
public class MoveJumpAgent : Agent
{
    private GlobalAgentConfig config;

    private Transform goal;
    private Rigidbody rb;
    private AgentEnvironment agentEnvironment;

    public delegate void EpisodeBeginHandler(object source, System.EventArgs args);
    public event EpisodeBeginHandler episodeBeginHandler;

    private float jumpCooldown = 0f;
    
    [HideInInspector]
    public bool WasPreviousEpisodeSuccess = false;

    private bool isGrounded = true;
    private bool wasGrounded = true;

    private bool isGroundedOnElevation = false;
    private Vector3 previousVelocity = Vector3.zero;

    private ForceMode jumpForceMode = ForceMode.Acceleration;
    private ForceMode gravityForceMode = ForceMode.Acceleration;

    float halfScale;

    public void Awake()
    {
        config = FindObjectOfType<GlobalAgentConfig>();

        if (config == null)
        {
            gameObject.SetActive(false);
            Debug.LogError("NO CONFIG FOUND");
            return;
        }

        BehaviorParameters agentBehaviour = GetComponent<BehaviorParameters>();
        agentBehaviour.Model = config.Model;

        agentEnvironment = GetComponentInParent<AgentEnvironment>();
        if (agentEnvironment == null)
        {
            gameObject.SetActive(false);
            Debug.LogError("NO PARENT ENVIRONMENT FOUND");
            return;
        }

        foreach(Transform child in transform.parent)
        {
            if (child.tag == "Goal")
            {
                goal = child;
                break;
            }
        }
        if (goal == null)
        {
            gameObject.SetActive(false);
            Debug.LogError("NO GOAL IN ENVIRONMENT FOUND");
            return;
        }

        halfScale = transform.localScale.magnitude * 0.30f;

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
        sensor.AddObservation(transform.InverseTransformDirection(rb.velocity)); // 3
        sensor.AddObservation(IsOnElevation()); // 1
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
        isGroundedOnElevation = IsOnElevation();

        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized * Time.deltaTime * config.MovmentSpeed;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, Time.deltaTime * 10f, 5.5f);

        if (newDirection != Vector3.zero)
            transform.localRotation = Quaternion.LookRotation(newDirection);

        // Jump
        if (jumpCooldown <= 0 && isGrounded && moveJumpInput > 0.5f)
        {
            rb.AddForce(Vector3.up * config.JumpForce, jumpForceMode);
            jumpCooldown = config.CooldownAfterJump;
        }
        else if (jumpCooldown > 0)
            jumpCooldown -= Time.deltaTime;

        // Grounded movment speed
        if (isGrounded)
        {
            //transform.localPosition += direction;
            rb.AddForce(newDirection * config.MovmentSpeed, ForceMode.Impulse);
        }// Air movment speed
        if (!isGrounded)
        {
            rb.AddForce((newDirection * config.MovmentSpeed) * config.AirControl, ForceMode.Impulse);
            rb.AddForce(-Vector3.up * config.GravityMultiplier, gravityForceMode);
        }
        // We landed
        if (wasGrounded == false && isGrounded == true)
        {
            Vector3 correction = new Vector3(previousVelocity.x, 0, previousVelocity.z);
            Vector3 position = transform.position;
            float y = transform.position.y;

            if (y < 0)
                y = 0;

            transform.position = new Vector3(position.x, y, position.z);
            rb.velocity = (new Vector3(correction.x, 0, correction.z)*0.5f);
        }


        wasGrounded = isGrounded;
        previousVelocity = rb.velocity;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Horizontal");
        actionsOut[1] = -Input.GetAxis("Vertical");
        actionsOut[2] = (Input.GetKey(KeyCode.Space) ? 1 : 0);
    }

    private bool IsOnElevation()
    {
        if (Physics.Raycast(transform.position + new Vector3(halfScale, 0, halfScale), -Vector3.up, config.GroundCheckHeight, config.ElevationLayer))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + new Vector3(halfScale, 0, -halfScale), -Vector3.up, config.GroundCheckHeight, config.ElevationLayer))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + new Vector3(-halfScale, 0, halfScale), -Vector3.up, config.GroundCheckHeight, config.ElevationLayer))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + new Vector3(-halfScale, 0, -halfScale), -Vector3.up, config.GroundCheckHeight, config.ElevationLayer))
        {
            return true;
        }

        return false;
    }

    private bool IsGrounded()
    {
        if (Physics.Raycast(transform.position + new Vector3(halfScale, 0, halfScale), -Vector3.up, config.GroundCheckHeight, config.GroundLayer | config.ElevationLayer))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + new Vector3(halfScale, 0, -halfScale), -Vector3.up, config.GroundCheckHeight, config.GroundLayer | config.ElevationLayer))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + new Vector3(-halfScale, 0, halfScale), -Vector3.up, config.GroundCheckHeight, config.GroundLayer | config.ElevationLayer))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + new Vector3(-halfScale, 0, -halfScale), -Vector3.up, config.GroundCheckHeight, config.GroundLayer | config.ElevationLayer))
        {
            return true;
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            AddReward(1f);
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
