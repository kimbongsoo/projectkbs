using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterPlayerController : MonoBehaviour
{
    [Header("Character Setting")]
    public Animator characterAnimator;

    public float moveSpeed = 3.0f;
    public float noneStrafeRotationSpeed = 1f;
    public float strafeRotationSpeed = 180f;

    private bool isCrouch = false;
    private float blendCrouch = 0f;
    private float blendRunning = 0f;

    [Header("Fire Setting")]
    public Transform fireStartPoint;
    public enum FireMode { SingleShot, Burst, Automatic }
    public FireMode fireMode = FireMode.SingleShot;  
    public float fireRate = 0.2f; 
    private float lastFireTime = 0f;

    public GameObject originalBullet;
    public GameObject originalMuzzle;

    [Header("Camera Setting")]
    public Transform cameraPivot;
    public float bottomClampLimit = -80f;
    public float topClampLimit = 80f;
    private float threshold = 0.01f;
    private float targetYaw = 0f;
    private float targetPitch = 0f;

    [Header("IK Setting")]
    public Rig aimingRig;
    public Transform aimingTargetPoint;

    private void Start()
    {
        SetCursorVisible(false);
    }

    public void SetCursorVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        
    }

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

        bool isAimingInput = Input.GetMouseButton(1); // 우클릭?
        characterAnimator.SetFloat("Aiming", isAimingInput ? 1f : 0f);

        if (Input.GetKeyDown(KeyCode.R))
        {
            characterAnimator.SetTrigger("Reload Trigger");
        }
    
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            fireMode = FireMode.SingleShot;
            Debug.Log("Fire Mode: Single Shot");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            fireMode = FireMode.Burst; 
            Debug.Log("Fire Mode: Burst");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            fireMode = FireMode.Automatic; 
            Debug.Log("Fire Mode: Automatic");
        }

        if (isAimingInput)
        {
            if (fireMode == FireMode.SingleShot && Input.GetMouseButtonDown(0))
            {
                Fire();
            }
            else if (fireMode == FireMode.Automatic && Input.GetMouseButton(0) && Time.time >= lastFireTime + fireRate)
            {
                Fire();
                lastFireTime = Time.time;
            }
        }


        characterAnimator.SetFloat("Horizontal", movementInput.x);
        characterAnimator.SetFloat("Vertical", movementInput.y);
        characterAnimator.SetFloat("Magnitude", movementInput.magnitude);


        if(isAimingInput)
        {
            Vector3 movement = (transform.right * horizontal) + (transform.forward * vertical);
            this.transform.position += movement * moveSpeed *Time.deltaTime;  //회전을 할 때 곱연산(*)을 사용하는 이유는 Quaternion 같은 회전은 곱셈을 하면 회전이 더 해지는 형태이다
            float mouseX = Input.GetAxis("Mouse X");     //Input.GetAxis("Mouse X") : 마우스 X 축 움직임 입력 값
            float rotationY = mouseX * strafeRotationSpeed * Time.deltaTime;      // rotation : 마우스 X 축 움직임 값 * 회전 속도 * Time.deltaTime
            this.transform.rotation *= Quaternion.Euler(0, rotationY, 0); //transform.rotation : 현재 회전값 * Qiaternion.Euler(0, rotation,0)

            Vector3 aimingPoint = CameraSystem.Instance.AimingPoint;

            aimingRig.weight = 1f;
            aimingTargetPoint.position = aimingPoint;
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

            aimingRig.weight = 0f;

            if(movementInput.magnitude > 0f)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, noneStrafeRotationSpeed * Time.deltaTime);
            }
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    public void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector2 look = new Vector2(mouseX, mouseY);

        if (look.sqrMagnitude > threshold)
        {
            float yaw = look.x;
            float pitch = -look.y;

            targetYaw = ClampAngle(targetYaw + yaw, float.MinValue, float.MaxValue);
            targetPitch = ClampAngle(targetPitch + pitch, bottomClampLimit, topClampLimit);
        }

        targetPitch = ClampAngle(targetPitch, bottomClampLimit, topClampLimit);
        cameraPivot.rotation = Quaternion.Euler(targetPitch, targetYaw, 0f);


    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
        {
            angle += 360f;
        }
        if (angle > 360f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }
    public void Fire()
    {    
        GameObject bullet = Instantiate(originalBullet, fireStartPoint.position, fireStartPoint.rotation);
        bullet.SetActive(true);

        GameObject muzzle = Instantiate(originalMuzzle, fireStartPoint.position, fireStartPoint.rotation);
        muzzle.SetActive(true);
        Destroy(muzzle.gameObject, 1f);

        // if(EffectManager.Instance.GetEffect("Muzzle"))
    }

}
