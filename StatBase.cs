using UnityEngine;
using System;
public abstract class StatBase : MonoBehaviour
{
    [SerializeField] protected float max = 100f;
    [SerializeField] protected float current;

    public float Max => max;
    public float Current => current;

    public event Action<float, float> OnStatChanged;

    protected virtual void Awake()
    {
        max = Mathf.Max(0f, max);
        current = Mathf.Clamp(current, 0f, max);
    }

    protected void SetCurrent(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, max);

        if (Mathf.Approximately(current, clamped))
            return;

        current = clamped;
        Notify();
    }

    protected void Change(float delta)
    {
        SetCurrent(current + delta);
    }

    protected void Notify()
    {
        OnStatChanged?.Invoke(current, max);
    }
}
