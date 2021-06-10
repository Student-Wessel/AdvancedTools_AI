using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.Barracuda;

public class GlobalAgentConfig : MonoBehaviour
{
    [SerializeField]
    private int maxStep = 3000;

    [SerializeField]
    [Range(1.0f, 20.0f)]
    private float movmentSpeed = 2f;

    [SerializeField]
    private LayerMask groundLayer, elevationLayer;

    [SerializeField]
    private float jumpForce = 5f, gravityMultiplier = 2f, groundCheckHeight = 0.01f, cooldownAfterJump = 0.05f, airControl = 0.1f;

    [SerializeField]
    NNModel model;

    public int MaxStep { get => MaxStep; }
    public float MovmentSpeed { get => movmentSpeed; }
    public float JumpForce { get => jumpForce; }
    public float GravityMultiplier { get => gravityMultiplier; }
    public float GroundCheckHeight { get => groundCheckHeight; }
    public float CooldownAfterJump { get => cooldownAfterJump; }
    public float AirControl { get => airControl; }
    public LayerMask GroundLayer { get => groundLayer; }
    public LayerMask ElevationLayer { get => elevationLayer; }
    public NNModel Model { get => model; }
}
