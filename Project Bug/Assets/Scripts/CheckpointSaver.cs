using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSaver : MonoBehaviour
{
    public Vector3 checkpointP;
    public Quaternion checkpointR;
    public float lookSensitivityValue;
    public float volumeValue;
    public float playTime;
    public int bugDeaths;
    public int humanDeaths;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("CheckpointSaver") != null)
        {
            if (GameObject.FindGameObjectsWithTag("CheckpointSaver").Length > 1)
                Destroy(this.gameObject);
            else
            {
                QualitySettings.SetQualityLevel(1);
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       playTime += Time.deltaTime; 
    }
}
