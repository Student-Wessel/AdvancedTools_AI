using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEnviorment : MonoBehaviour
{
    [SerializeField]
    private Spawner goalSpawner;

    private MeshRenderer floorMeshRenderer;

    [SerializeField]
    private GameObject floorToColor;

    [SerializeField]
    private Material winMat, loseMat;

    [SerializeField]
    private SimpleAimAgent agent;

    [SerializeField]
    private Shootable shootable;

    private void Awake()
    {
        agent.episodeBeginHandler += onEpisodeBegin;

        floorMeshRenderer = floorToColor.GetComponent<MeshRenderer>();
    }

    private void onEpisodeBegin(object source, System.EventArgs args)
    {
        // Check if the last episode was success full. Then we set the floor color and make it false again.
        if (agent.WasPreviousEpisodeSuccess)
        {
            floorMeshRenderer.material = winMat;
        }
        else
        {
            floorMeshRenderer.material = loseMat;
        }
        agent.WasPreviousEpisodeSuccess = false;

        shootable.Respawn();
        goalSpawner.SpawnRandomPosition();
    }
}
