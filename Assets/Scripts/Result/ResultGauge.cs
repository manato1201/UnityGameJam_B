using System.Runtime.CompilerServices;
using GM;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ゲージがだんだん上がっていくスクリプト
public class ResultGauge : MonoBehaviour
{
    private float heightPos;
    private float turnPoint;
    [SerializeField] private float turnPointMAX;
    [SerializeField] private float turnSpeed;
    private float scoreMax = 100;
    [SerializeField] private float score;
    [SerializeField] private float xpos;

    private RectTransform rectTransform;
    private Vector2 pos;

    [SerializeField] private TextMeshProUGUI text;
    private ScoreFall scoreFall;

    [SerializeField] private AudioSource gaugeSound;

    private int scoreCount = 0;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        heightPos = 0f;
        pos = rectTransform.position;
        pos.x = xpos;
        pos.y = -431.2f;


        // メインシーンからスコアを取得できるようになったら削除する
        ScoreRatio(GameManager.totalScore);

        scoreFall = text.GetComponent<ScoreFall>();

        if (gaugeSound != null)
        {
            gaugeSound.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 現在のゲージの高さが止まる高さより低い時
        if (heightPos <= turnPoint)
        {
            pos.y += 0.05f * turnSpeed;
            heightPos += 0.1f * turnSpeed;

            rectTransform.anchoredPosition = pos;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightPos);
        }
        else
        {
            if (gaugeSound != null)
            {
                gaugeSound.Stop();
            }
            if (scoreCount > 120)
            {
                scoreFall.Fall(GameManager.totalScore);
            }
            scoreCount++;
        }
    }

    // スコアをパーセントに変換する
    void ScoreRatio(float score)
    {
        if (Select.isEasy) score *= 2;
        else if (Select.isHard) score /= 2;

        float ratio = score / scoreMax;
        Debug.Log(ratio);
        turnPoint = turnPointMAX * ratio;
    }
}
