using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamGrabber : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
	// Grab texture
        WebCamTexture webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name, 1280, 720, 30);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webcamTexture;
        webcamTexture.Play();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
