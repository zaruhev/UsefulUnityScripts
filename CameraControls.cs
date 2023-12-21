/*
    Simple and elegant camera movement controls intended for Unity camera objects.
    Copyright (C) 2023 Sarah Evans

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float rotationSpeed = 2f;
    public float movementSpeed = 5f;

    private bool isCursorVisible = true;
    private readonly float doubleClickTime = 0.3f;
    private float lastClickTime = 0f;

    private float horizontalRotation = 0f;
    private float verticalRotation = 0f;

    private float baseFieldOfView;
    public float sprintSpeedFOVMult = 1.20f;

    public float linearFOVDeltaMult = 10.0f;

    void Start()
    {
        baseFieldOfView = GetComponent<Camera>().fieldOfView;
    }

    void Update()
    {
        // Double-click to toggle cursor visibility
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick < doubleClickTime)
            {
                isCursorVisible = !isCursorVisible;
                Cursor.lockState = isCursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isCursorVisible;
            }
            lastClickTime = Time.time;
        }

        // Rotate only when the cursor is hidden
        if (!Cursor.visible)
        {
            // Rotation with mouse movement
            horizontalRotation += Input.GetAxis("Mouse X") * rotationSpeed;
            verticalRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;

            // Clamp vertical rotation to avoid flipping
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        }

        // Movement with WASD
        float horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed;
        float verticalMovement = Input.GetAxis("Vertical") * movementSpeed;

        Vector3 movement = new Vector3(horizontalMovement, 0, verticalMovement);

        float finalFOV = baseFieldOfView;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement *= 1.5f;
            finalFOV *= sprintSpeedFOVMult;
        }
        // Super clever interpolation
        GetComponent<Camera>().fieldOfView = Mathf.Lerp(
            GetComponent<Camera>().fieldOfView, 
            finalFOV, 
            Time.deltaTime * linearFOVDeltaMult);

        transform.Translate(movement * Time.deltaTime);
    }
}
