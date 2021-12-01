using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float speed;
    public Rigidbody body;
    public Transform cameraPivot;
    public float jumpHeight;
    public float groundHeight;
    public string type;
    public Vector3 orientation;
    Vector3 bugGravity;
    public bool grounded;
    public Transform model;
    Vector3 lastVelocity;
    public Animator animator;
    public Transform bugCam;
    public Player player;
    public bool hovering;
    public float health;
    Vector3 anchoredPoint;
    public Transform cameraParent;
    int layerMask;
    float bugDeathTimer;
    public Animator wingAnimator;
    public AudioSource footsteps;
    public AudioSource wingSound;
    bool firstStep;
    int animTime;
    public AudioSource electrocution;
    bool checkLanding;

    // Start is called before the first frame update
    void Start()
    {
        health = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        orientation = new Vector3(0,1,0);
        if (type == "Bug")
        {
            orientation = new Vector3(0,1,0);
            bugGravity = -orientation * 15;
            layerMask = ((1 << 0) | (1 << 10));
        }
        else
        {
            layerMask = 1 << 0;
        }
        firstStep = true;
        checkLanding = true;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, -transform.up, groundHeight, layerMask);
        if (grounded & !checkLanding & ((player.controlIndex == 0 & type == "Human") | (player.controlIndex == 1 & type == "Bug")))
        {
            footsteps.volume = 1;
            footsteps.PlayOneShot(footsteps.clip);
        }
        checkLanding = grounded;
        if (type == "Bug")
        {
            if (!Physics.Raycast(transform.position, -transform.up, groundHeight + .25f, layerMask))
            {
                if (Physics.Raycast(transform.position + ((body.velocity - (transform.up * Vector3.Dot(body.velocity, transform.up))).normalized * groundHeight) - (transform.up * groundHeight * 1.5f), -model.forward, out RaycastHit cornerHit, groundHeight * 4, layerMask))
                {
                    if (cornerHit.collider.tag == "Electric")
                    {
                        if (!cornerHit.collider.GetComponent<Electric>().active)
                        {
                            orientation = cornerHit.normal;
                            bugGravity = -orientation * 15;  
                        }
                    }
                    else
                    {
                        orientation = cornerHit.normal;
                        bugGravity = -orientation * 15;
                    }
                }
                else
                    ResetOrientation();
            }
            BugCamManager();
            if (Physics.Raycast(transform.position, (body.velocity + model.forward - (transform.up * Vector3.Dot(body.velocity, transform.up))).normalized * groundHeight * 2, out RaycastHit hit, speed * .01f, layerMask))
            {
                if (hit.collider.tag == "Electric")
                {
                    if (!hit.collider.GetComponent<Electric>().active)
                    {
                        orientation = hit.normal + (hit.point - transform.position);
                        bugGravity = -orientation * 15;
                    }
                }
                else
                {
                    orientation = hit.normal + (hit.point - transform.position);
                    bugGravity = -orientation * 15;
                }
            }
            BugModelManager();
            body.velocity += bugGravity * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.Cross(transform.right, orientation), orientation), Time.deltaTime * 5);
            Debug.DrawRay(transform.position, orientation);
            if (hovering)
            {
                ResetOrientation();
                body.velocity = new Vector3(body.velocity.x, -.5f, body.velocity.z);
            }
        }
        else
        {
            if (!grounded)
            {
                body.velocity = new Vector3(body.velocity.x, body.velocity.y * 1.1f, body.velocity.z);
            }
            HumanModelManager();
            if (health < 0)
            {
                if (health < -2)
                    AudioListener.volume -= Time.deltaTime * 3;
                if (health < -5)
                {
                    player.checkpointSaver.humanDeaths++;
                    player.Reset();
                }
            }
            else if (health < 1)
            {
                if (!electrocution.isPlaying)
                    electrocution.Play();
                electrocution.volume = 1 - health;
                health += Time.deltaTime * .5f;
            }
            else if (electrocution.isPlaying)
                electrocution.Stop();
        }

    }
    void OnCollisionStay(Collision other)
    {
        if (type == "Bug" & !hovering)
        {
                if (!Physics.Raycast(other.contacts[0].point - (other.contacts[0].normal * groundHeight), other.contacts[0].normal, groundHeight * 2, layerMask))
                {
                    orientation = other.contacts[0].normal;
                    bugGravity = -orientation * 15;
                }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint" & type == "Human")
        {
            player.checkpointSaver.checkpointP = other.transform.position;
            player.checkpointSaver.checkpointR = other.transform.rotation;
        }
    }
    void ResetOrientation()
    {
        orientation = new Vector3(0,1,0);
        bugGravity = -orientation * 15;
        animator.speed = 1;
    }
    void BugModelManager()
    {
        model.parent = transform.parent;
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Walking", body.velocity.magnitude - (transform.up * Vector3.Dot(body.velocity, transform.up)).magnitude > .1f);     
        animator.SetBool("Jump", ((transform.up * Vector3.Dot(body.velocity, transform.up)).magnitude > 1)); 
        wingAnimator.SetBool("Flapping", hovering);
        if (animator.GetBool("Walking"))
        {
            animTime = (int)((animator.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)animator.GetCurrentAnimatorStateInfo(0).normalizedTime) * 10);
            lastVelocity = body.velocity;
            if (grounded)
            {
                animator.speed = Mathf.Abs(body.velocity.magnitude - (transform.up * Vector3.Dot(body.velocity, transform.up)).magnitude);
            }
            else
                animator.speed = 1;

            if ((animTime >= 1 & animTime < 6 & firstStep | animTime >= 6 & !firstStep) & grounded)
            {
                footsteps.volume = 1 + Random.Range(-.2f, .2f);
                footsteps.pitch = 1 + Random.Range(-.2f, .2f);
                footsteps.PlayOneShot(footsteps.clip);
                firstStep = !firstStep;
            }
        }
        if (hovering & !player.readyToDeploy)
        {
            wingSound.volume = Mathf.Lerp(wingSound.volume, .5f, Time.deltaTime * 30);
        }
        else 
        {
            wingSound.volume = Mathf.Lerp(wingSound.volume, 0, Time.deltaTime * 30);
        }
        model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(lastVelocity - transform.up * Vector3.Dot(lastVelocity, transform.up), orientation), Time.deltaTime * 10);    

    }
    void HumanModelManager()
    {
        model.position = transform.position;
        model.parent = transform.parent;
        cameraParent.position = new Vector3(cameraParent.transform.parent.position.x, player.headPos.position.y + .05f, cameraParent.transform.parent.position.z);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Walking", body.velocity.magnitude - (transform.up * Vector3.Dot(body.velocity, transform.up)).magnitude > 1);
        if (player.active)
            animator.SetBool("LookingAtPad", player.controlIndex == 1);
        if (animator.GetBool("Walking"))
        {
            animTime = (int)((animator.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)animator.GetCurrentAnimatorStateInfo(0).normalizedTime) * 10);
            lastVelocity = body.velocity;
            if (grounded)
            {
                animator.speed = Mathf.Abs(body.velocity.magnitude - (transform.up * Vector3.Dot(body.velocity, transform.up)).magnitude) * .25f;
            }
            else
                animator.speed = 1;

            if ((animTime == 1 & firstStep | animTime == 6 & !firstStep) & grounded)
            {
                footsteps.volume = .15f + Random.Range(-.1f, .1f);
                footsteps.pitch = 1 + Random.Range(-.1f, .1f);
                footsteps.PlayOneShot(footsteps.clip);
                firstStep = !firstStep;
            }
        }
        else
        {
            animator.speed = 1;
            lastVelocity = transform.forward;
            firstStep = true;
        }
        if (animator.GetBool("LookingAtPad"))
        {
            cameraParent.localEulerAngles = Vector3.Lerp(cameraParent.localEulerAngles, new Vector3(35,0,0), Time.deltaTime * 10);
            cameraPivot.rotation = Quaternion.Lerp(cameraPivot.rotation, cameraParent.rotation, Time.deltaTime * 10);
        }
        else
            cameraParent.localEulerAngles = Vector3.Lerp(cameraParent.localEulerAngles, Vector3.zero, Time.deltaTime * 10);
        if (Vector3.Angle(body.velocity, transform.forward) <= 90)
            model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(lastVelocity - transform.up * Vector3.Dot(lastVelocity, transform.up), orientation), Time.deltaTime * 10); 
        else if (animator.GetBool("Walking"))
            model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(-lastVelocity - transform.up * Vector3.Dot(-lastVelocity, transform.up), orientation), Time.deltaTime * 10); 

    }
    void BugCamManager()
    {
        if (Physics.Raycast(cameraPivot.position, cameraPivot.GetChild(0).position - cameraPivot.position, out RaycastHit hit, Vector3.Distance(cameraPivot.GetChild(0).position, cameraPivot.position), 11))
            bugCam.position = cameraPivot.position + ((hit.point - cameraPivot.position) / 2);
    }
    public void Deploy()
    {
        transform.position = player.characters[0].transform.position + player.characters[0].transform.forward * .1f;
        body.velocity = player.characters[0].transform.forward * 10;
        player.screen.BlankScreen(false);
    }
}
