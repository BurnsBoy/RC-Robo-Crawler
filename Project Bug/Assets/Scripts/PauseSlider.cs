using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSlider : MonoBehaviour
{
    public float minPos;
    public float maxPos;
    public float value;
    public bool active;
    public Transform cursor;
    public CheckpointSaver checkpointSaver;
    public string type;

    // Start is called before the first frame update
    void Start()
    {
        checkpointSaver = GameObject.FindGameObjectWithTag("CheckpointSaver").GetComponent<CheckpointSaver>();
        if (type == "Look")
            SetValue(checkpointSaver.lookSensitivityValue);
        if (type == "Volume")
            SetValue(checkpointSaver.volumeValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (cursor.position.z > maxPos)
                transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y, maxPos);
            else if (cursor.position.z < minPos)
                transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y, minPos);
            else
                transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y, cursor.position.z);
            value = (transform.parent.position.z - minPos) / (maxPos - minPos);
            if (type == "Look")
                checkpointSaver.lookSensitivityValue = value;
            if (type == "Volume")
                checkpointSaver.volumeValue = value;
        }
    }
    public void SetValue(float v)
    {
        value = v;
        transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y, minPos + (value * (maxPos - minPos)));
    }
}
