using UnityEngine;
namespace menu
{

    public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        // Verificar si ya existe una instancia
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // No destruir en el cambio de escena
        }
        else
        {
            Destroy(gameObject); // Destruir el duplicado
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
           
        }
        else
        {
            audioSource.Play(); // Reproducir la música al inicio
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
}