using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class PastelUIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI buttonText; // Référence au texte du bouton
    private Image buttonBackground; // Référence à l'image de fond du bouton

    // Couleurs modernes et pastel
    private Color normalColor = HexToColor("#EAC4d5");// Rose
    private Color pressedColor = HexToColor("#809bce"); // Bleu
    public Color32 colorText= HexToColor("#525252");   

    private float animationSpeed = 0.2f; // Vitesse de transition des couleurs
    private float scaleFactor = 0.9f; // Facteur de rétrécissement du bouton lors de l'appui
    private float textSize = 60f; // Taille du texte pour qu'il soit lisible et adapté
    private float outlineWidth = 3f; // Largeur du contour noir
    private Vector3 initialScale; // Enregistrement de la taille initiale du bouton

    private void Start()
    {
        // Enregistrer la taille initiale du bouton
        initialScale = transform.localScale;
        buttonBackground=GetComponent<Image>();
        // Applique les couleurs de fond dès le début
        if (buttonBackground != null)
        {
            buttonBackground.color = normalColor; // Applique la couleur normale
        }

        // Applique la couleur et la taille du texte
        if (buttonText != null)
        {
            buttonText.color = colorText; // Applique la couleur du texte
            buttonText.fontSize = textSize; // Applique une taille de texte adaptée
            buttonText.alignment = TextAlignmentOptions.Center; // Centre le texte dans le bouton

            // Ajouter un contour au texte
            Outline textOutline = buttonText.gameObject.AddComponent<Outline>();
            textOutline.effectDistance = new Vector2(outlineWidth, outlineWidth); // Largeur du contour
        }
    }
    public static Color HexToColor(string hex)
    {
        Color color;
        if (UnityEngine.ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Code couleur hexadécimal invalide !");
            return Color.white; // Retourne une couleur par défaut si l'hexadécimal est invalide
        }
    }
    // Quand le bouton est appuyé
    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonBackground != null)
        {
            StopAllCoroutines(); // Arrête toute animation précédente
            StartCoroutine(ChangeColor(buttonBackground.color, pressedColor)); // Applique la couleur pressée
        }
        ScaleButton(scaleFactor); // Réduit la taille du bouton pour l'effet d'enfoncement
    }

    // Quand le bouton est relâché
    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonBackground != null)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeColor(buttonBackground.color, normalColor)); // Reviens à la couleur normale
        }
        ScaleButton(1f); // Restaure la taille normale
    }

    // Méthode pour animer le changement de couleur de fond du bouton
    private System.Collections.IEnumerator ChangeColor(Color fromColor, Color toColor)
    {
        float elapsedTime = 0;
        while (elapsedTime < animationSpeed)
        {
            elapsedTime += Time.deltaTime;
            if (buttonBackground != null)
            {
                buttonBackground.color = Color.Lerp(fromColor, toColor, elapsedTime / animationSpeed);
            }
            yield return null;
        }
        if (buttonBackground != null)
        {
            buttonBackground.color = toColor; // Applique la couleur finale
        }
    }

    // Méthode pour agrandir ou rétrécir le bouton
    private void ScaleButton(float scaleFactor)
    {
        transform.localScale = initialScale * scaleFactor; // Applique l'échelle donnée
    }
}

