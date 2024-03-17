using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    PlayerStatus pStat;
    PlayerInputValue pValue;

    Animator anim;

    private void Awake()
    {
        pStat = GetComponent<PlayerStatus>();
        pValue = GetComponent<PlayerInputValue>();

        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        WeaponUpdate();
    }

    private void WeaponUpdate()
    {
        if(!pValue.onAction)
        {
            if(pValue.norAtk || pValue.spcAtk)
            {
                anim.SetBool("InCombat", true);
                anim.SetLayerWeight(1, 1f);
            }

            anim.SetBool("Atk_Ult", pValue.ultAtk);
            anim.SetBool("Atk_Spc", pValue.spcAtk);
            anim.SetBool("Atk_Nor", pValue.norAtk);
            anim.SetBool("Atk_Chg", pValue.chgAtk);
        }
    }

    public void AwakeWeapon()
    {

    }
}