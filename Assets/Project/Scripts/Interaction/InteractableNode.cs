using System.Collections;
using UnityEngine;
using EcoSurvive.Core;

//Identifica el tipo de recurso o accion y descuenta PA mediante script GameTurnManager

namespace EcoSurvive.Interaction
{
    //Lista desplegable de los tipos de recursos y acciones en el GDD
    public enum NodeType
    {
        ArbolesMadera,
        ZonaAgua,
        RanuraConstruccion
    }
    //Si se intenta añadir este script a un obj sin collider unity le agregara uno automaticamente
    [RequireComponent(typeof(Collider))]

    //**inicia la clase**
    public class InteractableNode : MonoBehaviour
    {
        [Header("Configuración del Nodo")]
        [SerializeField] private NodeType nodeType = NodeType.ArbolesMadera;
        [SerializeField] private int actionPointCost = 1;

        [Header("Feedback Visual Temporal")]
        [SerializeField] private Color clickFeedbackColor = Color.yellow;
        //variable tipo color guardar color de cubo y restaurarlo. Inicia en (0, 0, 0, 0) Negro 100% transparente (Color.clear)
        private Color originalColor;
        // Var clase meshrenderer por defecto null para cambiar el color
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            //Guarda referencia de componente visual para restaurarlo luego del parpadeo
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                //Lee el color real del objeto
                originalColor = meshRenderer.material.color;
            }
        }

        // Ejecuta la acción si el TurnManager aprueba el gasto del punto de acción.
        
        //Invocado por el Raycaster 
        public void Interact()
        {
            //Se comunica con script GameTurnManager, si no envia nada, no hace nada
            if (GameTurnManager.Instance == null) return;

            // Ejecuta el metodo TrySpendActionPoints del script GameTurnManager y envia 1 en su parametro
            if (GameTurnManager.Instance.TrySpendActionPoints(actionPointCost))
            {
                ExecuteAction();
            }
            else
            {
                Debug.LogWarning($"No tienes suficientes PA para interactuar con {gameObject.name}.");
            }
        }

        //Bifurca el comportamiento según el rol del cubo. 
        //En fases posteriores, aquí se llamará al inventario o a la navegación de NPCs
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

        //Cambia el color del cubo a amarillo y lo devuelve a su tono original tras 120 milisegundos,
        private IEnumerator FlashRoutine()
        {
            meshRenderer.material.color = clickFeedbackColor;
            yield return new WaitForSeconds(0.12f);
            meshRenderer.material.color = originalColor;
        }
    }
}