using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Transition;
using static UnityEngine.Rendering.DebugUI;

namespace GM
{

    public class GameManager : MonoBehaviour
    {
        // シングルトンパターンの実装
        public static GameManager Instance { get; private set; }

        //[Header("難易度設定 (いずれかtrueにする)")]
        //public bool isEasy;
        //public bool isNormal;
        //public bool isHard;
        //public bool isEndless;

        [Header("スコア閾値 (超えたらタイマー10倍)")]
        public int scoreThreshold = 100;

        [Header("Easyモード: 非表示にするオブジェクト")]
        public GameObject easyHideObject;

        [Header("Hardモード: _ScrollEnabled を設定するマテリアル")]
        public Material hardMaterial;

        [Header("Endlessモード: _Value を減少させるマテリアル")]
        public Material endlessMaterial;
        [Tooltip("Endlessモード時に_Valueを毎秒どれだけ減らすか")] public float endlessValueDecreaseSpeed = 0.1f;


        [Header("ゲーム設定")]
        public float startingTime = 60f;
        public float comboWindow = 1f;

        [Header("スコア・ボーナス")]
        public int baseScore = 10;
        public float bonusTimePerCombo = 1f;

        [Header("コンボ表示設定")]
        [SerializeField] private float comboDisplayDuration = 2f;

        [Header("UI表示（TextMeshPro）")]
        [SerializeField] public TextMeshProUGUI scoreText;
        [SerializeField] public TextMeshProUGUI timeText;
        [SerializeField] public TextMeshProUGUI comboText;
        [SerializeField] public TextMeshProUGUI timeUpText;

        [Header("アニメーション設定")]
        [SerializeField] private float normalScale = 1.5f;
        [SerializeField] private float normalDuration = 0.5f;
        [SerializeField] private float fiveScale = 2f;
        [SerializeField] private float fiveDuration = 0.7f;
        [SerializeField] private float timeUpScale = 2f;
        [SerializeField] private float timeUpDuration = 1f;

        [Header("シェーダートランジションコントローラ")]
        [SerializeField] public ShaderTransitionController shaderTransitionController;

        //public GameObject gameObjects;
        private float remainingTime;
        //public static int totalScore;
        private static float _totalScore = 0;

        public static float totalScore
        {
            get => _totalScore;
            set
            {
                int Maxamount = 100;
                if (Select.isEasy) Maxamount /= 2;
                else if (Select.isHard) Maxamount *= 2;
                value = Mathf.Clamp(value, 0, Maxamount);
                _totalScore = value;
            }
        }
        private int currentCombo;
        private float lastIClickTime;
        private float comboDisplayTimer;
        private bool isGameOver;
        private bool speedUpApplied;
        // 時間減少速度倍率（スコア閾値超過で変更）
        private float timeDecreaseMultiplier = 1f;

        private Tween normalComboTween;
        private Tween fiveComboTween;


        //void OnEnable()
        //{
        //    SceneManager.sceneLoaded += OnSceneLoaded;
        //}

        //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        //{
        //    // 新シーンの Canvas 内から TextMeshProUGUI を Find

        //}


        void Awake()
        {
            // シングルトンと永続化
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // シーンロードイベントに登録(常に発火)
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            // 初期化
            remainingTime = startingTime;
            totalScore = 0;
            currentCombo = 0;
            lastIClickTime = -comboWindow;
            comboDisplayTimer = comboDisplayDuration;
            isGameOver = false;
            speedUpApplied = false;
            timeDecreaseMultiplier = 1f;

            // 難易度別プレ処理
            if (Select.isEasy && easyHideObject != null)
                easyHideObject.SetActive(false);
            if ((Select.isHard && hardMaterial != null) || Select.isEndless)
                hardMaterial?.SetInt("_ScrollEnabled", 1);
            if (Select.isEasy) scoreThreshold = 50;
            if (Select.isNormal) scoreThreshold = 100;
            if (Select.isHard) scoreThreshold = 200;
            
           
            shaderTransitionController?.ResetTransition();
            SetupComboAnimation();
            SetupTimeUpText();
        }

        void Start()
        {
            // 初回シーン用バインド
            BindUIIfMain();
        }

        void OnDestroy()
        {
            // イベント解除
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            BindUIIfMain();
        }
        private void OnSceneUnloaded(Scene scene)
        {
            if (scene.name == "Main")
            {
                scoreText = null;
                timeText = null;
                comboText = null;
                timeUpText = null;
            }
        }

        // MainSceneのみUIを再取得
        private void BindUIIfMain()
        {
            if (SceneManager.GetActiveScene().name != "Main")
                return;
            scoreText = GameObject.Find("Score")?.GetComponent<TextMeshProUGUI>();
            timeText = GameObject.Find("Time")?.GetComponent<TextMeshProUGUI>();
            comboText = GameObject.Find("Combo")?.GetComponent<TextMeshProUGUI>();
            timeUpText = GameObject.Find("TimeUp")?.GetComponent<TextMeshProUGUI>();
            UpdateUI();
        }




        void Update()
        {
            if (isGameOver) return;

            // Endlessモード以外のタイマー
            if (!Select.isEndless)
            {
                // スコア閾値超過でタイマー加速
                if (!speedUpApplied && totalScore >= scoreThreshold)
                {
                    totalScore = 100;
                    timeDecreaseMultiplier = 30f;
                    speedUpApplied = true;
                }

                // 残り時間減少（倍率適用）
                remainingTime -= Time.deltaTime * timeDecreaseMultiplier;
                remainingTime = Mathf.Max(remainingTime, 0f);
                UpdateTimeUI();

                if (remainingTime <= 0f) GameOver();
            }
            else
            {
                // Endlessモード: マテリアルの _Value を減少
                if (endlessMaterial != null)
                {
                    float v = endlessMaterial.GetFloat("_Value");
                    v -= endlessValueDecreaseSpeed * Time.deltaTime;
                    endlessMaterial.SetFloat("_Value", v);
                    //timeText.text = $"Value: {v:F2}";
                    if (v <= 0f) GameOver();
                }
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
            if (!Select.isEndless) remainingTime += bonusTimePerCombo * currentCombo;

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

        public void GameOver()
        {
           
            
            isGameOver = true;
            Time.timeScale = 0f;
            ShowTimeUpAnimation();
            Time.timeScale = 1f;
            // 入力待ちでシーン遷移
            WaitForAnyInputAndTransitionAsync().Forget();
            //gameObjects.SetActive(false);
            Debug.Log($"Game Over! Final Score: {totalScore}");
        }

        private void UpdateUI()
        {
            //if(scoreText!=null)scoreText.text =totalScore.ToString();
            remainingTime = startingTime;
            totalScore = 0;
            currentCombo = 0;
            lastIClickTime = -comboWindow;
            comboDisplayTimer = comboDisplayDuration;
            isGameOver = false;
            speedUpApplied = false;
            timeDecreaseMultiplier = 1f;
            timeUpText.gameObject.SetActive(false);
            shaderTransitionController?.ResetTransition();

            UpdateScoreUI();
            UpdateTimeUI();
            UpdateComboUI();
        }

        

        // 難易度に応じたスコア表示
        private void UpdateScoreUI()
        {
            if (!scoreText) return;
            int displayScore = (int)totalScore;
            if (Select.isEasy) displayScore *= 2;
            else if (Select.isHard) displayScore /= 2;
            scoreText.text = $"{displayScore} %";
        }

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