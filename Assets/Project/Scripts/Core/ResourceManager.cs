using System;
using UnityEngine;
using EcoSurvive.Interaction;

namespace EcoSurvive.Core
{
    [DefaultExecutionOrder(-40)] // Despierta después del TurnManager pero antes de la UI
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [Header("Inventario Base (GDD Sección 5)")]
        [SerializeField] private int madera = 15;
        [SerializeField] private int agua = 20;
        [SerializeField] private int alimentos = 20;
        [SerializeField] private int energia = 5;

        // Propiedades públicas de solo lectura
        public int Madera => madera;
        public int Agua => agua;
        public int Alimentos => alimentos;
        public int Energia => energia;

        // Evento Observer: Notifica a la UI los 4 recursos actualizados
        public static event Action<int, int, int, int> OnResourcesUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Notificar valores iniciales al HUD al arrancar la partida
            NotifyUI();
        }

        private void OnEnable()
        {
            // Suscribirse a la llamada de recolección de los cubos/nodos
            InteractableNode.OnNodeHarvested += HandleNodeHarvested;
        }

        private void OnDisable()
        {
            // Desuscribirse para evitar fugas de memoria
            InteractableNode.OnNodeHarvested -= HandleNodeHarvested;
        }

        /// <summary>
        /// Recibe el botín del nodo interactuable y lo suma a las reservas correspondientes.
        /// </summary>
        private void HandleNodeHarvested(NodeType type, int amount)
        {
            switch (type)
            {
                case NodeType.ArbolesMadera:
                    madera += amount;
                    break;

                case NodeType.ZonaAgua:
                    agua += amount;
                    break;
            }

            NotifyUI();
        }

        /// <summary>
        /// Método de utilidad para descontar recursos al construir o mejorar edificios.
        /// </summary>
        public bool TrySpendResources(int costMadera, int costAgua = 0, int costAlimento = 0, int costEnergia = 0)
        {
            if (madera >= costMadera && agua >= costAgua && alimentos >= costAlimento && energia >= costEnergia)
            {
                madera -= costMadera;
                agua -= costAgua;
                alimentos -= costAlimento;
                energia -= costEnergia;
                NotifyUI();
                return true;
            }

            Debug.LogWarning("[ResourceManager] Recursos insuficientes para realizar la acción.");
            return false;
        }

        private void NotifyUI()
        {
            OnResourcesUpdated?.Invoke(madera, agua, alimentos, energia);
        }
    }
}
