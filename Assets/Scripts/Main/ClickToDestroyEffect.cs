using UnityEngine;
using Cysharp.Threading.Tasks;

public class ClickToDestroyEffect : MonoBehaviour
{
    public GameObject effectPrefab; // エフェクト用のプレハブ
    public AudioClip sound;         // クリック時の音声
    private AudioSource audioSource;

    // フラッシュ用の設定
    public Color flashColor = Color.yellow; // フラッシュ時の色
    public float flashDuration = 0.1f;        // フラッシュ時間（秒）

    private Color originalColor;              // 元の色

    void Start()
    {
        // AudioSourceはメインカメラにアタッチされている前提
        audioSource = Camera.main.GetComponent<AudioSource>();
        // Rendererがある場合、元の色を保存
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    void OnMouseDown()
    {
        // エフェクト生成
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }

        // 音再生
        if (sound != null && audioSource != null)
        {
            audioSource.PlayOneShot(sound);
        }

        // タグが "Love" の場合はフラッシュ処理を行う
        if (gameObject.tag == "Love")
        {
            // asyncメソッドは UniTaskVoid で呼び出し、返り値を無視するため .Forget() を利用
            FlashAndDestroyAsync().Forget();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // async/awaitを利用してフラッシュ処理後にオブジェクトを破棄する
    async UniTaskVoid FlashAndDestroyAsync()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            // フラッシュ用に色を変更
            rend.material.color = flashColor;
        }

        // flashDuration 秒（ミリ秒に変換）待つ
        await UniTask.Delay((int)(flashDuration * 1000));

        // （必要なら）元の色に戻す
        if (rend != null)
        {
            rend.material.color = originalColor;
        }

        // オブジェクトを破棄
        Destroy(gameObject);
    }
}
