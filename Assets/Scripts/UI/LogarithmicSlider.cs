using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogarithmicSliderWithDynamicButtons : MonoBehaviour
{
    public Slider slider;                  // R�f�rence au Slider
    public TMP_Text valueText;            // R�f�rence au TMP_Text pour afficher la valeur
    public Button plusButton;             // R�f�rence au bouton "+"
    public Button minusButton;            // R�f�rence au bouton "-"

    public float minValue = 1;            // Valeur minimale (r�elle) du slider
    public float maxValue = 1000000;      // Valeur maximale (r�elle) du slider

    void Start()
    {
        // Configure l'�chelle logarithmique du slider
        slider.minValue = 0;  // Logarithmique interne (0 � 1 correspond � 10^min � 10^max)
        slider.maxValue = 1;

        // Ajoute les listeners pour les boutons + et -
        plusButton.onClick.AddListener(OnPlusButtonClicked);
        minusButton.onClick.AddListener(OnMinusButtonClicked);

        // Listener pour mettre � jour l'affichage
        slider.onValueChanged.AddListener(UpdateSliderLabel);
        UpdateSliderLabel(slider.value);  // Met � jour l'affichage initial
    }

    void UpdateSliderLabel(float value)
    {
        // Convertit la valeur logarithmique en une valeur r�elle pour l'affichage
        float logValue = Mathf.Lerp(Mathf.Log10(minValue), Mathf.Log10(maxValue), value);
        float displayValue = Mathf.Pow(10, logValue); // Convertit en valeur lin�aire

        // Met � jour le texte TMP
        valueText.text = "Valeur : \n" + displayValue.ToString("N2");
    }

    // Fonction appel�e lorsque le bouton "+" est press�
    void OnPlusButtonClicked()
    {
        AdjustSliderValue(1);
    }

    // Fonction appel�e lorsque le bouton "-" est press�
    void OnMinusButtonClicked()
    {
        AdjustSliderValue(-1);
    }

    void AdjustSliderValue(int direction)
    {
        // Convertit la valeur actuelle du slider en valeur r�elle
        float logValue = Mathf.Lerp(Mathf.Log10(minValue), Mathf.Log10(maxValue), slider.value);
        float currentValue = Mathf.Pow(10, logValue);

        // Calcule l'ordre de grandeur actuel (exemple : 10, 100, 1000)
        float magnitude = Mathf.Pow(10, Mathf.Floor(Mathf.Log10(currentValue)));

        // D�termine la taille du pas en fonction de l'ordre de grandeur
        float step = magnitude / 10;  // Par exemple, 10x plus pr�cis

        // Modifie la valeur r�elle
        float newValue = Mathf.Clamp(currentValue + direction * step, minValue, maxValue);

        // Reconvertit en �chelle logarithmique et met � jour le slider
        float newLogValue = Mathf.Log10(newValue);
        slider.value = Mathf.InverseLerp(Mathf.Log10(minValue), Mathf.Log10(maxValue), newLogValue);
    }
}

