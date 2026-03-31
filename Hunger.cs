using System;
using UnityEngine;

public class Hunger : StatBase
{
    [SerializeField] private float hungerAmount = 0.3f;
    [SerializeField] private float tickInterval = 10f;

    private float tickTimer = 0f;

    public event Action<float, float> OnHungerChanged;

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
        OnHungerChanged?.Invoke(current, max);
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer < tickInterval) return;

        Change(-hungerAmount);
        tickTimer = 0f;
    }

    public void Eat(float amount)
    {
        if (amount <= 0f) return;
        Change(amount);
    }
}