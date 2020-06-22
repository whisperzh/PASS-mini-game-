using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    public QRCode qrCode;
    public bool isWearingMask;
    public float bodyTemperature;
    private bool canMove = true;
    public GameObject MaskPrefab;
    public Sprite[] HumanPool;
    public enum Gender { man, woman };

    public Gender gender;

    public Vector3 tgtPos;

    public Sprite MaleMask, FemaleMask;

    [Header("36.0-37.2的权重")]
    [Range(0, 10)]
    public int TempFirstWeight;
    [HideInInspector]
    public int TempSecondWeight;

    [Header("3种码的权重,分别为绿，黄，红")]
    [Range(0, 100)]
    public int QRGreenWeight;
    [Range(0, 100)]
    public int QRYellowWeight;
    [Range(0, 100)]
    public int QRRedWeight;

    private bool isDrag = false;

    public GameObject humanSprite;
    public GameObject Phone;

    public GameController gameController;

    private Animator humanAnimator;
    private bool IsShow;
    private Vector3 lastMousePos = Vector3.zero;
    private bool mouseEnterForModify = false;
    public GameObject QRcode;
    public GameObject TempratureSensor;

    private AudioSource audioSource;
    public AudioClip takeMask;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        TempSecondWeight = 10 - TempFirstWeight;
        humanAnimator = humanSprite.GetComponent<Animator>();
        TempratureSensor = GameObject.Find("realsensor");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.interactable)
        {
            humanAnimator.SetBool("IsDrag", isDrag);
            if (isDrag)
            {
                Vector3 objectPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                float moveDis = objectPosition.x - lastMousePos.x;
                humanAnimator.SetFloat("Move", moveDis);
                lastMousePos = objectPosition;

                //把鼠标位置传给物体
                objectPosition = new Vector3(objectPosition.x, transform.position.y, 0);
                transform.position = objectPosition;
            }
            else
            {
                lastMousePos = Vector3.zero;
                movetoposition();
            }

            if (transform.position.x <= -1.3 || transform.position.x >= 1.3)
            {
                if (!isDrag)
                {
                    humanAnimator.SetBool("IsShow", false);

                    canMove = false;
                    isDrag = false;
                    //gameObject.SetActive(false);
                    GameController.isChange = true;
                    if ((transform.position.x <= -1.3 && ifCanPass() == 0) || (transform.position.x >= -1.3 && ifCanPass() == 2))
                        gameController.MadeRightChoice();
                    else if (ifCanPass() == 1)
                        gameController.MadeWrongChoice();
                    else
                        gameController.MadeFatalChoice();
                    if(GameController.Mode==Constant.ENDLESS)
                        gameController.resetTime();
                }
            }
            else
            {
                canMove = true;

            }
        }
    }

    public void init()
    {
        //QRCode初始化
        HumanSpriteInit();
        QRCodeInit();
        MaskInit();
        TemperatureInit();
    }

    private void HumanSpriteInit()
    {
        try
        {
            humanAnimator.SetBool("IsShow", false);
        }
        catch (Exception e) { }
        int randomNum = UnityEngine.Random.Range(0, HumanPool.Length);
        humanSprite.GetComponent<SpriteRenderer>().sprite = HumanPool[randomNum];
        if (randomNum >= 0 && randomNum <= 4)
            gender = Gender.man;
        else
            gender = Gender.woman;

    }

    private void QRCodeInit() {
        int t = rand(new int[] { QRGreenWeight, QRYellowWeight,QRRedWeight }, QRGreenWeight + QRYellowWeight+ QRRedWeight);
        if (t==0)
            qrCode.qrType = QRCode.QRType.Green;
        else if (t==1)
            qrCode.qrType = QRCode.QRType.Yellow;
        else
            qrCode.qrType = QRCode.QRType.Red;
        setQRcodeSprite();
    }

    private void MaskInit() {
        int isWearingPara = UnityEngine.Random.Range(0, 100);
        if (isWearingPara % 2 == 0)
            isWearingMask = true;
        else
            isWearingMask = false;
        if (gender == Gender.man)
            MaskPrefab.GetComponent<SpriteRenderer>().sprite = MaleMask;
        else
            MaskPrefab.GetComponent<SpriteRenderer>().sprite = FemaleMask;
        MaskPrefab.SetActive(isWearingMask);
    }

    private void TemperatureInit() {
        int TempPara;
        int rd = rand(new int[] { TempFirstWeight, TempSecondWeight }, TempSecondWeight + TempFirstWeight);
        if (rd == 0)
            TempPara = UnityEngine.Random.Range(366, 372);
        else
            TempPara = UnityEngine.Random.Range(373, 379);

        bodyTemperature = (float)TempPara / 10.0f;
    }

    /**加*/
    //rate:几率数组（%），  total：几率总和（100%）
    // Debug.Log(rand(new int[] { 10, 5, 15, 20, 30, 5, 5,10 }, 100));
    public static int rand(int[] rate, int total)
    {
        int r = UnityEngine.Random.Range(1, total + 1);
        int t = 0;
        for (int i = 0; i < rate.Length; i++)
        {
            t += rate[i];
            if (r < t)
            {
                return i;
            }
        }
        return 0;
    }


    private void OnMouseDown()
    {
        if (canMove)
            isDrag = true;

    }

    private void OnMouseUp()
    {
        isDrag = false;
    }

    /// <summary>
    /// 移动位置s
    /// </summary>
    public void movetoposition()
    {
        transform.position = Vector3.Lerp(transform.position, tgtPos, 0.3f);
    }

    public SpriteRenderer GetHumanSprite
    {
        get
        {
            return humanSprite.GetComponent<SpriteRenderer>();
        }
    }

    public void whenitsYourTurn()
    {
        myOnMouseOver();
        GetComponent<BoxCollider2D>().enabled = true;
        Phone.SetActive(true);
        Phone.GetComponent<SpriteRenderer>().sortingOrder = humanSprite.GetComponent<SpriteRenderer>().sortingOrder;
        tgtPos = new Vector3(tgtPos.x, -2, 0);
        humanAnimator.SetBool("IsShow", true);
        QRcode.GetComponent<SpriteRenderer>().sortingOrder = Phone.GetComponent<SpriteRenderer>().sortingOrder + 1;
        if (mouseEnterForModify && GameController.isDragMask&&!isWearingMask)
            AddMask();
        if (mouseEnterForModify && GameController.isDragSensor)
            getTemprature();
    }

    private int ifCanPass() {
        if (bodyTemperature <= 37.2 && isWearingMask && qrCode.qrType == QRCode.QRType.Green)
            return 0;
        else if (bodyTemperature <= 37.2 && !isWearingMask && qrCode.qrType == QRCode.QRType.Green)
            return 1;
        else
            return 2;
    }

    public void setLayer(int i)
    {
        GetHumanSprite.sortingOrder = i;
        MaskPrefab.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
    }

    public void setHumanSpriteScale(float para)
    {
        GetHumanSprite.transform.localScale = new Vector3(para, para, para);
    }

    private void setQRcodeSprite() {
        switch (qrCode.qrType)
        {
            case QRCode.QRType.Green:
                QRcode.GetComponent<SpriteRenderer>().sprite =qrCode.spritePool[0];
                break;
            case QRCode.QRType.Yellow:
                QRcode.GetComponent<SpriteRenderer>().sprite = qrCode.spritePool[1];
                break;
            case QRCode.QRType.Red:
                QRcode.GetComponent<SpriteRenderer>().sprite = qrCode.spritePool[2];
                break;

        }
        
     }

    public void showcode() {
        QRcode.GetComponent<SpriteRenderer>().sortingOrder = MaskPrefab.GetComponent<SpriteRenderer>().sortingOrder+1;
    }

    private void myOnMouseOver()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 pos2 = pos;
        pos2.z = 10;
        pos.z = -4;
        Vector3 tgt = pos2 - pos;
        Debug.DrawRay(pos, tgt, Color.red);
        if (Physics2D.Raycast(pos, tgt, 5, 1 << 8))
        {
            if (pos.y >= transform.position.y - 1 && pos.y <= transform.position.y + 0.5f)
                mouseEnterForModify = true;
            else
                mouseEnterForModify = false;
        }
        else
            mouseEnterForModify = false;
    }

    public void AddMask() {
        isWearingMask = true;
        MaskPrefab.SetActive(true);
        audioSource.PlayOneShot(takeMask);
        if (gender == Gender.man)
            MaskPrefab.GetComponent<SpriteRenderer>().sprite = MaleMask;
        else
            MaskPrefab.GetComponent<SpriteRenderer>().sprite = FemaleMask;
        MaskPrefab.GetComponent<SpriteRenderer>().sortingOrder = QRcode.GetComponent<SpriteRenderer>().sortingOrder;
        GameController.isDragMask = false;
    }

    public void getTemprature() {
        TempratureSensor.GetComponent<Sensor>().setText(bodyTemperature);
    }
}
