using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenDevice2 : MonoBehaviour
{
    [SerializeField]
    float openDoor;

    [SerializeField]
    float closeDoor;

    [SerializeField]
    float speed = 1;

    private bool _open;

    public void Activate()
    {
        if (!_open)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, openDoor, transform.rotation.z), speed * Time.deltaTime);
            _open = true;
        }
    }

    public void Deactivate()
    {
        if (_open)
        {

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, closeDoor, transform.rotation.z), speed * Time.deltaTime);
            _open = false;
        }

    }
}