using System;
using UnityEngine;

public class Health : StatBase, IDamageable
{
    [SerializeField] private StatusEffects statusEffects;
    [SerializeField] private ArmorSystem armorSystem;

    public float MaxHealth => Max; // StatBase를 상속을 받아도 기기존 코드 호환과 편의성
    public float CurrentHealth => Current;
    public bool IsDead { get; private set; }

    public event Action onDeath;
    public event Action<float, float> onHealthChanged;
    public event Action<float> onDamaged;

    protected override void Awake()
    {
        base.Awake();

        if (statusEffects == null) statusEffects = GetComponent<StatusEffects>();
        if (armorSystem == null) armorSystem = GetComponent<ArmorSystem>();

        OnStatChanged += HandleStatChanged;

        // 초기 UI 동기화용
        HandleStatChanged(Current, Max);
    }
        private void OnDestroy()
    {
        OnStatChanged -= HandleStatChanged;
    }
    private void HandleStatChanged(float current, float max)
    {
        onHealthChanged?.Invoke(current, max);

        if (!IsDead && current <= 0f)
            Die();
    }
    public void TakeDamage(int damage, StatusEffects.DamageType damageType)
    {
        if (IsDead || damage <= 0) return;

        float finalDamage = armorSystem != null
            ? armorSystem.CalculateDamage(damage)
            : damage;

        ApplyRawDamage(finalDamage);

        if (statusEffects != null)
            statusEffects.HandleStatusEffect(damageType);

        onDamaged?.Invoke(finalDamage);
    }
    public void ApplyRawDamage(float damage)
    {
        if (IsDead || damage <= 0f) return;
        Change(-damage);
    }
    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;
        Change(amount);
    }
    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log($"{gameObject.name} is Dead");
        onDeath?.Invoke();
    }
}

