using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayerController : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float rotationSpeed = 180f;
    public GameObject originalBall;

    private void Start()
    {
        SetCursorVisible(false);
    }

    public void SetCursorVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            Destroy(collision.gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetCursorVisible(false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //TODO : 복제 생성
            GameObject newCopyVall = Instantiate(originalBall, transform.position, Quaternion.identity);
            newCopyVall.SetActive(true);
        }

        float horizontal = Input.GetAxis("Horizontal");     //Input.GetAxis ("Horizontal") : A,D Key 또는 Keyboard Left/Right Arrow Key
        float vertical = Input.GetAxis("Vertical");         //Input.GetAxis ("Vertical") : W, S Key 또는 Keyboard Up/Down Arrow Key
        Vector3 movement = (transform.right * horizontal) + (transform.forward * vertical);
        this.transform.position += movement * moveSpeed *Time.deltaTime;  //회전을 할 때 곱연산(*)을 사용하는 이유는 Quaternion 같은 회전은 곱셈을 하면 회전이 더 해지는 형태이다

        float mouseX = Input.GetAxis("Mouse X");     //Input.GetAxis("Mouse X") : 마우스 X 축 움직임 입력 값
        float rotationY = mouseX * rotationSpeed * Time.deltaTime;      // rotation : 마우스 X 축 움직임 값 * 회전 속도 * Time.deltaTime
        this.transform.rotation *= Quaternion.Euler(0, rotationY, 0); //transform.rotation : 현재 회전값 * Qiaternion.Euler(0, rotation,0)

    
    }
}
