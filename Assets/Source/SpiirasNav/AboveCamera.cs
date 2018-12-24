using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AboveCamera: MonoBehaviour 
{
	bool isDragging;
	public bool active;
	Camera cam;

	private void Start() 
	{
		cam = GetComponent<Camera>();
		cam.orthographicSize = 25;
		cam.gameObject.SetActive (true);
		cam.orthographic = true;
		active = true;
	}

	private void Update() 
	{
        
		/* Блокировка нажатий сквозь интерфейс */
		if(EventSystem.current.IsPointerOverGameObject () || !active) return;
		/**************************************/

		if(Input.GetMouseButtonDown (0)) 
		{
			isDragging = true;
		}

		if(Input.GetMouseButtonUp (0)) 
		{
			isDragging = false;
		}

		Vector3 newPosition = transform.position;

		if(isDragging) 
		{
			newPosition.x -= Input.GetAxis ("Mouse X");
			newPosition.z -= Input.GetAxis ("Mouse Y");
		}

        // zoom
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float newOthographicSize = cam.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * 10;
            if(newOthographicSize > 0)
            {
                cam.orthographicSize = newOthographicSize;
            }
        }

        transform.position = newPosition;
	}
}