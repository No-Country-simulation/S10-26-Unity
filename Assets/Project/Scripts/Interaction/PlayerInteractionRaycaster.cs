using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

//Detecta clicks y utiliza API de UnityEngine.InputSystem para evitar clicks accidentales a traves de botones de interfaz

namespace EcoSurvive.Interaction
{
    public class PlayerInteractionRaycaster : MonoBehaviour
    {
        [Header("Configuración de Detección")]
        [SerializeField] private float maxDistance = 100f;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current == null) return;

            // Detectar clic izquierdo en el nuevo Input System
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Ignorar el clic en el mundo si el cursor está sobre un botón de la UI
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                PerformRaycast();
            }
        }

        private void PerformRaycast()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                // Si el objeto impactado tiene el script InteractableNode
                if (hit.collider.TryGetComponent<InteractableNode>(out var node))
                {
                    node.Interact();
                }
            }
        }
    }
}
