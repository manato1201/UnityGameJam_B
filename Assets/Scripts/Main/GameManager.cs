using UnityEngine;

namespace GM
{

    public class GameManager : MonoBehaviour
    {
        // シングルトンパターンの実装
        public static GameManager Instance { get; private set; }

        [Header("ゲーム設定")]
        // ゲーム開始時の残り時間（秒）
        public float startingTime = 60f;
        // Combo（連続クリック）判定の許容時間
        public float comboWindow = 1f;

        [Header("スコア・ボーナス")]
        // Iクリック時の基本スコア
        public int baseScore = 10;
        // コンボ成立時に加算される残り時間のボーナス（秒）
        public float bonusTimePerCombo = 1f;

        // 現在のゲーム状態（インスペクター上で確認可能）
        [HideInInspector] public float remainingTime;
        [HideInInspector] public int totalScore;
        [HideInInspector] public int currentCombo;
        [HideInInspector] public float lastIClickTime;

        void Awake()
        {
            // すでに存在する場合は削除
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // 必要ならシーン遷移時にも破棄しない
            // DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            // ゲーム開始時の初期値設定
            remainingTime = startingTime;
            totalScore = 0;
            currentCombo = 0;
            lastIClickTime = -comboWindow;  // すぐにリセットされないように設定
        }

        void Update()
        {
            // ゲーム中は残り時間をカウントダウン
            if (remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime;
            }
            else
            {
                GameOver();
            }

            // コンボ判定：comboWindow より時間が経過していたらリセット
            if (Time.time - lastIClickTime > comboWindow)
            {
                currentCombo = 0;
            }
        }

        /// <summary>
        /// Iタグのオブジェクトがクリックされた場合の処理。
        /// 連続クリック（コンボ）が成立していればコンボ数を加算し、
        /// スコアと残り時間にボーナスを付与します。
        /// </summary>
        public void IClicked()
        {
            float currentTime = Time.time;
            // 前回のクリックから comboWindow 内であればコンボ継続、そうでなければリセット
            if (currentTime - lastIClickTime <= comboWindow)
            {
                currentCombo++;
            }
            else
            {
                currentCombo = 1;
            }
            lastIClickTime = currentTime;

            // コンボ数に応じたボーナスの計算
            int bonusPoints = baseScore * currentCombo;
            totalScore += bonusPoints;
            remainingTime += bonusTimePerCombo * currentCombo;

            Debug.Log($"IClicked: Combo: {currentCombo}, Bonus Points: {bonusPoints}, Total Score: {totalScore}, Remaining Time: {remainingTime}");
        }

        /// <summary>
        /// 残り時間が0秒以下になったときのゲームオーバー処理
        /// </summary>
        private void GameOver()
        {
            Debug.Log("Game Over! Final Score: " + totalScore);
            // ここにゲームオーバー時の処理（画面遷移、リザルト表示など）を追加
        }
    }
}