using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public RectTransform TopMenu;
    public RectTransform BottomMenu;
    public TextMeshProUGUI Title;
    public RectTransform Credits;
    public RectTransform PlayOptions;
    public RectTransform OptionsMenu;
    public DOTweenAnimation pressAnyFade;
    public List<TextMeshProUGUI> title = new List<TextMeshProUGUI>();
    public AudioClip click;

    private bool onMenu;
    private AudioSource source;


    private void Start()
    {
        source = GetComponent<AudioSource>();
       
    }

    // Did you know: Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !onMenu)
        {
            pressAnyFade.DOKill();
            pressAnyFade.GetComponent<TextMeshProUGUI>().DOFade(0, .2f);
            TopMenu.DOAnchorPosX(0, 1.0f);
            BottomMenu.DOAnchorPosX(0, 1.0f);
            Title.DOFade(1f, 0.5f);
            onMenu = true;
        }
    }

    public void OptionsTransition()
    {
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(-1100, 1.0f);
        BottomMenu.DOAnchorPosX(1100, 1.0f);
        Title.DOFade(0f, 0.5f);
        OptionsMenu.DOAnchorPosX(0f, 1.0f);
    }

    public void OptionsExitTransition(){
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(0, 1.0f);
        BottomMenu.DOAnchorPosX(0, 1.0f);
        Title.DOFade(1f, 0.5f);
        OptionsMenu.DOAnchorPosX(2000f, 1.0f);
    }

    public void CreditsFlyIn()
    {
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(-1100, 1.0f);
        BottomMenu.DOAnchorPosX(1100, 1.0f);
        Title.DOFade(0f, 0.5f);
        Credits.DOAnchorPosY(0f, 2f);
    }

    public void CreditsFlyOut()
    {
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(0, 1.0f);
        BottomMenu.DOAnchorPosX(0, 1.0f);
        Title.DOFade(1f, 0.5f);
        Credits.DOAnchorPosY(1100f, 1.0f);
    }

    public void PlayOptionsFlyIn(){
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(-1100, 1.0f);
        BottomMenu.DOAnchorPosX(1100, 1.0f);
        PlayOptions.DOAnchorPosY(-75f, 1.0f);
    }

    
    public void PlayOptionsFlyOut()
    {
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(0, 1.0f);
        BottomMenu.DOAnchorPosX(0, 1.0f);
        PlayOptions.DOAnchorPosY(-1100f, 1.0f);
    }

    public void QuitGame()
    {
        source.PlayOneShot(click);
        Application.Quit();
    }

    public void PlayPong()
    {
        source.PlayOneShot(click);
        SceneLoader.Instance.LoadSceneWithScreen("Pong");
    }
}
