using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using Rewired;
using UnityEngine.EventSystems;

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
    private IList<Player> players;
    public EventSystem eventSystem;


    private void Start()
    {
        source = GetComponent<AudioSource>();
        players = ReInput.players.GetPlayers();
       
    }

    bool getAnyButtonDown(IList<Player> players){
        foreach(Player player in players){
            if(player.GetAnyButtonDown()) return true;
        }
        return false;
    }

    // Did you know: Update is called once per frame
    void Update()
    {
        if (getAnyButtonDown(players) && !onMenu)
        {
            pressAnyFade.DOKill();
            pressAnyFade.GetComponent<TextMeshProUGUI>().DOFade(0, .2f);
            TopMenu.DOAnchorPosX(0, 1.0f);
            BottomMenu.DOAnchorPosX(0, 1.0f);
            Title.DOFade(1f, 0.5f);
            eventSystem.SetSelectedGameObject(GameObject.Find("Top Menus/Play"));
            onMenu = true;
        }
    }

    public void OptionsTransition()
    {
        OptionsMenu.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Options Menu/Sliders/Master"));
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(-1100, 1.0f);
        BottomMenu.DOAnchorPosX(1100, 1.0f);
        Title.DOFade(0f, 0.5f);
        OptionsMenu.DOAnchorPosX(0f, 1.0f);
        TopMenu.gameObject.SetActive(false);
        BottomMenu.gameObject.SetActive(false);
    }

    public void OptionsExitTransition(){
        TopMenu.gameObject.SetActive(true);
        BottomMenu.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Top Menus/Play"));
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(0, 1.0f);
        BottomMenu.DOAnchorPosX(0, 1.0f);
        Title.DOFade(1f, 0.5f);
        OptionsMenu.DOAnchorPosX(2000f, 1.0f);
        OptionsMenu.gameObject.SetActive(false);
    }

    public void CreditsFlyIn()
    {
        Credits.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Credits Menu/Back Button"));
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(-1100, 1.0f);
        BottomMenu.DOAnchorPosX(1100, 1.0f);
        Title.DOFade(0f, 0.5f);
        Credits.DOAnchorPosY(0f, 2f);
        TopMenu.gameObject.SetActive(false);
        BottomMenu.gameObject.SetActive(false);
    }

    public void CreditsFlyOut()
    {
        TopMenu.gameObject.SetActive(true);
        BottomMenu.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Top Menus/Play"));
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(0, 1.0f);
        BottomMenu.DOAnchorPosX(0, 1.0f);
        Title.DOFade(1f, 0.5f);
        Credits.DOAnchorPosY(1100f, 1.0f);
        Credits.gameObject.SetActive(false);
    }

    public void PlayOptionsFlyIn(){
        PlayOptions.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Play Buttons/PlayLocal"));
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(-1100, 1.0f);
        BottomMenu.DOAnchorPosX(1100, 1.0f);
        PlayOptions.DOAnchorPosY(-75f, 1.0f);
        TopMenu.gameObject.SetActive(false);
        BottomMenu.gameObject.SetActive(false);
    }

    
    public void PlayOptionsFlyOut()
    {
        TopMenu.gameObject.SetActive(true);
        BottomMenu.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(GameObject.Find("Top Menus/Play"));
        source.PlayOneShot(click);
        TopMenu.DOAnchorPosX(0, 1.0f);
        BottomMenu.DOAnchorPosX(0, 1.0f);
        PlayOptions.DOAnchorPosY(-1100f, 1.0f);
        PlayOptions.gameObject.SetActive(false);
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
