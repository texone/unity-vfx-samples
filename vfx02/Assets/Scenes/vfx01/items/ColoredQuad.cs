using System;
using UnityEngine;

[Serializable]
public class ColoredQuad : PlaceableQuad
{

    public Vector2 scale = new Vector2(1,1);

    public ColoredQuad(int theQueue) : base(theQueue) { }

    public override void SetupMaterial(Material theMaterial)
    {
       
    }

    public override void Update()
    {
        quad.transform.position = new Vector3(offset.x + scale.x / 2, offset.y + scale.y / 2, 0);
        quad.transform.localScale = new Vector3(scale.x, scale.y, 1);
    }
}