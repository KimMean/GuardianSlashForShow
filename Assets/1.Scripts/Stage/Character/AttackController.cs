using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    BoxCollider2D boxCollider;
    LayerMask layerMask;
    ParticleManager frostParticle;

    long attackPower = 0;
    int additionalAttackPowerRatio = 0; // �߰� ������ ����

    int slowdownProbability = 0;    // ���� Ȯ��

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
    /// ������Ʈ�� Ȱ��ȭ �Ǹ� �浹ü�� �˻��մϴ�.
    /// </summary>
    private void OnEnable()
    {
        Bounds bounds = boxCollider.bounds;

        // �ݶ��̴��� �߽ɰ� ũ�� ����
        Vector2 center = bounds.center;
        Vector2 size = bounds.size;

        // ��ġ�� ������Ʈ���� �˻��մϴ�.
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0, layerMask);

        foreach (Collider2D hit in hits)
        {
            OnTriggerEnterBlock(hit);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �浹ü�� ����� ��� ����ó���� �մϴ�.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnterBlock(Collider2D other)
    {
        // �������� ���� �������� �߰� ���ݷ� ���̿��� �����˴ϴ�.
        long damage = attackPower;
        if(additionalAttackPowerRatio > 0)
        {
            long maxAttackDamage = attackPower + (long)(attackPower * (additionalAttackPowerRatio / 100f));
            float t = Random.value; // 0�� 1 ������ ���� ��
            damage = (long)(attackPower * (1 - t) + maxAttackDamage * t); // ���� ����
        }
        other.GetComponent<BlockController>().ApplyDamage(damage);
        UIManager.Instance.ShowDamage(Camera.main.WorldToScreenPoint(transform.position), damage);

        // Ȯ���� �ӵ� ����
        if (Random.Range(0, 100) < slowdownProbability)
        {
            CustomPhysics otherPhysics = other.transform.parent.GetComponent<CustomPhysics>();
            otherPhysics.ReduceVelocity();

            frostParticle.ParticleActivation(other.bounds.center);
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Frost);
        }
    }

}
