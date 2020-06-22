using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusMove : MonoBehaviour
{
    public float offsetX;
    public float offsetY;
    private Vector3 pos;
    public float frequency = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.parent.transform.position;
        transform.position = new Vector3(pos.x + Mathf.Sin(Time.time * Mathf.PI * frequency/10) * offsetX,
                                            pos.y + Mathf.Sin(Time.time * Mathf.PI * frequency/10) * offsetY,
                                                0);
    }

}
