using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue - ", menuName = "Dialogue")]
public class DialogueObject : ScriptableObject
{
    [TextArea] public string[] Lines;
}