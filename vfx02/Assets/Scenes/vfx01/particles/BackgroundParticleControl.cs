using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;



public class BackgroundParticleControl : MonoBehaviour
{

    VisualEffect myEffect;
    
    public List<Texture2D> _myBuildings;
    public TextureContainer background = new TextureContainer("Background", 3003);
    
    public float myBuildingSwitchTime = 10;
    public bool _mySwitchBuildingByTimer = false;
    public int _myCurrentBuilding = 0;

    private Camera mainCamera;

    // Start is called before the first frame update
    private void Start()
    {
        myEffect = GetComponent<VisualEffect>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private float _myTimer = 1000;

    private void SetTexture(string theTextureName, Texture2D theTexture)
    {
        myEffect.SetTexture(theTextureName +"Texture", theTexture);
        myEffect.SetFloat(theTextureName + "TextureAspect", (float)theTexture.width / theTexture.height);
    }

    private Vector3 _myLastWorldMouse;

    private void handleMouseDown()
    {
        myEffect.SetBool("MousePressed", false);

        if (!Input.GetMouseButton(0)) return;

        Vector3 myMouseScreenCoord = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane);
        Vector3 myMouseWorldPos = mainCamera.ScreenToWorldPoint(myMouseScreenCoord);
        myEffect.SetVector3("MousePosition", myMouseWorldPos);
        myEffect.SetVector3("LastMousePosition", _myLastWorldMouse);
        myEffect.SetBool("MousePressed", true);
    }

    // Update is called once per frame
    private void Update()
    {
        _myTimer += Time.deltaTime;
        
        if(_myTimer > myBuildingSwitchTime && _mySwitchBuildingByTimer)
        {
            _myCurrentBuilding++;
            _myCurrentBuilding %= _myBuildings.Count;
            _myTimer = 0;
        }

        background.texture = _myBuildings[_myCurrentBuilding];
        background.set(myEffect);

        handleMouseDown();
    }

    private void OnDrawGizmos()
    {
    }
}
