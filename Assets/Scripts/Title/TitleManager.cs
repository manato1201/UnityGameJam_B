using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Transition;

public class TitleManager : MonoBehaviour
{
    [SerializeField] GameObject[] Effect;
    [SerializeField] Button startButton;
    [SerializeField] private ShaderTransitionController shaderTransitionController;
    [SerializeField] AudioSource ButtonAudio;
    [SerializeField] GameObject IObject;
    private Vector3 IScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IScale = new Vector3(1,1,1);
        Effect[0].SetActive(false);
        Effect[1].SetActive(false);
        Effect[2].SetActive(false);
        Effect[3].SetActive(false);
    }

    // Update is called once per frame
    //void Update()
    //{
    //startButton.onClick.AddListener(EffectOn);
    //}

    public void EffectOn()
    {
        //SceneManager.LoadScene("Main");
        Effect[0].SetActive(true);
        Effect[1].SetActive(true);
        Effect[2].SetActive(true);
        Effect[3].SetActive(true);


        ButtonAudio.Play();
        while (true)
        {
            if (IObject.transform.localScale.x >= 0.0f)
            {
                IObject.transform.localScale -= IScale;
            }
            else
            {
                break;
            }
        }



        GoToMain().Forget();


    }

    public void EffectOff()
    {

    }



    public async UniTask  GoToMain()
    {

        await UniTask.Delay(2000);
        await shaderTransitionController.StartTransition();

        await SceneManager.LoadSceneAsync("Main");
        await shaderTransitionController.EndTransition();
    }
}
