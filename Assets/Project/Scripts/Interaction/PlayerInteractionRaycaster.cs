using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

//Traduce los clics bidimensionales de la pantalla del mouse en impactos dentro del mundo físico 3D
//**NOTA: UnityEngine.InputSystem y EventSystems permiten acceder al hardware de entrada moderno y a la capa de colisión de interfaces de Unity.

namespace EcoSurvive.Interaction
{
    public class PlayerInteractionRaycaster : MonoBehaviour
    {
        [Header("Configuración de Detección")]
        [SerializeField] private float maxDistance = 100f;
        //se define variable de clase camera
        private Camera mainCamera;

        private void Awake()
        {
            //referencia la cámara principal de la escena para optimizar y evitar llamarlo en un update frame por frame
            mainCamera = Camera.main;
        }

        private void Update()
        {
            //Comprueba si hay conectado un mouse, si no lo tiene no provoca errores y se detiene
            if (Mouse.current == null) return;

            // Detectar si click izquierdo en el frame exacto wasPressedThisFrame (pulso boolenao) devuelve true en primer fotograma y se desactiva enseguida
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Ignorar el clic en el mundo si el cursor está sobre un botón de la UI y evita gastar PA accidentalmente
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                // Si mouse existe && se hizo click en este frame && no fue sobre la interfaz, ejecuta metodo deteccion elementos 3D
                PerformRaycast();
            }
        }

        private void PerformRaycast()
        {
            //Obtiene las coordenadas (X,Y)
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            //Traza una línea matemática tridimensional desde la camara pasando por el punto del mouse
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            //Physics.Raycast (booleano) Raycast Dispara rayo hasta 100 unidades de distancia. Si impacta contra obj con Collider, almacena los datos de la colisión en la variable hit
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                // Si el objeto impactado tiene el script InteractableNode lo guarda en variable node
                if (hit.collider.TryGetComponent<InteractableNode>(out var node))
                {
                    //Toma el script buscado y ejecuta el metodo Interact para validar y descontar PA
                    node.Interact();
                }

                /* Con GetComponent (3 líneas y menos eficiente) sin proteccion NullReferenceException
                InteractableNode node = hit.collider.GetComponent<InteractableNode>();
                if (node != null)
                {
                    node.Interact();
                }
                */
            }
        }
    }
}
