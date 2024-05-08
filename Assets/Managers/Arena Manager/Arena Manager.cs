using UnityEngine;
using System;
using System.Collections.Generic;
using UltimateAttributesPack;

public class ArenaManager : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField] Camera _cam;
    [SerializeField] LayerMask _wallsLayerMask;
    public LayerMask WallsLayerMask { get { return _wallsLayerMask;} }
    [Space]
    [SerializeField] GameObject _wallLeft;
    public GameObject WallLeft { get { return _wallLeft; } }
    [SerializeField] GameObject _wallUp;
    public GameObject WallUp { get { return _wallUp; } }
    [SerializeField] GameObject _wallRight;
    public GameObject WallRight { get { return _wallRight; } }
    [SerializeField] GameObject _wallDown;
    public GameObject WallDown { get { return _wallDown; } }

    [FunctionButton("Refresh arena size" ,nameof(RefreshArenaSize), typeof(ArenaManager))]
    [SerializeField] Transform _slabParent;
    [SerializeField] GameObject _slabPrefab;
    [SerializeField] Vector2Int _arenaSize;

    [Space]
    [SerializeField] SlabsPattern[] _slabsPatterns;

    List<SlabScript> _activatedSlabs = new List<SlabScript>();

    private void Awake()
    {
        // Get game manager
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        SetSlabsOfWave(0); // Set slabs to the first wave
    }

    void RefreshArenaSize()
    {
        // Destroy all slabs
        foreach(SlabScript slab in _slabParent.GetComponentsInChildren<SlabScript>())
        {
            if(slab != _slabParent.transform)
                DestroyImmediate(slab.gameObject);
        }

        // Spawn new slabs on map
        Vector2 downLeftPoint = new Vector2(-0.5f - _arenaSize.x / 2 + 1, -0.5f - _arenaSize.y / 2 + 1);
        for(int x = 0; x < _arenaSize.x; x++)
        {
            for(int y = 0; y < _arenaSize.y; y++)
            {
                Vector2 slabPos = new Vector2(downLeftPoint.x + x, downLeftPoint.y + y);
                GameObject newSlab = Instantiate(_slabPrefab, slabPos, Quaternion.identity, _slabParent);
                newSlab.transform.parent = _slabParent;
                newSlab.name = $"Slab ({downLeftPoint.x + x + (downLeftPoint.x + x <= 0 ? -0.5f : 0.5f)}, {downLeftPoint.y + y + (downLeftPoint.y + y <= 0 ? -0.5f : 0.5f)})";
            }
        }

        // Set wall positions
        _wallLeft.transform.position = new Vector2(-_arenaSize.x / 2 - 1f, 0);
        _wallLeft.transform.localScale = new Vector2(2, _arenaSize.y + 4);

        _wallRight.transform.position = new Vector2(_arenaSize.x / 2 + 1f, 0);
        _wallRight.transform.localScale = new Vector2(2, _arenaSize.y + 4);

        _wallDown.transform.position = new Vector2(0, -_arenaSize.y / 2 - 1f);
        _wallDown.transform.localScale = new Vector2(_arenaSize.x + 4, 2);

        _wallUp.transform.position = new Vector2(0, _arenaSize.y / 2 + 1f);
        _wallUp.transform.localScale = new Vector2(_arenaSize.x + 4, 2);

        _cam.transform.position = new Vector3(0, 0, -10);
        _cam.orthographicSize = _arenaSize.y / 2 + 1;
    }

    public void SetSlabsOfWave(int waveIndex)
    {
        _activatedSlabs.Clear();

        foreach(SlabScript slab in _slabParent.GetComponentsInChildren<SlabScript>())
        {
            if (_slabsPatterns[waveIndex].SlabsActivated.Contains(slab.gameObject))
            {
                slab.SetActivated(true, _slabsPatterns[waveIndex].ActivatedColor);
                _activatedSlabs.Add(slab);
            }
            else
            {

            }
        }
    }

    public void WalkOnSlab(SlabScript slab)
    {
        // Deactivate slab
        if (_activatedSlabs.Contains(slab))
        {
            _activatedSlabs.Remove(slab);
            slab.SetActivated(false, _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].NormalColor);
        }

        // If there is no other slabs activated, go to next wave
        if (_activatedSlabs.Count == 0)
            WaveFinished();
    }

    void WaveFinished()
    {
        _gameManager.GameLoopManager.SetWaveFinished(true);
    }

}

// ----- Serializable classes ----- //

[Serializable]
public class SlabsPattern
{
    public List<GameObject> SlabsActivated;
    public Color ActivatedColor;
    public Color NormalColor;
}