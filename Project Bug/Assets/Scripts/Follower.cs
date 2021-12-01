using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;
    public float rotationSpeed;
    public bool copyRotation;
    public bool instantaneous;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!instantaneous)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * moveSpeed);
            if (copyRotation)
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            transform.position = target.position;
            if (copyRotation)
                transform.rotation = target.rotation;            
        }
    }
}
