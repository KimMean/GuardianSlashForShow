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
    /// 활성화 된 경우 충돌체를 검사합니다.
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
    /// 블록이 충돌한 경우 밀어냅니다.
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
    /// 블록이 충돌한 경우 대상 오브젝트를 겹친만큼 밀어냅니다.
    /// </summary>
    /// <param name="other"></param>
    private void PushBlock(Collider2D other)
    {
        float overlapY = Calculator.GetOverlapValue(GetComponent<Collider2D>().bounds, other.bounds).y;

        Vector2 position = other.transform.parent.position;
        position.y += overlapY;
        other.transform.parent.position = position;    // 겹친만큼 이동

        CustomPhysics otherPhysics = other.transform.parent.GetComponent<CustomPhysics>();
        stickMan.GetComponent<CustomPhysics>().velocity = otherPhysics.velocity;
        //Debug.Log(otherPhysics.Velocity);
        otherPhysics.velocity = Vector2.zero;
        otherPhysics.AddForce(Vector2.up * PushPower);
        //Debug.Log($"PushPower : {PushPower}");
    }
}
