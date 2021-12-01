using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPad : MonoBehaviour
{
    public Button top;
    public Button bottom;
    public Transform pressLocation;
    public bool activated;
    public string type;
    public Animator animator;
    public AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Clear()
    {
        top.pressed = false;
        bottom.pressed = false;
        top.pressTimer = .5f;
        bottom.pressTimer = .5f;
    }
    public void Press(bool b)
    {
        activated = b;
        audioSource.pitch = 1 + Random.Range(-.1f, .1f);
        audioSource.volume = 1 + Random.Range(-.1f, .1f);
        audioSource.Play();
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Human")
        {
            if (other.GetComponent<Character>().grounded)
            {
                other.GetComponent<Character>().player.interactBackground.localScale = Vector3.one;
                if (type == "Buttons")
                    other.GetComponent<Character>().player.interactText.text = "[E]: Push Button";
                if (type == "Lever")
                    other.GetComponent<Character>().player.interactText.text = "[E]: Pull Lever";
            }
            else
                other.GetComponent<Character>().player.interactBackground.localScale = Vector3.zero;
            if (other.GetComponent<Character>().player.interact & other.GetComponent<Character>().grounded)
            {
                audioSource.pitch = 1 + Random.Range(-.1f, .1f);
                audioSource.volume = 1 + Random.Range(-.1f, .1f);
                audioSource.Play();
                other.GetComponent<Character>().player.interact = false;
                activated = !activated;
                if (type == "Buttons")
                {
                    top.pressed = activated;
                    bottom.pressed = !activated;
                    top.pressTimer = .5f;
                    bottom.pressTimer = .5f;
                }
                else if (type == "Lever")
                {
                    animator.SetBool("Activated", activated);
                }
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Human")
            other.GetComponent<Character>().player.interactBackground.localScale = Vector3.zero;
    }
}
