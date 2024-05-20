using UnityEngine;
using UltimateAttributesPack;

[CreateAssetMenu(fileName = "Bear Trap Params - ", menuName = "Traps Params/Falling/Bear Trap")]
public class BearTrapParamsObject : ScriptableObject
{
    [MinValue(0)] public float SpawnYOffset;
    [MinValue(0)] public float SpawnRadius;
    public AnimationCurve FallCurve;
    [Space]
    [MinValue(0)] public float FallDurationStart;
    [MinValue(0)] public float FallDurationEnd;
    [Space]
    [MinValue(0)] public float DeployDurationStart;
    [MinValue(0)] public float DeployDurationEnd;
    [Space]
    [MinValue(0)] public float DestroyDurationStart;
    [MinValue(0)] public float DestroyDurationEnd;
}