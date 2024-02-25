using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [SerializeField] GameObject _wallLeft;
    public GameObject WallLeft { get { return _wallLeft; } }
    [SerializeField] GameObject _wallUp;
    public GameObject WallUp { get { return _wallUp; } }
    [SerializeField] GameObject _wallRight;
    public GameObject WallRight { get { return _wallRight; } }
    [SerializeField] GameObject _wallDown;
    public GameObject WallDown { get { return _wallDown; } }




}
