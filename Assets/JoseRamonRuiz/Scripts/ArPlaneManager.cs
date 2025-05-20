using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class PlaneCounter : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public Text horizontalPlaneText;
    public Text verticalPlaneText;

    public GameObject successPanel; 
    public GameObject PreGamePanel;

    private int requiredHorizontal;
    private int requiredVertical;
    private bool goalReached = false;

    public GameObject capsulePrefab; // Prefab de cápsula a instanciar

    void Start()
    {
        // Cargar los valores mínimos requeridos desde PlayerPrefs
        requiredHorizontal = PlayerPrefs.GetInt("RequiredHorizontal", (int)PlayerPrefs.GetFloat("Slider2Value", 0));
        requiredVertical = PlayerPrefs.GetInt("RequiredVertical", (int)PlayerPrefs.GetFloat("Slider3Value", 0));

        if (successPanel != null)
            successPanel.SetActive(false);
    }

    void Update()
    {
        if (planeManager != null && horizontalPlaneText != null && verticalPlaneText != null)
        {
            int horizontalCount = 0;
            int verticalCount = 0;

            foreach (ARPlane plane in planeManager.trackables)
            {
                if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
                {
                    horizontalCount++;
                }
                else if (plane.alignment == PlaneAlignment.Vertical)
                {
                    verticalCount++;
                }
            }

            horizontalPlaneText.text = "Horizontales: " + horizontalCount;
            verticalPlaneText.text = "Verticales: " + verticalCount;

            // Verifica si se alcanzó el mínimo y aún no se ha cumplido antes
            if (!goalReached && horizontalCount >= requiredHorizontal && verticalCount >= requiredVertical)
            {
                goalReached = true;
                successPanel.SetActive(true);
            }
        }
    }
    public void SpawnCapsulesOnPlanes()
    {
        if (capsulePrefab == null || planeManager == null) return;

        int horizontalSpawned = 0;
        int verticalSpawned = 0;

        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown)
            {
                if (horizontalSpawned < requiredHorizontal)
                {
                    SpawnCapsuleOnPlane(plane);
                    horizontalSpawned++;
                }
            }
            else if (plane.alignment == PlaneAlignment.Vertical)
            {
                if (verticalSpawned < requiredVertical)
                {
                    SpawnCapsuleOnPlane(plane);
                    verticalSpawned++;
                }
            }

            // Salimos si ya alcanzamos ambos mínimos
            if (horizontalSpawned >= requiredHorizontal && verticalSpawned >= requiredVertical)
                break;
        }
    }

    private void SpawnCapsuleOnPlane(ARPlane plane)
    {
        Vector3 randomPoint = plane.transform.position;

        // Opcional: aplicar una pequeña aleatoriedad para no apilar todas las cápsulas
        randomPoint += new Vector3(Random.Range(-0.2f, 0.2f), 0.05f, Random.Range(-0.2f, 0.2f));

        Instantiate(capsulePrefab, randomPoint, Quaternion.identity);
    }
}
