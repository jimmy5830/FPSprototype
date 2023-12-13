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
    public float attackRate; // ���� �ӵ�
    public float attackDistance; // ���� ��Ÿ�
    public bool  isAutomaticAttack; // ���� ���� ����
}
