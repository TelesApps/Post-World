using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    Camera camera;
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        if(camera.isActiveAndEnabled)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                float moveForward = Input.GetAxis("Mouse ScrollWheel") * 10;
                camera.orthographicSize += -moveForward;
                //transform.position += Vector3.forward * moveForward * moveSpeed * Time.deltaTime;
            }
            if (Input.anyKey)
            {
                float moveHorizontal = Input.GetAxis("Horizontal");
                float moveVertical = Input.GetAxis("Vertical");
                if (moveHorizontal != 0 || moveVertical != 0)
                {
                    //Debug.Log(moveHorizontal);
                    transform.position += Vector3.right * Mathf.Round(moveHorizontal) * moveSpeed * Time.deltaTime;
                    transform.position += Vector3.up * moveVertical * moveSpeed * Time.deltaTime;
                    //transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
                }
            }
        }
    }
}
