using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    ParticleManager attackParticle;
    ParticleManager jumpParticle;
    ParticleManager slashParticleLR;
    ParticleManager slashParticleRL;
    CustomPhysics _Physics;
    Animator _Animator;

    private float jumpPower = 10.0f;
    float extraJumpPower = 0;

    [SerializeField] private GameObject attackPos;
    private float attackDelay = 0.1f;
    //[SerializeField] private float AttackPower = 0.1f;

    [SerializeField] private GameObject shieldPos;

    [SerializeField] SpriteRenderer Weapon_SpriteRenderer;

    Vector2 groundPosition;

    private bool isAttacking = false;
    bool isAttackR = false;
    private bool isDefending = false;


    private void Awake()
    {
        _Physics = GetComponent<CustomPhysics>();
        _Animator = GetComponent<Animator>();

        string weaponCode = GameManager.Instance.GetEquipmentWeapon();
        Weapon_SpriteRenderer.sprite = WeaponManager.Instance.GetWeaponData(weaponCode).GetWeaponSprite();

        GameObject _atkParticle = GameObject.Find("AttackParticle");
        if (_atkParticle != null)
        {
            attackParticle = _atkParticle.GetComponent<ParticleManager>();
        }
        GameObject _jumpParticle = GameObject.Find("JumpParticle");
        if (_jumpParticle != null)
        {
            jumpParticle = _jumpParticle.GetComponent<ParticleManager>();
        }
        GameObject slashParticleA = GameObject.Find("SlashLtoR");
        if (slashParticleA != null)
        {
            slashParticleLR = slashParticleA.GetComponent<ParticleManager>();
        }
        GameObject slashParticleB = GameObject.Find("SlashRtoL");
        if (slashParticleB != null)
        {
            slashParticleRL = slashParticleB.GetComponent<ParticleManager>();
        }

        string ringCode = GameManager.Instance.GetEquipmentRing();
        if (ringCode == "R000")
            extraJumpPower = 0;
        else
        {
            int ratio = RingManager.Instance.GetRingData(ringCode).GetJump();
            extraJumpPower = jumpPower * (ratio / 100f);
        }
        //Debug.Log($"JumpPower : {JumpPower}, ExtraJump : {ExtraJumpPower}");
        jumpPower += extraJumpPower;
    }

    /// <summary>
    /// ���� �� ���鿡 �����ߴ��� �˻��մϴ�.
    /// </summary>
    /// <param name="collision"></param>
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
                groundPosition = position;    // ��ģ��ŭ �̵�
                transform.position = groundPosition;
                Debug.Log(groundPosition);
            }
            else
            {
                transform.position = groundPosition;
            }

            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Landing);
            jumpParticle.ParticleActivation(groundPosition);
        }

    }

    /// <summary>
    /// OnTriggerEnter2D�� ���� ����� ������
    /// ����� ������ �� ������ ���������� �ϸ� Enter����� �� ������ ���Ͽ� �߰��߽��ϴ�.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("GroundTile"))
        {
            if (_Physics.GetIsGround()) return;
            _Physics.SetIsGround(true);
            _Animator.SetBool("IsGround", true);

            transform.position = groundPosition;
        }
    }

    /// <summary>
    /// ĳ���� ���� ���θ� Ȯ���մϴ�.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("GroundTile"))
        {
            _Physics.SetIsGround(false);
            //if (!_Physics.GetIsGround()) return;
            //_Physics.SetIsGround(false);
            //_Animator.SetBool("IsGround", false);

            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Jump);
            jumpParticle.ParticleActivation(groundPosition);
        }
    }

    /// <summary>
    /// ĳ���� ����
    /// </summary>
    public void Attack()
    {
        if (isAttacking) return;
        if (isDefending) return;

        _Animator.SetTrigger("Attack");
        isAttacking = true;
        //StartCoroutine(AttackDelayCoroutine());
        //Vector2 pos = this.transform.position;
        //pos.y += 10.0f;
        attackPos.SetActive(true);
        attackParticle.ParticleActivation(attackPos.transform.position);

        if (isAttackR)
        {
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.AttackA);
            slashParticleRL.ParticleActivation(GetComponent<Collider2D>().bounds.center);
        }
        else
        {
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.AttackB);
            slashParticleLR.ParticleActivation(GetComponent<Collider2D>().bounds.center);
        }

        isAttackR = !isAttackR;
        //Debug.Log("Attack");
    }

    /// <summary>
    /// ���� �ִϸ��̼��� ������� ������ �̺�Ʈ�� ȣ��˴ϴ�.
    /// </summary>
    public void EndAttack()
    {
        isAttacking = false;
    }

    /// <summary>
    /// ĳ���� ����
    /// </summary>
    public void Jump()
    {
        if (!_Physics.GetIsGround()) return;
        if (BlockOverlapCheck()) return;
        _Physics.SetIsGround(false);
        _Animator.SetBool("IsGround", false);

        //SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Jump);
        //Debug.Log("Jump");
        _Physics.velocity = Vector2.up * jumpPower;
    }

    /// <summary>
    /// ĳ���� ���
    /// </summary>
    public void Defend()
    {
        if (isAttacking) return;
        if (isDefending) return;
        
        isDefending = true;
        //Debug.Log("Defend");

        //if (!shieldPos.activeSelf)
        shieldPos.SetActive(true);

        _Animator.SetTrigger("Defense");
    }

    /// <summary>
    /// ��� �ִϸ��̼� ������ �����ӿ��� ȣ��˴ϴ�.
    /// </summary>
    public void EndDefend()
    {
        isDefending = false;
    }
    
    /// <summary>
    /// ����� �ٷ� �����ִ� ��츦 �˻��մϴ�.
    /// </summary>
    /// <returns></returns>
    private bool BlockOverlapCheck()
    {
        Bounds bounds = GetComponent<BoxCollider2D>().bounds;

        // �ݶ��̴��� �߽ɰ� ũ�� ����
        Vector2 center = bounds.center;
        Vector2 size = bounds.size;

        // ��ġ�� ������Ʈ���� �˻��մϴ�.
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0, LayerMask.GetMask("Block"));

        if (hits.Length > 0) return true;
        return false;
    }
}
