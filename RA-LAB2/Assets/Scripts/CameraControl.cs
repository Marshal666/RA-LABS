using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    public float ZSpeed = 15f;
    public float XSpeed = 15f;
    public float YSpeed = 15f;
    public float RotYSpeed = 90f;

    Vector3 oldMousePos = default;
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position += transform.rotation * Vector3.forward * Input.GetAxis("Vertical") * ZSpeed * Time.deltaTime;
        transform.position += transform.rotation * Vector3.right * Input.GetAxis("Horizontal") * ZSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E))
            transform.position += Vector3.up * YSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
            transform.position -= Vector3.up * YSpeed * Time.deltaTime;
        if(Input.GetMouseButton(1))
		{
            transform.eulerAngles += Vector3.up * RotYSpeed * Time.deltaTime * (Input.mousePosition - oldMousePos).x;
		}
        oldMousePos = Input.mousePosition;
    }
}
