using UnityEngine;

public class TrapTypeScript : MonoBehaviour
{
    [SerializeField] TrapType _trapType;
    public TrapType TrapType { get { return _trapType; } }
}
