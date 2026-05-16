using UnityEngine;
using UnityEngine.UI;

public class FlashlightAttack : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlight; // ÇÊÑßåÇ İÇÑÛÉ İí ÇáÅäÓÈíßÊæÑ! ÇáßæÏ ÓíÌÏåÇ ÊáŞÇÆíÇğ
    public float batteryLevel = 100f;
    public float drainNormal = 2f;
    public float drainAttack = 10f;

    [Header("Attack Settings")]
    public float attackAngle = 15f;
    public float normalAngle = 50f;
    public float damagePerSecond = 50f;
    public float range = 10f;
    private Enemy lastHitEnemy;
    private bool isAttacking = false;

    [Header("UI References")]
    public GameObject FlashlightIcon; // ÕæÑÉ ÇáßÔÇİ
    public Image batteryBarImage; // ÇáÚÏÇÏ ÇáÏÇÆÑí ÊÍÊ íÓÇÑ

    [Header("Script References")]
    public PlayerStats playerstats;
    public InventoryManager inventoryManager;

    private GameObject handsFlashlightObject;

    void Start()
    {
        FindFlashlightAutomatically();
    }

    void FindFlashlightAutomatically()
    {
        // ÇáÈÍË ÇáÊáŞÇÆí Úä ãÓÇÑ ãÌÓã ÇáßÔÇİ ÏÇÎá íÏ ÇááÇÚÈ ÍÓÈ ÍÒãÉ UHFPS
        Transform handsHolder = transform.Find("FPView/PlayerVirtualCamera/HandsHolder");
        if (handsHolder != null)
        {
            Transform flTransform = handsHolder.Find("Flashlight");
            if (flTransform != null)
            {
                handsFlashlightObject = flTransform.gameObject;
                // ÌáÈ ãÑÌÚ ÇááãÈÉ ÇáÏÇÎáí ÊáŞÇÆíÇğ ÈÏæä ÓÍÈ íÏæí
                flashlight = handsFlashlightObject.GetComponentInChildren<Light>(true);
            }
        }
    }

    void Update()
    {
        if (playerstats.isDead) return;

        // ÅĞÇ ÖÇÚÊ ÇáäÓÎÉ Ãæ áã íÚËÑ ÚáíåÇ ÈÚÏ¡ íÈÍË ÚäåÇ İæÑÇğ
        if (handsFlashlightObject == null || flashlight == null)
        {
            FindFlashlightAutomatically();
            return;
        }

        // ÇáÊÍŞŞ: åá ÇááÇÚÈ ãÇÓß ÇáßÔÇİ İí íÏå ÍÇáíÇğ æãİÚøá ãä ÇáÍÒãÉ¿
        bool isHoldingFlashlight = handsFlashlightObject.activeInHierarchy;

        // ÅÙåÇÑ æÅÎİÇÁ ÇáÃíŞæäÉ æÇáÚÏÇÏ ÈäÇÁğ Úáì åá ÇáßÔÇİ İí íÏß Ãæ áÇ
        if (FlashlightIcon != null) FlashlightIcon.SetActive(isHoldingFlashlight);
        if (batteryBarImage != null) batteryBarImage.gameObject.SetActive(isHoldingFlashlight);

        // ÅĞÇ ÇááÇÚÈ ãæ ãÇÓß ÇáßÔÇİ ÍÇáíÇğ¡ äæŞİ ÈŞíÉ ÇáßæÏ
        if (!isHoldingFlashlight)
        {
            StopAttack();
            return;
        }

        // 3. ÃãÑ ÇáÑíáæÏ ÈÖÛØ ÒÑ R (ÔÛÇá ÏÇÆãÇğ ØÇáãÇ ÇáßÔÇİ İí íÏß)
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (batteryLevel < 100f && inventoryManager.HasItem("Battery"))
            {
                batteryLevel = 100f; // ÔÍä ÇáÚÏÇÏ ÇáÏÇÆÑí
                inventoryManager.RemoveItem("Battery"); // ÎÕã ÍÈÉ ãä ÇáÔäØÉ
                flashlight.enabled = true; // ÊÔÛíá ÇáÖæÁ İæÑÇğ
                Debug.Log("Êã ÅÚÇÏÉ ÇáÔÍä ÈäÌÇÍ æÈÔßá ÏíäÇãíßí!");
            }
        }

        // 4. ÅĞÇ İÖÊ ÇáÈØÇÑíÉ¡ äØİí ÇááãÈÉ æäÕİÑ ÇáÚÏÇÏ
        if (batteryLevel <= 0)
        {
            batteryLevel = 0;
            flashlight.enabled = false; // íØİí ÇáäæÑ
            StopAttack();
            if (batteryBarImage != null) batteryBarImage.fillAmount = 0;
            return;
        }

        // 5. ÇÓÊåáÇß ÇáØÇŞÉ ÃËäÇÁ ÇáåÌæã Ãæ ÇáæÖÚ ÇáÚÇÏí
        if (flashlight.enabled)
        {
            if (Input.GetMouseButton(1)) // ßáíß íãíä (ÊÑßíÒ ÇáäæÑ æÇáåÌæã)
            {
                flashlight.spotAngle = Mathf.Lerp(flashlight.spotAngle, attackAngle, Time.deltaTime * 10f);
                batteryLevel -= drainAttack * Time.deltaTime;
                PerformAttack();
            }
            else // æÖÚ ÚÇÏí
            {
                flashlight.spotAngle = Mathf.Lerp(flashlight.spotAngle, normalAngle, Time.deltaTime * 10f);
                batteryLevel -= drainNormal * Time.deltaTime;
                StopAttack();
            }
        }
        else
        {
            StopAttack();
        }

        // ÊÍÏíË äÓÈÉ ÇáÚÏÇÏ ÇáÏÇÆÑí ÊÍÊ íÓÇÑ
        if (batteryBarImage != null)
        {
            batteryBarImage.fillAmount = batteryLevel / 100f;
        }
    }

    void PerformAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(flashlight.transform.position, flashlight.transform.forward, out hit, range))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (lastHitEnemy != null && lastHitEnemy != enemy) enemy.StopAttackSound();
                enemy.PlayAttackSound();
                enemy.TakeDamage(damagePerSecond * Time.deltaTime);
                isAttacking = true;
                lastHitEnemy = enemy;
                return;
            }
        }
        StopCurrentEnemySound();
    }

    void StopAttack() { isAttacking = false; }
    void StopCurrentEnemySound() { if (lastHitEnemy != null) { lastHitEnemy.StopAttackSound(); lastHitEnemy = null; } }
}