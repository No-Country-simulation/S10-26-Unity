using System.Collections;
using UnityEngine;
using EcoSurvive.Core;

//Identifica el tipo de recurso o accion y descuenta PA mediante script GameTurnManager

namespace EcoSurvive.Interaction
{
    public enum NodeType
    {
        ArbolesMadera,
        ZonaAgua,
        RanuraConstruccion
    }

    [RequireComponent(typeof(Collider))]
    public class InteractableNode : MonoBehaviour
    {
        [Header("Configuración del Nodo")]
        [SerializeField] private NodeType nodeType = NodeType.ArbolesMadera;
        [SerializeField] private int actionPointCost = 1;

        [Header("Feedback Visual Temporal")]
        [SerializeField] private Color clickFeedbackColor = Color.yellow;
        private Color originalColor;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                originalColor = meshRenderer.material.color;
            }
        }

        /// <summary>
        /// Ejecuta la acción si el TurnManager aprueba el gasto del punto de acción.
        /// </summary>
        public void Interact()
        {
            if (GameTurnManager.Instance == null) return;

            // 1. Validar y descontar el punto de acción
            if (GameTurnManager.Instance.TrySpendActionPoints(actionPointCost))
            {
                ExecuteAction();
            }
            else
            {
                Debug.LogWarning($"[InteractableNode] No tienes suficientes PA para interactuar con {gameObject.name}.");
            }
        }

        private void ExecuteAction()
        {
            switch (nodeType)
            {
                case NodeType.ArbolesMadera:
                    Debug.Log($"<color=orange>[Madera]</color> Has talado materiales en {gameObject.name}. (-{actionPointCost} PA)");
                    break;

                case NodeType.ZonaAgua:
                    Debug.Log($"<color=cyan>[Agua]</color> Has extraído agua en {gameObject.name}. (-{actionPointCost} PA)");
                    break;

                case NodeType.RanuraConstruccion:
                    Debug.Log($"<color=green>[Construcción]</color> Iniciando construcción en {gameObject.name}. (-{actionPointCost} PA)");
                    break;
            }

            // Destello visual rápido para confirmar el clic
            if (meshRenderer != null)
            {
                StartCoroutine(FlashRoutine());
            }
        }

        private IEnumerator FlashRoutine()
        {
            meshRenderer.material.color = clickFeedbackColor;
            yield return new WaitForSeconds(0.12f);
            meshRenderer.material.color = originalColor;
        }
    }
}