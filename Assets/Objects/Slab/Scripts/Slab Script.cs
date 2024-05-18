using UnityEngine;

public class SlabScript : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void SetActivated(bool state, Color color, Material material)
    {
        _boxCollider.enabled = state;
        _spriteRenderer.color = color;
        _spriteRenderer.material = material;
    }
}