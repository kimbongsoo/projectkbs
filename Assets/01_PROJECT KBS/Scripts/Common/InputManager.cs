using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class InputManager : SingletonBase<InputManager>
    {
        public bool InputFire => isLeftMouseButton;
        private bool isLeftMouseButton = false;
        public bool InputAim => isRightMouseButton;
        private bool isRightMouseButton = false;
        public bool InputSprint => isLeftShift;
        private bool isLeftShift = false;
        public Vector2 InputMove => move;
        private Vector2 move =Vector2.zero;
        public Vector2 InputLook => look;
        private Vector2 look = Vector2.zero;
        public event System.Action OnTab;
        public event System.Action OnCrouch;
        public event System.Action OnReload;
        public event System.Action OnCombat;


        private void Start()
        {
            SetCursorVisible(false);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                SetCursorVisible(false);
            }
        }

        private void Update()
        {
            bool isForceCursorVisible = Input.GetKey(KeyCode.LeftAlt);
            SetCursorVisible(isForceCursorVisible);
            
            move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            isLeftMouseButton = Input.GetMouseButton(0);
            isLeftShift = Input.GetKey(KeyCode.LeftShift);
            isRightMouseButton = Input.GetMouseButton(1);

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OnTab?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                OnCrouch?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                OnReload?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                OnCombat?.Invoke();
            }
        }
        
        public void SetCursorVisible(bool isVisible)
        {
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
            
        }
    }
}
