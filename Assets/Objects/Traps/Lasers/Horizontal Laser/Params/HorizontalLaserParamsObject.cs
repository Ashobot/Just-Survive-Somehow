using UnityEngine;
using UltimateAttributesPack;

[CreateAssetMenu(fileName = "Horizontal Laser Params - ", menuName = "Traps Params/Lasers/Horizontal Laser")]
public class HorizontalLaserParamsObject : ScriptableObject
{
    [MinValue(0)] public float SpawnOffset;
    [MinValue(0)] public float LaserWidth;
    public AnimationCurve MovementCurve;
    [Space]
    [MinValue(0)] public float MovementTimeStart;
    [MinValue(0)] public float MovementTimeEnd;
}