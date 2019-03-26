using System;
using UnityEngine;

[Serializable]
public abstract class PlaceableQuad
{

    public GameObject quad;

    public Vector3 offset = new Vector3();

    public int queue;

    public PlaceableQuad(int theQueue)
    {

        queue = theQueue;
    }

    public abstract void SetupMaterial(Material theMaterial);

    public abstract void Update();

    public virtual void Setup(GameObject parent, Material theMaterial)
    {
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.SetParent(parent.transform);
        quad.GetComponent<Renderer>().material = theMaterial;
        quad.GetComponent<Renderer>().material.renderQueue = queue;
        SetupMaterial(quad.GetComponent<Renderer>().material);
        quad.layer = parent.layer;
    }
}