using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CarreraFinalizadaController : MonoBehaviour
{
    // UI
    public TMP_Text textoMejorTiempo;
    // Método para volver al menú
    public Button playButton;
    public Button exit;


    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        exit.onClick.AddListener(OnExitButtonClick);

        // Obtener el mejor tiempo desde PlayerPrefs y mostrarlo en el UI
        float mejorTiempo = PlayerPrefs.GetFloat("MejorTiempo", Mathf.Infinity);
        textoMejorTiempo.text = FormatoTiempo(mejorTiempo);
    }

    // Método para dar formato al tiempo
    private string FormatoTiempo(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        int milisegundos = Mathf.FloorToInt((tiempo * 100) % 100);

        return string.Format("{0:00}:{1:00}.{2:00}", minutos, segundos, milisegundos);
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
