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

    List<GameObject> _activatedSlabs = new List<GameObject>();
    int _currentActivatedSlabsIndex = 0;

    private void Awake()
    {
        // Get game manager
        _gameManager = FindObjectOfType<GameManager>();
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
        _currentActivatedSlabsIndex = 0;

        // Get current slabs to activate in this wave
        foreach (GameObject go in _slabsPatterns[waveIndex].SlabsActivated[0].Slabs)
        {
            _activatedSlabs.Add(go);
        }
        
        // If slabs are activated one by one in the wave
        if (_slabsPatterns[waveIndex].OneByOne)
        {
            // Activate only the first slab of first list
            if(_activatedSlabs[0].TryGetComponent<SlabScript>(out SlabScript slabScript))
            {
                slabScript.SetActivated(true, _slabsPatterns[waveIndex].ActivatedColor, _slabsPatterns[waveIndex].ActivatedMaterial);
            }
        }
        // If slabs are activated list by list in the wave
        else
        {
            // Activate all slabs of first list
            foreach(GameObject slab in _activatedSlabs)
            {
                if(slab.TryGetComponent<SlabScript>(out SlabScript slabScript))
                {
                    slabScript.SetActivated(true, _slabsPatterns[waveIndex].ActivatedColor, _slabsPatterns[waveIndex].ActivatedMaterial);
                }
            }
        }
    }

    public void WalkOnSlab(SlabScript slab)
    {
        // If slabs are activated one by one in the wave
        if (_slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].OneByOne)
        {
            if(_activatedSlabs.Count >= 1)
            {
                // Deactivate and remove the slab of list
                slab.SetActivated(false, _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].NormalColor, _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].NormalMaterial);                
            }

            if (_activatedSlabs.Count >= 2)
            {
                // Activate the next slab of list
                if(_activatedSlabs[1].TryGetComponent<SlabScript>(out SlabScript newFirstSlabScript))
                {
                    newFirstSlabScript.SetActivated(true, _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].ActivatedColor, _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].ActivatedMaterial);
                }
            }
            else
            {
                _gameManager.GameLoopManager.SetWaveFinished(true);
            }

            _activatedSlabs.RemoveAt(0);
        }
        else
        {
            slab.SetActivated(false, _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].NormalColor, _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].NormalMaterial);

            // If it's the last slab of current list
            if (_activatedSlabs.Count == 1)
            {
                _currentActivatedSlabsIndex++;

                // If the next slabs list is not empty
                if(_currentActivatedSlabsIndex <= _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].SlabsActivated.Count - 1)
                {
                    // Set the list to the new list of slabs
                    _activatedSlabs.Clear();
                    foreach (GameObject go in _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].SlabsActivated[_currentActivatedSlabsIndex].Slabs)
                    {
                        _activatedSlabs.Add(go);
                    }

                    // Activate all slabs of next list in current wave
                    foreach(GameObject obj in _activatedSlabs)
                    {
                        if(obj.TryGetComponent<SlabScript>(out SlabScript slabScript))
                        {
                            slabScript.SetActivated(true, _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].ActivatedColor, _slabsPatterns[_gameManager.GameLoopManager.CurrentWaveIndex].ActivatedMaterial);
                        }
                    }
                }
                // If the next slabs list is empty, set next wave
                else
                {
                    _gameManager.GameLoopManager.SetWaveFinished(true);
                }
            }

            _activatedSlabs.Remove(slab.gameObject);
        }
    }
}

// ----- Serializable classes ----- //

[Serializable]
public class SlabList
{
    public List<GameObject> Slabs;
}

[Serializable]
public class SlabsPattern
{
    public string Name;
    [Space]
    public bool OneByOne;
    public List<SlabList> SlabsActivated = new List<SlabList>();
    [Space]
    public Color ActivatedColor;
    public Material ActivatedMaterial;
    [Space]
    public Color NormalColor;
    public Material NormalMaterial;
}