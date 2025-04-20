using UnityEngine;
using TMPro;
using DG.Tweening;

namespace GM
{

    public class GameManager : MonoBehaviour
    {
        // シングルトンパターンの実装
        public static GameManager Instance { get; private set; }

        [Header("ゲーム設定")]
        public float startingTime = 60f;
        public float comboWindow = 1f;

        [Header("スコア・ボーナス")]
        public int baseScore = 10;
        public float bonusTimePerCombo = 1f;

        [Header("コンボ表示設定")]
        [SerializeField] private float comboDisplayDuration = 2f;

        [Header("UI表示（TextMeshPro）")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI comboText;

        [Header("アニメーション設定")]
        [SerializeField] private float normalScale = 1.5f;
        [SerializeField] private float normalDuration = 0.5f;
        [SerializeField] private float fiveScale = 2f;
        [SerializeField] private float fiveDuration = 0.7f;

        private float remainingTime;
        private int totalScore;
        private int currentCombo;
        private float lastIClickTime;
        private float comboDisplayTimer;

        // Tweens
        private Tween normalComboTween;
        private Tween fiveComboTween;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            remainingTime = startingTime;
            totalScore = 0;
            currentCombo = 0;
            lastIClickTime = -comboWindow;
            comboDisplayTimer = comboDisplayDuration;
            SetupComboAnimation();
            UpdateUI();
        }

        void Update()
        {
            // 時間カウント
            if (remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime;
                remainingTime = Mathf.Max(remainingTime, 0f);
                UpdateTimeUI();
            }
            else GameOver();

            // クリック間隔でコンボリセット
            if (Time.time - lastIClickTime > comboWindow && currentCombo != 0)
            {
                ResetCombo();
            }

            // 表示時間経過でコンボ非表示・リセット
            if (currentCombo > 0)
            {
                comboDisplayTimer += Time.deltaTime;
                if (comboDisplayTimer >= comboDisplayDuration)
                {
                    ResetCombo();
                }
            }
        }

        /// <summary> Iクリック時に呼ぶ </summary>
        public void IClicked()
        {
            float now = Time.time;
            if (now - lastIClickTime <= comboWindow) currentCombo++;
            else currentCombo = 1;
            lastIClickTime = now;
            comboDisplayTimer = 0f;

            int bonusPoints = baseScore * currentCombo;
            totalScore += bonusPoints;
            remainingTime += bonusTimePerCombo * currentCombo;

            UpdateScoreUI();
            UpdateTimeUI();
            UpdateComboUI();

            // アニメーション再生
            if (currentCombo % 5 == 0)
            {
                if (fiveComboTween == null )
                {
                    fiveComboTween.Restart();
                }
            }
            else
            {
                normalComboTween.Restart();
            }
        }

        /// <summary> I以外クリックで呼ぶ </summary>
        public void NonIClicked()
        {
            ResetCombo();
        }

        private void ResetCombo()
        {
            currentCombo = 0;
            UpdateComboUI();
        }

        private void GameOver()
        {
            Debug.Log($"Game Over! Final Score: {totalScore}");
        }

        private void UpdateUI()
        {
            UpdateScoreUI();
            UpdateTimeUI();
            UpdateComboUI();
        }
        private void UpdateScoreUI() { if (scoreText) scoreText.text = $"{totalScore}%"; }
        private void UpdateTimeUI() { if (timeText) timeText.text = $"Time: {remainingTime:F1}"; }
        private void UpdateComboUI() { if (comboText) comboText.text = currentCombo > 1 ? $"Combo x{currentCombo}!" : string.Empty; }

        private void SetupComboAnimation()
        {
            var rt = comboText.rectTransform;
            comboText.alpha = 1;
            rt.localScale = Vector3.one;

            // 通常用Tween
            normalComboTween = rt.DOScale(normalScale, normalDuration)
                .SetAutoKill(false)
                .Pause()
                .OnComplete(() => rt.DOScale(1f, normalDuration));
            // 5コンボ用Tween
            fiveComboTween = rt.DOScale(fiveScale, fiveDuration)
                .SetAutoKill(false)
                .Pause()
                .OnComplete(() => rt.DOScale(1f, normalDuration));
            // フェードアウト for normal
            normalComboTween.OnPlay(() => comboText.DOFade(1f, 0));
            normalComboTween.OnStepComplete(() => comboText.DOFade(0f, normalDuration));
            // フェード非表示 for five
            fiveComboTween.OnPlay(() => comboText.DOFade(1f, 0));
            fiveComboTween.OnStepComplete(() => comboText.DOFade(0f, fiveDuration));
        }
    }
}