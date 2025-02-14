using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayerController : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 2f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 3f;
        }

        bool isPressed_W_Key = Input.GetKey(KeyCode.W);
        bool isPressed_A_Key = Input.GetKey(KeyCode.A);
        bool isPressed_S_Key = Input.GetKey(KeyCode.S);
        bool isPressed_D_Key = Input.GetKey(KeyCode.D);

        Vector3 movement = Vector3.zero;
        if (isPressed_W_Key)
        {
            movement.z += 1;
        }
        if (isPressed_A_Key)
        {
            movement.x -= 1;
        }
        if (isPressed_S_Key)
        {
            movement.z -= 1;
        }
        if (isPressed_D_Key)
        {
            movement.x += 1;
        }

        this.transform.position += movement * moveSpeed *Time.deltaTime;
    }
}
