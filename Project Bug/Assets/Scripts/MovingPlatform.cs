using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3[] movements;
    Vector3 basePosition;
    public ButtonPad[] buttonPads;
    Vector3 targetPosition;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        basePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = basePosition;
        for (int i = 0; i < movements.Length; i++)
        {
            if (buttonPads[i].activated)
                targetPosition += movements[i];
        }
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime);
            audioSource.volume = Mathf.Lerp(audioSource.volume, 1, Time.deltaTime * 30);
        }
        else
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime * 30);
    }
    void OnCollisionStay(Collision other)
    {
        if (other.collider.tag == "Human" | other.collider.tag == "Bug")
            other.transform.parent = this.transform;
    }
    void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "Human" | other.collider.tag == "Bug")
            other.transform.parent = null;
    }
}
