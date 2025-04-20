using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using GM;

public class ClickToDestroyEffect : MonoBehaviour
{
    public GameObject effectPrefab;      // エフェクト用のプレハブ]
    public AudioClip sound;              // クリック時の音声
    public Material transitionMaterial;  // Sad時に操作するマテリアル
    public float valueDecrease = 0.1f;   // Sad時に減少させる値

    private AudioSource audioSource;
    public Color flashColor = Color.yellow; // フラッシュ時の色
    public float flashDuration = 0.1f;      // フラッシュ時間（秒）
   


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
            audioSource.PlayOneShot(sound);
            Debug.Log("Love");
            if (transitionMaterial != null)
            {
                float v = transitionMaterial.GetFloat("_Value");
                transitionMaterial.SetFloat("_Value", v + valueDecrease);
            }
            // Iタグのオブジェクトの回転・移動を停止
            var iObjects = GameObject.FindGameObjectsWithTag("I");
            foreach (var obj in iObjects)
            {
                var mover = obj.GetComponent<RandomMover>();
                if (mover != null)
                    mover.enabled = false;
            }
            // FlashIObjectsAsync().Forget();
                Destroy(gameObject);
        }
        else if (gameObject.tag == "Sad")
        {

            audioSource.PlayOneShot(sound);
            Debug.Log("Sad");
            
            if (transitionMaterial != null)
            {
                float v = transitionMaterial.GetFloat("_Value");
                transitionMaterial.SetFloat("_Value", v - valueDecrease);
                if (v==0) GameManager.Instance.GameOver();



            }
                Destroy(gameObject);
        }
        else if (gameObject.tag == "I") {
            audioSource.PlayOneShot(sound);
            GameManager.Instance.IClicked();
                Destroy(gameObject);
        }
        else
        {
            audioSource.PlayOneShot(sound);
                Destroy(gameObject);
        }
    }

    // "I" タグのオブジェクト全てを一瞬フラッシュさせる
    async UniTaskVoid FlashIObjectsAsync()
    {

      

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
        
        // 元に戻す
        for (int i = 0; i < rends.Count; i++)
        {
            rends[i].material.color = originals[i];
        }
    }
}
