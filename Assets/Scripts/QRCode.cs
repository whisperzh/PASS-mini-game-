using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QRCode
{
    public enum QRType {Green,Yellow,Red };

    public QRType qrType;

    public Sprite[] spritePool;



}
