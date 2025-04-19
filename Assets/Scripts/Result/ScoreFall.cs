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
    [SerializeField] AudioSource loveSound;
    [SerializeField] AudioSource sadSound;
    public bool soundFlug;
    int up = 0;

    [SerializeField] GameObject loveEffect;
    [SerializeField] GameObject sadEffect;
    [SerializeField] GameObject sadEffect2;
    //private ParticleSystem loveParticle;
    //private ParticleSystem sadParticle;
    //private ParticleSystem sadParticle2;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        soundFlug = true;
        //score.text = "0%";
        pos = rectTransform.position;
        pos.x = 0;
        pos.y = 1000;
        rectTransform.anchoredPosition = pos;

        loveEffect.SetActive(false);
        sadEffect.SetActive(false);
        sadEffect2.SetActive(false);
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
            if(result >= 80)
            {
                loveEffect.SetActive(true);
                loveSound.PlayOneShot(loveSound.clip);
                //loveParticle.Play();
            }
            else if(result <= 20)
            {
                sadEffect.SetActive(true);
                sadEffect2.SetActive(true);
                sadSound.PlayOneShot(sadSound.clip);
                //sadParticle.Play();
                //sadParticle2.Play();
            }
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
