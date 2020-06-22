
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
    public static bool interactable = true;

    public static int Mode = Constant.ENDLESS;

    public Human HumanPrefab;

    public static Queue<Human> checkQueue;

    public static bool isChange = false;

    private bool nowAlign=false;

    private bool setTime = true;

    public static bool isDragSensor = false;

    public static bool isDragMask = false;

    public GameObject indicator;

    public GameObject sensor;

    [Header("生命值")]
    private int HPValue = 3;
    public GameObject[] LIFEs;

    [Header("时间管理")]
    private float curTime;
    private float timeOffset;
    public float timePre = 10f;
    public float frequency = 1f;
    public GameObject clock;
    public Image clockHand;
    public Image clockBK;
    public Text timeText;

    private Color colorStart;
    public Color colorEnd;
    private float preSecond = 0;


    [Header("游戏开始提示")]
    public static bool IsShowGameTip = true;
    public GameObject GameTip;

    [Header("音频")]
    public GameObject BGM;
    public AudioClip Choose_True;
    public AudioClip Choose_False;
    public AudioClip ClockAudio;
    private AudioSource audioSource;

    [Header("暂停")]
    public AudioClip OpenPauseAudio;
    public GameObject PauseUI;

    [Header("分数统计")]
    private int SuccessCheck = 0;
    public Image[] MathText = new Image[3];
    public Sprite[] MathTextSprite;

    // Start is called before the first frame update
    void Start()
    {
        if(!BGMset.IsExist)
        {
            BGMset.IsExist = true;
            Instantiate(BGM);
        }

        interactable = true;
        if (Mode == Constant.LIMITED)
            timePre = 60;

        //设置生命值
        HPValue = LIFEs.Length;
        //隐藏十位百位
        MathText[1].gameObject.SetActive(false);
        MathText[2].gameObject.SetActive(false);
        //时间设定
        curTime = timePre;
        timeOffset = timePre;

        colorStart = clockBK.color;
        audioSource = GetComponent<AudioSource>();
        Human temphuman;
        checkQueue = new Queue<Human>();
        for (int i = 0; i < 10; i++)
        {
            
            Vector3 pos = new Vector3(transform.position.x, transform.position.y+i*2, 0);
            temphuman = Instantiate(HumanPrefab, pos, Quaternion.identity);
            temphuman.GetComponent<BoxCollider2D>().enabled = false;//默认关闭Collider
            temphuman.tgtPos = pos;
            temphuman.init();
            //temphuman.GetHumanSprite.sortingOrder = 10-i;
            temphuman.setLayer(10 - i);
            temphuman.showcode();
            checkQueue.Enqueue(temphuman);            
        }
        if (Mode==Constant.ENDLESS)
            resetTime();
        PauseUI.gameObject.SetActive(false);

        if(IsShowGameTip)
        {
            GameTip.SetActive(true);
        }
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetMouseButton(0) && IsShowGameTip)
        {
            IsShowGameTip = false;
            GameTip.SetActive(false);
        }


        if (interactable && !IsShowGameTip)
        {
            DeathJudgement();
            UpdateText();
            DragSensor();
            DragMask();
            CountDown();
            //checkQueue.Peek().GetComponent<BoxCollider2D>().enabled = true;
            checkQueue.Peek().whenitsYourTurn();
            if (isChange)
            {
                madeChoice();
                isChange = false;
            }
        }
    }

    public void UpdateText()
    {
        int c1 = 0;
        int c2 = 0;
        int c3 = 0;

        //个位
        c1 = SuccessCheck % 10;
        MathText[0].sprite = MathTextSprite[c1];
        //十位
        if (SuccessCheck / 10 !=0)
        {
            c2 = (SuccessCheck % 100) / 10;
            MathText[1].gameObject.SetActive(true);
            MathText[1].sprite = MathTextSprite[c2];
        }
        //百位
        if (SuccessCheck / 100 != 0)
        {
            c3 = SuccessCheck / 100;
            MathText[2].gameObject.SetActive(true);
            MathText[2].sprite = MathTextSprite[c3];
        }

    }

    public void madeChoice() {

        Human temp = checkQueue.Dequeue();
        temp.init();
        checkQueue.Enqueue(temp);
        for (int i = 0; i < checkQueue.Count; i++)
        {
            Human h = checkQueue.ElementAt(i);
            h.setHumanSpriteScale(Mathf.Pow(0.9f, i));
            //获取HumanSprite
            h.setLayer((checkQueue.Count - i) * 2);

            h.GetComponent<BoxCollider2D>().enabled = false;
            h.Phone.SetActive(false);
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + i*2, 0);
            h.tgtPos = pos;

        }
        //第一个打开Collider
        //checkQueue.Peek().GetComponent<BoxCollider2D>().enabled = true;
        checkQueue.Peek().whenitsYourTurn();
       
    }

    public void MadeRightChoice()
    {
        SuccessCheck++;
        audioSource.PlayOneShot(Choose_True);
        Debug.Log("选对了");
    }

    public void MadeWrongChoice()
    {
        audioSource.PlayOneShot(Choose_False);

        if(HPValue >0)
        {
            Animator LIFE_anim = LIFEs[HPValue - 1].GetComponent<Animator>();
            LIFE_anim.SetTrigger("Disappear");

            HPValue = HPValue - 1;
        }

        Debug.Log("选错了"); 

    }

    public void MadeFatalChoice()
    {
        audioSource.PlayOneShot(Choose_False);

        while (HPValue > 0)
        {
            Animator LIFE_anim = LIFEs[HPValue - 1].GetComponent<Animator>();
            LIFE_anim.SetTrigger("Disappear");

            HPValue = HPValue - 1;
        }

        Debug.Log("你屎定了");

    }

    private void CountDown()//倒计时
    {
        float time = curTime - Time.deltaTime;
        if (Mode == Constant.LIMITED)
        {
            int t=Mathf.FloorToInt(time);
            if (t <= 0)
                t = 0;
            timeText.text = t.ToString();

        }//归零
        if (time <= 0)
            time = 0;
        else
        {
            //音效
            preSecond += Time.deltaTime;
            if (preSecond >= 1)
            {
                preSecond = 0;
                if(ClockAudio)
                    audioSource.PlayOneShot(ClockAudio);
            }
            //表针转动
            clockHand.transform.Rotate(Vector3.forward, 360 / timeOffset * Time.deltaTime);
            //背景变换  
            clockBK.fillAmount = time / timeOffset;
            //var lerp = (Mathf.PingPong(Time.deltaTime, timePre) / timePre) / 2;
            if(Mode==Constant.LIMITED)
                clockBK.color = Color.Lerp(clockBK.color, colorEnd, 0.005f/6);
            else
                clockBK.color = Color.Lerp(clockBK.color, colorEnd, 0.005f);
            //时钟晃动
            if (time/timePre<=0.4)
            {
                clock.transform.rotation = Quaternion.Euler(Vector3.zero);
                clock.transform.Rotate(Vector3.forward, Mathf.Sin(Time.time * Mathf.PI * frequency) *2);
            }
        }

        curTime = time;

    }

    public void resetTime()
    {
        Debug.Log("时间:" + timeOffset);
        
        if (timeOffset > 6)
        {
            timeOffset =  timePre - (SuccessCheck / 5);
        }
        curTime = timeOffset;
        clockBK.fillAmount = 1;
        clockBK.color = colorStart;
        clockHand.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void DragMask() {
        if (isDragMask)
        {
            indicator.SetActive(true);
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            indicator.transform.position = pos;

        }
        else {
            indicator.SetActive(false);
        }
    }

    public void DragSensor()
    {
        Sensor s = sensor.GetComponent<Sensor>();
        if (isDragSensor)
        {
            //indicator.SetActive(true);
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            sensor.transform.position = pos;
        }
        else
        {
            s.goBacktoOriginalState();
        }
    }

    public bool ifDead() {
        if (HPValue == 0 || curTime <= 0)
            return true;
        return false;
    }

    public void DeathJudgement() {
        if (ifDead())
            Invoke("Pause", 1f);// Pause();

    }

    public void Pause()
    {
        interactable = false;
        Time.timeScale = 0;
        audioSource.PlayOneShot(OpenPauseAudio);
        PauseUI.gameObject.SetActive(true);
    }

}
