using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public DOTweenAnimation pressAnyFade;
    public DOTweenAnimation menuFlyIn;
    public DOTweenAnimation creditsFlyIn;
    public DOTweenAnimation optionsFlyIn;
    public List<TextMeshProUGUI> title = new List<TextMeshProUGUI>();
    public AudioClip click;

    private AudioSource source;


    private void Start()
    {
        source = GetComponent<AudioSource>();
       
    }

    // Update is called once per frame
    void Update()
    {
      
        if (Input.anyKeyDown)
        {
            source.PlayOneShot(click);
            pressAnyFade.DOKill();
            pressAnyFade.GetComponent<TextMeshProUGUI>().DOFade(0, .2f);
            menuFlyIn.DOPlayAllById("menuFlyIn");
        }
    }

    public void OptionsTransition()
    {
        menuFlyIn.DOPlayBackwardsAllById("menuFlyIn");
        title.ForEach(x => x.DOFade(0, .5f));
    }

    public void CreditsFlyIn()
    {
        menuFlyIn.DOPlayBackwardsAllById("menuFlyIn");
        title.ForEach(x => x.DOFade(0, .5f));
        creditsFlyIn.DOPlay();
    }

    public void CreditsFlyOut()
    {
        menuFlyIn.DORestartAllById("menuFlyIn");
        title.ForEach(x => x.DOFade(1, .5f));
        creditsFlyIn.DORewind();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayPong()
    {
        SceneLoader.Instance.LoadSceneWithScreen("Pong");
    }
}
