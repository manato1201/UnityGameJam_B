using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using GM;

public class ClickToDestroyEffect : MonoBehaviour
{
    public GameObject effectPrefab;      // エフェクト用のプレハブ
    public AudioClip sound;              // クリック時の音声
    public Material transitionMaterial;  // Sad時に操作するマテリアル
    public float valueDecrease = 0.1f;   // Sad時に減少させる値

    private AudioSource audioSource;
    public Color flashColor = Color.yellow; // フラッシュ時の色
    public float flashDuration = 0.1f;      // フラッシュ時間（秒）
    [Header("Loveクリック時に表示する別オブジェクト（Inspectorで非アクティブ状態にしておく）")]
    public GameObject otherObjectToShow;


    void Start()
    {
        audioSource = Camera.main.GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        // 共通：エフェクト生成＆音再生
        if (effectPrefab != null)
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        if (sound != null && audioSource != null)
            audioSource.PlayOneShot(sound);

        // タグ判定
        if (gameObject.tag == "Love")
        {
            Debug.Log("Love");
            if (transitionMaterial != null)
            {
                float v = transitionMaterial.GetFloat("_Value");
                transitionMaterial.SetFloat("_Value", v + valueDecrease);
            }
            FlashIObjectsAsync().Forget();
            Destroy(gameObject);
        }
        else if (gameObject.tag == "Sad")
        {
            Debug.Log("Sad");
            if (transitionMaterial != null)
            {
                float v = transitionMaterial.GetFloat("_Value");
                transitionMaterial.SetFloat("_Value", v - valueDecrease);
            }
            Destroy(gameObject);
        }
        else if (gameObject.tag == "I") {
            GameManager.Instance.IClicked();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // "I" タグのオブジェクト全てを一瞬フラッシュさせる
    async UniTaskVoid FlashIObjectsAsync()
    {

        // 別オブジェクトを表示
        if (otherObjectToShow != null)
            otherObjectToShow.SetActive(true);

        var objs = GameObject.FindGameObjectsWithTag("I");
        var rends = new List<Renderer>();
        var originals = new List<Color>();

        Debug.Log("LoveLove");
        // 色を変更
        foreach (var obj in objs)
        {
            var rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                rends.Add(rend);
                originals.Add(rend.material.color);
                rend.material.color = flashColor;
            }
        }

        // 指定時間待機
        await UniTask.Delay((int)(flashDuration * 1000));
        otherObjectToShow.SetActive(false);
        // 元に戻す
        for (int i = 0; i < rends.Count; i++)
        {
            rends[i].material.color = originals[i];
        }
    }
}
