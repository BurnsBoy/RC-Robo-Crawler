using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public string buttonName;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string Select()
    {
        animator.SetBool("Selected", true);
        return buttonName;
    }
    public void DeSelect()
    {
        animator.SetBool("Selected", false);
    }
}
