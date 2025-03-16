using UnityEngine;

public class RatioFactor
{
    public int CurrentValue { get; private set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }

    public RatioFactor(int currentValue, int minValue, int maxValue)
    {
        CurrentValue = currentValue;
        MaxValue = minValue;
        MaxValue = maxValue;
    }

    public int AddValue(int value)
    {
        CurrentValue += value;
        int increase = value;
        if (CurrentValue < MinValue)
        {
            increase = value - (MinValue - CurrentValue);
            CurrentValue = MinValue;
        }
        else if (CurrentValue > MaxValue)
        {
            increase = value - (CurrentValue - MaxValue);
            CurrentValue = MaxValue;
        }
        return increase;
    }

    public void SetValue(int value)
    {
        CurrentValue = value;
        CurrentValue = Mathf.Clamp(CurrentValue, MinValue, MaxValue);
    }
}