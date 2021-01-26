﻿using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offsetPosition = new Vector3(0, 2, -1);

    private SpriteRenderer darkEffect;
    public float darkEffectSpeed = 0.01f;
    private bool isLocked = false;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        darkEffect = gameObject.GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(LightsOff());
    }

    private void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Missing Target ref!", this);
            return;
        }
        
        if (!isLocked) transform.position = target.position + offsetPosition;
    }

    public void LockCamera() { isLocked = true; }

    public IEnumerator LightsOff() 
    {
        while (darkEffect.color.a < 1)
        {
            darkEffect.color = new Color(darkEffect.color.r, darkEffect.color.g,
                darkEffect.color.b, darkEffect.color.a + 0.01f);
            yield return new WaitForSeconds(darkEffectSpeed);
        }
        StartCoroutine(LightsOn());
    }

    public IEnumerator LightsOn()
    {
        while (darkEffect.color.a > 0)
        {
            darkEffect.color = new Color(darkEffect.color.r, darkEffect.color.g, 
                darkEffect.color.b, darkEffect.color.a - 0.01f);
            yield return new WaitForSeconds(darkEffectSpeed);
        }
    }
}