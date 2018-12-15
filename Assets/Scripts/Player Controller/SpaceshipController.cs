using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{

    public float speed = 0.8f;
    public float turnSpeed = 0.15f;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Forward"))
        { //Spacebar by default will make it move forward 
            GetComponent<Rigidbody>().AddRelativeForce (Vector3.forward*speed);
        }

        //GetComponent<Rigidbody>().AddRelativeTorque ((Input.GetAxis("Vertical"))*turnSpeed,0,0);
   
        GetComponent<Rigidbody>().AddRelativeTorque(0, (Input.GetAxis("Horizontal")) * turnSpeed, 0);
    }
}