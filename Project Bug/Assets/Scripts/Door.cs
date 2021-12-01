using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public ButtonPad buttonPad;
    public Animator animator;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Open") != buttonPad.activated)
        {
            audioSource.pitch = 1 + Random.Range(-.1f, .1f);
            audioSource.Play();
            animator.SetBool("Open", buttonPad.activated);
        }

    }
}
