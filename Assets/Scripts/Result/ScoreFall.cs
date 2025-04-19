using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class ScoreFall : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI score;
    
    RectTransform rectTransform;
    Vector2 pos;
    [SerializeField] float speed;
    [SerializeField] AudioSource fallSound;
    public bool soundFlug;
    int up = 0;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        soundFlug = true;
        //score.text = "0%";
        pos = rectTransform.position;
        pos.x = 0;
        pos.y = 1000;
        rectTransform.anchoredPosition = pos;
    }

    public void Fall(float result)
    {
        score.text = result + "%";
        if (pos.y >= 0)
        { 
            rectTransform.anchoredPosition = pos;
            pos.y -= 0.1f * speed;
        }
        if(pos.y <= 20 && soundFlug)
        {
            fallSound.PlayOneShot(fallSound.clip);
            soundFlug=false;
        }
    }

    public void ScoreUp(float resultscore)
    {
        up++;
        if (resultscore >= up)
        {
            score.text = (up + "%");
        }
    }
}
