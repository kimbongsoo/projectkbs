using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayerController : MonoBehaviour
{
    public Animator characterAnimator;

    public float moveSpeed = 3.0f;
    public float noneStrafeRotationSpeed = 1f;
    public float strafeRotationSpeed = 180f;

    private bool isCrouch = false;
    private float blendCrouch = 0f;
    private float blendRunning = 0f;


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


        float horizontal = Input.GetAxis("Horizontal");     //Input.GetAxis ("Horizontal") : A,D Key 또는 Keyboard Left/Right Arrow Key
        float vertical = Input.GetAxis("Vertical");         //Input.GetAxis ("Vertical") : W, S Key 또는 Keyboard Up/Down Arrow Key
        Vector2 movementInput = new Vector2(horizontal, vertical);

        bool isAmingInput = Input.GetMouseButton(1); // 우클릭?
        characterAnimator.SetFloat("Aiming", isAmingInput ? 1f : 0f);


        characterAnimator.SetFloat("Horizontal", movementInput.x);
        characterAnimator.SetFloat("Vertical", movementInput.y);
        characterAnimator.SetFloat("Magnitude", movementInput.magnitude);


        if(isAmingInput)
        {
            Vector3 movement = (transform.right * horizontal) + (transform.forward * vertical);
            this.transform.position += movement * moveSpeed *Time.deltaTime;  //회전을 할 때 곱연산(*)을 사용하는 이유는 Quaternion 같은 회전은 곱셈을 하면 회전이 더 해지는 형태이다
            float mouseX = Input.GetAxis("Mouse X");     //Input.GetAxis("Mouse X") : 마우스 X 축 움직임 입력 값
            float rotationY = mouseX * strafeRotationSpeed * Time.deltaTime;      // rotation : 마우스 X 축 움직임 값 * 회전 속도 * Time.deltaTime
            this.transform.rotation *= Quaternion.Euler(0, rotationY, 0); //transform.rotation : 현재 회전값 * Qiaternion.Euler(0, rotation,0)
        }
        else
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 movement = forward * movementInput.y + right * movementInput.x;
            transform.position += movement * moveSpeed * Time.deltaTime;

            if(movementInput.magnitude > 0f)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, noneStrafeRotationSpeed * Time.deltaTime);
            }
        }

    }

}
