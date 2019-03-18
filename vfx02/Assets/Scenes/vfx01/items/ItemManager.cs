using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

[Serializable]
public class TextureContainer
{
    public GameObject quad;

    public Texture2D texture;

    public Vector2 offset = new Vector2();

    public float scale = 1;

    public string name;

    public Boolean flip = false;

    private int queue;

    public TextureContainer(string theName, int theQueue)
    {
        name = theName;
        queue = theQueue;
    }

    public void Setup(GameObject parent, Material theMaterial)
    {
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.SetParent(parent.transform);
        quad.GetComponent<Renderer>().material = theMaterial;
        quad.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
        quad.GetComponent<Renderer>().material.renderQueue = queue;
        quad.layer = parent.layer;
        Debug.Log(parent.layer + "," + quad.layer);
    }

    public void Update() {
        quad.transform.position = new Vector3(offset.x, offset.y, 0);
        quad.transform.localEulerAngles = new Vector3(0,0, flip ? 180 : 0);
        quad.transform.localScale = new Vector3(scale * Aspect(), scale , 1);
    }

    private float Aspect()
    {
        return (float)texture.width / texture.height;
    }

    public void set(VisualEffect effect)
    {
        effect.SetTexture(name + "Texture", texture);
        effect.SetFloat(name + "TextureAspect", Aspect());
        effect.SetFloat(name + "Scale", scale);
        effect.SetVector2(name + "Offset", offset);
    }

    public void OnDrawGizmos()
    {
        float x0 = offset.x;
        float x1 = offset.x + scale;
        float y0 = offset.y;
        float y1 = offset.y + scale;
        Gizmos.DrawLine(new Vector3(x0, y0, 0), new Vector3(x1, y0, 0));
        Gizmos.DrawLine(new Vector3(x0, y1, 0), new Vector3(x1, y1, 0));
        Gizmos.DrawLine(new Vector3(x0, y0, 0), new Vector3(x0, y1, 0));
        Gizmos.DrawLine(new Vector3(x1, y0, 0), new Vector3(x1, y1, 0));
    }
}

[Serializable]
public class Item
{
    public TextureContainer content = new TextureContainer("Content", 3003);
    public TextureContainer title = new TextureContainer("Title", 3002);
    public TextureContainer date = new TextureContainer("Date", 3001);
    public TextureContainer illu = new TextureContainer("Illu", 3000);

    public void Setup(GameObject parent, Material theMaterial) {
        content.Setup(parent, theMaterial);
        illu.Setup(parent, theMaterial);
        title.Setup(parent, theMaterial);
        date.Setup(parent, theMaterial);
    }
    // Update is called once per frame
    public void Update()
    {
        content.Update();
        illu.Update();
        title.Update();
        date.Update();
    }
}

public class ItemManager : MonoBehaviour
{
    public Material textureMaterial;
    public Item[] items;
    // Start is called before the first frame update
    void Start()
    {
        Array.ForEach(items, item => {
            item.Setup(gameObject, textureMaterial);
        });
    }

    // Update is called once per frame
    void Update()
    {
        Array.ForEach(items, item => {
            item.Update();
        });
    }
}
