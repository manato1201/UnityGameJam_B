using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

//�X�R�A�\���̃X�N���v�g

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
        //�X�R�A�\��
        score.text = result + "%";
        //�����Ă��鏈��
        if (pos.y >= 0)
        { 
            rectTransform.anchoredPosition = pos;
            pos.y -= 0.1f * speed;
        }
        //�����Ă�����̏���
        if(pos.y <= 20 && soundFlug)
        {
            fallSound.PlayOneShot(fallSound.clip);
            soundFlug=false;

            //80���ȏ�Ȃ�n�[�g�̃G�t�F�N�g���o��
            if(result >= 80)
            {
                loveEffect.SetActive(true);
                loveSound.PlayOneShot(loveSound.clip);
                //loveParticle.Play();
            }

            //20���ȏ�Ȃ犄�ꂽ�n�[�g�̃G�t�F�N�g���o��
            else if (result <= 20)
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
