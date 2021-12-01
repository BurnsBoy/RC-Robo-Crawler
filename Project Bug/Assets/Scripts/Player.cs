using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Character[] characters;
    public Vector2 moveInput;
    Vector2 lookInput;
    Controls controls;
    public float lookSensitivity;
    public int controlIndex;
    float verticalVelocity;
    public float minmaxLookAngle;
    Transform padTarget;
    public bool interact;
    public CheckpointSaver checkpointSaver;
    public Vector3 padFace;
    public Transform[] leftStick;
    public Transform[] rightStick;
    public Transform[] stickPositions;
    public Animator humanAnim;
    public PadScreen screen;
    public bool active;
    public bool readyToDeploy;
    public PauseSlider lookSensitivitySlider;
    public PauseSlider volumeSlider;
    public HelpMenu helpMenu;
    
    public AudioSource leftButtonSound;
    public AudioSource rightButtonSound;
    public AudioClip pressDown;
    public AudioClip pressUp;
    public bool leftButtonDown;
    public bool rightButtonDown;
    public Transform headPos;
    public AudioSource selectSound;
    public Image electricScreen;
    public float bugDeathTimer;
    public AudioSource bugJump;
    public Text interactText;
    public RectTransform interactBackground;
    public Transform[] graphicsButtons;
    public RectTransform statsBox;
    public Text stats;
    bool victory;
    // Start is called before the first frame update
    void Start()
    {
        readyToDeploy = true;
        characters[1].transform.position = new Vector3(0, 10, 0);
        Application.targetFrameRate = 60;
        controls = new Controls();
        controls.Enable();
        checkpointSaver = GameObject.FindGameObjectWithTag("CheckpointSaver").GetComponent<CheckpointSaver>();
        if (checkpointSaver.checkpointP != Vector3.zero)
        {
            characters[0].transform.rotation = checkpointSaver.checkpointR;
            characters[1].transform.rotation = checkpointSaver.checkpointR;
            characters[0].transform.position = checkpointSaver.checkpointP;
        }
        else
        {
            OnPause();
            screen.HelpMode();
        }
        AudioListener.volume = (volumeSlider.value * 3);
        Physics.IgnoreLayerCollision(9,11);
        Physics.IgnoreLayerCollision(11,10);
        if (QualitySettings.GetQualityLevel() == 0)
        {
            graphicsButtons[0].position = new Vector3(graphicsButtons[0].position.x, -103, graphicsButtons[0].position.z);
            graphicsButtons[1].position = new Vector3(graphicsButtons[1].position.x, -103.45f, graphicsButtons[1].position.z);      
        }
        else if (QualitySettings.GetQualityLevel() == 2)
        {
            graphicsButtons[2].position = new Vector3(graphicsButtons[2].position.x, -103, graphicsButtons[2].position.z);
            graphicsButtons[1].position = new Vector3(graphicsButtons[1].position.x, -103.45f, graphicsButtons[1].position.z);            
        }
    }

    // Update is called once per frame
    void Update()
    {
        Input();
        if (characters[0].health > 0)
        {
            if (active)
            {
                if (controlIndex == 1 & !readyToDeploy)
                    Action();
                else if (controlIndex == 0)
                    Action();
            }
            else
                PauseControl();
        }
        if (controlIndex == 1 | !active)
        {
            characters[0].transform.rotation = Quaternion.Slerp(characters[0].transform.rotation, Quaternion.LookRotation(padFace, characters[0].transform.up), Time.deltaTime * 20);
            if (Vector3.Angle(characters[0].transform.eulerAngles, padFace) < 90)
                CheckPadClearence();
        }
        if (controlIndex == 0 | readyToDeploy)
        {
            transform.position = characters[0].transform.position;
            transform.rotation = characters[0].transform.rotation;
        }
        else
        {
            transform.position = characters[1].transform.position;
            transform.rotation = characters[1].transform.rotation;
        }
        electricScreen.color = new Color(0,3 + characters[0].health,3 + characters[0].health,1 - ((characters[0].health + 1) / 2));
        if (readyToDeploy & controlIndex == 1)
        {
            bugDeathTimer -= Time.deltaTime;
            if (bugDeathTimer <= 0)
                OnSwitchCharacters();
        }
        CheckVictory();

    }
    void LateUpdate()
    {

    }
    void Input()
    {
        if (controls.Gameplay.Interact.ReadValue<float>() == 0)
            interact = false;
        moveInput = controls.Gameplay.Move.ReadValue<Vector2>() * characters[controlIndex].speed * Time.deltaTime; 
        lookInput = controls.Gameplay.Look.ReadValue<Vector2>() * lookSensitivity;
        if (controls.Gameplay.Hover.ReadValue<float>() > 0 & controlIndex == 1 & active)
            characters[1].hovering = true;
        else
            characters[1].hovering = false;
        if (controlIndex == 1 | !active)
            MoveSticks();
        else if (humanAnim.GetLayerWeight(1) > 0)
        {
            humanAnim.SetLayerWeight(1, 0);
            humanAnim.SetLayerWeight(2, 0);
            humanAnim.SetLayerWeight(3, 0);
        }
    }
    public void OnSwitchCharacters()
    {
        if (active & characters[0].health > 0)
        {
            if (controlIndex == 0 & characters[0].health >= 1 & characters[0].grounded)
            {
                if (readyToDeploy)
                {
                    readyToDeploy = false;
                    characters[1].Deploy();
                }
                controlIndex = 1;
                padFace = characters[0].transform.forward;
                CheckPadClearence();
            }
            else
            {
                controlIndex = 0;
            }
        }

    }
    void Action()
    {
        if (characters[controlIndex].grounded)
            characters[controlIndex].body.velocity += (characters[controlIndex].transform.forward * moveInput.y) + (characters[controlIndex].transform.right * moveInput.x);
        else 
            characters[controlIndex].body.velocity += (characters[controlIndex].transform.forward * moveInput.y / 2) + (characters[controlIndex].transform.right * moveInput.x / 2);
        characters[controlIndex].transform.Rotate(0,lookInput.x,0);
        if (characters[controlIndex].cameraPivot.localEulerAngles.x - lookInput.y < minmaxLookAngle - 20 | characters[controlIndex].cameraPivot.localEulerAngles.x - lookInput.y > 360 - minmaxLookAngle)
            characters[controlIndex].cameraPivot.transform.Rotate(new Vector3(-lookInput.y, 0, 0));  
    }
    void OnJump()
    {
        if (controls.Gameplay.enabled & Time.timeScale > 0 & characters[controlIndex].health >= 1 & active)
        {
            if (characters[controlIndex].grounded)
            {
                characters[controlIndex].body.velocity += characters[controlIndex].transform.up * characters[controlIndex].jumpHeight;
                if (controlIndex == 1)
                    bugJump.Play();
            }

        }
    }
    void CheckVictory()
    {
        if (transform.position.y > 12 & !victory)
        {
            statsBox.localScale = Vector3.one;
            string secondText;
            string minuteText;
            victory = true;
            if (checkpointSaver.playTime % 60 > 10)
                secondText = "" + (int)(checkpointSaver.playTime % 60);
            else
                secondText = "0" + (int)(checkpointSaver.playTime % 60);
            if ((checkpointSaver.playTime % 3600) / 60 > 10)
                minuteText = "" + (int)((checkpointSaver.playTime / 60) % 60);
            else
                minuteText = "0" + (int)((checkpointSaver.playTime / 60) % 60);
            if (checkpointSaver.playTime < 3600)
               stats.text = "Play Time\n" + minuteText + ":" + secondText;
            else
               stats.text = "Play Time\n" + (int)(checkpointSaver.playTime / 3600) + ":" + minuteText + ":" + secondText;
            stats.text += "\n\nBug Deaths\n" + checkpointSaver.bugDeaths + "\n\nHuman Deaths\n" + checkpointSaver.humanDeaths;
        }
        else if (victory)
        {
            if (active)
                statsBox.localScale = Vector3.one;
            else
                statsBox.localScale = Vector3.zero;
        }
    }
    void CheckPadClearence()
    {
        if (controlIndex == 1 | !active)
        {
            if (Physics.SphereCast(characters[0].transform.position - characters[0].transform.forward * .25f, 1, characters[0].transform.forward, out RaycastHit wallSphereHit, 1.25f , 1 << 0))
            {
                padFace = wallSphereHit.normal - (characters[0].transform.up * Vector3.Dot(wallSphereHit.normal, characters[0].transform.up));
            }
            else if (Physics.Raycast(characters[0].transform.position, characters[0].transform.forward, out RaycastHit wallRayHit, 2, 1 << 0))
            {
                padFace = wallRayHit.normal - (characters[0].transform.up * Vector3.Dot(wallRayHit.normal, characters[0].transform.up));
            }
        }

    }
    void MoveSticks()
    {
        if (humanAnim.GetLayerWeight(1) < 1)
        {
            humanAnim.SetLayerWeight(1, humanAnim.GetLayerWeight(1) + Time.deltaTime);
            humanAnim.SetLayerWeight(3, humanAnim.GetLayerWeight(1));
        }
        if (moveInput.magnitude < .175f)
            leftStick[0].localRotation = Quaternion.Slerp(leftStick[0].localRotation, Quaternion.Euler(new Vector3(-moveInput.y, -moveInput.x, 0) * 80), Time.deltaTime * 10);
        else
            leftStick[0].localRotation = Quaternion.Slerp(leftStick[0].localRotation, Quaternion.Euler(new Vector3(-moveInput.normalized.y, -moveInput.normalized.x, 0) * 20), Time.deltaTime * 10);
        if ((lookInput / lookSensitivity).magnitude < .175f)
            rightStick[0].localRotation = Quaternion.Slerp(rightStick[0].localRotation, Quaternion.Euler(new Vector3(-lookInput.y, -lookInput.x, 0) * (80 / lookSensitivity)), 10 * Time.deltaTime + (lookInput.magnitude / lookSensitivity));
        else
            rightStick[0].localRotation = Quaternion.Slerp(rightStick[0].localRotation, Quaternion.Euler(new Vector3(-lookInput.normalized.y, -lookInput.normalized.x, 0) * 20), 10 * Time.deltaTime + (lookInput.magnitude / lookSensitivity));
        if ((controls.Gameplay.Jump.ReadValue<float>() > 0 & active) | (controls.Gameplay.Select.ReadValue<float>() > 0 & !active))
        {
            rightStick[0].position = Vector3.Slerp(rightStick[0].position, stickPositions[3].position, Time.deltaTime * 10);
            rightStick[1].position = Vector3.Slerp(rightStick[1].position, stickPositions[3].position, Time.deltaTime * 10);
            if (!rightButtonDown)
            {
                rightButtonSound.volume = .25f + Random.Range(-.1f, .1f);
                rightButtonSound.pitch = 1 + Random.Range(-.2f, .2f);
                rightButtonSound.PlayOneShot(pressDown);
                rightButtonDown = true;
            }
        }
        else
        {
            rightStick[0].position = Vector3.Slerp(rightStick[0].position, stickPositions[1].position, Time.deltaTime * 10);
            rightStick[1].position = Vector3.Slerp(rightStick[1].position, stickPositions[1].position, Time.deltaTime * 10);
            if (rightButtonDown)
            {
                rightButtonSound.volume = .25f + Random.Range(-.1f, .1f);
                rightButtonSound.pitch = 1 + Random.Range(-.2f, .2f);
                rightButtonSound.PlayOneShot(pressUp);
                rightButtonDown = false;
            }
        }
        if (controls.Gameplay.Hover.ReadValue<float>() > 0 & active)
        {
            leftStick[0].position = Vector3.Slerp(leftStick[0].position, stickPositions[2].position, Time.deltaTime * 10);
            leftStick[1].position = Vector3.Slerp(leftStick[1].position, stickPositions[2].position, Time.deltaTime * 10);
            if (!leftButtonDown)
            {
                leftButtonSound.volume = .25f + Random.Range(-.1f, .1f);
                leftButtonSound.pitch = 1 + Random.Range(-.2f, .2f);
                leftButtonSound.PlayOneShot(pressDown);
                leftButtonDown = true;
            }
        }
        else
        {
            leftStick[0].position = Vector3.Slerp(leftStick[0].position, stickPositions[0].position, Time.deltaTime * 10);
            leftStick[1].position = Vector3.Slerp(leftStick[1].position, stickPositions[0].position, Time.deltaTime * 10);
            if (leftButtonDown)
            {
                leftButtonSound.volume = .25f + Random.Range(-.1f, .1f);
                leftButtonSound.pitch = 1 + Random.Range(-.2f, .2f);
                leftButtonSound.PlayOneShot(pressUp);
                leftButtonDown = false;
            }
        }
        if (active)
        {
            humanAnim.SetLayerWeight(2, humanAnim.GetLayerWeight(2) * .7f + controls.Gameplay.Hover.ReadValue<float>() * .05f);
            humanAnim.SetLayerWeight(4, humanAnim.GetLayerWeight(4) * .7f + controls.Gameplay.Jump.ReadValue<float>() * .05f);
        }
        else
            humanAnim.SetLayerWeight(4, humanAnim.GetLayerWeight(4) * .7f + controls.Gameplay.Select.ReadValue<float>() * .05f);   
        humanAnim.SetFloat("LHorizontal", (humanAnim.GetFloat("LHorizontal") * .75f) + moveInput.x);
        humanAnim.SetFloat("LVertical", (humanAnim.GetFloat("LVertical") * .75f) + moveInput.y);
        humanAnim.SetFloat("RHorizontal", (humanAnim.GetFloat("RHorizontal") * .75f) + lookInput.x / lookSensitivity);
        humanAnim.SetFloat("RVertical", (humanAnim.GetFloat("RVertical") * .75f) + lookInput.y / lookSensitivity);

    }
    void PauseControl()
    {
        screen.pauseCursor.transform.position += ((screen.pauseCursor.transform.right * (moveInput.x + lookInput.x / lookSensitivity) + screen.pauseCursor.transform.up * (moveInput.y + lookInput.y / lookSensitivity)) * .3f);
    }
    void OnPause()
    {
        padFace = characters[0].transform.forward;
        humanAnim.SetBool("LookingAtPad", active);
        screen.PauseGame(active);
        active = !active;
        
    }
    void OnInteract()
    {
        if (active)
            interact = true;
    }
    void OnSelect()
    {
        if (!active)
        {
            selectSound.Play();
            switch(screen.pauseCursor.selected)
            {
                case "Play":
                    OnPause();
                    break;
                
                case "Return":
                    screen.MainMode();
                    break;
                
                case "Look Slider":
                    screen.SliderSelect();
                    lookSensitivity = 2.5f + (lookSensitivitySlider.value * 15);
                    break;

                case "Volume Slider":
                    screen.SliderSelect();
                    AudioListener.volume = (volumeSlider.value * 3);
                    break;

                case "Options":
                    screen.OptionsMode();
                    break;

                case "Performance":
                    graphicsButtons[0].position = new Vector3(graphicsButtons[0].position.x, -103, graphicsButtons[0].position.z);
                    graphicsButtons[1].position = new Vector3(graphicsButtons[1].position.x, -103.45f, graphicsButtons[1].position.z);
                    graphicsButtons[2].position = new Vector3(graphicsButtons[2].position.x, -103.45f, graphicsButtons[2].position.z);
                    QualitySettings.SetQualityLevel(0);
                    break;
                
                    case "Mid":
                    graphicsButtons[0].position = new Vector3(graphicsButtons[0].position.x, -103.45f, graphicsButtons[0].position.z);
                    graphicsButtons[1].position = new Vector3(graphicsButtons[1].position.x, -103, graphicsButtons[1].position.z);
                    graphicsButtons[2].position = new Vector3(graphicsButtons[2].position.x, -103.45f, graphicsButtons[2].position.z);
                    QualitySettings.SetQualityLevel(1);
                    break;

                    case "Quality":
                    graphicsButtons[0].position = new Vector3(graphicsButtons[0].position.x, -103.45f, graphicsButtons[0].position.z);
                    graphicsButtons[1].position = new Vector3(graphicsButtons[1].position.x, -103.45f, graphicsButtons[1].position.z);
                    graphicsButtons[2].position = new Vector3(graphicsButtons[2].position.x, -103, graphicsButtons[2].position.z);
                    QualitySettings.SetQualityLevel(2);
                    break;

                case "Help":
                    screen.HelpMode();
                    break;

                case "Previous":
                    helpMenu.PreviousPage();
                    break;

                case "Next":
                    helpMenu.NextPage();
                    break;

                case "Last Checkpoint":
                    if (!victory)
                        Reset();
                    break;

                case "Respawn Bug":
                    characters[1].Deploy();
                    readyToDeploy = false;
                    controlIndex = 1;
                    OnPause();   
                    break;

                case "Restart":
                    Destroy(checkpointSaver.gameObject);
                    Reset();       
                    break;

                case "Quit":
                    Application.Quit();
                    break;

            }

            
        }
    }
    public void Reset()
    {
        SceneManager.LoadScene("Main");
    }
}
