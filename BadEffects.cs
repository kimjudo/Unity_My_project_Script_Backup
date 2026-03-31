using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BadEffects : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Hunger hunger;
    [SerializeField] private Thirst thirst;
    [SerializeField] private Health health;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Sanity sanity;
    [SerializeField] private Volume volume;

    [Header("Rules")]
    [SerializeField] private float starveDamagePerSecond = 1f;
    [SerializeField] private float dehydrateDamagePerSecond = 1f;
    [SerializeField] private float zeroGraceSeconds = 8f;
    [SerializeField] private float runBlockThreshold = 10f;

    [Header("Panic Effect")]
    [SerializeField] private float vignetteBaseIntensity = 0.05f;
    [SerializeField] private float vignettePanicIntensity = 0.45f;
    [SerializeField] private float vignetteLerpSpeed = 8f;

    private bool isStarving;
    private bool isDehydrated;
    private bool isInPanic;

    private float starvingZeroTimer;
    private float dehydratedZeroTimer;

    private Vignette vignette;
    private float vignetteCurrentIntensity;

    private void Awake()
    {
        if (volume != null)
        {
            if (!volume.profile.TryGet(out vignette))
                Debug.LogWarning("Vignette override가 Volume Profile에 없음!");
        }

        vignetteCurrentIntensity = vignetteBaseIntensity;

        if (vignette != null)
            vignette.intensity.Override(vignetteCurrentIntensity);
    }

    private void OnEnable()
    {
        if (hunger != null)
        {
            hunger.OnHungerChanged += OnHungerChanged;
            OnHungerChanged(hunger.Current, hunger.Max);
        }

        if (thirst != null)
        {
            thirst.OnThirstChanged += OnThirstChanged;
            OnThirstChanged(thirst.Current, thirst.Max);
        }

        if (sanity != null)
        {
            sanity.OnSanityChanged += OnSanityChange;
            OnSanityChange(sanity.Current, sanity.Max);
        }

        RefreshRunAbility();
    }

    private void OnDisable()
    {
        if (hunger != null)
            hunger.OnHungerChanged -= OnHungerChanged;

        if (thirst != null)
            thirst.OnThirstChanged -= OnThirstChanged;

        if (sanity != null)
            sanity.OnSanityChanged -= OnSanityChange;
    }

    private void OnHungerChanged(float cur, float max)
    {
        isStarving = cur <= 0f;

        if (!isStarving)
            starvingZeroTimer = 0f;

        RefreshRunAbility();
    }

    private void OnThirstChanged(float cur, float max)
    {
        isDehydrated = cur <= 0f;

        if (!isDehydrated)
            dehydratedZeroTimer = 0f;

        RefreshRunAbility();
    }

    private void OnSanityChange(float cur, float max)
    {
        isInPanic = cur <= 0f;
    }

    private void RefreshRunAbility()
    {
        if (movement == null) return;

        bool hungerOk = (hunger == null) || (hunger.Current > runBlockThreshold);
        bool thirstOk = (thirst == null) || (thirst.Current > runBlockThreshold);

        movement.SetCanRun(hungerOk && thirstOk);
    }

    private void Update()
    {
        UpdateSurvivalDamage();
        UpdatePanicVignette();
    }

    private void UpdateSurvivalDamage()
    {
        if (health == null) return;

        float damageThisFrame = 0f;

        if (isStarving)
        {
            starvingZeroTimer += Time.deltaTime;

            if (starvingZeroTimer >= zeroGraceSeconds)
                damageThisFrame += starveDamagePerSecond * Time.deltaTime;
        }

        if (isDehydrated)
        {
            dehydratedZeroTimer += Time.deltaTime;

            if (dehydratedZeroTimer >= zeroGraceSeconds)
                damageThisFrame += dehydrateDamagePerSecond * Time.deltaTime;
        }

        if (damageThisFrame > 0f)
            health.ApplyRawDamage(damageThisFrame);
    }

    private void UpdatePanicVignette()
    {
        if (vignette == null) return;

        float target = isInPanic ? vignettePanicIntensity : vignetteBaseIntensity;

        vignetteCurrentIntensity = Mathf.Lerp(
            vignetteCurrentIntensity,
            target,
            Time.deltaTime * vignetteLerpSpeed
        );

        vignette.intensity.Override(vignetteCurrentIntensity);
    }
}