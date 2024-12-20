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
    /// 블록 체력을 설정합니다.
    /// 레벨에 따라 피보나치 수열 값을 반환합니다.
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
    /// 블록 방어막 레벨을 설정하고 쉴드 체력을 설정합니다.
    /// </summary>
    /// <param name="level">0~9</param>
    public void SetBlockShieldLevel(int level)
    {
        shieldLevel = level;
        // 레벨이 0인 경우 쉴드가 없습니다.
        if (level == 0)
        {
            maxShield = 0;
            shieldType = 0;
            shieldStrength = 0;
            blockShield.SetActive(false);
            return;
        }

        // 쉴드 사용
        shieldType = (level - 1) / 3 + 1; // 1, 2, 3
        shieldStrength = (level - 1) % 3 + 1; // 1, 2, 3
        maxShield = FibonacciDataManager.Instance.GetFibonacciData(blockLevel-1) * (shieldLevel - 1);

        blockShield.GetComponent<SpriteRenderer>().sprite = StageManager.Instance.GetShieldSprite(shieldLevel - 1);
        blockShield.SetActive(true);
    }

    /// <summary>
    /// 캐릭터의 공격으로 인해 피해를 받습니다.
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(long damage)
    {
        // 스코어는 피해량을 기준으로 설정됩니다.
        float scorePercentage = 0;
        if (damage > GetBlockTotalCurrentHealth())
            scorePercentage = (float)((double)GetBlockTotalCurrentHealth() / GetBlockTotalMaxHealth());
        else
            scorePercentage = (float)((double)damage / GetBlockTotalMaxHealth());


        int baseScore = (blockLevel) * (blockLevel + 1) / 2;
        int score = (int)(baseScore * scorePercentage * 100);
        StageManager.Instance.ScoreUp(score);

        // 방어막에 따라 사운드를 달리합니다.
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

        // 쉴드의 체력을 먼저 깍은 후 본 체력을 깍습니다.
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
            // 방어막 이미지를 교체합니다.
            int strength = CalculateShieldStrength(curShield, maxShield, shieldStrength);
            int type = CalculateShieldType(strength);
            blockShield.GetComponent<SpriteRenderer>().sprite = StageManager.Instance.GetShieldSprite(type);
        }

        // 블록의 체력이 0 이하인 경우 파괴합니다.
        if (curHealth <= 0)
        {
            curHealth = 0;

            CustomPhysics parentPhysics = transform.parent.GetComponent<CustomPhysics>();
            parentPhysics.SetIsGround(false);

            gameObject.SetActive(false);
            // 이벤트 호출?
            StageManager.Instance.OnDestroyBlock();
            dynamicParticleManager.PlayParticle(transform.position, GetComponent<SpriteRenderer>().sprite.texture);

            return;
        }
        UIManager.Instance.SetBlockHealth(GetBlockTotalCurrentHealth(), GetBlockTotalMaxHealth());
        
    }

    /// <summary>
    /// 방어막 강도를 계산합니다.
    /// </summary>
    /// <param name="currentShield">현재 블록 방어막</param>
    /// <param name="maxShield">최대 방어막</param>
    /// <param name="shieldStrengthSegments">방어막의 종류 (나뭇잎, 얼음, 돌)</param>
    /// <returns></returns>
    int CalculateShieldStrength(float currentShield, float maxShield, int shieldStrengthSegments)
    {
        // 강도
        int strength = (int)(currentShield / (maxShield / shieldStrengthSegments));

        // 정확히 나눠떨어질 경우, 강도를 한 단계 낮춤
        if (currentShield % (maxShield / shieldStrengthSegments) == 0)
            strength -= 1;

        return Mathf.Max(0, strength); // 강도는 0 이상
    }

    /// <summary>
    /// 방어막 타입을 확인합니다.
    /// </summary>
    /// <param name="shieldStrength">방어막 종류 (나뭇잎, 얼음, 돌)</param>
    /// <returns></returns>
    int CalculateShieldType(int shieldStrength)
    {
        return 3 * (shieldType - 1) + shieldStrength;
    }

    /// <summary>
    /// 블록이 지면에 닿았는지 확인합니다.
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
            parentTransform.position = position;    // 겹친만큼 이동

            StageManager.Instance.CharacterLifeDecrease();

            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Crash);
        }
    }

    /// <summary>
    /// 블록이 캐릭터와 겹친경우 캐릭터를 밀어냅니다.
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
    /// 지면에서 벗어난 경우 블록의 시뮬레이션을 킵니다.
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
