using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;



public class ParticleControl : MonoBehaviour
{

    VisualEffect myEffect;
    


    public List<Texture2D> _myBuildings;
    public TextureContainer background = new TextureContainer("Background", 3003);
    
    public float myBuildingSwitchTime = 10;
    public bool _mySwitchBuildingByTimer = false;
    public int _myCurrentBuilding = 0;

    // Start is called before the first frame update
    private void Start()
    {
        myEffect = GetComponent<VisualEffect>();
    }

    


    private float _myTimer = 1000;

    private void SetTexture(string theTextureName, Texture2D theTexture)
    {
        myEffect.SetTexture(theTextureName +"Texture", theTexture);
        myEffect.SetFloat(theTextureName + "TextureAspect", (float)theTexture.width / theTexture.height);
    }

    // Update is called once per frame
    private void Update()
    {
        _myTimer += Time.deltaTime;
        
        if(_myTimer > myBuildingSwitchTime && _mySwitchBuildingByTimer)
        {
            print(_myTimer);
            _myCurrentBuilding++;
            _myCurrentBuilding %= _myBuildings.Count;
            _myTimer = 0;
        }

        background.texture = _myBuildings[_myCurrentBuilding];
        background.set(myEffect);

        
    }

    private void OnDrawGizmos()
    {
    }
}
