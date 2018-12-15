using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float minDistance;
    public float maxDistance;
    public float height;
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    private Transform _transform;
    private float x, y;
    private bool camButtonDown = false;
    private bool rotateCameraKeyPressed = false;

    void Awake()
    {
        _transform = transform;
    }

	void Start () {

        if (target == null)
            Debug.LogWarning("Camera do not have target");
        else
        {
            CameraSetup();
        }
	}

    void Update()
    {
        if (Input.GetButtonDown("Rotate Camera Button"))
        {
            camButtonDown = true;
        }
        if (Input.GetButtonUp("Rotate Camera Button"))
        {
            x = 0;
            y = 0;
            camButtonDown = false;
        }
        if (Input.GetButtonDown("Rotate Camera Horizontal Button"))
        {
            rotateCameraKeyPressed = true;
        }
        if (Input.GetButtonUp("Rotate Camera Horizontal Button"))
        {
            x = 0;
            y = 0;
            rotateCameraKeyPressed = false;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            if (rotateCameraKeyPressed)
            {
                x += Input.GetAxis("Rotate Camera Horizontal Button") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                //y = Mathf.LerpAngle(y, minDistance, maxDistance);

                RotateCamera();
            }
            else if (camButtonDown)
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                //y = Mathf.LerpAngle(y, minDistance, maxDistance);

                RotateCamera();
            }
            else
            {
                //CameraSetup();

                float wantedRotationAngle = target.eulerAngles.y;
                float wantedHeight = target.position.y + height;

                float currentRotationAngle = _transform.eulerAngles.y;
                float currentHeight = target.position.y + height;

                currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

                currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

                Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

                _transform.position = target.position;
                _transform.position -= currentRotation * Vector3.forward * minDistance;

                _transform.position = new Vector3(_transform.position.x, currentHeight, _transform.position.z);

                _transform.LookAt(target);
            }
        }
    }

    private void RotateCamera()
    {
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        var position = rotation * new Vector3(0.0f, 0.0f, -minDistance) + target.position;

        _transform.rotation = rotation;
        _transform.position = position;
    }

    public void CameraSetup()
    {
        _transform.position = new Vector3(target.position.x, target.position.y + height, target.position.z - minDistance);
        _transform.LookAt(target);
    }
}
