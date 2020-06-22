using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Toggle MODE_Toggle;
    public Image Pass_Hard_Image;
    public Image MODE_light;

    public GameObject Virus;
    private Animator virusAnim;

    public float frequency = 1;
    public float A = 1;
    private CanvasGroup lightCanvasGroup;

    public Text MODE_Text;

    private AudioSource audioSource;
    public AudioClip musicClick;
    public AudioClip[] BGMs = new AudioClip[2]; 

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        virusAnim = Virus.GetComponent<Animator>();
        lightCanvasGroup = MODE_light.GetComponent<CanvasGroup>();
        lightCanvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ToggleListener(MODE_Toggle);
        if(!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void ToggleListener(Toggle toggle)
    {
        
        if (toggle.isOn)
        {
            MODE_Text.text = "无尽模式";
            audioSource.clip = BGMs[1];
            lightCanvasGroup.alpha = Mathf.Abs(Mathf.Sin(Time.time * Mathf.PI * frequency) * A);
            Pass_Hard_Image.fillAmount = Mathf.Lerp(Pass_Hard_Image.fillAmount, 1, Time.deltaTime*20);
            virusAnim.SetBool("Show", true);
        }
        else
        {
            MODE_Text.text = "竞速模式";
            audioSource.clip = BGMs[0];
            lightCanvasGroup.alpha = 0;
            Pass_Hard_Image.fillAmount = 0;
            virusAnim.SetBool("Show", false);
        }
    }

    public void StartGameButton()
    {
        MusicClick();
        Invoke("ChangeScene", 0.3f);
        GameController.IsShowGameTip = true;
    }

    public void ChangeScene()
    {
        if (MODE_Toggle.isOn)
        {
            GameController.Mode = Constant.ENDLESS;
            SceneManager.LoadScene("Endless");
        }
        else
        {
            GameController.Mode = Constant.LIMITED;
            SceneManager.LoadScene("limited");
        }
    }

    public void MusicClick()
    {
        audioSource.PlayOneShot(musicClick);
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }
}
