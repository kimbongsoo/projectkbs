using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayerController : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float rotationSpeed = 180f;
    
    private bool isAttacking = false;
    private bool isCrouch = false;
    private float blendCrouch = 0f;
    private float blendRunning = 0f;
    private bool isRolling = false;
    public int stamina = 0;


    public Animator characterAnimator;

 

    private void Start()
    {
        SetCursorVisible(false);
    }

    public void SetCursorVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        
    }

    // // }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetCursorVisible(false);
        }

        bool isRunningInput = Input.GetKey(KeyCode.LeftShift);
        float targetBlendRunning = isRunningInput ? 1f : 0f;
        blendRunning = Mathf.Lerp(blendRunning, targetBlendRunning, Time.deltaTime * 10f);
        characterAnimator.SetFloat("Running", blendRunning);

        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouch = !isCrouch;
        }
        float targetBlendCrouch = isCrouch ? 1f : 0f;
        blendCrouch = Mathf.Lerp(blendCrouch, targetBlendCrouch, Time.deltaTime * 10f);
        characterAnimator.SetFloat("Crouch", blendCrouch);

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            int attackIndex = Random.Range(0, 3);
            // isRolling = !isRolling;
            isAttacking = true;
            characterAnimator.SetInteger("RandomAttack", attackIndex);
            characterAnimator.SetTrigger("Attack Trigger");
            // stamina++;
            Debug.Log("index: " + attackIndex);
            // if (stamina == 3)
            // {
            //     characterAnimator.SetInteger("Stamina", stamina);
            //     characterAnimator.SetTrigger("ComboAttack Trigger");
            //     stamina = 0;
            // }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            isRolling = true;
            characterAnimator.SetTrigger("Roll Trigger");
        }

        float horizontal = Input.GetAxis("Horizontal");     //Input.GetAxis ("Horizontal") : A,D Key 또는 Keyboard Left/Right Arrow Key
        float vertical = Input.GetAxis("Vertical");         //Input.GetAxis ("Vertical") : W, S Key 또는 Keyboard Up/Down Arrow Key

        Vector2 movementInput = new Vector2(horizontal, vertical);
        characterAnimator.SetFloat("Horizontal", movementInput.x);
        characterAnimator.SetFloat("Vertical", movementInput.y);
        characterAnimator.SetFloat("Magnitude", movementInput.magnitude);

        if(!isAttacking)
        {
            Vector3 movement = (transform.right * horizontal) + (transform.forward * vertical);
            this.transform.position += movement * moveSpeed *Time.deltaTime;  //회전을 할 때 곱연산(*)을 사용하는 이유는 Quaternion 같은 회전은 곱셈을 하면 회전이 더 해지는 형태이다

            float mouseX = Input.GetAxis("Mouse X");     //Input.GetAxis("Mouse X") : 마우스 X 축 움직임 입력 값
            float rotationY = mouseX * rotationSpeed * Time.deltaTime;      // rotation : 마우스 X 축 움직임 값 * 회전 속도 * Time.deltaTime
            this.transform.rotation *= Quaternion.Euler(0, rotationY, 0); //transform.rotation : 현재 회전값 * Qiaternion.Euler(0, rotation,0)
        }
        if(!isRolling)
        {
            Vector3 movement = (transform.right * horizontal) + (transform.forward * vertical);
            this.transform.position += movement * moveSpeed *Time.deltaTime;  //회전을 할 때 곱연산(*)을 사용하는 이유는 Quaternion 같은 회전은 곱셈을 하면 회전이 더 해지는 형태이다

            float mouseX = Input.GetAxis("Mouse X");     //Input.GetAxis("Mouse X") : 마우스 X 축 움직임 입력 값
            float rotationY = mouseX * rotationSpeed * Time.deltaTime;      // rotation : 마우스 X 축 움직임 값 * 회전 속도 * Time.deltaTime
            this.transform.rotation *= Quaternion.Euler(0, rotationY, 0); //transform.rotation : 현재 회전값 * Qiaternion.Euler(0, rotation,0)
        }
    }

    public void AttackComplete()
    {
        isAttacking = false;
    }
    public void ComboAttackComplete()
    {
        isAttacking = false;
        stamina = 0;
    }
    public void RollComplete()
    {
        isRolling = false;
    }


}
