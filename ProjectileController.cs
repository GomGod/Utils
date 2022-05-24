﻿using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Fly Ease Curve - 0~1f, progress")] [SerializeField]
    private AnimationCurve flyEaseCurve;

    [Header("Y Axis Offset - 1:1 = position : evaluated Value")] [SerializeField]
    private AnimationCurve yAxisOffsetCurve;

    [Header("RotateCurve(Z Axis rotation) - 1f = 360")] [SerializeField]
    private AnimationCurve rotateCurve;

    [SerializeField] private bool isSeperatedDirAndRot;

    [SerializeField] private GameObject bulletImageObject;
    [SerializeField] private GameObject destroyParticleObject;
    [SerializeField] private float flyTime; //투사체가 목표에 도달할 때 까지 필요한 시간

    public delegate void ProjectileEvent(ProjectileController bCont);

    public ProjectileEvent onDestroyBullet; //투사체가 목표 지점에 도달했을 때
    public ProjectileEvent onUpdate; //투사체가 이동할 때 마다 (per frame)

    private Vector3 initPosition;
    private Transform targetTransform;
    private Vector3 destPos;

    /// <summary>
    /// 투사체를 타게팅으로 초기화
    /// </summary>
    /// <param name="initPosition"></param>
    public void InitializeTargetBullet(Vector3 initPosition, Transform targetTf)
    {
        this.initPosition = initPosition;
        transform.position = initPosition;
        targetTransform = targetTf;

        InitializeBulletMovement(flyTime, true);
    }

    /// <summary>
    /// 투사체를 논타겟으로 초기화
    /// </summary>
    /// <param name="initPosition">시작 지점</param>
    /// <param name="destPosition">목표 지점</param>
    public void InitializeNonTargetBullet(Vector3 initPosition, Vector3 destPosition)
    {
        this.initPosition = initPosition;
        transform.position = initPosition;
        destPos = destPosition;
        InitializeBulletMovement(flyTime, false);
    }

    private void InitializeBulletMovement(float flyTime, bool isTargeting)
    {
        if (isTargeting)
        {
            StartCoroutine(UpdateTargetMove(targetTransform, flyTime));
        }

        if (destroyParticleObject)
            destroyParticleObject.SetActive(false);
        bulletImageObject.SetActive(true);

        StartCoroutine(UpdateXAxisMove(flyTime));
        StartCoroutine(UpdateYAxisOffset(flyTime));
        StartCoroutine(UpdateRotate(flyTime));
    }

    private IEnumerator UpdateRotate(float duration)
    {
        var timer = 0.0f;
        var thisTransform = transform;
        while (timer <= duration)
        {
            if (isSeperatedDirAndRot)
            {
                //투사체의 진행방향과 회전이 독립인 경우, 커브의 제어를 받음
                var rotationValue = rotateCurve.Evaluate(timer / duration) * 360f;
                thisTransform.rotation = Quaternion.Euler(0f, 0f, rotationValue);
            }
            else
            {
                var rotationValue = Quaternion.LookRotation(destPos - thisTransform.position);
                thisTransform.rotation = rotationValue;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator UpdateXAxisMove(float duration)
    {
        var timer = 0.0f;
        while (timer <= duration)
        {
            var evaluatedValue = flyEaseCurve.Evaluate(timer / duration);
            var newPosition = Vector3.Lerp(initPosition, destPos, evaluatedValue);

            transform.position = newPosition;
            onUpdate?.Invoke(this);
            timer += Time.deltaTime;
            yield return null;
        }

        DestroyBullet();
    }

    private IEnumerator UpdateYAxisOffset(float duration)
    {
        var timer = 0.0f;
        var thisTransform = transform;

        while (timer <= duration)
        {
            var referenceAxisVector = destPos - initPosition;
            var yAxisVector = Vector3.Cross(new Vector3(0, 0, 1), referenceAxisVector).normalized; //y축 벡터
            var evaluatedYOffset = yAxisOffsetCurve.Evaluate(timer / duration);
            var moveOffset = yAxisVector * evaluatedYOffset;

            thisTransform.localPosition += moveOffset;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator UpdateTargetMove(Transform destTransform, float duration)
    {
        var timer = 0.0f;
        while (timer <= duration)
        {
            destPos = destTransform.position;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void DestroyBullet()
    {
        onDestroyBullet?.Invoke(this);
        bulletImageObject.SetActive(false);

        if (destroyParticleObject)
            destroyParticleObject.SetActive(true);

        StartCoroutine(DelayedActiveFalse(1.5f));
    }

    private IEnumerator DelayedActiveFalse(float delay)
    {
        var timer = 0.0f;
        while (timer < delay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}

