using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Toggle toggle;

    public Text text1;
    public Text text2;
    public Text text3;

    void Start()
    {
        // Cargar valores guardados
        slider1.value = PlayerPrefs.GetFloat("Slider1Value", slider1.value);
        slider2.value = PlayerPrefs.GetFloat("Slider2Value", slider2.value);
        slider3.value = PlayerPrefs.GetFloat("Slider3Value", slider3.value);
        toggle.isOn = PlayerPrefs.GetInt("ToggleValue", toggle.isOn ? 1 : 0) == 1;

        // Mostrar valores iniciales
        UpdateTexts();
    }
    private void Update()
    {
        UpdateTexts(); // Actualiza los textos en cada frame
    }
    void UpdateTexts()
    {
        text1.text = "Tiempo de búsqueda: " + slider1.value.ToString("F0");
        text2.text = "Gemas Horizontales: " + slider2.value.ToString("F0");
        text3.text = "Gemas Verticales: " + slider3.value.ToString("F0");
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Slider1Value", slider1.value);
        PlayerPrefs.SetFloat("Slider2Value", slider2.value);
        PlayerPrefs.SetFloat("Slider3Value", slider3.value);
        PlayerPrefs.SetInt("ToggleValue", toggle.isOn ? 1 : 0);

        PlayerPrefs.Save();
    }

}
