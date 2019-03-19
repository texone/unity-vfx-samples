using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

[Serializable]
public abstract class PlaceableQuad
{

    public GameObject quad;

    public Vector2 offset = new Vector2();

    public int queue;

    public PlaceableQuad(int theQueue)
    {

        queue = theQueue;
    }

    public abstract void SetupMaterial(Material theMaterial);

    public abstract void UpdateTransform();

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

[Serializable]
public class ColoredQuad : PlaceableQuad
{

    public Vector2 scale = new Vector2(1,1);

    public ColoredQuad(int theQueue) : base(theQueue) { }

    public override void SetupMaterial(Material theMaterial)
    {
       
    }

    public override void UpdateTransform()
    {
        quad.transform.position = new Vector3(offset.x + scale.x / 2, offset.y + scale.y / 2, 0);
        quad.transform.localScale = new Vector3(scale.x, scale.y, 1);
    }
}

[Serializable]
public class TextureContainer : PlaceableQuad
{

    public Texture2D texture;

    private string textureName;

    public Boolean flip = false;

    public float scale = 1;

    public TextureContainer(string theName, int theQueue) : base(theQueue)
    {
        textureName = theName;
    }

    public override void SetupMaterial(Material theMaterial)
    {
        theMaterial.SetTexture("_MainTex", texture);
    }

    public override void UpdateTransform() {
        quad.transform.position = new Vector3(offset.x + (scale * Aspect() )/ 2, offset.y + scale / 2, 0);
        quad.transform.localEulerAngles = new Vector3(0,0, flip ? 180 : 0);
        quad.transform.localScale = new Vector3(scale * Aspect(), scale , 1);
    }

    private float Aspect()
    {
        return (float)texture.width / texture.height;
    }

    public void set(VisualEffect effect)
    {
        if (!effect.HasTexture(textureName + "Texture")) return;

        effect.SetTexture(textureName + "Texture", texture);
        effect.SetFloat(textureName + "TextureAspect", Aspect());
        effect.SetFloat(textureName + "Scale", scale);
        effect.SetVector2(textureName + "Offset", offset);
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
    public ColoredQuad mouseArea = new ColoredQuad(2999);

    public TextureContainer content = new TextureContainer("Content", 3003);
    public TextureContainer title = new TextureContainer("Title", 3002);
    public TextureContainer date = new TextureContainer("Date", 3001);
    public TextureContainer illu = new TextureContainer("Illu", 3000);

    private List<PlaceableQuad> _myQuads = new List<PlaceableQuad>();

    public void Setup(GameObject theParent, Material theTextureMaterial, Material theColorMaterial) {
        _myQuads.Add(mouseArea);
        _myQuads.Add(content);
        _myQuads.Add(title);
        _myQuads.Add(date);
        _myQuads.Add(illu);

        mouseArea.Setup(theParent, theColorMaterial);
        content.Setup(theParent, theTextureMaterial);
        title.Setup(theParent, theTextureMaterial);
        date.Setup(theParent, theTextureMaterial);
        illu.Setup(theParent, theTextureMaterial);
    }

    public void SetEffect(VisualEffect theEffect)
    {
        content.set(theEffect);
        title.set(theEffect);
        date.set(theEffect);
        illu.set(theEffect);
    }
    // Update is called once per frame
    public void Update(ItemManager theManager)
    {
        mouseArea.quad.SetActive(theManager.previewPositions);
        title.quad.SetActive(theManager.previewPositions);
        date.quad.SetActive(theManager.previewPositions);
        illu.quad.SetActive(theManager.previewPositions);

        _myQuads.ForEach(q => q.UpdateTransform());
    }


    public Boolean IsMouseInside(Vector3 theMouseWorldPosition)
    {
        return 
            theMouseWorldPosition.x > mouseArea.offset.x && 
            theMouseWorldPosition.x < mouseArea.offset.x + mouseArea.scale.x &&
            theMouseWorldPosition.y > mouseArea.offset.y &&
            theMouseWorldPosition.y < mouseArea.offset.y + mouseArea.scale.y;
    }
}

public class ItemManager : MonoBehaviour
{
    public Material textureMaterial;
    public Material colorMaterial;

    public GameObject contentPrefab;
    public GameObject contentParent;

    public Boolean previewPositions = true;

    public Item[] items;
    // Start is called before the first frame update
    void Start()
    {
        Array.ForEach(items, item => {
            item.Setup(gameObject, textureMaterial, colorMaterial);

            GameObject myObject = Instantiate(contentPrefab, contentParent.transform);
            ContentParticleControl myParticleControl = myObject.GetComponent<ContentParticleControl>();
            myParticleControl.setItem(item);
        });
    }

    // Update is called once per frame
    void Update()
    {
        Array.ForEach(items, item => item.Update(this));
    }
}
