using System;
using UnityEngine;

public class Sanity : StatBase
{
    [SerializeField] private float sanityDecreasePerSecond = 1f;
    [SerializeField] private float sanityDecreaseWhenCaught = 10f;
    [SerializeField] private bool isInSafeZone = true;
    [SerializeField] private float damageSanityCooldown = 3f;
    [SerializeField] private float sanityRegenPerSecondInSafeZone = 2f;
    [SerializeField] private Health health;

    private float lastDamageSanityTime = -999f;
    public float DecreaseWhenCaught => sanityDecreaseWhenCaught;

    public event Action<float, float> OnSanityChanged;
    protected override void Awake()
    {
        base.Awake();
        OnStatChanged += HandleStatChanged;
    }
    private void OnDestroy()
    {
        OnStatChanged -= HandleStatChanged;
    }
    private void HandleStatChanged(float current, float max)
    {
        OnSanityChanged?.Invoke(current, max);
    }
    private void Update()
    {
        if (!isInSafeZone)
        {
            DecreaseSanity(sanityDecreasePerSecond * Time.deltaTime);
        }
        else
        {
            HealSanity(sanityRegenPerSecondInSafeZone * Time.deltaTime);
        }
    }
    void OnEnable()
    {
        if (health == null) health = GetComponent<Health>();
        if (health != null) health.onDamaged += HandleDamaged;
    }

    void OnDisable()
    {
        if (health != null) health.onDamaged -= HandleDamaged;
    }
    public void DecreaseSanity(float amount)
    {
        if (amount <= 0f) return;
        Change(-amount);
    }
    public void HandleDamaged(float amount)
    {
        if (Time.time < lastDamageSanityTime + damageSanityCooldown) return;
        lastDamageSanityTime = Time.time;
        DecreaseSanity(amount);
    }
    public void HealSanity(float amount)
    {
        if (amount <= 0f) return;

        Change(amount);
    }
    private void Panicked()
    {
        //여기 터널링 효과 + 화면 흔들림 + 환청
    }
}
