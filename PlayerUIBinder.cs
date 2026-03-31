using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerUIBinder : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private BarUI healthBar;
    [SerializeField] private ArmorSystem armorSystem;
    [SerializeField] private Thirst thirst;
    [SerializeField] private BarUI thirstBar;
    [SerializeField] private Hunger hunger;
    [SerializeField] private BarUI hungerBar;
    [SerializeField] private Sanity sanity;
    [SerializeField] private BarUI sanityBar;
    [SerializeField] private Addiction addiction;
    [SerializeField] private BarUI addictionBar;

    private StateHudManager Hud => UIManager.Instance ? UIManager.Instance.StateHud : null;

    private void Awake()
    {
        if (!health) health = GetComponent<Health>();
        if (!armorSystem) armorSystem = GetComponent<ArmorSystem>();
        if (!thirst) thirst = GetComponent<Thirst>();
        if(!hunger) hunger = GetComponent<Hunger>();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.onDeath += OnDeath;
            health.onHealthChanged += OnHealthChanged;
            health.onDamaged += OnDamaged;
            OnHealthChanged(health.CurrentHealth, health.MaxHealth);
        }
        if (thirst != null)
        {
            thirst.OnThirstChanged += OnThirstChanged;
            OnThirstChanged(thirst.Current, thirst.Max);
        }
        if(hunger != null)
        {
            hunger.OnHungerChanged += OnHungerChanged;
            OnHungerChanged(hunger.Current, hunger.Max);
        }
        if(sanity != null)
        {
            sanity.OnSanityChanged += OnSanityChanged;
            OnSanityChanged(sanity.Current, sanity.Max);
        }
        if(addiction != null)
        {
            addiction.OnAddictionChanged += OnAddiction;
            OnAddiction(addiction.Current, addiction.Max);
        }
        Hud?.SetArmor(armorSystem != null ? armorSystem.protectionAmount : 0);
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.onDeath -= OnDeath;
            health.onHealthChanged -= OnHealthChanged;
            health.onDamaged -= OnDamaged;
        }
        if (thirst != null)
            thirst.OnThirstChanged -= OnThirstChanged;
        if (hunger != null)
            hunger.OnHungerChanged -= OnHungerChanged;
        if(sanity != null)
            sanity.OnSanityChanged -= OnSanityChanged;
        if(addiction != null)
            addiction.OnAddictionChanged -= OnAddiction;
    }

    private void OnHealthChanged(float cur, float max)
    {
        if (healthBar != null)
            healthBar.SetNormalized(max <= 0 ? 0f : cur / max);
    }
    private void OnThirstChanged(float cur, float max)
    {
        if (thirstBar != null)
            thirstBar.SetNormalized(max <= 0 ? 0f : cur / max);
    }
    private void OnHungerChanged(float cur, float max)
    {
        if(hungerBar != null)
            hungerBar.SetNormalized(max <= 0 ? 0f : cur / max);
    }
    private void OnSanityChanged(float cur, float max)
    {
        if (sanityBar != null)
        {
            sanityBar.SetNormalized(max <= 0 ? 0f : cur / max);
        }
    }
    private void OnAddiction(float cur, float max)
    {
        if(addictionBar != null)
        {
            addictionBar.SetNormalized(max <= 0 ? 0f : cur / max);
        }
    }
    private void OnDeath()
    {
        Debug.Log("Player Died (UI Binder)");
    }
    private void OnDamaged(float damage)
    {
        Debug.Log($"Player took {damage} damage (UI Binder)");
    }

    public void RefreshArmorUI()
    {
        Hud?.SetArmor(armorSystem != null ? armorSystem.protectionAmount : 0);
    }
}
