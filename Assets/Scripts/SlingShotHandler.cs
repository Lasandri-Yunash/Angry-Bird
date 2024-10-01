using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{

    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;               
    [SerializeField] private LineRenderer _rightLineRenderer;              

    [Header("Transform References")]
    [SerializeField] private Transform _leftStartPosition;                  
    [SerializeField] private Transform _rightStartPosition;                 
    [SerializeField] private Transform _centerPosition;                     
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;
    [SerializeField] private CameraManager _cameraManeger;

    [Header("Slingshot Stats")]
    [SerializeField] private float _maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 5f;
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;
    [SerializeField] private float _maxAnimationTime = 1f;

    [Header("Bird")]
    [SerializeField] private AngryBird _angryBirdPrefab;                   
    [SerializeField] private float _angryBirdPositionOffset = 2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip _elasticPulledclip;
    [SerializeField] private AudioClip _elasticReleasedClips;

    private Vector2 _slingShotLinePosition;                                 
    private Vector2 _direction;                                             
    private Vector2 _directionNormalized;                                

    private bool _clickedWithinArea;
    private bool _birdOnSlingShot;

    private AngryBird _spawnedAngryBird;
    private AudioSource _audioSource;

    private void Awake()
    {

        _audioSource = GetComponent<AudioSource>();
        // Disable the line renderers initially

        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnAngryBird();                                                       
    }


    private void Update()
    {
                
        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.IsWithinSlingShotArea()) 
        { 
            _clickedWithinArea = true;

            if (_birdOnSlingShot)
            {
                SoubdManager.instance.PlayClip(_elasticPulledclip, _audioSource);
                _cameraManeger.SwitchFollowCam(_spawnedAngryBird.transform);
            }
        }

              
        if (InputManager.IsLeftMousePressed && _clickedWithinArea && _birdOnSlingShot)  
        {
            DrawSlingShot();
            PositionAndRotationAngryBird();
        } 

        if (InputManager.WasLeftMouseButtonReleased && _birdOnSlingShot && _clickedWithinArea)
        {
            if (GameManager.Instance.HasEnoughShots())
            {

                _clickedWithinArea = false;
                _birdOnSlingShot = false;

                _spawnedAngryBird.LaunchBird(_direction, _shotForce);

                SoubdManager.instance.PlayClip(_elasticReleasedClips, _audioSource);

                GameManager.Instance.UseShot();
                // SetLines(_centerPosition.position);
                AnimateSlingShot();

                if (GameManager.Instance.HasEnoughShots())
                {

                StartCoroutine(SpawnAngryBirdAfterTime());

                }



            }

        }


    }

    #region Slingshot Methods

    private void DrawSlingShot ()
    {
               
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition); 
            
        _slingShotLinePosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance); 
               
        SetLines(_slingShotLinePosition); 


        _direction = (Vector2)_centerPosition.position - _slingShotLinePosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {

        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)     // Enable the line renderers if they are not already enabled
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        // Set the positions of the left and right line renderers

        _leftLineRenderer.SetPosition(0,position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);

        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);

    }

    #endregion


    #region Angry Bird Methods

    private void SpawnAngryBird()
    {
        _elasticTransform.DOComplete();
        SetLines(_idlePosition.position);         


        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2) _idlePosition.position + dir * _angryBirdPositionOffset;


        _spawnedAngryBird = Instantiate(_angryBirdPrefab, spawnPosition, Quaternion.identity); // 0 rotation
        _spawnedAngryBird.transform.right = dir;

        _birdOnSlingShot = true;
    }

    private void PositionAndRotationAngryBird()
    {

        _spawnedAngryBird.transform.position = _slingShotLinePosition + _directionNormalized * _angryBirdPositionOffset;
        _spawnedAngryBird.transform.right = _directionNormalized;
    }

    private IEnumerator SpawnAngryBirdAfterTime()
    {

        yield return new WaitForSeconds(_timeBetweenBirdRespawns);

        SpawnAngryBird();
        _cameraManeger.SwitchToIdleCam();
    }

    #endregion

    #region Animate Slingshot

    private void AnimateSlingShot()
    {
        _elasticTransform.position = _leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);

        float time = dist / _elasticDivider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform,time));
    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time && elapsedTime < _maxAnimationTime)
        {

            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;
        }
    }
    #endregion
}
