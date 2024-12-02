using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    CharacterController _CharacterController;
    BoxCollider2D _BoxCollider;
    LayerMask _LayerMask;
    ParticleManager frostParticle;

    long AttackPower = 0;
    int AdditionalAttackPowerRatio = 0; // 추가 데미지 비율

    int SlowdownProbability = 0;    // 감속 확률

    private void Awake()
    {
        _BoxCollider = GetComponent<BoxCollider2D>();
        _LayerMask = LayerMask.GetMask("Block");

        string weaponCode = GameManager.Instance.GetEquipmentWeapon();
        AttackPower = WeaponManager.Instance.GetWeaponData(weaponCode).GetAttackPower();

        string ringCode = GameManager.Instance.GetEquipmentRing();
        if (ringCode == "R000")
            AdditionalAttackPowerRatio = 0;
        else
            AdditionalAttackPowerRatio = RingManager.Instance.GetRingData(ringCode).GetAdditionalAttackPowerRatio();


        string necklaceCode = GameManager.Instance.GetEquipmentNecklace();
        if (necklaceCode == "N000")
            SlowdownProbability = 0;
        else
            SlowdownProbability = NecklaceManager.Instance.GetNecklaceData(necklaceCode).GetHell();

        GameObject particle = GameObject.Find("FrostParticle");
        if (particle != null)
        {
            frostParticle = particle.GetComponent<ParticleManager>();
        }
    }

    private void OnEnable()
    {
        Bounds bounds = _BoxCollider.bounds;

        // 콜라이더의 중심과 크기 설정
        Vector2 center = bounds.center;
        Vector2 size = bounds.size;

        // 겹치는 오브젝트들을 검사합니다.
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0, _LayerMask);

        foreach (Collider2D hit in hits)
        {
            OnTriggerEnterBlock(hit);
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnterBlock(Collider2D other)
    {
        // 데미지는 무기 데미지와 추가 공격력 사이에서 결정됩니다.
        long damage = AttackPower;
        if(AdditionalAttackPowerRatio > 0)
        {
            long MaxAttackDamage = AttackPower + (long)(AttackPower * (AdditionalAttackPowerRatio / 100f));
            float t = Random.value; // 0과 1 사이의 랜덤 값
            damage = (long)(AttackPower * (1 - t) + MaxAttackDamage * t); // 선형 보간
        }
        other.GetComponent<BlockController>().ApplyDamage(damage);
        UIManager.Instance.ShowDamage(Camera.main.WorldToScreenPoint(transform.position), damage);

        // 확률적 속도 감속
        if (Random.Range(0, 100) < SlowdownProbability)
        {
            CustomPhysics otherPhysics = other.transform.parent.GetComponent<CustomPhysics>();
            otherPhysics.ReduceVelocity();

            frostParticle.ParticleActivation(other.bounds.center);
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Frost);
        }
    }

}
