using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Transition;

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
        [SerializeField] private TextMeshProUGUI timeUpText;

        [Header("アニメーション設定")]
        [SerializeField] private float normalScale = 1.5f;
        [SerializeField] private float normalDuration = 0.5f;
        [SerializeField] private float fiveScale = 2f;
        [SerializeField] private float fiveDuration = 0.7f;
        [SerializeField] private float timeUpScale = 2f;
        [SerializeField] private float timeUpDuration = 1f;

        [Header("シェーダートランジションコントローラ")]
        [SerializeField] private ShaderTransitionController shaderTransitionController;

        private float remainingTime;
        private int totalScore;
        private int currentCombo;
        private float lastIClickTime;
        private float comboDisplayTimer;
        private bool isGameOver;

        private Tween normalComboTween;
        private Tween fiveComboTween;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            remainingTime = startingTime;
            totalScore = 0;
            currentCombo = 0;
            lastIClickTime = -comboWindow;
            comboDisplayTimer = comboDisplayDuration;
            isGameOver = false;
            shaderTransitionController.ResetTransition();
            SetupComboAnimation();
            SetupTimeUpText();
            UpdateUI();
        }

        void Update()
        {
            if (isGameOver) return;

            // タイマー
            if (remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime;
                remainingTime = Mathf.Max(remainingTime, 0f);
                UpdateTimeUI();
            }
            if (remainingTime <= 0f && !isGameOver)
            {
                GameOver();
            }

            // コンボリセット判定
            if (Time.time - lastIClickTime > comboWindow && currentCombo != 0)
            {
                ResetCombo();
            }

            // 表示時間経過でコンボリセット
            if (currentCombo > 0)
            {
                comboDisplayTimer += Time.deltaTime;
                if (comboDisplayTimer >= comboDisplayDuration)
                {
                    ResetCombo();
                }
            }
        }

        public void IClicked()
        {
            if (isGameOver) return;

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

            if (currentCombo % 5 == 0)
            {
                if (fiveComboTween == null || !fiveComboTween.IsPlaying())
                    fiveComboTween.Restart();
            }
            else
            {
                normalComboTween.Restart();
            }
        }

        public void NonIClicked()
        {
            if (isGameOver) return;
            ResetCombo();
        }

        private void ResetCombo()
        {
            currentCombo = 0;
            UpdateComboUI();
        }

        private void GameOver()
        {
            isGameOver = true;
            Time.timeScale = 0f;
            ShowTimeUpAnimation();
            Time.timeScale = 1f;
            // 入力待ちでシーン遷移
            WaitForAnyInputAndTransitionAsync().Forget();
            Debug.Log($"Game Over! Final Score: {totalScore}");
        }

        private void UpdateUI()
        {
            UpdateScoreUI();
            UpdateTimeUI();
            UpdateComboUI();
        }

        private void UpdateScoreUI() { if (scoreText) scoreText.text = $"{totalScore} %"; }
        private void UpdateTimeUI() { if (timeText) timeText.text = $"Time: {remainingTime:F1}"; }
        private void UpdateComboUI() { if (comboText) comboText.text = currentCombo > 1 ? $"Combo x{currentCombo}!" : string.Empty; }

        private void SetupComboAnimation()
        {
            var rt = comboText.rectTransform;
            comboText.alpha = 1f;
            rt.localScale = Vector3.one;

            normalComboTween = rt.DOScale(normalScale, normalDuration)
                .SetAutoKill(false).Pause().SetUpdate(true)
                .OnComplete(() => rt.DOScale(1f, normalDuration).SetUpdate(true));
            normalComboTween.OnPlay(() => comboText.DOFade(1f, 0f).SetUpdate(true));
            normalComboTween.OnStepComplete(() => comboText.DOFade(0f, normalDuration).SetUpdate(true));

            fiveComboTween = rt.DOScale(fiveScale, fiveDuration)
                .SetAutoKill(false).Pause().SetUpdate(true)
                .OnComplete(() => rt.DOScale(1f, normalDuration).SetUpdate(true));
            fiveComboTween.OnPlay(() => comboText.DOFade(1f, 0f).SetUpdate(true));
            fiveComboTween.OnStepComplete(() => comboText.DOFade(0f, fiveDuration).SetUpdate(true));
        }

        private void SetupTimeUpText()
        {
            if (timeUpText)
            {
                timeUpText.alpha = 0f;
                timeUpText.rectTransform.localScale = Vector3.zero;
            }
        }

        private void ShowTimeUpAnimation()
        {
            TransitionAsync1().Forget();
            if (!timeUpText) return;
            timeUpText.gameObject.SetActive(true);
            var rt = timeUpText.rectTransform;
            rt.localScale = Vector3.zero;
            timeUpText.alpha = 0f;

            rt.DOScale(timeUpScale, timeUpDuration)
                .SetEase(Ease.OutBack).SetUpdate(true);
            timeUpText.DOFade(1f, timeUpDuration)
                .SetUpdate(true);
            //TransitionAsync2().Forget();
        }
        private async UniTask TransitionAsync1() { await shaderTransitionController.EndTransition(); }
        private async UniTaskVoid WaitForAnyInputAndTransitionAsync()
        {
            // real-time で入力待ち
            await UniTask.WaitUntil(() => Input.anyKeyDown, PlayerLoopTiming.Update);
            await TransitionAsync2();
        }
        private async UniTask TransitionAsync2()
        {
            // 遅延
            await UniTask.Delay(100);

           
            // シーンロード
            await SceneManager.LoadSceneAsync("ResultScene");

            // トランジション終了
            //if (shaderTransitionController != null)
            await shaderTransitionController.EndTransition();
        }

    }
}