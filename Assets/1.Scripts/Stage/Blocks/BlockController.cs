using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BlockController;
using static InputManager;

public class BlockController : MonoBehaviour
{
    public delegate void DestroyBlock();
    public event DestroyBlock OnDestroyBlock = null;

    SpriteRenderer _Sprite;

    [SerializeField] GameObject blockShield;

    int blockLevel = 0;
    int shieldLevel = 0;
    int shieldType = 0;
    int shieldStrength = 0;

    long maxHealth = 0;
    long curHealth = 0;
    
    long maxShield = 0;
    long curShield = 0;

    DynamicParticleManager dynamicParticleManager;

    private void Awake()
    {
        _Sprite = GetComponent<SpriteRenderer>();
        dynamicParticleManager = GameObject.Find("BlockDestroyParticle").GetComponent<DynamicParticleManager>();
    }

    private void OnEnable()
    {
        curHealth = maxHealth;
        curShield = maxShield;
    }


    public long GetBlockCurrentHealthPoint()
    {
        return curHealth;
    }
    public long GetBlockMaxHealthPoint()
    {
        return maxHealth;
    }

    public long GetBlockTotalMaxHealth()
    {
        return maxHealth + maxShield;
    }

    public long GetBlockTotalCurrentHealth()
    {
        return curHealth + curShield;
    }
    public void SetBlockMaxHealthPoint(long health)
    {
        maxHealth = health;
    }
    /// <summary>
    /// ��� ü���� �����մϴ�.
    /// ������ ���� �Ǻ���ġ ���� ���� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="level">1~80</param>
    public void SetBlockHealthLevel(int level)
    {
        blockLevel = level;
        maxHealth = FibonacciDataManager.Instance.GetFibonacciData(blockLevel-1);
        _Sprite.sprite = StageManager.Instance.GetBlockSprite(blockLevel);
        curHealth = maxHealth;
    }

    /// <summary>
    /// ��� �� ������ �����ϰ� ���� ü���� �����մϴ�.
    /// </summary>
    /// <param name="level">0~9</param>
    public void SetBlockShieldLevel(int level)
    {
        shieldLevel = level;
        // ������ 0�� ��� ���尡 �����ϴ�.
        if (level == 0)
        {
            maxShield = 0;
            shieldType = 0;
            shieldStrength = 0;
            blockShield.SetActive(false);
            return;
        }

        // ���� ���
        shieldType = (level - 1) / 3 + 1; // 1, 2, 3
        shieldStrength = (level - 1) % 3 + 1; // 1, 2, 3
        maxShield = FibonacciDataManager.Instance.GetFibonacciData(blockLevel-1) * (shieldLevel - 1);

        blockShield.GetComponent<SpriteRenderer>().sprite = StageManager.Instance.GetShieldSprite(shieldLevel - 1);
        blockShield.SetActive(true);
    }

    /// <summary>
    /// ĳ������ �������� ���� ���ظ� �޽��ϴ�.
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(long damage)
    {
        // ���ھ�� ���ط��� �������� �����˴ϴ�.
        float scorePercentage = 0;
        if (damage > GetBlockTotalCurrentHealth())
            scorePercentage = (float)((double)GetBlockTotalCurrentHealth() / GetBlockTotalMaxHealth());
        else
            scorePercentage = (float)((double)damage / GetBlockTotalMaxHealth());


        int baseScore = (blockLevel) * (blockLevel + 1) / 2;
        int score = (int)(baseScore * scorePercentage * 100);
        StageManager.Instance.ScoreUp(score);

        // ���� ���� ���带 �޸��մϴ�.
        if(curShield <= 0)
        {
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Block);
        }
        else
        {
            switch(shieldType)
            {
                case 1:
                    SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Leaf);
                    break;
                case 2:
                    SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Ice);
                    break;
                case 3:
                    SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Rock);
                    break;
            }
        }

        // ������ ü���� ���� ���� �� �� ü���� ����ϴ�.
        curShield -= damage;
        if(curShield <= 0)
        {
            curHealth += curShield;
            curShield = 0;
            blockShield.SetActive(false);
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Block);
        }
        else
        {
            // �� �̹����� ��ü�մϴ�.
            int strength = CalculateShieldStrength(curShield, maxShield, shieldStrength);
            int type = CalculateShieldType(strength);
            blockShield.GetComponent<SpriteRenderer>().sprite = StageManager.Instance.GetShieldSprite(type);
        }

        // ����� ü���� 0 ������ ��� �ı��մϴ�.
        if (curHealth <= 0)
        {
            curHealth = 0;

            CustomPhysics parentPhysics = transform.parent.GetComponent<CustomPhysics>();
            parentPhysics.SetIsGround(false);

            gameObject.SetActive(false);
            // �̺�Ʈ ȣ��?
            StageManager.Instance.OnDestroyBlock();
            dynamicParticleManager.PlayParticle(transform.position, GetComponent<SpriteRenderer>().sprite.texture);

            return;
        }
        UIManager.Instance.SetBlockHealth(GetBlockTotalCurrentHealth(), GetBlockTotalMaxHealth());
        
    }

    /// <summary>
    /// �� ������ ����մϴ�.
    /// </summary>
    /// <param name="currentShield">���� ��� ��</param>
    /// <param name="maxShield">�ִ� ��</param>
    /// <param name="shieldStrengthSegments">���� ���� (������, ����, ��)</param>
    /// <returns></returns>
    int CalculateShieldStrength(float currentShield, float maxShield, int shieldStrengthSegments)
    {
        // ����
        int strength = (int)(currentShield / (maxShield / shieldStrengthSegments));

        // ��Ȯ�� ���������� ���, ������ �� �ܰ� ����
        if (currentShield % (maxShield / shieldStrengthSegments) == 0)
            strength -= 1;

        return Mathf.Max(0, strength); // ������ 0 �̻�
    }

    /// <summary>
    /// �� Ÿ���� Ȯ���մϴ�.
    /// </summary>
    /// <param name="shieldStrength">�� ���� (������, ����, ��)</param>
    /// <returns></returns>
    int CalculateShieldType(int shieldStrength)
    {
        return 3 * (shieldType - 1) + shieldStrength;
    }

    /// <summary>
    /// ����� ���鿡 ��Ҵ��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("GroundTile"))
        {
            CustomPhysics parentPhysics = transform.parent.GetComponent<CustomPhysics>();
            if (parentPhysics.GetIsGround()) return;
            parentPhysics.SetIsGround(true);

            float overlapY = Calculator.GetOverlapValue(GetComponent<Collider2D>().bounds, collision.bounds).y;

            Transform parentTransform = transform.parent;
            Vector2 position = parentTransform.position;
            position.y += overlapY;
            parentTransform.position = position;    // ��ģ��ŭ �̵�

            StageManager.Instance.CharacterLifeDecrease();

            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Crash);
        }
    }

    /// <summary>
    /// ����� ĳ���Ϳ� ��ģ��� ĳ���͸� �о���ϴ�.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            CustomPhysics otherPhysics = collision.GetComponent<CustomPhysics>();
            if (otherPhysics.GetIsGround())
                return;

            float overlapY = Calculator.GetOverlapValue(GetComponent<Collider2D>().bounds, collision.bounds).y;
            Vector2 position = collision.transform.position;
            position.y -= overlapY;
            collision.transform.position = position;

            otherPhysics.velocity = transform.parent.GetComponent<CustomPhysics>().velocity;
        }
    }

    /// <summary>
    /// ���鿡�� ��� ��� ����� �ùķ��̼��� ŵ�ϴ�.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("GroundTile"))
        {
            if (!gameObject.activeSelf) return;

            CustomPhysics parentPhysics = transform.parent.GetComponent<CustomPhysics>();

            if (!parentPhysics.GetIsGround()) return;
            parentPhysics.SetIsGround(false);
        }
    }
}
