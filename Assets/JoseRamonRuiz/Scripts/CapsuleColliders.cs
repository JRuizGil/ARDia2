using UnityEngine;

public class CapsuleColliders : MonoBehaviour
{
    private PlaneCounter planeCounter;

    private void Start()
    {
        planeCounter = FindObjectOfType<PlaneCounter>();

        if (planeCounter == null)
        {
            Debug.LogError("No se encontró ningún objeto con el script PlaneCounter en la escena.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (planeCounter != null)
            {
                planeCounter.CapsulesDestroyed++;
            }

            Destroy(gameObject);
        }
    }
}
