using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayerManager;

public class Reset_Button : MonoBehaviour
{
    
    public GameObject canvas;
    public Button ResetButton;
    public Text time;

    int Seconds=0;

    private void OnEnable()
    {
        Player_Manager.stopGameScreen += Show;
        
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
        Player_Manager.stopGameScreen -= Show;
    }


    private void Awake()
    {
        ResetButton.onClick.AddListener(ResetFunction);
        StartCoroutine(Timer());
    }

    private void ResetFunction()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Show()
    {
        //pausing the game while UI is on
        Time.timeScale = 0;

        canvas.SetActive(true);

        time.text = $"Battle Time {0}:{Seconds} \n Team {Player_Manager.winner} won!";

        StopCoroutine(Timer());

    }

    //Coroutine to count each int second
    IEnumerator Timer()
    {
        while (true)
        {
            timeInSeconds();
            yield return new WaitForSeconds(1);
        }
    }
    void timeInSeconds()
    {
        Seconds += 1;
    }
}