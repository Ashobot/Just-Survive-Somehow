using UnityEngine;
using UltimateAttributesPack;

[CreateAssetMenu(fileName = "Rocket Params - ", menuName = "Traps Params/Followers/Rocket")]
public class RocketParamsObject : ScriptableObject
{
    [MinValue(0)] public float SpawnOffset;
    [Space]
    [MinValue(0)] public float MovementSpeedStart;
    [MinValue(0)] public float MovementSpeedEnd;
    [Space]
    [MinValue(0)] public float RotationSpeedStart;
    [MinValue(0)] public float RotationSpeedEnd;
}