using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogarithmicSliderWithDynamicButtons : MonoBehaviour
{
    public Slider slider;                  // Référence au Slider
    public TMP_Text valueText;            // Référence au TMP_Text pour afficher la valeur
    public Button plusButton;             // Référence au bouton "+"
    public Button minusButton;            // Référence au bouton "-"

    public float minValue = 1;            // Valeur minimale (réelle) du slider
    public float maxValue = 1000000;      // Valeur maximale (réelle) du slider

    void Start()
    {
        // Configure l'échelle logarithmique du slider
        slider.minValue = 0;  // Logarithmique interne (0 à 1 correspond à 10^min à 10^max)
        slider.maxValue = 1;

        // Ajoute les listeners pour les boutons + et -
        plusButton.onClick.AddListener(OnPlusButtonClicked);
        minusButton.onClick.AddListener(OnMinusButtonClicked);

        // Listener pour mettre à jour l'affichage
        slider.onValueChanged.AddListener(UpdateSliderLabel);
        UpdateSliderLabel(slider.value);  // Met à jour l'affichage initial
    }

    void UpdateSliderLabel(float value)
    {
        // Convertit la valeur logarithmique en une valeur réelle pour l'affichage
        float logValue = Mathf.Lerp(Mathf.Log10(minValue), Mathf.Log10(maxValue), value);
        float displayValue = Mathf.Pow(10, logValue); // Convertit en valeur linéaire

        // Met à jour le texte TMP
        valueText.text = "Valeur : \n" + displayValue.ToString("N2");
    }

    // Fonction appelée lorsque le bouton "+" est pressé
    void OnPlusButtonClicked()
    {
        AdjustSliderValue(1);
    }

    // Fonction appelée lorsque le bouton "-" est pressé
    void OnMinusButtonClicked()
    {
        AdjustSliderValue(-1);
    }

    void AdjustSliderValue(int direction)
    {
        // Convertit la valeur actuelle du slider en valeur réelle
        float logValue = Mathf.Lerp(Mathf.Log10(minValue), Mathf.Log10(maxValue), slider.value);
        float currentValue = Mathf.Pow(10, logValue);

        // Calcule l'ordre de grandeur actuel (exemple : 10, 100, 1000)
        float magnitude = Mathf.Pow(10, Mathf.Floor(Mathf.Log10(currentValue)));

        // Détermine la taille du pas en fonction de l'ordre de grandeur
        float step = magnitude / 10;  // Par exemple, 10x plus précis

        // Modifie la valeur réelle
        float newValue = Mathf.Clamp(currentValue + direction * step, minValue, maxValue);

        // Reconvertit en échelle logarithmique et met à jour le slider
        float newLogValue = Mathf.Log10(newValue);
        slider.value = Mathf.InverseLerp(Mathf.Log10(minValue), Mathf.Log10(maxValue), newLogValue);
    }
}

