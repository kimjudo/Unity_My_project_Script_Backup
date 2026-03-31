using System;
using UnityEngine;

public class Thirst : StatBase
{
    [SerializeField] private float drainAmount = 0.3f;   // 감소량
    [SerializeField] private float tickInterval = 10f;   // 몇 초마다 감소할지

    public event Action<float, float> OnThirstChanged;

    private float tickTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        OnStatChanged += HandleThirstChanged;
    }

    private void Start()
    {
        HandleThirstChanged(Current, Max); // 초기 동기화
    }

    private void OnDestroy()
    {
        OnStatChanged -= HandleThirstChanged;
    }

    private void HandleThirstChanged(float current, float max)
    {
        OnThirstChanged?.Invoke(current, max);
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer < tickInterval) return;

        Change(-drainAmount);
        tickTimer = 0f;
    }

    public void Drink(float amount)
    {
        if (amount <= 0f) return;
        Change(amount);
    }
}