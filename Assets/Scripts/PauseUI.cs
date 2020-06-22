using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public GameObject HelpUI;

    private AudioSource audioSource;
    public AudioClip toBaoGao;
    public GameController gameController;
    public AudioClip toHelp;
    public AudioClip CloseAudio;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ShowBaoGaoButton()
    {
        HelpUI.SetActive(false);
        audioSource.PlayOneShot(toBaoGao);
    }

    public void ShowHelpButton()
    {
        HelpUI.SetActive(true);
        audioSource.PlayOneShot(toHelp);
    }

    public void ClosePause()
    {
        Time.timeScale = 1;
        if (!gameController.ifDead())
        {
            GameController.interactable = true;           
            audioSource.PlayOneShot(CloseAudio);
            Invoke("Fade", 0.3f);
        }      
    }

    public void ToMenu()
    {
        BGMset.IsExist = false;
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    public void Restart(int mode)
    {
        Time.timeScale = 1;
        GameController.interactable = true;
        GameController.Mode = mode;
        if (mode == 1)
        {
            SceneManager.LoadScene("Endless");
        }
        else {
            SceneManager.LoadScene("limited");
        }
    }

    private void Fade()
    {
        gameObject.SetActive(false);
    }
}
