using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceToPlane : MonoBehaviour
{
    public Transform cam;
    public Transform actor;
    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var plane = new Plane(cam.rotation * Vector3.forward, cam.position);
        var distToPlane = plane.GetDistanceToPoint(actor.position);
        Debug.Log("DistToPlane = " + distToPlane);
        canvas.planeDistance = distToPlane;
        // create a plane using the camera
    //    var trans = camera.GetComponent<
    }
}
