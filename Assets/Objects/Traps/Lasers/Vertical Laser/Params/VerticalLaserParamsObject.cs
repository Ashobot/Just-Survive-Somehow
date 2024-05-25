using UnityEngine;
using UltimateAttributesPack;

[CreateAssetMenu(fileName = "Vertical Laser Params - ", menuName = "Traps Params/Lasers/Vertical Laser")]
public class VerticalLaserParamsObject : ScriptableObject
{
    [MinValue(0)] public float SpawnOffset;
    [MinValue(0)] public float WarningDuration;
    [MinValue(0)] public float LaserWidth;
    public AnimationCurve MovementCurve;
    [Space]
    [MinValue(0)] public float MovementTimeStart;
    [MinValue(0)] public float MovementTimeEnd;
}