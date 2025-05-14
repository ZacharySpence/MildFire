using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    [SerializeField] float yMin, yMax;
    [SerializeField] float edgeThreshold = 10f; //how far from edge to allow scrolling
    [SerializeField] float scrollSpeed = 2f;
    // Update is called once per frame
    void Update()
    {
        //Check reach edge of screen, if so then move camera up/down dependent on y value,
        Vector3 cameraPos = Camera.main.transform.position;

        // Mouse near top of screen
        if (Input.mousePosition.y >= Screen.height - edgeThreshold)
        {
            cameraPos.y += scrollSpeed * Time.deltaTime;
        }
        // Mouse near bottom of screen
        else if (Input.mousePosition.y <= edgeThreshold)
        {
            cameraPos.y -= scrollSpeed * Time.deltaTime;
        }

        // Clamp Y position
        cameraPos.y = Mathf.Clamp(cameraPos.y, yMin, yMax);

        Camera.main.transform.position = cameraPos;
    }
}
