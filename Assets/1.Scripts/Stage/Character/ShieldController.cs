using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [SerializeField] GameObject StickMan;
    BoxCollider2D _BoxCollider;
    LayerMask _LayerMask;
    ParticleManager defenseParticle;
    CustomPhysics physics;


    int PushPower = 2;

    private void Awake()
    {
        _BoxCollider = GetComponent<BoxCollider2D>();
        _LayerMask = LayerMask.GetMask("Block");
        physics = StickMan.GetComponent<CustomPhysics>();

        string necklaceCode = GameManager.Instance.GetEquipmentNecklace();
        if (necklaceCode != "N000")
            PushPower += NecklaceManager.Instance.GetNecklaceData(necklaceCode).GetVoid() / 5;


        GameObject particle = GameObject.Find("DefenseParticle");
        if (particle != null)
        {
            defenseParticle = particle.GetComponent<ParticleManager>();
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
        PushBlock(other);
        defenseParticle.ParticleActivation(transform.position);
        if (physics.GetIsGround())
        {
            StageManager.Instance.GroundDefense();
        }
        SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Defense);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag.Equals("Block"))
    //    {
    //        PushBlock(collision);
    //    }
    //}

    private void PushBlock(Collider2D other)
    {
        float overlapY = Calculator.GetOverlapValue(GetComponent<Collider2D>().bounds, other.bounds).y;

        Vector2 position = other.transform.parent.position;
        position.y += overlapY;
        other.transform.parent.position = position;    // 겹친만큼 이동

        CustomPhysics otherPhysics = other.transform.parent.GetComponent<CustomPhysics>();
        StickMan.GetComponent<CustomPhysics>().Velocity = otherPhysics.Velocity;
        //Debug.Log(otherPhysics.Velocity);
        otherPhysics.Velocity = Vector2.zero;
        otherPhysics.AddForce(Vector2.up * PushPower);
        //Debug.Log($"PushPower : {PushPower}");
    }
}
