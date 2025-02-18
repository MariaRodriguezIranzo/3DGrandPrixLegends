using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace menu { 

public class LoadingScreen : MonoBehaviour
{
    public Button playButton;
    public Button exit;


    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        exit.onClick.AddListener(OnExitButtonClick);

    }

    private void OnPlayButtonClick()
    {
        
        SceneManager.LoadScene("SeleccionCircuito");
    }

    private void OnExitButtonClick()
    {
       
        Application.Quit();
    }
}
}