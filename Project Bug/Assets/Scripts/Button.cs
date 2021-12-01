using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public bool pressed;
    public ButtonPad buttonPad;
    public float pressTimer;
    public bool on;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pressTimer > 0)
        {
            if (pressed)
                transform.position = Vector3.Slerp(transform.position, buttonPad.pressLocation.position, Time.deltaTime * 10);
            else
                transform.position = Vector3.Slerp(transform.position, buttonPad.transform.position, Time.deltaTime * 10);
            pressTimer -= Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bug" & !pressed)
        {
            buttonPad.Clear();
            pressed = true;
            pressTimer = .5f;
            buttonPad.Press(on);
        }
    }
}
