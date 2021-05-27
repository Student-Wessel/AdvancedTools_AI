using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(ParticleSystem))]
public class Bottle : Shootable
{
    private ParticleSystem particleSystem = null;
    private MeshRenderer meshRenderer = null;
    private Collider collider = null;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    public override void OnBeingShot()
    {
        meshRenderer.enabled = false;
        particleSystem.Play();
        collider.enabled = false;
    }

    public override void Respawn()
    {
        meshRenderer.enabled = true;
        collider.enabled = true;
    }
}
