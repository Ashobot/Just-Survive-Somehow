using UnityEngine;
using System.Collections.Generic;
using UltimateAttributesPack;

public class PlayerTriggers : MonoBehaviour
{
    PlayerController _playerController;
    [SerializeField] UIManager _uiManager;

    [Title("Death", "dark red")]
    [SerializeField, Tag] string _deathZoneTag;
    [SerializeField] float _invincibleTime;
    [SerializeField] float _riskDeathTime;

    List<Collider2D> _overlapedTraps = new List<Collider2D>();
    float _invincibleTimer;
    bool _isInvincible;
    float _riskDeathTimer;
    bool _isRiskingDeath;
    bool _isDead;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(_isInvincible)
            InvincibleCooldown();
        else
        {
            if(_isRiskingDeath)
                RiskingDeathCooldown();
            
            DetectIfDamage();
        }        
    }

    void DetectIfDamage()
    {
        if (!_playerController.PlayerMovement.IsDashing && _overlapedTraps.Count > 0)
        {
            // If we are risking death, the die
            if (_isRiskingDeath)
                Death();
            else // If we are not risking death, then become invincible and activate risking death
            {
                _invincibleTimer = 0f;
                _uiManager.SetRinskingDeathImage(0f);
                _isInvincible = true;
            }
        }
    }

    void InvincibleCooldown()
    {
        if(_invincibleTimer < _invincibleTime)
        {            
            _invincibleTimer += Time.deltaTime;
        }
        else
        {
            _isInvincible = false;
            _isRiskingDeath = true;
            _riskDeathTimer = 0f;
            DetectIfDamage();
        }
    }

    void RiskingDeathCooldown()
    {
        if (_riskDeathTimer < _riskDeathTime)
        {
            _uiManager.SetRinskingDeathImage(_riskDeathTimer / _riskDeathTime); // Refresh risking death image color and opacity
            _riskDeathTimer += Time.deltaTime;
        }
        else
        {
            _uiManager.SetRinskingDeathImage(1f); // Set risking death image color and opacity to 0
            _isRiskingDeath = false;
        }
    }

    void Death()
    {
        if (!_isDead)
        {
            _uiManager.SetDeathMenu(true);
            _isDead = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == _deathZoneTag)
        {
            if(!_overlapedTraps.Contains(collision))
                _overlapedTraps.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == _deathZoneTag)
        {
            if(_overlapedTraps.Contains(collision))
                _overlapedTraps.Remove(collision);
        }
    }
}
