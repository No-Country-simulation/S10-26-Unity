using UnityEngine;
using TMPro;
using EcoSurvive.Core;

namespace EcoSurvive.UI
{
    public class ResourceUIController : MonoBehaviour
    {
        [Header("Textos de Recursos (TextMeshPro)")]
        [SerializeField] private TextMeshProUGUI textMadera;
        [SerializeField] private TextMeshProUGUI textAgua;
        [SerializeField] private TextMeshProUGUI textAlimentos;
        [SerializeField] private TextMeshProUGUI textEnergia;

        [Header("Colores y Alertas de Umbrales (GDD Sección 12)")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color maderaThresholdColor = new Color(0.2f, 1f, 0.2f, 1f); // Verde (Madera >= 50)
        [SerializeField] private Color aguaAlertColor = new Color(1f, 0.25f, 0.25f, 1f);       // Rojo (Agua < 30)

        private void OnEnable()
        {
            ResourceManager.OnResourcesUpdated += UpdateResourceDisplay;
        }

        private void OnDisable()
        {
            ResourceManager.OnResourcesUpdated -= UpdateResourceDisplay;
        }

        private void UpdateResourceDisplay(int madera, int agua, int alimentos, int energia)
        {
            // Actualizar Madera y comprobar umbral de construcción (>= 50)
            if (textMadera != null)
            {
                textMadera.text = $"Madera: {madera}";
                textMadera.color = (madera >= 50) ? maderaThresholdColor : normalColor;
            }

            // Actualizar Agua y comprobar umbral de deshidratación (< 30)
            if (textAgua != null)
            {
                textAgua.text = $"Agua: {agua}";
                textAgua.color = (agua < 30) ? aguaAlertColor : normalColor;
            }

            // Actualizar Alimentos
            if (textAlimentos != null)
            {
                textAlimentos.text = $"Comida: {alimentos}";
                textAlimentos.color = normalColor;
            }

            // Actualizar Energía
            if (textEnergia != null)
            {
                textEnergia.text = $"Energía: {energia}";
                textEnergia.color = normalColor;
            }
        }
    }
}