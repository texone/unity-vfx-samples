using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;



public class ContentParticleControl : MonoBehaviour
{

    VisualEffect _myEffect;


    public float _myMaxNoIpnutTime = 10;

    private Camera mainCamera;

    private Item _myItem = null;

    // Start is called before the first frame update
    private void Start()
    {
        _myEffect = GetComponent<VisualEffect>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void setItem(Item theItem)
    {
        _myItem = theItem;
    }

    private float _myNoInputTimer = 0;

    private void handleStop()
    {
        if (!_myEffect.HasFloat("StopPoint")) return;

        _myNoInputTimer += Time.deltaTime;
        if (_myNoInputTimer > _myMaxNoIpnutTime)
        {
            _myEffect.SetFloat("StopPoint", 1.1f);
        }
        else
        {
            _myEffect.SetFloat("StopPoint", 0.5f);
        }
    }

    private Vector3 _myLastWorldMouse;

    private void handleMouseDown()
    {
        _myEffect.SetBool("MousePressed", false);
        if (!Input.GetMouseButton(0)) return;

        Vector3 myMouseScreenCoord = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane);
        Vector3 myMouseWorldPos = mainCamera.ScreenToWorldPoint(myMouseScreenCoord);

        if (_myItem.IsMouseInside(myMouseWorldPos))
        {
            Debug.Log("YO");
            _myEffect.SetVector3("MousePosition", myMouseWorldPos);
            _myEffect.SetVector3("LastMousePosition", _myLastWorldMouse);
            _myEffect.SetBool("MousePressed", true);
            _myNoInputTimer = 0;
        }

        
    }

    private void UpdateEffectFromItem()
    {
        if (_myItem == null) return;
        _myItem.SetEffect(_myEffect);
    }

    // Update is called once per frame
    private void Update()
    {
        handleStop();
        handleMouseDown();
        UpdateEffectFromItem();


    }
}
