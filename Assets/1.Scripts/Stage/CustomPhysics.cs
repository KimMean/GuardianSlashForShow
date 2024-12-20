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
    /// �ùķ��̼��� Ȱ��ȭ �� ��� �߷��� ������ �޽��ϴ�.
    /// </summary>
    void Update()
    {
        if (!simulated) return;
        if (isGround) return;

        // �߷� ����
        velocity.y += EnvironmentManager.Gravity * gravityScale * Time.deltaTime;

        // ��ġ ������Ʈ
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    /// <summary>
    /// �߷��� ũ�⸦ �����մϴ�.
    /// </summary>
    /// <param name="scale">�߷��� ����</param>
    public void SetGravityScale(float scale)
    {
        gravityScale = scale;
    }

    /// <summary>
    /// ������ �������� ���� ���մϴ�.
    /// </summary>
    /// <param name="force">Vector2 direction</param>
    public void AddForce(Vector2 force)
    {
        Vector2 velocity = force / mass;
        velocity += velocity;
    }

    /// <summary>
    /// �ӵ��� ��� ���ҽ�ŵ�ϴ�.
    /// �������� ������ �޽��ϴ�.
    /// </summary>
    public void ReduceVelocity()
    {
        if (velocity.y > 0) return;
        
        //Debug.Log("�ӵ� ���� !");
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
