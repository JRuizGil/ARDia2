using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class PlaneCounter : MonoBehaviour
{
    ButtonManager buttonManager;
    public ARPlaneManager planeManager;
    public Text horizontalPlaneText;
    public Text verticalPlaneText;

    public GameObject successPanel;
    public GameObject PreGamePanel;

    public GameObject capsulePrefab;
    public GameObject EndGame;

    private int requiredHorizontal;
    private int requiredVertical;
    private bool goalReached = false;

    public float CapsulesDestroyed;
    public Text CapsulesDestroyedtxt;

    private float countdownTime;          // Tiempo total en segundos
    private float currentTime;            // Tiempo actual restante
    private bool countdownActive = false; // Estado del contador

    public Text timerText;                // UI Text para mostrar el tiempo


    void Start()
    {
        EndGame.SetActive(false);
        // Valores por defecto si PlayerPrefs no tiene datos
        requiredHorizontal = PlayerPrefs.GetInt("RequiredHorizontal", (int)PlayerPrefs.GetFloat("Slider2Value", 5));
        requiredVertical = PlayerPrefs.GetInt("RequiredVertical", (int)PlayerPrefs.GetFloat("Slider3Value", 5));
        
        Debug.Log($"Se requieren: Horizontales={requiredHorizontal}, Verticales={requiredVertical}");

        if (successPanel != null)
            successPanel.SetActive(false);
        
    }
    private void LateUpdate()
    {
        CapsulesDestroyedtxt.text = "Gemas Encontradas: " + CapsulesDestroyed;
        if (CapsulesDestroyed >= requiredHorizontal + requiredVertical)
        {
            EndGame.SetActive(true);
        }
    }
    void Update()
    {
        if (planeManager != null && horizontalPlaneText != null && verticalPlaneText != null)
        {
            int horizontalCount = 0;
            int verticalCount = 0;

            foreach (ARPlane plane in planeManager.trackables)
            {
                if (!plane.gameObject.activeInHierarchy) continue;

                if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
                    horizontalCount++;
                else if (plane.alignment == PlaneAlignment.Vertical)
                    verticalCount++;
            }

            horizontalPlaneText.text = "Horizontales: " + horizontalCount;
            verticalPlaneText.text = "Verticales: " + verticalCount;

            if (!goalReached && horizontalCount >= requiredHorizontal && verticalCount >= requiredVertical)
            {
                goalReached = true;
                successPanel.SetActive(true);
            }
        }

        if (countdownActive)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                countdownActive = false;
                Debug.Log("¡Tiempo terminado!");
                buttonManager.ChangeToMenuScene(0);
            }

            if (timerText != null)
            {
                timerText.text = "Tiempo: " + currentTime.ToString("F1") + "s";
            }
        }
    }

    public void SpawnCapsulesOnPlanes()
    {
        if (capsulePrefab == null || planeManager == null)
        {
            Debug.LogError("Capsule prefab o planeManager no asignado.");
            return;
        }

        Debug.Log($"Instanciando cápsulas... Requiere: H {requiredHorizontal} / V {requiredVertical}");
        // Inicia el temporizador al finalizar la instanciación
        countdownTime = PlayerPrefs.GetFloat("Slider1Value", 30f); // Valor por defecto 30 segundos
        currentTime = countdownTime;
        countdownActive = true;
        Debug.Log($"Temporizador iniciado con {countdownTime} segundos.");

        int horizontalSpawned = 0;
        int verticalSpawned = 0;
        int totalPlanes = 0;

        foreach (ARPlane plane in planeManager.trackables)
        {
            if (!plane.gameObject.activeInHierarchy)
            {
                Debug.Log("Plano inactivo ignorado.");
                continue;
            }

            totalPlanes++;

            Debug.Log($"Plano detectado: {plane.alignment}");

            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
            {
                if (horizontalSpawned < requiredHorizontal)
                {
                    SpawnCapsuleOnPlane(plane, false);
                    horizontalSpawned++;
                    Debug.Log("Cápsula horizontal instanciada.");
                }
            }
            else if (plane.alignment == PlaneAlignment.Vertical)
            {
                if (verticalSpawned < requiredVertical)
                {
                    SpawnCapsuleOnPlane(plane, true);
                    verticalSpawned++;
                    Debug.Log("Cápsula vertical instanciada.");
                }
            }

            if (horizontalSpawned >= requiredHorizontal && verticalSpawned >= requiredVertical)
                break;
        }

        Debug.Log($"Total de planos recorridos: {totalPlanes}");
        Debug.Log($"Instanciadas: Horizontales = {horizontalSpawned}, Verticales = {verticalSpawned}");
    }


    private void SpawnCapsuleOnPlane(ARPlane plane, bool isVertical)
    {
        // Centro del plano en coordenadas locales -> transformarlas a globales
        Vector3 center = plane.transform.TransformPoint(plane.center);

        // Agregar variación para evitar solapamiento
        Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), 0.05f, Random.Range(-0.2f, 0.2f));
        Vector3 spawnPosition = center + offset;

        // Rotación: si es vertical, que mire hacia adelante del plano
        Quaternion rotation = isVertical ? Quaternion.LookRotation(plane.transform.forward) : Quaternion.identity;

        Instantiate(capsulePrefab, spawnPosition, rotation);
    }
}
