using UnityEngine;
using System.Collections.Generic;
using UltimateAttributesPack;

public class PlayerTriggers : MonoBehaviour
{
    GameManager _gameManager;
    PlayerController _playerController;

    [Title("Death", "dark red")]
    [SerializeField, Tag] string _deathZoneTag;
    [SerializeField] float _invincibleTime;
    [SerializeField] float _damagedTime;

    bool _canTakeDamage;
    public bool CanTakeDamage => _canTakeDamage;
    bool _isInvincible;
    float _invincibleTimer;
    float _damagedTimer;
    bool _isDead;
    bool _isDamaged;

    [Title("Slabs")]
    [SerializeField, Tag] string _slabTag;

    private void Awake()
    {
        // Get game manager
        _gameManager = FindObjectOfType<GameManager>();

        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        ManageCanTakeDamage();

        if (_isDamaged)
            DamagedCooldown();   
    }

    void ManageCanTakeDamage()
    {
        if(_isInvincible || _playerController.PlayerMovement.IsDashing)
            _canTakeDamage = false;
        else
            _canTakeDamage = true;
    }

    public void SetDamage()
    {
        // If we are risking death, the die
        if (_isDamaged)
            Death();
        else // If we are not risking death, then become invincible and activate risking death
        {
            _gameManager.UIManager.SetRinskingDeathImage(0f);
            _playerController.PlayerMovement.SetDamagedPercent(_playerController.PlayerMovement.DamagedMovementSpeed);
            _invincibleTimer = 0f;
            _isInvincible = true;
            _damagedTimer = 0f;
            _isDamaged = true;
        }
    }

    void DamagedCooldown()
    {
        if (_isInvincible)
        {
            if(_invincibleTimer < _invincibleTime)
            {
                _invincibleTimer += Time.deltaTime;
            }
            else
            {             
                _isInvincible = false;               
            }
        }
        else
        {
            if (_damagedTimer < _damagedTime)
            {
                // Set new player movement and dash from damaged speed to normal
                _playerController.PlayerMovement.SetDamagedPercent(_damagedTimer / _damagedTime);               

                // Refresh risking death image color and opacity
                _gameManager.UIManager.SetRinskingDeathImage(_damagedTimer / _damagedTime);

                _damagedTimer += Time.deltaTime;
            }
            else
            {
                _playerController.PlayerMovement.SetDamagedPercent(1f); // Set new player movement and dash to normal
                _gameManager.UIManager.SetRinskingDeathImage(1f); // Set risking death image color and opacity to 0
                _isDamaged = false;
            }
        }
    }

    void Death()
    {
        if (!_isDead)
        {
            _gameManager.UIManager.SetDeathMenu(true);
            _isDead = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Walk on activated slab
        if(collision.tag == _slabTag)
        {
            if(collision.gameObject.TryGetComponent<SlabScript>(out SlabScript slab))
            {
                _gameManager.ArenaManager.WalkOnSlab(slab);
            }
        }
    }
}
