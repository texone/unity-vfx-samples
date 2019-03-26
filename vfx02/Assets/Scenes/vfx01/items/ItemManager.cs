using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

[Serializable]
public class TextureQuad : PlaceableQuad
{
    public Texture2D texture;


    public bool flip = false;

    public float scale = 1;

    public ContentParticleControl particleControl = null;

    public TextureQuad(int theQueue) : base(theQueue)
    {
    }

    public override void SetupMaterial(Material theMaterial)
    {
        theMaterial.SetTexture("_MainTex", texture);
    }

    public override void Update()
    {
        quad.transform.position = new Vector3(offset.x + (scale * Aspect()) / 2, offset.y + scale / 2, 0);
        quad.transform.localEulerAngles = new Vector3(0, 0, flip ? 180 : 0);
        quad.transform.localScale = new Vector3(scale * Aspect(), scale, 1);
    }

    public float Aspect()
    {
        return (float) texture.width / texture.height;
    }

    public void ResetQueue()
    {
        if (particleControl == null) return;
        if (!particleControl.IsIncue()) return;
        particleControl.IsIncue(!particleControl.isClosed());
    }
}

[Serializable]
public class Item
{
    public ColoredQuad mouseArea = new ColoredQuad(2999);

    public TextureQuad content = new TextureQuad(3003);
    public TextureQuad title = new TextureQuad(3002);
    public TextureQuad date = new TextureQuad(3001);
    public TextureQuad illu = new TextureQuad(3000);
    public TextureQuad lines = new TextureQuad(3004);

    private List<PlaceableQuad> _myQuads = new List<PlaceableQuad>();
    
    
    private List<TextureQuad> _myContentQuads = new List<TextureQuad>();

    private void createEffect(ItemManager theManager, TextureQuad theEffectQuad)
    {
        var myObject = theManager.CreateEffect();
        var myParticleControl = myObject.GetComponent<ContentParticleControl>();
        myParticleControl.setItem(this, theEffectQuad);
        theEffectQuad.particleControl = myParticleControl;
    }

    public void Setup(GameObject theParent, Material theTextureMaterial, Material theColorMaterial)
    {
        _myContentQuads.Add(content);
        _myContentQuads.Add(illu);
        _myContentQuads.Add(title);
        _myContentQuads.Add(date);
        _myContentQuads.Add(lines);
        
        _myQuads.AddRange(_myContentQuads);
        _myQuads.Add(mouseArea);

        mouseArea.Setup(theParent, theColorMaterial);
        _myContentQuads.ForEach(q => q.Setup(theParent, theTextureMaterial));
    }
    
    public void SetupEffect(ItemManager theManager)
    {
        createEffect(theManager, content);
        createEffect(theManager, illu);
        createEffect(theManager, title);
        createEffect(theManager, date);
        createEffect(theManager, lines);
    }

    // Update is called once per frame
    public void Update(ItemManager theManager)
    {
        mouseArea.quad.SetActive(theManager.previewPositions);
        title.quad.SetActive(theManager.previewPositions);
        date.quad.SetActive(theManager.previewPositions);
        illu.quad.SetActive(theManager.previewPositions);
        lines.quad.SetActive(theManager.previewPositions);

        _myContentQuads.ForEach(q => q.ResetQueue());
        foreach (var Quad in _myContentQuads)
        {
            Quad.particleControl.IsIncue(true);

            if (!Quad.particleControl.IsReached(250000)) break;
        }

        _myQuads.ForEach(q => q.Update());
    }


    public bool IsMouseInside(Vector3 theMouseWorldPosition)
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

    public bool previewPositions = true;

    public Item[] items;

    public GameObject CreateEffect()
    {
        return Instantiate(contentPrefab, contentParent.transform);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Array.ForEach(items, item =>
        {
            item.Setup(gameObject, textureMaterial, colorMaterial);
            item.SetupEffect(this);
        });
    }

    
    // Update is called once per frame
    private void Update()
    {
        Array.ForEach(items, item => item.Update(this));
    }
}