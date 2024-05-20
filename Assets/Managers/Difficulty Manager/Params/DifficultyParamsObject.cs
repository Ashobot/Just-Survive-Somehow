using UnityEngine;
using UltimateAttributesPack;

[CreateAssetMenu(fileName = "Difficulty Params - Wave ", menuName = "Difficulty Params")]
public class DifficultyParamsObject : ScriptableObject
{
    [MinValue(0), Suffix("Seconds")] public float WaveDuration;
    public Trap[] Traps;
    [Space]
    public WaveTrapTypes WaveTrapTypes;
    [Space]
    public AnimationCurve TrapSpawnRateCurve;
    [MinValue(0), Suffix("Per second")] public float TrapSpawnRateMin;
    [MinValue(0), Suffix("Per second")] public float TrapSpawnRateMax;
}