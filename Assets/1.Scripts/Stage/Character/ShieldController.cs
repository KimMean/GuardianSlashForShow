using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [SerializeField] GameObject stickMan;
    BoxCollider2D boxCollider;
    LayerMask layerMask;
    ParticleManager defenseParticle;
    CustomPhysics physics;


    int PushPower = 2;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        layerMask = LayerMask.GetMask("Block");
        physics = stickMan.GetComponent<CustomPhysics>();

        string necklaceCode = GameManager.Instance.GetEquipmentNecklace();
        if (necklaceCode != "N000")
            PushPower += NecklaceManager.Instance.GetNecklaceData(necklaceCode).GetVoid() / 5;


        GameObject particle = GameObject.Find("DefenseParticle");
        if (particle != null)
        {
            defenseParticle = particle.GetComponent<ParticleManager>();
        }
    }

    /// <summary>
    /// Ȱ��ȭ �� ��� �浹ü�� �˻��մϴ�.
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
    /// ����� �浹�� ��� �о���ϴ�.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnterBlock(Collider2D other)
    {
        SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Defense);
        PushBlock(other);
        defenseParticle.ParticleActivation(transform.position);
        if (physics.GetIsGround())
        {
            StageManager.Instance.GroundDefense();
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag.Equals("Block"))
    //    {
    //        PushBlock(collision);
    //    }
    //}

    /// <summary>
    /// ����� �浹�� ��� ��� ������Ʈ�� ��ģ��ŭ �о���ϴ�.
    /// </summary>
    /// <param name="other"></param>
    private void PushBlock(Collider2D other)
    {
        float overlapY = Calculator.GetOverlapValue(GetComponent<Collider2D>().bounds, other.bounds).y;

        Vector2 position = other.transform.parent.position;
        position.y += overlapY;
        other.transform.parent.position = position;    // ��ģ��ŭ �̵�

        CustomPhysics otherPhysics = other.transform.parent.GetComponent<CustomPhysics>();
        stickMan.GetComponent<CustomPhysics>().velocity = otherPhysics.velocity;
        //Debug.Log(otherPhysics.Velocity);
        otherPhysics.velocity = Vector2.zero;
        otherPhysics.AddForce(Vector2.up * PushPower);
        //Debug.Log($"PushPower : {PushPower}");
    }
}
