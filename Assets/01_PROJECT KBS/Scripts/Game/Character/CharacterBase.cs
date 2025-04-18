using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KBS
{
    public class CharacterBase : MonoBehaviour
    {

        public bool IsRunning {get => isRunning; set => isRunning = value;}
        private bool isRunning = false;

        public bool IsCrouch {get => isCrouch; set => isCrouch = value;}
        private bool isCrouch = false;

        public bool IsAiming {get => isAiming; set => isAiming = value;}

        public Vector3 AimingPoint {get => aimingTargetPoint.position; set => aimingTargetPoint.position = value;}

        public bool IsReloading {get; private set;} = false;
        private bool isAiming = false;

        public bool IsCombat {get; private set;} = false;
        private bool isCombat = false;

        [Header("Fire Setting")]
        public Transform fireStartPoint;

        public float fireRate = 0.2f; 
        public float lastFireTime = 0f;

        public int clipSize = 30;

        [Header("Character Setting")]

        public float moveSpeed = 3.0f;
        public float noneStrafeRotationSpeed = 1f;
        public float strafeRotationSpeed = 180f;
        private float blendCrouch = 0f;
        private float blendRunning = 0f;

        public GameObject originalBullet;

        [Header("IK Setting")]
        public Transform aimingTargetPoint;
        public TwoBoneIKConstraint leftHandIk;
        public Rig aimingRig;
        public Animator characterAnimator;
        private float targetRotation;
        private float targetHorizontal;
        private float targetVertical;

        public System.Action<int, int> onFireEvent;
        //추가
        public System.Action<int, int> onReloadCompleteEvent;

        private void Awake()
        {
            characterAnimator = GetComponent<Animator>();
        }
        private void Update()
        {
            float targetBlendRunning = isRunning ? 1f : 0f;
            blendRunning = Mathf.Lerp(blendRunning, targetBlendRunning, Time.deltaTime * 10f);
            characterAnimator.SetFloat("Running", blendRunning);

            float targetBlendCrouch = isCrouch ? 1f : 0f;
            blendCrouch = Mathf.Lerp(blendCrouch, targetBlendCrouch, Time.deltaTime * 10f);
            characterAnimator.SetFloat("Crouch", blendCrouch);

            characterAnimator.SetFloat("Aiming", isAiming ? 1f : 0f);
            characterAnimator.SetFloat("Horizontal", targetHorizontal);
            characterAnimator.SetFloat("Vertical", targetVertical);

            aimingRig.weight = isAiming ? 1f : 0f;
            leftHandIk.weight = IsReloading ? 0f : 1f;

        }

        public void Rotate(Vector3 targetPoint)
        {
            if(IsCombat)
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
            if(IsCombat)
                return; 
            characterAnimator.SetFloat("Magnitude", input.magnitude);
            Vector3 movement = Vector3.zero;
            if(input.magnitude > 0f)
            {
                if(!isAiming)
                {
                    Vector3 inputDirection = new Vector3(input.x, 0f, input.y);
                    targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + yAxisAngle;
                    transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
                }

                if(isAiming)
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

            transform.position += movement;
        }
        public void Fire()
        {   
            if(IsCombat)
                return; 
            if (isAiming && clipSize > 0 && Time.time >= lastFireTime + fireRate)
            {            
                GameObject bullet = Instantiate(originalBullet, fireStartPoint.position, fireStartPoint.rotation);
                bullet.SetActive(true);
                clipSize--;

                if(EffectManager.Instance.GetEffect("Muzzle", out GameObject muzzleEffect))
                {
                    muzzleEffect.transform.position = fireStartPoint.position;
                    muzzleEffect.transform.rotation = fireStartPoint.rotation;
                    Destroy(muzzleEffect.gameObject, 1f);
                }

                lastFireTime = Time.time;

                onFireEvent?.Invoke(clipSize, 30);
            }
        }

        public void Reload()
        {
            if(IsReloading || IsCombat)
                return;
            IsReloading = true;
            characterAnimator.SetTrigger("Reload Trigger");
            leftHandIk.weight = 0f;
        }

        public void ReloadComplete()
        {
            IsReloading = false;
            clipSize = 30;
            // onFireEvent?.Invoke(clipSize, 30);
            //추가
            onReloadCompleteEvent.Invoke(clipSize, 30);
        }

        public void Combat()
        {
            if(IsCombat || IsReloading)
                return;
            IsCombat = true;
            characterAnimator.SetTrigger("Combat Trigger");
        }

        public void CombatComplete()
        {
            IsCombat = false;
        }

    }
}