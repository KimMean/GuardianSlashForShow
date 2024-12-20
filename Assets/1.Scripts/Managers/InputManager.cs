using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 사용되지 않습니다.
/// </summary>
public class InputManager : MonoBehaviour
{
    public delegate void InputJump();
    public delegate void InputDefense();
    public delegate void CancelDefense();
    public delegate void InputAttack();
    public delegate void InputBasicSkill();
    public delegate void InputSpecialSkill();

    public event InputJump OnJumpKeyPressed = null;
    public event InputDefense OnDefenseKeyPressed = null;
    public event CancelDefense OnDefenseKeyRelease = null;
    public event InputAttack OnAttackKeyPressed = null;
    public event InputBasicSkill OnBasicSkillKeyPressed = null;
    public event InputSpecialSkill OnSpacialSkillKeyPressed = null;

    private void Awake()
    {
        Debug.Log("Input Manager Awake");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            OnJumpKeyPressed?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            OnDefenseKeyPressed?.Invoke();
        }
        if(Input.GetKeyUp(KeyCode.X))
        {
            OnDefenseKeyRelease?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            OnAttackKeyPressed?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            OnBasicSkillKeyPressed?.Invoke();
        }
        if( Input.GetKeyDown(KeyCode.D))
        {
            OnSpacialSkillKeyPressed?.Invoke();
        }
    }
}
