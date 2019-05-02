using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    private float sensitivity;
    private Vector3 mouseReference;
    private Vector3 mouseOffset;
    private Vector3 rotation;
    private bool isRotating;
    public bool active = false;

    void Start()
    {
        sensitivity = 0.4f;
        rotation = Vector3.zero;
    }

    void Update()
    {
        if (isRotating && active)
        {
            // offset
            mouseOffset = (Input.mousePosition - mouseReference);

            // apply rotation
            rotation.y = -(mouseOffset.x + mouseOffset.y) * sensitivity;
            rotation.y = -(mouseOffset.x) * sensitivity;
            rotation.x = -(mouseOffset.y) * sensitivity;

            // rotate
            transform.Rotate(rotation);

            // store mouse
            mouseReference = Input.mousePosition;
        }
    }

    void OnMouseDown()
    {
        // rotating flag
        isRotating = true;

        // store mouse
        mouseReference = Input.mousePosition;
    }

    void OnMouseUp()
    {
        // rotating flag
        isRotating = false;
    }

}
