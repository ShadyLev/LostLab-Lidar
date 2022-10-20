using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScannerCharge : MonoBehaviour
{
    public LIDARScanner scannerScript;
    public Image barImage;

    private float maxChargeTime;
    private float currentChargeTime;

    private float lerpSpeed;

    public Color startColor;
    public Color endColor;

    // Start is called before the first frame update
    void Start()
    {
        maxChargeTime = scannerScript.m_FullScanCharge;
        currentChargeTime = scannerScript.m_CurrentScanCharge;
    }

    // Update is called once per frame
    void Update()
    {
        currentChargeTime = scannerScript.m_CurrentScanCharge;

        lerpSpeed = 3f * Time.deltaTime;

        BarFiller();
        ColourChanger();
    }

    void BarFiller()
    {
        barImage.fillAmount = Mathf.Lerp(barImage.fillAmount, currentChargeTime / maxChargeTime, lerpSpeed);
    }

    void ColourChanger()
    {
        Color barColour = Color.Lerp(startColor, endColor, currentChargeTime/ maxChargeTime);
        barImage.color = barColour;
    }
}
