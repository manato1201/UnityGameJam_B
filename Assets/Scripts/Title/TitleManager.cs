using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Transition;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    //[SerializeField] GameObject[] Effect;
    //[SerializeField] Button startButton;
    //[SerializeField] private ShaderTransitionController shaderTransitionController;
    //[SerializeField] AudioSource ButtonSound;
    //[SerializeField] GameObject IObject;
    [SerializeField] AudioSource titleBGM;
    [SerializeField] AudioClip titleIntro;
    [SerializeField] AudioClip titleroop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Effect[0].SetActive(false);
        //Effect[1].SetActive(false);
        //Effect[2].SetActive(false);
        //Effect[3].SetActive(false);

        titleBGM.clip = titleIntro;
        titleBGM.Play();
    }


    void Update()
    {
        if (!titleBGM.isPlaying)
        {
            titleBGM.clip = titleroop;
            titleBGM.Play();
        }
    }

    //public void EffectOn()
    //{
    //    //SceneManager.LoadScene("Main");
    //    Effect[0].SetActive(true);
    //    Effect[1].SetActive(true);
    //    Effect[2].SetActive(true);
    //    Effect[3].SetActive(true);
    //    ButtonSound.PlayOneShot(ButtonSound.clip); // 効果音を再生
    //    Debug.Log(ButtonSound.isPlaying);
    //    GoToMain().Forget();

    //}

    //public void EffectOff()
    //{
    //}

    //public async UniTask  GoToMain()
    //{

    //    await UniTask.WaitUntil(() => !ButtonSound.isPlaying);
    //    //await UniTask.Delay((int)(ButtonSound.clip.length));
    //    //await UniTask.Delay(2000);
    //    await shaderTransitionController.StartTransition();

    //    await SceneManager.LoadSceneAsync("Main");
    //    await shaderTransitionController.EndTransition();
    //}
}
