using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public string weaponName;
    public Sprite weaponSprite;
    public AudioClip weaponShotAudio;
    public AudioClip weaponRechargeAudio;
    public int weaponDamage;

    public int weaponBulletsPerShot;
    public int weaponCharger;
    public int weaponChargerMax;
    public int weaponReserve;
    public int weaponReserveMax;

    public float weaponCadency;
    public float weaponRechargeTime;

    public int weaponBulletPref;

    public bool weaponCanShot = true;

    
    public object Clone()
    {
        return this.MemberwiseClone();
    }

}
