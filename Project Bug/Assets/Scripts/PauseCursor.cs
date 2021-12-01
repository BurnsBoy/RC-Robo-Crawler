using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCursor : MonoBehaviour
{
    public string selected;
    public string currentScreen;
    public bool controllingSlider;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        selected = transform.parent.GetComponent<MenuButton>().Select();
    }

    // Update is called once per frame
    void Update()
    {
        if (!controllingSlider)
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 20);
            transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
        }
        if (currentScreen == "Main")
            transform.position = new Vector3(transform.position.x, transform.position.y, 8.9f);
        else if (currentScreen == "Options")
            transform.position = new Vector3(-8.9f, transform.position.y, transform.position.z);
        else if (currentScreen == "Help")
            transform.position = new Vector3(8.9f, transform.position.y, transform.position.z);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pause")
        {
            if (transform.parent != other.transform)
            {
                audioSource.PlayOneShot(audioSource.clip);
                transform.parent.GetComponent<MenuButton>().DeSelect();
                transform.parent = other.transform;
                selected = other.GetComponent<MenuButton>().Select();
            }
        }
    }
}
