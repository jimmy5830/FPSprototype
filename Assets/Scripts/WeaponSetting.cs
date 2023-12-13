using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName {  AssaultRifle = 0 }

[System.Serializable]
public struct WeaponSetting 
{
    public WeaponName weaponName;
    public int damage;
    public int currentAmmo;
    public int maxAmmo;
    public float attackRate; // 공격 속도
    public float attackDistance; // 공격 사거리
    public bool  isAutomaticAttack; // 연속 공격 여부
}
