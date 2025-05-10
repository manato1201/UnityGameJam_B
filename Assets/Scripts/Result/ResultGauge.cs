using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//ゲージがだんだん上がっていくスクリプト


public class ResultGauge : MonoBehaviour
{

    private float heightPos;
    private float turnPoint;
    [SerializeField]private float turnPointMAX;
    [SerializeField] private float turnSpeed;
    private float scoreMax = 100;
    [SerializeField] private float score;
    [SerializeField] private float xpos;

    RectTransform rectTransform;
    Vector2 pos;

    [SerializeField] private TextMeshProUGUI text;
    ScoreFall scoreFall;

    [SerializeField] AudioSource gaugeSound;

    int scoreCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        heightPos = 0;
        pos = rectTransform.position;
        pos.x = xpos;
        pos.y = -431.2f;

        //メインシーンからスコアを取得できるようになったら削除する
        ScoreRatio(score);

        scoreFall = text.GetComponent<ScoreFall>();

        if(gaugeSound != null)
        {

            gaugeSound.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //現在のゲージの高さが止まる高さより低い時
        if (heightPos <= turnPoint)
        {

            pos.y += 0.05f * turnSpeed;
            heightPos += 0.1f * turnSpeed;

            rectTransform.anchoredPosition = pos;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightPos);

            //if (scoreCount > 20)
            //{
            //    scoreFall.ScoreUp(score);
            //    scoreCount = 0;
            //}
            //scoreCount++;
        }
        else
        {
            if (gaugeSound != null)
            {

                gaugeSound.Stop();
            }
            if (scoreCount > 120)
            {
                scoreFall.Fall(score);
            }
            scoreCount++;
        }
    }

    //スコアをパーセントに変換する
    void ScoreRatio(float score)
    {
        float ratio;
        ratio = score / scoreMax;
        Debug.Log(ratio);
        turnPoint = turnPointMAX * ratio;
    }
}
