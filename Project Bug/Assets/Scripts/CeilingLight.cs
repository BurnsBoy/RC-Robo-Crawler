using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingLight : MonoBehaviour
{
    public Player player;
    public Light ceilingLight;
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (!Physics.Raycast(transform.position, player.characters[0].cameraPivot.position - transform.position, 
            Vector3.Distance(player.characters[0].cameraPivot.position,transform.position), 1 << 0) | 
            !Physics.Raycast(transform.position, player.characters[1].cameraPivot.position - transform.position, 
            Vector3.Distance(player.characters[1].cameraPivot.position,transform.position), 1 << 0))
            {
                ceilingLight.enabled = true;
                timer = 3;
            }

        else if (timer <= 0)
            ceilingLight.enabled = false;

    }
}
