using UnityEngine;

public class BGM : MonoBehaviour
{

    [SerializeField] AudioSource mainBGM;
    [SerializeField] AudioClip main;
    [SerializeField] AudioClip mainIntro;
    [SerializeField] AudioClip mainroop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!Select.isEndless)
        {
            mainBGM.clip = main;
            mainBGM.loop = true;
            mainBGM.Play();
        }
        else
        {
            mainBGM.clip = mainIntro;
            mainBGM.Play();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainBGM.isPlaying && Select.isEndless)
        {
            mainBGM.clip = mainIntro;
            mainBGM.Play();
        }
    }
}
