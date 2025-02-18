using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace menu
{
    public class SeleccionGP : MonoBehaviour
{
    public Button playMonaco;
    public Button playAustralia;


    void Start()
    {
        playMonaco.onClick.AddListener(OnPlayMonacoClick);
        playAustralia.onClick.AddListener(OnPlayAustraliaClick);

    }

    private void OnPlayMonacoClick()
    {
        SceneManager.LoadScene("GPMonaco");
    }

    private void OnPlayAustraliaClick()
    {

        SceneManager.LoadScene("GPAutralia");
    }
}
}
