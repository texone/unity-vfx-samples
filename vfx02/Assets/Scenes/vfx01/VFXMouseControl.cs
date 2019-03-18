using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;



public class VFXMouseControl : MonoBehaviour
{

    VisualEffect myEffect;
    Plane myGroundPlane;
    
    //This is the distance the clickable plane is from the camera. Set it in the Inspector before running.
    public float myPlaneCameraDistance;
    Vector3 myPlanePosFromCamera;

    public float _myMaxNoIpnutTime = 10;

    // Start is called before the first frame update
    private void Start()
    {
        myEffect = GetComponent<VisualEffect>();

        myGroundPlane = new Plane();
    }

    void DrawPlane(Vector3 position, Vector3 normal)
    {

        Vector3 v3 ;

        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;

        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        Debug.DrawLine(corner0, corner2, Color.green);
        Debug.DrawLine(corner1, corner3, Color.green);
        Debug.DrawLine(corner0, corner1, Color.green);
        Debug.DrawLine(corner1, corner2, Color.green);
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner0, Color.green);
        Debug.DrawRay(position, normal, Color.red);
    }

    Ray myCameraMouseRay;
    Vector3 myPlaneHit;
    Vector3 myLastPlaneHit;

    

    private float _myNoInputTimer = 0;

   

    // Update is called once per frame
    private void Update()
    {
        

        _myNoInputTimer += Time.deltaTime;
        if (_myNoInputTimer > _myMaxNoIpnutTime)
        {
            myEffect.SetFloat("StopPoint", 1.1f);
        }
        else
        {
            myEffect.SetFloat("StopPoint", 0.5f);
        }
  

        myEffect.SetBool("MousePressed", false);
        if (Input.GetMouseButton(0))
        {
            ;
            Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition));
            //This is how far away from the Camera the plane is placed
            myPlanePosFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - myPlaneCameraDistance);
            myGroundPlane.SetNormalAndPosition(Vector3.forward, myPlanePosFromCamera);

            myCameraMouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(myCameraMouseRay.origin, myCameraMouseRay.direction * 10, Color.yellow);
           
    
            //Initialise the enter variable
            float enter = 0.0f;

            if (myGroundPlane.Raycast(myCameraMouseRay, out enter))
            {
                //Get the point that is clicked
                myLastPlaneHit = myPlaneHit;
                myPlaneHit = myCameraMouseRay.GetPoint(enter);

                myEffect.SetVector3("MousePosition", myPlaneHit);
                myEffect.SetVector3("LastMousePosition", myLastPlaneHit);
                myEffect.SetBool("MousePressed", true);
                _myNoInputTimer = 0;
            }

            DrawPlane(myPlanePosFromCamera, myGroundPlane.normal); 

            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(myCameraMouseRay);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(myPlaneHit, 1);
    }
}
