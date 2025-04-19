using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ResultManager : MonoBehaviour
{
    [SerializeField] private GameObject Score;
    [SerializeField] private ScoreFall scoreFall;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreFall = Score.GetComponent<ScoreFall>();
        panel.SetActive(false);
        Debug.Log(scoreFall.soundFlug);
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreFall.soundFlug!=true) 
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                panel.SetActive(true);
            }
        }

        //restartButton.onClick.AddListener(GotoRestart);
        //restartButton.onClick.AddListener(GotoTitle);
    }

    void GotoRestart()
    {
       // SceneManager.LoadScene(/*メインシーン*/);
    }
    void GotoTitle()
    {
       // SceneManager.LoadScene(/*タイトルシーン*/);
    }
}
