using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Transition;
using System.Collections.Generic;

[System.Serializable]
public struct ButtonScenePair
{
    public Button button;
    public string sceneName;
}

public class SceneTransitionManager : MonoBehaviour
{
    [Header("エフェクトオブジェクト（Inspectorで非アクティブ設定）")]
    [SerializeField] private GameObject[] effects;

    [Header("ボタンと遷移先シーンの設定")]
    [SerializeField] private List<ButtonScenePair> buttonScenePairs;

    [Header("シェーダートランジションコントローラ")]
    [SerializeField] private ShaderTransitionController shaderTransitionController;

    [Header("遷移前の遅延時間（秒）")]
    [SerializeField] private float transitionDelay = 2f;

    [Header("ボタンの音")]
    [SerializeField] AudioSource buttonSound;

    [Header("タイトル用（別のシーンではNull可）")]
    [SerializeField] GameObject IObject;
    Vector3 IscaleChange = new Vector3(0,0,0);
    void Start()
    {
        // エフェクトを非表示
        SetEffects(false);

        // 各ボタンにリスナー登録
        foreach (var pair in buttonScenePairs)
        {
            if (pair.button != null)
            {
                string scene = pair.sceneName;
                pair.button.onClick.AddListener(() => TriggerTransition(scene));
            }
        }
    }

    // エフェクトの表示/非表示をまとめて設定
    private void SetEffects(bool isActive)
    {
        foreach (var go in effects)
        {
            if (go != null)
                go.SetActive(isActive);
        }
    }

    // 任意シーンへの遷移を開始
    public void TriggerTransition(string sceneName)
    {
        SetEffects(true);
        PerformTransitionAsync(sceneName).Forget();
    }

    // 遷移共通処理
    private async UniTask PerformTransitionAsync(string sceneName)
    {
        buttonSound.Play();
        if(IObject != null)
            IObject.transform.localScale = IscaleChange;

        // 遅延
        await UniTask.Delay((int)(transitionDelay * 1000));

        // トランジション開始
        //if (shaderTransitionController != null)
        await shaderTransitionController.StartTransition();

        // シーンロード
        await SceneManager.LoadSceneAsync(sceneName);

        // トランジション終了
        //if (shaderTransitionController != null)
        await shaderTransitionController.EndTransition();
    }
}
