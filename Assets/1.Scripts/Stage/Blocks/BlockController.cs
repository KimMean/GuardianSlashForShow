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

    [SerializeField] GameObject BlockShield;

    int BlockLevel = 0;
    int ShieldLevel = 0;
    int ShieldType = 0;
    int ShieldStrength = 0;

    long MaxHealth = 0;
    long CurHealth = 0;
    
    long MaxShield = 0;
    long CurShield = 0;

    DynamicParticleManager dynamicParticleManager;

    private void Awake()
    {
        _Sprite = GetComponent<SpriteRenderer>();
        dynamicParticleManager = GameObject.Find("BlockDestroyParticle").GetComponent<DynamicParticleManager>();
    }

    private void OnEnable()
    {
        CurHealth = MaxHealth;
        CurShield = MaxShield;
    }


    public long GetBlockCurrentHealthPoint()
    {
        return CurHealth;
    }
    public long GetBlockMaxHealthPoint()
    {
        return MaxHealth;
    }

    public long GetBlockTotalMaxHealth()
    {
        return MaxHealth + MaxShield;
    }

    public long GetBlockTotalCurrentHealth()
    {
        return CurHealth + CurShield;
    }
    public void SetBlockMaxHealthPoint(long maxHealth)
    {
        MaxHealth = maxHealth;
    }
    public void SetBlockHealthLevel(int level)
    {
        BlockLevel = level;
        MaxHealth = FibonacciDataManager.Instance.GetFibonacciData(BlockLevel-1);
        _Sprite.sprite = StageManager.Instance.GetBlockSprite(BlockLevel);
        CurHealth = MaxHealth;
    }

    public void SetBlockShieldLevel(int level)
    {
        ShieldLevel = level;
        if (level == 0)
        {
            MaxShield = 0;
            ShieldType = 0;
            ShieldStrength = 0;
            BlockShield.SetActive(false);
            return;
        }

        // 쉴드 사용
        ShieldType = (level - 1) / 3 + 1; // 1, 2, 3
        ShieldStrength = (level - 1) % 3 + 1; // 1, 2, 3
        //MaxShield = FibonacciDataManager.Instance.GetFibonacciData(BlockLevel-1) * ShieldType * ShieldStrength;
        MaxShield = FibonacciDataManager.Instance.GetFibonacciData(BlockLevel-1) * (level - 1);

        //Debug.Log($"ShieldLevel : {ShieldLevel}, ShieldType : {ShieldType}, ShieldStrength : {ShieldStrength}");
        BlockShield.GetComponent<SpriteRenderer>().sprite = StageManager.Instance.GetShieldSprite(ShieldLevel - 1);
        BlockShield.SetActive(true);
        // health *= hard;
    }
    public void ApplyDamage(long damage)
    {
        //Debug.Log(transform.position);
        float scorePercentage = 0;
        if (damage > GetBlockTotalCurrentHealth())
            scorePercentage = (float)((double)GetBlockTotalCurrentHealth() / GetBlockTotalMaxHealth());
        else
            scorePercentage = (float)((double)damage / GetBlockTotalMaxHealth());

        //Debug.Log($"ScorePercent : {scorePercentage}");

        // 1~BlockLevel까지 더한 수
        int baseScore = (BlockLevel) * (BlockLevel + 1) / 2;
        int score = (int)(baseScore * scorePercentage * 100);
        //Debug.Log($"BaseScore : {baseScore}, Score : {score}");
        StageManager.Instance.ScoreUp(score);

        // 데미지 계산 전 블록의 hit사운드
        if(CurShield <= 0)
        {
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Block);
        }
        else
        {
            switch(ShieldType)
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

        CurShield -= damage;

        if(CurShield <= 0)
        {
            CurHealth += CurShield;
            CurShield = 0;
            BlockShield.SetActive(false);
            SoundManager.Instance.PlaySfx(SoundManager.SFX_Clip.Block);
        }
        else
        {
            // 0.9 / 1 = 0
            // 2 / 3 = 0.8
            // 3 / 3 = 1
            // 4 / 3 = 1.2
            // 7 / 3 = 2.2
            // 8.9 / 3 = 2.9
            // 방어막 강도 계산
            //int strength = (int)(CurShield / (MaxShield / ShieldStrength));

            // 정확히 나눠떨어질 경우, 강도를 한 단계 낮춤
            //if (CurShield % (float)(MaxShield / ShieldStrength) == 0) 
            //    strength -= 1;

            int strength = CalculateShieldStrength(CurShield, MaxShield, ShieldStrength);
            // 방어막 타입 계산
            //int type = 3 * (ShieldType - 1) + strength;
            int type = CalculateShieldType(strength);
            //Debug.Log($"CurShield : {CurShield}, (MaxShield / ShieldStrength) : {(MaxShield / ShieldStrength)}");
            //Debug.Log($"Strength : {strength}, Type : {type}, ShieldType : {ShieldType}");
            BlockShield.GetComponent<SpriteRenderer>().sprite = StageManager.Instance.GetShieldSprite(type);
        }

        if (CurHealth <= 0)
        {
            CurHealth = 0;

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

    // 방어막 강도 계산
    int CalculateShieldStrength(float currentShield, float maxShield, int shieldStrengthSegments)
    {
        // 강도
        int strength = (int)(currentShield / (maxShield / shieldStrengthSegments));

        // 정확히 나눠떨어질 경우, 강도를 한 단계 낮춤
        if (currentShield % (maxShield / shieldStrengthSegments) == 0)
            strength -= 1;

        return Mathf.Max(0, strength); // 강도는 0 이상
    }

    // 방어막 타입 계산
    int CalculateShieldType(int shieldStrength)
    {
        return 3 * (ShieldType - 1) + shieldStrength;
    }

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

            otherPhysics.Velocity = transform.parent.GetComponent<CustomPhysics>().Velocity;
        }
    }

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
