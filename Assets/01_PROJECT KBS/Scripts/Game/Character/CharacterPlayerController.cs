using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KBS
{
        public class CharacterPlayerController : MonoBehaviour
    {
        private CharacterBase characterBase;

        [Header("Camera Setting")]
        public Transform cameraPivot;
        public float bottomClampLimit = -80f;
        public float topClampLimit = 80f;
        private float threshold = 0.01f;
        private float targetYaw = 0f;
        private float targetPitch = 0f;

        [Header("Corsshair Setting")]
        public float crosshairSpreadSpeed = 0.1f;
        public float crosshairRecoverySpeed = 0.2f;
        public float crosshairSpreadMax = 1f;
        public float crosshairSpreadMin = 0.1f;
        private float crosshairCurrentSpread = 0f;





        public void SetCursorVisible(bool isVisible)
        {
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
            
        }
        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
        }

        private void OnEnable()
        {
            characterBase.onFireEvent += OnFired;
            characterBase.onReloadCompleteEvent += OnReloadCompleted;
            characterBase.OnchangedHP += OnChangedHP;
            characterBase.OnChangedSP += OnChangedSP;
        }
        private void OnReloadCompleted(int current, int max)
        {
            MainHUD.Instance.SetAmmoText(current, max);
        }

        private void OnChangedHP(float current, float max)
        {
            MainHUD.Instance.SetHP(current, max);
}
        private void OnChangedSP(float current, float max)
        {
            MainHUD.Instance.SetSP(current, max);
        }
        private void OnFired(int current, int max)
        {
            MainHUD.Instance.SetAmmoText(current, max);

            crosshairCurrentSpread = Mathf.Clamp(crosshairCurrentSpread + crosshairSpreadSpeed, crosshairSpreadMin, crosshairSpreadMax);
            CrossHairUI.Instance.SetCrosshairSpread(crosshairCurrentSpread / crosshairSpreadMax);
        }
        private void Start()
        {
            SetCursorVisible(false);
            OnFired(characterBase.primaryWeapon.RemainAmmo, characterBase.primaryWeapon.MaxAmmo);

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CameraSystem.Instance.SetChangeCameraSide();
            }
            if (Input.GetMouseButtonDown(0))
            {
                SetCursorVisible(false);
            }

            bool isInputRunning = Input.GetKey(KeyCode.LeftShift);
            characterBase.IsRunning = isInputRunning;

            if (Input.GetKeyDown(KeyCode.C))
            {
                characterBase.IsCrouch = !characterBase.IsCrouch;
            }
            
            bool isAimingInput = Input.GetMouseButton(1); // 우클릭?
            characterBase.IsAiming = isAimingInput;

            if (Input.GetKeyDown(KeyCode.R))
            {
                characterBase.Reload();
            }

            if (Input.GetMouseButton(0))
            {
                characterBase.Fire();
            }

            if (Input.GetKeyDown(KeyCode.V))
                {
                    Debug.Log("v 입력");
                    characterBase.Combat();
                }

            float inputX = Input.GetAxis("Horizontal");
            float inputy = Input.GetAxis("Vertical");
            characterBase.Move(new Vector2(inputX, inputy), Camera.main.transform.eulerAngles.y);
            characterBase.Rotate(CameraSystem.Instance.AimingPoint);

            characterBase.AimingPoint = CameraSystem.Instance.AimingPoint;

            //크로스헤어 줄어들기
            crosshairCurrentSpread = Mathf.Clamp(
                crosshairCurrentSpread - (crosshairRecoverySpeed * Time.deltaTime)
                , crosshairSpreadMin
                , crosshairSpreadMax);

            CrossHairUI.Instance.SetCrosshairSpread(crosshairCurrentSpread / crosshairSpreadMax);
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

    }

}
