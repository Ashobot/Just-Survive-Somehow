using UnityEngine;
using UltimateAttributesPack;
using EZCameraShake;

public class PlayerTriggers : MonoBehaviour
{
    GameManager _gameManager;
    PlayerController _playerController;

    [Title("Death", "")]
    [SerializeField, Tag] string _deathZoneTag;
    [SerializeField] float _invincibleTime;
    [SerializeField] float _damagedTime;
    [Space]
    [LineTitle("Sounds")]
    [SerializeField] float _damageSoundMaxVolume;
    [SerializeField] AudioClip _damageSound;
    [Space]
    [SerializeField] float _deathSoundMaxVolume;
    [SerializeField] AudioClip _deathSound;

    bool _canTakeDamage;
    public bool CanTakeDamage => _canTakeDamage;
    bool _isInvincible;
    float _invincibleTimer;
    float _damagedTimer;
    bool _isDead;
    public bool IsDead => _isDead;
    bool _isDamaged;

    [SubTitle("Damage camera shake", "")]
    [SerializeField] float _damageShakeMagnitude;
    [SerializeField] float _damageShakeRoughness;
    [SerializeField] float _damageShakeFadeInTime;
    [SerializeField] float _damageShakeFadeOutTime;

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
        if (_isDead)
            return;

        // Shake camera when damage
        CameraShaker.Instance.ShakeOnce(_damageShakeMagnitude, _damageShakeRoughness, _damageShakeFadeInTime, _damageShakeFadeOutTime);

        // If we are risking death, the die
        if (_isDamaged)
            Death();
        else // If we are not risking death, then become invincible and activate risking death
        {
            // Play damage sound
            SoundManager.instance.PlaySound(_damageSound, transform, _damageSoundMaxVolume, false, 0f);

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
            SoundManager.instance.PlaySound(_deathSound, transform, _deathSoundMaxVolume, false, 0f);

            _gameManager.UIManager.SetDeathMenu(true);
            _playerController.PlayerAnimations.SetDeathAnimation();
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
