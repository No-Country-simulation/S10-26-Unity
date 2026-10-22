using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EcoSurvive.Core;

// Componente que escucha los eventos de GameTurnManager 
// actualiza los contadores numericos y puntos de actividad (PA)

namespace EcoSurvive.UI
{
    public class TurnUIController : MonoBehaviour
    {
        [Header("Contadores y Textos")]
        [SerializeField] private TextMeshProUGUI dayText;

        [Header("Slots Gráficos de PA")]
        [Tooltip("Arreglo de imágenes que representan visualmente las 3 gemas o slots de PA.")]
        [SerializeField] private Image[] actionPointIcons;
        [SerializeField] private Color activePointColor = Color.cyan;
        [SerializeField] private Color depletedPointColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);

        [Header("Controles")]
        [SerializeField] private Button endDayButton;

        private void OnEnable()
        {
            GameTurnManager.OnDayChanged += UpdateDayDisplay;
            GameTurnManager.OnActionPointsChanged += UpdateActionPointsDisplay;
            GameTurnManager.OnTurnProcessingStarted += HandleTurnProcessingStarted;
            GameTurnManager.OnTurnProcessingCompleted += HandleTurnProcessingCompleted;

            if (endDayButton != null)
            {
                endDayButton.onClick.AddListener(OnEndDayButtonClicked);
            }
        }

        private void OnDisable()
        {
            GameTurnManager.OnDayChanged -= UpdateDayDisplay;
            GameTurnManager.OnActionPointsChanged -= UpdateActionPointsDisplay;
            GameTurnManager.OnTurnProcessingStarted -= HandleTurnProcessingStarted;
            GameTurnManager.OnTurnProcessingCompleted -= HandleTurnProcessingCompleted;

            if (endDayButton != null)
            {
                endDayButton.onClick.RemoveListener(OnEndDayButtonClicked);
            }
        }

        private void UpdateDayDisplay(int currentDay)
        {
            if (dayText != null)
            {
                dayText.text = $"DÍA {currentDay}";
            }
        }

        private void UpdateActionPointsDisplay(int currentPA, int maxPA)
        {
            if (actionPointIcons == null) return;

            for (int i = 0; i < actionPointIcons.Length; i++)
            {
                if (actionPointIcons[i] == null) continue;

                // Si el índice es menor que los PA restantes, se muestra activo
                if (i < currentPA)
                {
                    actionPointIcons[i].color = activePointColor;
                }
                else
                {
                    actionPointIcons[i].color = depletedPointColor;
                }
            }
        }

        private void HandleTurnProcessingStarted()
        {
            if (endDayButton != null)
            {
                endDayButton.interactable = false;
            }
        }

        private void HandleTurnProcessingCompleted()
        {
            if (endDayButton != null)
            {
                endDayButton.interactable = true;
            }
        }

        private void OnEndDayButtonClicked()
        {
            GameTurnManager.Instance.EndDay();
        }
    }
}
