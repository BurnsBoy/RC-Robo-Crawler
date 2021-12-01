using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenu : MonoBehaviour
{
    public MeshRenderer pageRender;
    public Material[] page;
    public int pageIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void NextPage()
    {
        if (pageIndex < page.Length - 1)
        {
            pageIndex++;
            pageRender.material = page[pageIndex];
        }
    }
    public void PreviousPage()
    {
        if (pageIndex > 0)
        {
            pageIndex--;
            pageRender.material = page[pageIndex];
        }
    }
}
