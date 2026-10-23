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
        // Var para texto del dia
        [SerializeField] private TextMeshProUGUI dayText;

        [Header("Slots Gráficos de PA")]
        [Tooltip("Arreglo de imágenes que representan visualmente las 3 gemas o slots de PA.")]
        //Coleccion de imagenes de slots PA
        [SerializeField] private Image[] actionPointIcons;
        //Color que ilumina PA disponibles
        [SerializeField] private Color activePointColor = Color.cyan;
        //Apagado color de PA
        [SerializeField] private Color depletedPointColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);

        [Header("Controles")]
        // Boton Terminar Dia
        [SerializeField] private Button endDayButton;

        //Suscripcion a eventos de GameTurnManager cuando se activa obj en escena
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

        //Desuscripciones para evitar fugas de memoria y evitar llamados de metodos de obj destruidos
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
        
        //Recibe el entero transmitido por el evento y actualiza la cadena de texto en pantalla
        private void UpdateDayDisplay(int currentDay)
        {
            if (dayText != null)
            {
                dayText.text = $"DÍA {currentDay}";
            }
        }

        //Recibe los datos (currentPA, maxPA) y actualiza PA    
        private void UpdateActionPointsDisplay(int currentPA, int maxPA)
        {
            //PA nulos, no hace nada
            if (actionPointIcons == null) return;

            //Itera con un ciclo for a través de las imágenes registradas en el array
            for (int i = 0; i < actionPointIcons.Length; i++)
            {
                if (actionPointIcons[i] == null) continue;

                // Si el índice es menor que los PA restantes, se muestra activo con color cian
                if (i < currentPA)
                {
                    actionPointIcons[i].color = activePointColor;
                }
                //Si el índice es igual o mayor, le asigna el color gris apagado translúcido
                else
                {
                    actionPointIcons[i].color = depletedPointColor;
                }
            }
        }

        //Desactiva la propiedad interactable del botón mientras la corrutina simula las fases de la noche, 
        //impidiendo que el jugador presione el botón repetidas veces para generar bugs en el calendario.
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

        //Conecta el clic físico del usuario en la pantalla con la función EndDay() del gestor.
        //Llama a GamTurnManager y su metodo EndDay
        private void OnEndDayButtonClicked()
        {
            GameTurnManager.Instance.EndDay();
        }
    }
}
