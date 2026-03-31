using System;
using UnityEngine;

public class Stamina : StatBase
{
    [Header("Stamina")]
    [SerializeField] private float regenPerSec = 15f;
    [SerializeField] private float regenDelay = 0.5f; // 회복 대기 시간
    [SerializeField] private BarUI staminaUI;

    public float MaxStamina => Max; // 기존 코드 호환용

    public event Action<float, float> OnStaminaChanged;

    private float regenTimer;

    protected override void Awake()
    {
        base.Awake();
        OnStatChanged += HandleStatChanged;
    }

    private void Start()
    {
        HandleStatChanged(Current, Max); // 초기 동기화
    }

    private void OnDestroy()
    {
        OnStatChanged -= HandleStatChanged;
    }

    private void HandleStatChanged(float current, float max)
    {
        OnStaminaChanged?.Invoke(current, max);

        if (max > 0f)
            staminaUI?.SetNormalized(current / max);
        else
            staminaUI?.SetNormalized(0f);
    }

    private void Update()
    {
        if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        if (Current < Max)
        {
            Change(regenPerSec * Time.deltaTime);
        }
    }

    public bool CanSpend(float amount) => amount > 0f && Current >= amount;

    public bool Spend(float amount)
    {
        if (!CanSpend(amount)) return false;

        Change(-amount);
        regenTimer = regenDelay;
        return true;
    }
}