using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    // ---------- bullet
    public float force = 1500.0f;

    public enum FireMode { SingleShot, Burst, Automatic }
    public FireMode fireMode = FireMode.SingleShot;  
    public float fireRate = 0.2f; 
    private float lastFireTime = 0f;
    private int burstCount = 0;
    private float burstDelay = 0.2f;
    private float burstCoolDown = 0.5f;

    public GameObject originalBullet;


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
            else if (fireMode == FireMode.Burst && Input.GetMouseButton(0) && Time.time >= burstCoolDown)
            {
                if (burstCount <3 && Time.time >= lastFireTime + fireRate)
                {
                    Fire();
                    burstCount++;
                    lastFireTime = Time.time;
                    // burstCount++;
                }
                else
                {
                    burstCount = 0;
                    burstCoolDown = Time.time + burstDelay;
                }
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
    public void Fire()
    {    
        GameObject bullet = Instantiate(originalBullet, transform.position + new Vector3(0, 1, 1), transform.rotation);
        bullet.SetActive(true);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null) 
        {
            rb.AddForce(transform.forward * force);
        }
    }

}
