using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskHeap : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.isDragMask&&Input.GetMouseButtonUp(0))
            GameController.isDragMask = false;
    }

    private void OnMouseDown()
    {
        GameController.isDragMask = true;   
    }

}
