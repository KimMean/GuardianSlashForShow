using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    [SerializeField] bool Simulated = true;

    [SerializeField] float Mass = 1.0f;
    [SerializeField] float GravityScale = 1.0f;
    [SerializeField] public Vector2 Velocity = Vector2.zero;

    [SerializeField] private bool IsGround = false;
    float DecelerationFactor = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Simulated) return;
        if (IsGround) return;

        // 중력 적용
        Velocity.y += EnvironmentManager.Gravity * GravityScale * Time.deltaTime;

        // 위치 업데이트
        transform.position += (Vector3)(Velocity * Time.deltaTime);
    }

    public void AddForce(Vector2 force)
    {
        Vector2 velocity = force / Mass;
        Velocity += velocity;
    }

    public void ReduceVelocity()
    {
        if (Velocity.y > 0) return;
        
        //Debug.Log("속도 감소 !");
        Velocity.y *= DecelerationFactor;
    }

    public void SetIsGround(bool isGround)
    {
        IsGround = isGround;
        if (IsGround)
            Velocity = Vector2.zero;
    }
    public bool GetIsGround()
    {
        return IsGround;
    }

    public void SetSimulated(bool simulated)
    {
        Simulated = simulated;
    }

    public void SetVelocity(Vector2 velocity)
    {
        Velocity = velocity;
    }

}
