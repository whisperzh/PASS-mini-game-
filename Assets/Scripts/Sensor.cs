using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sensor : MonoBehaviour
{

    // Start is called before the first frame update
    private bool IsChecked = false;
    public Text text;
    Vector3 originPos;

    private AudioSource audioSource;
    public AudioClip sensorCheckAudio;

    void Start()
    {
        originPos = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameController.interactable)
        {
            if (GameController.isDragSensor && Input.GetMouseButtonUp(0))
            {
                GameController.isDragSensor = false;
            }
        }
    }

    private void OnMouseDown()
    {
        if (GameController.interactable)
        {
            GameController.isDragSensor = true;
            setChosenSize();
        }
    }

    public void setChosenSize(){
        transform.localScale *= 2f;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void setUnchosenSize() {
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        transform.rotation = Quaternion.Euler(new Vector3(30,0,0));
    }

    public void goBacktoOriginalState()
    {
        IsChecked = false;
        transform.position = originPos;
        setUnchosenSize();
        text.text = " ";
    }

    public void setText(float i)
    {
        if(!IsChecked)
        {
            audioSource.PlayOneShot(sensorCheckAudio);
            text.text = i.ToString();
            IsChecked = true;
        }
        
    }
   
}
