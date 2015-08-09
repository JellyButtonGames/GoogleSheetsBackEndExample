using System;
using UnityEngine;

/// <summary>
/// Moves a 3D Cube (or any 3D object) from side to side on the X-axis according to the provided 
/// configuration from Google Sheets if available.
/// 
/// The class loads the configuration from Google Docs each run of the game, so to see
/// the effect of changing the doc, reload the game.
/// </summary>
public class CubeMover : MonoBehaviour {

    /// <summary>
    /// The cube's size
    /// </summary>
    [SerializeField]
    private float _cubeSize;
    /// <summary>
    /// The cube's step size each frame
    /// </summary>
    [SerializeField]
    private float _cubeMoveStep;
    /// <summary>
    /// The cube's maximal move position
    /// </summary>
    [SerializeField]
    private float _cubeMoveMax;
    /// <summary>
    /// Allows to turn on/off the configuration loading from Google Sheets
    /// </summary>
    [SerializeField]
    private bool _loadFromGoogle = true;

    private bool _isMoving;
    private GoogleSheetsLoader _googleSheetLoader;

    private Vector3 _originalPosition;
    private float _currentRelativePositionX;
    private float _currentDirection;

	void Start ()
	{
	    if (_loadFromGoogle)
	    {
            _googleSheetLoader = GetComponent<GoogleSheetsLoader>();
            if (_googleSheetLoader != null)
            {
                // if google loader is available use it for configuration
                _googleSheetLoader.SheetLoaded += OnSheetLoaded;
                _googleSheetLoader.SheetLoadFailed += OnSheetLoadFailed;
                _googleSheetLoader.LoadSheet();
                return;
            }
	    }
	    
        // otherwise fallback to the editor values
        FinalizeInitialization();
	}

    void Update()
    {
        if (!_isMoving)
        {
            return;
        }

        // move the cube in world space on X-Axis
        transform.position = _originalPosition + (Vector3.right * _currentRelativePositionX);

        // step the cube for this frame
        StepCube();
        if (Mathf.Abs(_currentRelativePositionX) > _cubeMoveMax)
        {
            // cube reached the virtual edge go back
            SwitchDirection();
            // step once to prevent switch locking
            StepCube();
        }
    }

    /// <summary>
    /// Called when the Google Sheet has been loaded and parsed
    /// </summary>
    private void OnSheetLoaded()
    {
        foreach (var configEntry in _googleSheetLoader.LoadedSheet)
        {
            // apply the configuration according to predetermined parameter names
            switch (configEntry.Key)
            {
                case "cubeSize":
                    _cubeSize = Convert.ToSingle(configEntry.Value);
                    break;
                case "cubeMoveStep":
                    _cubeMoveStep = Convert.ToSingle(configEntry.Value);
                    break;
                case "cubeMoveMax":
                    _cubeMoveMax = Convert.ToSingle(configEntry.Value);
                    break;
                default:
                    Debug.Log("CubeMover does not support param " + configEntry.Key);
                    break;
            }
        }

        FinalizeInitialization();
    }

    /// <summary>
    /// Called when the Google Sheet has failed to load
    /// </summary>
    private void OnSheetLoadFailed()
    {
        // fallback to editor configuration
        FinalizeInitialization();
    }

    private void FinalizeInitialization()
    {
        ApplyToCube();
        StartMoving();
    }

    private void ApplyToCube()
    {
        transform.localScale = Vector3.one * _cubeSize;
    }
    private void StepCube()
    {
        _currentRelativePositionX += _currentDirection * _cubeMoveStep * Time.deltaTime;
    }

    private void StartMoving()
    {
        // select random direction of initial movement
        _currentDirection = (UnityEngine.Random.Range(0, 2) == 0) ? -1.0f : 1.0f;
        _originalPosition = transform.position;
        _isMoving = true;
    }

    private void SwitchDirection()
    {
        _currentDirection = -_currentDirection;
    }
}
