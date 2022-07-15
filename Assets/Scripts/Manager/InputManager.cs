using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private LevelManager levelManager;
    public Vector3 direction;
    private Vector3 touchPosition;


    private void Update()
    {
        if (!LevelManager.Instance.isGameOn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                LevelManager.Instance.isGameOn = true;
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
            touchPosition = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            direction = -(touchPosition - Input.mousePosition).normalized;
        }

        if(Input.GetMouseButtonUp(0))
        {
            direction = Vector3.zero;
        }
    }
}
