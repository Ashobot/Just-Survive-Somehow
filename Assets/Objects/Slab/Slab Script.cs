using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateAttributesPack;

public class SlabScript : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void SetActivated(bool state, Color color)
    {
        _boxCollider.enabled = state;
        _spriteRenderer.color = color;
    }
    



}
