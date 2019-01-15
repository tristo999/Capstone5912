using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MainMenuController : MonoBehaviour
{

    public DOTweenAnimation pressAnyFade;
    public DOTweenAnimation menuFlyIn;
    public DOTweenAnimation creditsFlyIn;
    public DOTweenAnimation optionsFlyIn;
    public List<TextMeshProUGUI> title = new List<TextMeshProUGUI>();

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
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
}
