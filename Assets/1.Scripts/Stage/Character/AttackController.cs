using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    BoxCollider2D boxCollider;
    LayerMask layerMask;
    ParticleManager frostParticle;

    long attackPower = 0;
    int additionalAttackPowerRatio = 0; // 추가 데미지 비율

    int slowdownProbability = 0;    // 감속 확률

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        layerMask = LayerMask.GetMask("Block");

        string weaponCode = GameManager.Instance.GetEquipmentWeapon();
        attackPower = WeaponManager.Instance.GetWeaponData(weaponCode).GetAttackPower();

        string ringCode = GameManager.Instance.GetEquipmentRing();
        if (ringCode == "R000")
            additionalAttackPowerRatio = 0;
        else
            additionalAttackPowerRatio = RingManager.Instance.GetRingData(ringCode).GetAdditionalAttackPowerRatio();


        string necklaceCode = GameManager.Instance.GetEquipmentNecklace();
        if (necklaceCode == "N000")
            slowdownProbability = 0;
        else
            slowdownProbability = NecklaceManager.Instance.GetNecklaceData(necklaceCode).GetHell();

        GameObject particle = GameObject.Find("FrostParticle");
        if (particle != null)
        {
            frostParticle = particle.GetComponent<ParticleManager>();
        }
    }

    /// <summary>
    /// 오브젝트가 활성화 되면 충돌체를 검사합니다.
    /// </summary>
    private void OnEnable()
    {
        Bounds bounds = boxCollider.bounds;

        // 콜라이더의 중심과 크기 설정
        Vector2 center = bounds.center;
        Vector2 size = bounds.size;

        // 겹치는 오브젝트들을 검사합니다.
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0, layerMask);

        foreach (Collider2D hit in hits)
        {
            OnTriggerEnterBlock(hit);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 충돌체가 블록인 경우 공격처리를 합니다.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnterBlock(Collider2D other)
    {
        // 데미지는 무기 데미지와 추가 공격력 사이에서 결정됩니다.
        long damage = attackPower;
        if(additionalAttackPowerRatio > 0)
        {
            long maxAttackDamage = attackPower + (long)(attackPower * (additionalAttackPowerRatio / 100f));
            float t = Random.value; // 0과 1 사이의 랜덤 값
            damage = (long)(attackPower * (1 - t) + maxAttackDamage * t); // 선형 보간
        }
        other.GetComponent<BlockController>().ApplyDamage(damage);
        UIManager.Instance.ShowDamage(Camera.main.WorldToScreenPoint(transform.position), damage);

        // 확률적 속도 감속
        if (Random.Range(0, 100) < slowdownProbability)
        {
            CustomPhysics otherPhysics = other.transform.parent.GetComponent<CustomPhysics>();
            otherPhysics.ReduceVelocity();

            frostParticle.ParticleActivation(other.bounds.center);
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Frost);
        }
    }

}
