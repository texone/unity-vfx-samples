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

    private TextureQuad _myTextureQuad = null;

    // Start is called before the first frame update
    private void Start()
    {
        _myEffect = GetComponent<VisualEffect>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void setItem(Item theItem, TextureQuad theTextureQuad)
    {
        _myItem = theItem;
        _myTextureQuad = theTextureQuad;
    }

    private float _myNoInputTimer = 0;
    private float _myNoLastInputTimer = 0;

    private void HandleTimer()
    {
        _myNoLastInputTimer = _myNoInputTimer;
        _myNoInputTimer += Time.deltaTime;

        if (_myNoInputTimer > _myMaxNoIpnutTime && _myNoLastInputTimer < _myMaxNoIpnutTime)
        {
           // _myEffect.Reinit();
        }
    }

    private void HandleStop()
    {
        if (!_myEffect.HasFloat("StopPoint")) return;

        if (_myNoInputTimer > _myMaxNoIpnutTime)
        {
            _myEffect.SetFloat("StopPoint", 1.1f);
           // Debug.Log("GO STOP");
        }
        else
        {
            _myEffect.SetFloat("StopPoint", 0.5f);
        }
    }

    private Vector3 _myLastWorldMouse;

    private void HandleMouseDown()
    {
        _myEffect.SetBool("MousePressed", false);
        if (!Input.GetMouseButton(0)) return;

        var myMouseScreenCoord =
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane);
        var myMouseWorldPos = mainCamera.ScreenToWorldPoint(myMouseScreenCoord);

        if (_myItem.IsMouseInside(myMouseWorldPos) && _myIsInCue)
        {
            _myEffect.SetVector3("MousePosition", myMouseWorldPos);
            _myEffect.SetVector3("LastMousePosition", _myLastWorldMouse);
            _myEffect.SetBool("MousePressed", true);
            _myNoInputTimer = 0;
        }
    }

    private int _myLastCount = 0;

    private void HandleNextStep()
    {
        if (_myItem == null) return;

        if (_myLastCount > 0 && _myEffect.aliveParticleCount == 0)
        {
            _myEffect.Reinit();
        }

        _myLastCount = _myEffect.aliveParticleCount;
    }

    private void UpdateEffectFromItem()
    {
        if (_myItem == null) return;

        if (!_myEffect.HasTexture("ContentTexture")) return;

        _myEffect.SetTexture("ContentTexture", _myTextureQuad.texture);
        _myEffect.SetFloat("ContentTextureAspect", _myTextureQuad.Aspect());
        _myEffect.SetFloat("ContentScale", _myTextureQuad.scale);
        _myEffect.SetVector3("ContentOffset", _myTextureQuad.offset);
    }

    public bool isClosed()
    {
        return _myEffect.aliveParticleCount <= 0;
    }

    private bool _myIsInCue = false;

    public void IsIncue(bool theIsInCue)
    {
        _myIsInCue = theIsInCue;
    }
    
    public bool IsIncue()
    {
        
        return _myIsInCue;
    }

    public bool IsReached(int theLimit)
    {
        return _myEffect.aliveParticleCount >= theLimit;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleTimer();
        HandleStop();
        HandleNextStep();
        HandleMouseDown();
        UpdateEffectFromItem();
        
        
    }
}