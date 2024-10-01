using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public int MaxNumberOfShots = 3;
    [SerializeField] private float _secondToWiatBeforeDeathCheck = 3f;
    [SerializeField] private GameObject _restartScreenObject;
    [SerializeField] private SlingShotHandler _slingShotHandler;
    [SerializeField] private Image _nextLevelImage;


    private int _usedeNumberOfShots;

    private IconHandleer _iconHandleer;

    private List<Baddie> _baddies = new List<Baddie>();  //List



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _iconHandleer = FindObjectOfType<IconHandleer>();

        Baddie[] baddie = FindObjectsOfType<Baddie>();  //Array

        for (int i = 0; i < baddie.Length; i++)
        {

            _baddies.Add(baddie[i]);
        }

        _nextLevelImage.enabled = false;
    }
    public void UseShot()
    {

        _usedeNumberOfShots++;
        _iconHandleer.UseShot(_usedeNumberOfShots);
        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {

        if (_usedeNumberOfShots < MaxNumberOfShots)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckForLastShot()
    {

        if (_usedeNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWitTime());
        }
    }

    private IEnumerator CheckAfterWitTime()
    {
        yield return new  WaitForSeconds(_secondToWiatBeforeDeathCheck);


        if (_baddies.Count == 0)
        {
            WinGame();
        }
        else
        {

            RestartGame(); 

        }
    }

    public void RemoveBaddie(Baddie baddie)
    {
        _baddies.Remove(baddie);
        CheckForAllDeadBaddies();
    }


    private void CheckForAllDeadBaddies()
    {

        if (_baddies.Count == 0)
        {
            //win

            WinGame();
            _slingShotHandler.enabled = false;
        }
    }

    #region win/lose

    private void WinGame()
    {
        //Debug.Log("Winnnnn");

        _restartScreenObject.SetActive(true);
        _slingShotHandler.enabled = true;

        // do we have any more levels to load?

        int currentScenIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;

        if (currentScenIndex+1 < maxLevels)
        {
            _nextLevelImage.enabled = true;
        }

    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        //Debug.Log("Losee");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion
}
