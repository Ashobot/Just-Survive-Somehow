using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ----- Variables ----- //

    // Player components
    public PlayerMovement PlayerMovement { get { return _playerMovement; } }
    PlayerMovement _playerMovement;
    public PlayerInputs PlayerInputs { get { return _playerInputs; } }
    PlayerInputs _playerInputs;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerInputs = GetComponent<PlayerInputs>();
    }

}
