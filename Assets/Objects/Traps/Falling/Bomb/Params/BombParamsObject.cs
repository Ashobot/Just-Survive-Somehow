using UnityEngine;
using UltimateAttributesPack;

[CreateAssetMenu(fileName = "Bomb Params - ", menuName = "Traps Params/Falling/Bomb")]
public class BombParamsObject : ScriptableObject
{
    [MinValue(0)] public float SpawnYOffset;
    [MinValue(0)] public float SpawnRadius;
    [MinValue(0)] public float ExplosionRange;
    public AnimationCurve FallCurve;
    [Space]
    [MinValue(0)] public float FallDurationStart;
    [MinValue(0)] public float FallDurationEnd;
    [Space]
    [MinValue(0)] public float ExplodeDurationStart;
    [MinValue(0)] public float ExplodeDurationEnd;
    [Space]
    [MinValue(0)] public float ExplosionDurationStart;
    [MinValue(0)] public float ExplosionDurationEnd;
}