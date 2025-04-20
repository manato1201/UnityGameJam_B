using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;
public class ResultManager : MonoBehaviour
{
    [SerializeField] private GameObject Score;
    [SerializeField] private ScoreFall scoreFall;
    [SerializeField] private GameObject panel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreFall = Score.GetComponent<ScoreFall>();
        panel.SetActive(false);
        Debug.Log(scoreFall.soundFlug);
        //restartButton.onClick.AddListener(GotoRestart);
        //restartButton.onClick.AddListener(GotoTitle);
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
        
    }

    //void GotoRestart()
    //{
    //    StartCoroutine(PlaySoundAndChangeScene("Select"));
    //}
    //void GotoTitle()
    //{
    //    StartCoroutine(PlaySoundAndChangeScene("Title"));
    //}

    //IEnumerator PlaySoundAndChangeScene(string SceneName)
    //{
    //    buttonSound.Play(); // Œø‰Ê‰¹‚ğÄ¶
    //    yield return new WaitForSeconds(buttonSound.clip.length); // Œø‰Ê‰¹‚ªI‚í‚é‚Ì‚ğ‘Ò‚Â
    //    SceneManager.LoadScene(SceneName); // ƒV[ƒ“‚ğ‘JˆÚ
    //}
}
