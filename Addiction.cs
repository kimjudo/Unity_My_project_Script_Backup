using System;
using UnityEngine;

public class Addiction : StatBase
{
    [SerializeField] private Health health;
    public event Action<float, float> OnAddictionChanged;

    protected override void Awake()
    {
        base.Awake();
        OnStatChanged += HandleStatChanged;
        HandleStatChanged(Current, Max); // 초기 동기화
    }
    private void OnDestroy()
    {
        OnStatChanged -= HandleStatChanged;
    }
    private void HandleStatChanged(float current, float max)
    {
        OnAddictionChanged?.Invoke(current, max);
    }
    public void IncreaseAddiction(float amount)
    {
        if (amount <= 0f) return;
        Change(amount);
    }

    public void DecreaseAddiction(float amount)
    {
        if (amount <= 0f) return;
        Change(-amount);
    }
}
