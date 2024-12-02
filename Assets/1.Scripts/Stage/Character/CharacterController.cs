using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    ParticleManager AttackParticle;
    ParticleManager JumpParticle;
    ParticleManager SlashParticleLR;
    ParticleManager SlashParticleRL;
    CustomPhysics _Physics;
    Animator _Animator;

    [SerializeField] private float JumpPower = 10.0f;
    float ExtraJumpPower = 0;

    [SerializeField] private GameObject AttackPos;
    [SerializeField] private float AttackDelay = 0.1f;
    //[SerializeField] private float AttackPower = 0.1f;

    [SerializeField] private GameObject ShieldPos;

    [SerializeField] SpriteRenderer Weapon_SpriteRenderer;

    Vector2 groundPosition;

    bool isAttackR = false;

    

    private bool CanAttack = true;

    private void Awake()
    {
        _Physics = GetComponent<CustomPhysics>();
        _Animator = GetComponent<Animator>();

        string weaponCode = GameManager.Instance.GetEquipmentWeapon();
        Weapon_SpriteRenderer.sprite = WeaponManager.Instance.GetWeaponData(weaponCode).GetWeaponSprite();

        GameObject atkParticle = GameObject.Find("AttackParticle");
        if (atkParticle != null)
        {
            AttackParticle = atkParticle.GetComponent<ParticleManager>();
        }
        GameObject jumpParticle = GameObject.Find("JumpParticle");
        if (jumpParticle != null)
        {
            JumpParticle = jumpParticle.GetComponent<ParticleManager>();
        }
        GameObject slashParticleA = GameObject.Find("SlashLtoR");
        if (slashParticleA != null)
        {
            SlashParticleLR = slashParticleA.GetComponent<ParticleManager>();
        }
        GameObject slashParticleB = GameObject.Find("SlashRtoL");
        if (slashParticleB != null)
        {
            SlashParticleRL = slashParticleB.GetComponent<ParticleManager>();
        }

        string ringCode = GameManager.Instance.GetEquipmentRing();
        if (ringCode == "R000")
            ExtraJumpPower = 0;
        else
        {
            int ratio = RingManager.Instance.GetRingData(ringCode).GetJump();
            ExtraJumpPower = JumpPower * (ratio / 100f);
        }
        //Debug.Log($"JumpPower : {JumpPower}, ExtraJump : {ExtraJumpPower}");
        JumpPower += ExtraJumpPower;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("GroundTile"))
        {
            if (_Physics.GetIsGround()) return;
            _Physics.SetIsGround(true);
            _Animator.SetBool("IsGround", true);


            if(groundPosition == Vector2.zero)
            {
                float overlapY = Calculator.GetOverlapValue(GetComponent<Collider2D>().bounds, collision.bounds).y;

                Vector2 position = transform.position;
                position.y += overlapY;
                groundPosition = position;    // 겹친만큼 이동
                transform.position = groundPosition;
                Debug.Log(groundPosition);
            }
            else
            {
                transform.position = groundPosition;
            }

            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Landing);
            JumpParticle.ParticleActivation(groundPosition);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("GroundTile"))
        {
            //if (!_Physics.GetIsGround()) return;
            //_Physics.SetIsGround(false);
            //_Animator.SetBool("IsGround", false);

            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Jump);
            JumpParticle.ParticleActivation(groundPosition);
        }
    }


    public void Attack()
    {
        if (!CanAttack) return;

        _Animator.SetTrigger("Attack");
        CanAttack = false;
        StartCoroutine(AttackDelayCoroutine());
        //Vector2 pos = this.transform.position;
        //pos.y += 10.0f;
        AttackPos.SetActive(true);
        AttackParticle.ParticleActivation(AttackPos.transform.position);

        if (isAttackR)
        {
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.AttackA);
            SlashParticleRL.ParticleActivation(GetComponent<Collider2D>().bounds.center);
        }
        else
        {
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.AttackB);
            SlashParticleLR.ParticleActivation(GetComponent<Collider2D>().bounds.center);
        }

        isAttackR = !isAttackR;
        //Debug.Log("Attack");
    }

    private IEnumerator AttackDelayCoroutine()
    {
        yield return new WaitForSeconds(AttackDelay);
        CanAttack = true;
    }


    public void Jump()
    {
        if (!_Physics.GetIsGround()) return;
        if (BlockOverlapCheck()) return;
        _Physics.SetIsGround(false);
        _Animator.SetBool("IsGround", false);

        //SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Jump);
        //Debug.Log("Jump");
        _Physics.Velocity = Vector2.up * JumpPower;
    }

    public void Defend()
    {
        //Debug.Log("Defend");

        if (!ShieldPos.activeSelf)
            ShieldPos.SetActive(true);

        _Animator.SetTrigger("Defense");
    }
    public void BasicSkill() 
    {
        Debug.Log("BasicSkill");
    }

    public void SpacialSkill()
    {
        Debug.Log("SpacialSkill");
    }

    private bool BlockOverlapCheck()
    {
        Bounds bounds = GetComponent<BoxCollider2D>().bounds;

        // 콜라이더의 중심과 크기 설정
        Vector2 center = bounds.center;
        Vector2 size = bounds.size;

        // 겹치는 오브젝트들을 검사합니다.
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0, LayerMask.GetMask("Block"));

        if (hits.Length > 0) return true;
        return false;
    }
}
