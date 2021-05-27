using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

public class SimpleAimAgent : Agent
{
    [SerializeField]
    private ParticleSystem gunPartical = null;

    [SerializeField]
    private bool hideCursor = false;

    [SerializeField]
    [Range(0.01f, 5f)]
    private float sensitivity;

    [SerializeField]
    [Range(5f, 500f)]
    private float maxRange = 20f;

    [SerializeField]
    [Range(-90f, 90f)]
    private float minXTurnAngle = -45f, maxXTurnAngle = 45f, minYTurnAngle = -45f, maxYTurnAngle = 45f;

    [SerializeField]
    private Transform goal;

    public delegate void EpisodeBeginHandler(object source, System.EventArgs args);
    public event EpisodeBeginHandler episodeBeginHandler;

    public bool WasPreviousEpisodeSuccess = false;
    private float rotX;
    private int layerMask;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Goal");

        if (hideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.rotation = Quaternion.identity;
        rotX = 0;

        if (episodeBeginHandler != null)
            episodeBeginHandler(this, System.EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.rotation); // 4
        //sensor.AddObservation(transform.rotation.eulerAngles); // 3
        
        sensor.AddObservation(gameObject.transform.rotation.x); // 1
        sensor.AddObservation(gameObject.transform.rotation.y); // 1
        sensor.AddObservation(goal.transform.localPosition); // 3
    }

    public override void OnActionReceived(float[] vActions)
    {
        AddReward(-1f / MaxStep);

        float y = vActions[0] * sensitivity;
        rotX += vActions[1] * sensitivity;

        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);

        Vector3 euler = UnityEditor.TransformUtils.GetInspectorRotation(transform);

        if (euler.x < minXTurnAngle || euler.x > maxXTurnAngle || euler.y < minYTurnAngle || euler.y > maxYTurnAngle)
        {
            AddReward(-1f);
            EndEpisode();
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxRange, layerMask))
        {
            Shootable shootable = hit.transform.GetComponent<Shootable>();

            if (shootable != null)
            {
                shootable.OnBeingShot();
                WasPreviousEpisodeSuccess = true;

                if (vActions[2] >= 0.5f)
                {
                    SetReward(1f);
                    Shoot();
                }
                else
                {
                    SetReward(0.5f);
                }
                EndEpisode();
            }
        }
    }

    // If we hit a shootable we return true, otherwise we return false
    private bool Shoot()
    {
        gunPartical.Play();

        return false;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Mouse X");
        actionsOut[1] = Input.GetAxis("Mouse Y");
        actionsOut[2] = (Input.GetMouseButtonDown(0) ? 1f : 0f);
    }
}
