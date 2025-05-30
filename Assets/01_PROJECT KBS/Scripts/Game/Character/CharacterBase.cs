using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KBS
{
    public class CharacterBase : MonoBehaviour
    {
        public float CurrentHP => currentHP;
        public float CurrentSP => currentSP;
        public float MaxHP => maxHP;
        public float MaxSP => maxSP;

        public bool IsRunning { get => isRunning; set => isRunning = value; }
        private bool isRunning = false;

        public bool IsCrouch { get => isCrouch; set => isCrouch = value; }
        private bool isCrouch = false;

        public bool IsAiming { get => isAiming; set => isAiming = value; }

        public Vector3 AimingPoint { get => aimingTargetPoint.position; set => aimingTargetPoint.position = value; }

        public bool IsReloading { get; private set; } = false;
        private bool isAiming = false;

        public bool IsCombat { get; private set; } = false;
        private bool isCombat = false;

        public bool IsArmed { get; private set; } = false;
        private bool isArmed = false;

        [Header("Character Stat")]
        public float maxHP = 1000f;
        public float maxSP = 100f;
        private float currentHP = 1000f;
        private float currentSP = 100f;

        [Header("Weapon Setting")]
        public WeaponBase primaryWeapon;

        [Header("Character Setting")]
        public float moveSpeed = 3.0f;
        public float noneStrafeRotationSpeed = 1f;
        public float strafeRotationSpeed = 180f;
        private float blendCrouch = 0f;
        private float blendRunning = 0f;

        [Header("IK Setting")]
        public Transform aimingTargetPoint;
        public TwoBoneIKConstraint leftHandIk;
        public Rig aimingRig;
        private float targetRotation;
        private float targetHorizontal;
        private float targetVertical;

        [Header("Weapon Holster")]
        public Transform weaponHolsterPlace;
        public Transform weaponEquipPlace;

        [Header("Components")]
        public Animator characterAnimator;
        public UnityEngine.CharacterController unityCharacterController;

        [Header("GroundCheck")]
        public float groundCheckRadius = 0.05f;
        public float groundOffset = 0.1f;
        public LayerMask groundLayer;
        public bool isGrounded;

        [Header("Gravity")]
        public float verticalVelocity;
        public float terminalVelocity = 50f;
        public float gravity = -15f;

        public System.Action<int, int> onFireEvent;
        public System.Action<int, int> onReloadCompleteEvent;
        public System.Action<float, float> OnchangedHP; //체력이 바뀔 떄 호출되는 Event
        public System.Action<float, float> OnChangedSP; //스태미너가 바뀔 때 호출되는 Event


        private void Awake()
        {
            characterAnimator = GetComponent<Animator>();
            unityCharacterController = GetComponent<UnityEngine.CharacterController>();
        }

        private void Start()
        {
            currentHP = maxHP;
            currentSP = maxSP;

            OnchangedHP?.Invoke(currentHP, maxHP);
            OnChangedSP?.Invoke(currentSP, maxSP);
        }

        private void Update()
        {
            JumpAndGravity();
            CheckGround();

            if (IsRunning && currentSP > 0f)
            {
                currentSP -= Time.deltaTime;
                OnChangedSP?.Invoke(currentSP, maxSP);
            }
            else
            {
                currentSP += Time.deltaTime;
                OnChangedSP?.Invoke(currentSP, maxSP);
            }
            currentSP = Mathf.Clamp(currentSP, 0f, maxSP);
            if (!IsRunning && CurrentHP < maxHP)
            {
                currentHP += Time.deltaTime;
                OnchangedHP?.Invoke(currentHP, maxHP);
            }
            currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

            float targetBlendRunning = isRunning && currentSP > 0 ? 1f : 0f;
            blendRunning = Mathf.Lerp(blendRunning, targetBlendRunning, Time.deltaTime * 10f);
            characterAnimator.SetFloat("Running", blendRunning);

            float targetBlendCrouch = isCrouch ? 1f : 0f;
            blendCrouch = Mathf.Lerp(blendCrouch, targetBlendCrouch, Time.deltaTime * 10f);
            characterAnimator.SetFloat("Crouch", blendCrouch);

            characterAnimator.SetFloat("Aiming", isAiming ? 1f : 0f);
            characterAnimator.SetFloat("Horizontal", targetHorizontal);
            characterAnimator.SetFloat("Vertical", targetVertical);

            aimingRig.weight = IsArmed && isAiming ? 1f : 0f;
            leftHandIk.weight = IsArmed && IsReloading ? 0f : 1f;

        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Blue Cube"))
            {
                // currentHP -= 100f;
                // currentHP -= 1000f;
                currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
                OnchangedHP?.Invoke(currentHP, maxHP);
                Destroy(collider.gameObject);

                if (currentHP <= 0f)
                {
                    Main.Singleton.ChangeScene(SceneType.Title);
                }
            }

            if (collider.gameObject.CompareTag("Red Cube"))
            {
                if (currentHP < maxHP)
                {
                    currentHP += 100f;
                    currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
                    OnchangedHP?.Invoke(currentHP, maxHP);
                }
                Destroy(collider.gameObject);

            }

        }

        public void Rotate(Vector3 targetPoint)
        {
            if (IsCombat)
                return;
            if (isAiming)
            {
                Vector3 target = targetPoint;
                target.y = transform.position.y;
                Vector3 viewForward = Camera.main.transform.forward;
                viewForward.y = 0f;
                transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, viewForward, Time.deltaTime * 10f));
            }
        }

        public void Move(Vector2 input, float yAxisAngle)
        {
            if (IsCombat)
                return;
            characterAnimator.SetFloat("Magnitude", input.magnitude);
            Vector3 movement = Vector3.zero;
            if (input.magnitude > 0f)
            {
                if (!isAiming)
                {
                    Vector3 inputDirection = new Vector3(input.x, 0f, input.y);
                    targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + yAxisAngle;
                    transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
                }

                if (isAiming)
                {
                    targetHorizontal = input.x;
                    targetVertical = input.y;
                    movement = (transform.forward * input.y + transform.right * input.x) * moveSpeed * Time.deltaTime;
                }
                else
                {
                    targetVertical = 1f;
                    movement = transform.forward * moveSpeed * Time.deltaTime;
                }
            }
            else
            {
                targetHorizontal = 0f;
                targetVertical = 0f;
            }

            // transform.position += movement;
            unityCharacterController.Move(movement + new Vector3(0, verticalVelocity, 0));
        }
        public void Fire()
        {
            if (IsCombat || IsReloading || !IsArmed)
                return;

            if (isAiming)
            {
                if (primaryWeapon.Shoot(out int remain, out int max))
                {
                    onFireEvent?.Invoke(remain, max);
                }
                else
                {
                    if (primaryWeapon.IsEmpty())
                    {
                        Reload();
                    }
                }
            }
        }

        public void Reload()
        {
            if (IsReloading || IsCombat || !IsArmed)
                return;
            IsReloading = true;
            characterAnimator.SetTrigger("Reload Trigger");
            leftHandIk.weight = 0f;
        }

        public void ReloadComplete()
        {
            IsReloading = false;
            int fullAmmo = primaryWeapon.SetFullAmmo();
            onReloadCompleteEvent?.Invoke(fullAmmo, fullAmmo);
        }

        public void Combat()
        {
            if (IsCombat || IsReloading || IsArmed)
                return;
            IsCombat = true;
            characterAnimator.SetTrigger("Combat Trigger");
        }

        public void CombatComplete()
        {
            IsCombat = false;
        }

        public void EquipWeapon()
        {
            characterAnimator.SetTrigger("Equip Trigger");
            IsArmed = true;
        }

        public void HolsterWeapon()
        {
            characterAnimator.SetTrigger("Holster Trigger");
            IsArmed = false;
        }

        public void OnWeaponToEquipPlace()
        {
            primaryWeapon.transform.SetParent(weaponEquipPlace);
            primaryWeapon.transform.localPosition = Vector3.zero;
            primaryWeapon.transform.localRotation = Quaternion.Euler(0, -90f, 0);
        }

        public void OnWeaponToHolsterPlace()
        {
            primaryWeapon.transform.SetParent(weaponHolsterPlace);
            primaryWeapon.transform.localPosition = Vector3.zero;
            primaryWeapon.transform.localRotation = Quaternion.identity;
        }


        public void CheckGround()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
            isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
            characterAnimator.SetBool("IsGrounded", isGrounded);
        }

        public void JumpAndGravity()
        {
            if (isGrounded)
            {
                verticalVelocity = -2f;
            }
            else //점프관련 코드
            {
                // verticalVelocity = Mathf.Sqrt(2f * gravity);
                verticalVelocity = (-9.81f * Time.deltaTime);
            }

            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += (gravity * Time.deltaTime);
            }

        }
    }
}