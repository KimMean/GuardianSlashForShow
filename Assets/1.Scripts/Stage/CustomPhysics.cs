using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    [SerializeField] bool simulated = true;

    [SerializeField] float mass = 1.0f;
    [SerializeField] float gravityScale = 1.0f;
    [SerializeField] public Vector2 velocity = Vector2.zero;

    [SerializeField] private bool isGround = false;
    float decelerationFactor = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// 시뮬레이션이 활성화 된 경우 중력의 영향을 받습니다.
    /// </summary>
    void Update()
    {
        if (!simulated) return;
        if (isGround) return;

        // 중력 적용
        velocity.y += EnvironmentManager.Gravity * gravityScale * Time.deltaTime;

        // 위치 업데이트
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    /// <summary>
    /// 중력의 크기를 조절합니다.
    /// </summary>
    /// <param name="scale">중력의 배율</param>
    public void SetGravityScale(float scale)
    {
        gravityScale = scale;
    }

    /// <summary>
    /// 지정한 방향으로 힘을 가합니다.
    /// </summary>
    /// <param name="force">Vector2 direction</param>
    public void AddForce(Vector2 force)
    {
        Vector2 velocity = force / mass;
        velocity += velocity;
    }

    /// <summary>
    /// 속도를 즉시 감소시킵니다.
    /// 아이템의 영향을 받습니다.
    /// </summary>
    public void ReduceVelocity()
    {
        if (velocity.y > 0) return;
        
        //Debug.Log("속도 감소 !");
        velocity.y *= decelerationFactor;
    }

    public void SetIsGround(bool ground)
    {
        isGround = ground;
        if (isGround)
            velocity = Vector2.zero;
    }
    public bool GetIsGround()
    {
        return isGround;
    }

    public void SetSimulated(bool isSimulated)
    {
        simulated = isSimulated;
    }

    public void SetVelocity(Vector2 value)
    {
        velocity = value;
    }

}
