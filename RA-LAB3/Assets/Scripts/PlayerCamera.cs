using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCamera : MonoBehaviour
{

    public Transform Target;

    public float dx, dy = 240;

    public float Distance = 8f;

    public float RotateSpeed = 90;

    public float AngleYMax = 200, AngleYMin = 265;

    static PlayerCamera instance;

    public static PlayerCamera Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        dx += Input.GetAxis("Mouse X") * RotateSpeed * Time.deltaTime;
        dy -= Input.GetAxis("Mouse Y") * RotateSpeed * Time.deltaTime;
        dy = Mathf.Clamp(dy, AngleYMin, AngleYMax);
        Vector3 d = Vector3.forward * Distance;
        d = Quaternion.Euler(dy, dx, 0) * d;
        transform.position = Target.position + d;
        transform.LookAt(Target);

        if(Input.GetKey(KeyCode.Escape))
        {
            if(Input.GetKey(KeyCode.E))
            {
                Application.Quit();
            }
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}
