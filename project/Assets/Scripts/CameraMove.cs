using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float speed = 2f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 cameraPosition = mainCamera.transform.position;
        cameraPosition.x += horizontalInput * speed * Time.deltaTime;
        mainCamera.transform.position = cameraPosition;
    }
}
