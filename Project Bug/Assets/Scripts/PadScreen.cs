using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadScreen : MonoBehaviour
{
    public MeshRenderer screen;
    public Material[] onDisplay;
    public int displayIndex;
    public PauseCursor pauseCursor;
    bool paused;
    bool blank;
    public Transform pauseCam;
    public Vector3 rotationTarget;
    // Start is called before the first frame update
    void Start()
    {
        blank = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Quaternion.Angle(pauseCam.rotation, Quaternion.Euler(rotationTarget)) > .1f)
            pauseCam.rotation = Quaternion.Lerp(pauseCam.rotation, Quaternion.Euler(rotationTarget), Time.deltaTime * 10);
    }
    public void PauseGame(bool pausing)
    {
        paused = pausing;
        if (pausing)
        {
            screen.material = onDisplay[1];
        }
        else
        {
            if (pauseCursor.currentScreen == "Main")
                MainMode();
            else if (pauseCursor.currentScreen == "Options")
                OptionsMode();
            else if (pauseCursor.currentScreen == "Help")
                HelpMode();
            BlankScreen(blank);
        }

    }
    public void BlankScreen(bool b)
    {
        blank = b;
        if (blank)
            screen.material = onDisplay[2];
        else
            screen.material = onDisplay[0];
    }
    public void OptionsMode()
    {
        rotationTarget = new Vector3(0,-90,0);
        pauseCursor.transform.rotation = Quaternion.Euler(rotationTarget);
        pauseCursor.transform.position = new Vector3(-10,-97,2);
        pauseCursor.currentScreen = "Options";
    }
    public void MainMode()
    {
        rotationTarget = new Vector3(0,0,0);
        pauseCursor.transform.rotation = Quaternion.Euler(rotationTarget);
        pauseCursor.transform.position = new Vector3(0,-97,10);
        pauseCursor.currentScreen = "Main";
    }
    public void HelpMode()
    {
        rotationTarget = new Vector3(0,90,0);
        pauseCursor.transform.rotation = Quaternion.Euler(rotationTarget);
        pauseCursor.transform.position = new Vector3(10,-97,2);
        pauseCursor.currentScreen = "Help";
    }
    public void SliderSelect()
    {
        pauseCursor.transform.parent.GetComponent<PauseSlider>().active = !pauseCursor.transform.parent.GetComponent<PauseSlider>().active;
        pauseCursor.controllingSlider = pauseCursor.transform.parent.GetComponent<PauseSlider>().active;
    }
}
