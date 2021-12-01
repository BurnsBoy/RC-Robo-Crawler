using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electric : MonoBehaviour
{
    public bool active;
    public ButtonPad buttonPad;
    public Animator animator;
    public AudioSource audioSource;
    public bool inverted;
    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("Active", active);
        if (audioSource != null)
        {
            if (active)
                audioSource.volume = .5f;
            else
                audioSource.volume = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonPad != null)
        {
            if (active != buttonPad.activated)
            {
                active = buttonPad.activated;
                animator.SetBool("Active", active != inverted);
                if (audioSource != null)
                {
                    if (active != inverted)
                        audioSource.volume = .5f;
                    else
                        audioSource.volume = 0;
                }
            }
        }
    }
    void OnCollisionStay(Collision other)
    {
        if (active != inverted)
        {
            if (other.collider.tag == "Bug")
            {

                other.collider.GetComponent<Character>().electrocution.Play();
                other.collider.GetComponent<Character>().player.screen.BlankScreen(true);
                other.collider.GetComponent<Character>().player.readyToDeploy = true;
                other.collider.GetComponent<Character>().player.bugDeathTimer = 1;
                other.collider.GetComponent<Character>().transform.position = new Vector3(0, 10, 0);
                 other.collider.GetComponent<Character>().player.checkpointSaver.bugDeaths++;
            }
            else if (other.collider.tag == "Human")
            {
                other.collider.GetComponent<Character>().health -= Time.deltaTime * 2;
                other.collider.GetComponent<Character>().body.velocity += other.collider.GetComponent<Character>().transform.up * .01f;
                other.collider.GetComponent<Character>().cameraPivot.transform.Rotate(new Vector3(Random.Range(-100 * Time.deltaTime, 100 * Time.deltaTime), 0, 0));
            }
        }
    }
}
