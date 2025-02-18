using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    // Configuración de las ruedas
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    public Transform frontLeftWheelMesh;
    public Transform frontRightWheelMesh;
    public Transform rearLeftWheelMesh;
    public Transform rearRightWheelMesh;

    // UI - Velocidad, vueltas y tiempos
    public TMP_Text textoVelocidadActual1;
    public TMP_Text textoVelocidadActual2;
    public TMP_Text textoVueltas1;
    public TMP_Text textoVueltas2;
    public TMP_Text textoTiempo1;
    public TMP_Text textoTiempo2;
    public TMP_Text textoTiemposVueltaCamara1;
    public TMP_Text textoTiemposVueltaCamara2;

    // Cámaras
    public Camera thirdPerson;
    public Camera firstPerson;

    // Parámetros del coche
    public float motorForce = 1500f;
    public float brakeForce = 3000f;
    public float maxSteerAngle = 30f;

    private float currentBrakeForce = 0f;
    private float currentSteerAngle = 0f;
    private float currentSpeed = 0f;

    private int vueltasCompletadas = 1;
    private float tiempoInicioVuelta;
    private float tiempoTranscurrido = 0f;

    private bool haPasadoCheckpoint1 = false;  // Marca si pasó el primer checkpoint
    private bool haPasadoCheckpoint2 = false;  // Marca si pasó el segundo checkpoint
    private bool carreraIniciada = false;
    private Rigidbody rb;

    private List<float> tiemposVueltas = new List<float>();
    public float mejorTiempo = Mathf.Infinity; // Variable para almacenar el mejor tiempo

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Configurar UI inicial
        thirdPerson.enabled = true;
        firstPerson.enabled = false;
        textoVelocidadActual1.enabled = true;
        textoVelocidadActual2.enabled = false;
        textoVueltas1.text = "Vuelta: 1/3";
        textoVueltas2.text = "Vuelta: 1/3";
        textoTiempo1.text = "Tiempo: 00:00.00";
        textoTiempo2.text = "Tiempo: 00:00.00";

        textoTiemposVueltaCamara1.text = "Tiempos de vuelta:\n";
        textoTiemposVueltaCamara2.text = "Tiempos de vuelta:\n";

        EliminarAudioListener(thirdPerson);
        EliminarAudioListener(firstPerson);

        StartCoroutine(ContadorInicio());
    }

    void Update()
    {
        if (!carreraIniciada) return;

        // Calcular velocidad y actualizar UI
        currentSpeed = rb.velocity.magnitude * 3.6f;
        textoVelocidadActual1.text = currentSpeed.ToString("0") + " km/h";
        textoVelocidadActual2.text = currentSpeed.ToString("0") + " km/h";

        // Actualizar tiempo transcurrido en la vuelta
        tiempoTranscurrido = Time.time - tiempoInicioVuelta;
        textoTiempo1.text = "Tiempo: " + FormatoTiempo(tiempoTranscurrido);
        textoTiempo2.text = "Tiempo: " + FormatoTiempo(tiempoTranscurrido);

        // Actualizar tiempos de vuelta en ambas cámaras
        ActualizarTiemposVueltas();

        CambiarCamara();
    }

    void FixedUpdate()
    {
        if (!carreraIniciada) return;

        float moveInput = Input.GetAxis("Vertical");
        float steerInput = Input.GetAxis("Horizontal");
        bool isBraking = Input.GetKey(KeyCode.Space);

        HandleMotor(moveInput, isBraking);
        HandleSteering(steerInput);
        UpdateWheels();
    }

    void HandleMotor(float moveInput, bool isBraking)
    {
        rearLeftWheel.motorTorque = moveInput * motorForce;
        rearRightWheel.motorTorque = moveInput * motorForce;

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBrake();
    }

    void ApplyBrake()
    {
        frontLeftWheel.brakeTorque = currentBrakeForce;
        frontRightWheel.brakeTorque = currentBrakeForce;
        rearLeftWheel.brakeTorque = currentBrakeForce;
        rearRightWheel.brakeTorque = currentBrakeForce;
    }

    void HandleSteering(float steerInput)
    {
        currentSteerAngle = steerInput * maxSteerAngle;
        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;
    }

    void UpdateWheels()
    {
        UpdateWheelPose(frontLeftWheel, frontLeftWheelMesh);
        UpdateWheelPose(frontRightWheel, frontRightWheelMesh);
        UpdateWheelPose(rearLeftWheel, rearLeftWheelMesh);
        UpdateWheelPose(rearRightWheel, rearRightWheelMesh);
    }

    void UpdateWheelPose(WheelCollider collider, Transform transform)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        transform.position = pos;
        transform.rotation = rot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            // Si el coche entra en un checkpoint, verificamos si es el primero o el segundo
            if (!haPasadoCheckpoint1)
            {
                haPasadoCheckpoint1 = true;  // Marca como que se ha pasado el primer checkpoint
                Debug.Log("Checkpoint 1 alcanzado");
            }
            else if (haPasadoCheckpoint1 && !haPasadoCheckpoint2)
            {
                haPasadoCheckpoint2 = true;  // Marca como que se ha pasado el segundo checkpoint
                Debug.Log("Checkpoint 2 alcanzado");
            }
        }
        else if (other.CompareTag("FinishLine") && haPasadoCheckpoint1 && haPasadoCheckpoint2)
        {
            // El coche solo puede cruzar la meta si ha pasado los dos checkpoints
            haPasadoCheckpoint1 = false;
            haPasadoCheckpoint2 = false;

            // Guardar el tiempo de la vuelta
            float tiempoVuelta = Time.time - tiempoInicioVuelta;
            tiemposVueltas.Add(tiempoVuelta);
            Debug.Log("Tiempo de vuelta: " + FormatoTiempo(tiempoVuelta));

            // Verificar y actualizar el mejor tiempo
            if (tiempoVuelta < mejorTiempo)
            {
                mejorTiempo = tiempoVuelta;
            }

            // Guardar el mejor tiempo en PlayerPrefs
            PlayerPrefs.SetFloat("MejorTiempo", mejorTiempo);

            // Reiniciar el tiempo para la siguiente vuelta
            tiempoInicioVuelta = Time.time;
            vueltasCompletadas++;

            // Actualizar el contador de vueltas
            textoVueltas1.text = "Vuelta: " + vueltasCompletadas + "/3";
            textoVueltas2.text = "Vuelta: " + vueltasCompletadas + "/3";

            if (vueltasCompletadas > 3)
            {
                // Finalizar carrera
                textoVueltas1.text = "Carrera finalizada!";
                textoVueltas2.text = "Carrera finalizada!";
                carreraIniciada = false;
                rb.isKinematic = true;

                // Cargar la nueva escena "acabar"
                SceneManager.LoadScene("acabar");
            }
        }
    }

    public void CambiarCamara()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            thirdPerson.enabled = !thirdPerson.enabled;
            firstPerson.enabled = !firstPerson.enabled;

            // Asegurar que los textos de la UI de ambas cámaras sigan visibles
            bool primeraPersonaActiva = firstPerson.enabled;

            textoVelocidadActual1.enabled = !primeraPersonaActiva;
            textoVelocidadActual2.enabled = primeraPersonaActiva;
            textoVueltas1.enabled = !primeraPersonaActiva;
            textoVueltas2.enabled = primeraPersonaActiva;
            textoTiempo1.enabled = !primeraPersonaActiva;
            textoTiempo2.enabled = primeraPersonaActiva;
            textoTiemposVueltaCamara1.enabled = !primeraPersonaActiva;
            textoTiemposVueltaCamara2.enabled = primeraPersonaActiva;
        }
    }


    private IEnumerator ContadorInicio()
    {
        for (int i = 3; i > 0; i--)
        {
            textoTiempo1.text = "Comienza en " + i;
            textoTiempo2.text = "Comienza en " + i;
            yield return new WaitForSeconds(1);
        }

        textoTiempo1.text = "YA!";
        textoTiempo2.text = "YA!";
        yield return new WaitForSeconds(1);

        textoTiempo1.enabled = true;
        textoTiempo2.enabled = true;

        carreraIniciada = true;
        rb.isKinematic = false;
        tiempoInicioVuelta = Time.time;
    }

    public void ActualizarTiemposVueltas()
    {
        textoTiemposVueltaCamara1.text = "Tiempos de vuelta:\n";
        textoTiemposVueltaCamara2.text = "Tiempos de vuelta:\n";

        for (int i = 0; i < tiemposVueltas.Count; i++)
        {
            string tiempo = FormatoTiempo(tiemposVueltas[i]);
            textoTiemposVueltaCamara1.text += $"Vuelta {i + 1}: {tiempo}\n";
            textoTiemposVueltaCamara2.text += $"Vuelta {i + 1}: {tiempo}\n";
        }
    }

    public string FormatoTiempo(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        int milisegundos = Mathf.FloorToInt((tiempo * 100) % 100);

        return string.Format("{0:00}:{1:00}.{2:00}", minutos, segundos, milisegundos);
    }

    void EliminarAudioListener(Camera cam)
    {
        AudioListener listener = cam.GetComponent<AudioListener>();
        if (listener != null) Destroy(listener);
    }
}