using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private WeaponAssaultrifle weapon; // 현재 정보가 출력되는 무기
    [SerializeField]
    private Status              status;

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName; // 무기 이름
    [SerializeField]
    private Image imageWeaponIcon; // 무기 아이콘
    [SerializeField]
    private Sprite[] spriteWeaponIcons; // 무기 아이콘에 사용되는 sprite 배열

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP;
    [SerializeField]
    private Image imageBloodScreen;
    [SerializeField]
    private AnimationCurve curveBloodScreen;


    private void Awake()
    {
        SetupWeapon();

        // 메소드가 등록되어 있는 이벤트 클래스(weapon.xx)의
        // Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다.
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        status.onHPEvent.AddListener(UpdateHPHUD);
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];

    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP " + current;

        if ( previous - current > 0 )
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while ( percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
