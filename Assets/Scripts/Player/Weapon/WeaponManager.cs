using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    PlayerInputValue pValue;
    PlayerStatus pStat;
    PlayerWeapon pWeapon;

    Volume UIvolume;
    ColorAdjustments c;
    ChromaticAberration ch;
    LensDistortion l;

    [SerializeField]
    GameObject Panel;
    [SerializeField]
    RectTransform uipointer;
    [SerializeField]
    ParticleSystem outline;
    [SerializeField]
    Material outlineMat;

    private float UImaxdistance = 25;
    private Color[] outlineColorSet;

    private int onWeaponNum;

    private void Awake()
    {
        pValue = player.GetComponent<PlayerInputValue>();
        pStat = player.GetComponent<PlayerStatus>();
        pWeapon = player.GetComponent<PlayerWeapon>();

        UIvolume = GetComponentInChildren<Volume>();
        UIvolume.profile.TryGet(out c);
        UIvolume.profile.TryGet(out ch);
        UIvolume.profile.TryGet(out l);

        outlineColorSet = new Color[7];
        outlineColorSet[0] = new Color(0.19f, 0.66f, 0f, 0.45f);
        outlineColorSet[1] = new Color(0.66f, 0f, 0f, 0.45f);
        outlineColorSet[2] = new Color(0f, 0.66f, 0.66f, 0.45f);
        outlineColorSet[3] = new Color(0.66f, 0.29f, 0f, 0.45f);
        outlineColorSet[4] = new Color(0.19f, 0.05f, 0f, 0.45f);
        outlineColorSet[5] = new Color(0.6f, 0f, 0.6f, 0.45f);
        outlineColorSet[6] = new Color(0.45f, 0.45f, 0.45f, 0.45f);

    }

    private void Update()
    {
        UItapControl();

        if(pValue.UIOpened)
        {
            UIPointer();
            UIOnPointer();
        }
    }

    private void LateUpdate()
    {
        SelectWeapon();
    }


    private void UItapControl()
    {
        if (pValue.UIOpened)
        {
            Panel.SetActive(true);
            Time.timeScale = 0.5f;

            c.contrast.Override(Mathf.Lerp(c.contrast.value, 40f, 0.07f));
            ch.intensity.Override(Mathf.Lerp(ch.intensity.value, 1f, 0.07f));
            l.intensity.Override(Mathf.Lerp(l.intensity.value, -0.6f, 0.07f));

        }
        else
        {
            Panel.SetActive(false);
            Time.timeScale = 1f;

            c.contrast.Override(Mathf.Lerp(c.contrast.value, 0f, 0.1f));
            ch.intensity.Override(Mathf.Lerp(ch.intensity.value, 0f, 0.1f));
            l.intensity.Override(Mathf.Lerp(l.intensity.value, 0f, 0.1f));

            uipointer.anchoredPosition = Vector2.zero;
            outlineMat.SetColor("_TintColor", outlineColorSet[6]);
        }
    }

    private void UIPointer()
    {
        Vector2 temp = uipointer.anchoredPosition;

        temp += new Vector2(pValue.look.x, -pValue.look.y);
        if (temp.magnitude > UImaxdistance)
            temp = temp.normalized * UImaxdistance;

        uipointer.anchoredPosition = temp;
    }

    public void UIOnPointer()
    {
        float angle = Vector2.SignedAngle(Vector2.up, uipointer.anchoredPosition.normalized);

        if (0f <= angle && angle < 60f)
        {
            outlineMat.SetColor("_TintColor", outlineColorSet[0]);
            onWeaponNum = 0;
        }
        else if (0f > angle && angle >= -60f)
        {
            outlineMat.SetColor("_TintColor", outlineColorSet[1]);
            onWeaponNum = 1;
        }
        else if (60f <= angle && angle < 120f)
        {
            outlineMat.SetColor("_TintColor", outlineColorSet[2]);
            onWeaponNum = 2;
        }
        else if (-60f > angle && angle >= -120f)
        {
            outlineMat.SetColor("_TintColor", outlineColorSet[3]);
            onWeaponNum = 3;
        }
        else if (120f <= angle && angle < 180f)
        {
            outlineMat.SetColor("_TintColor", outlineColorSet[4]);
            onWeaponNum = 4;
        }
        else if (-120f > angle && angle >= -180f)
        {
            outlineMat.SetColor("_TintColor", outlineColorSet[5]);
            onWeaponNum = 5;
        }
    }

    public void SelectWeapon()
    {
        if(pValue.exitUI)
        {
            for (int i = 0; i < pStat.isWeaponed.Length; i++)
            {
                if (i == onWeaponNum)
                    pStat.isWeaponed[i] = true;
                else
                    pStat.isWeaponed[i] = false;
            }

            pWeapon.AwakeWeapon();

            pValue.exitUI = false;
        }
    }
}
