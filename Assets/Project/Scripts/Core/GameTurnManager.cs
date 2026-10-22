using System;
using System.Collections;
using UnityEngine;

// Orquestador del bucle por turnos que coordina la secuencia de fases 
// tale como: Producción -> Consumo -> Escasez/Supervivencia -> Eventos -> Nuevo Día

namespace EcoSurvive.Core
{
    [DefaultExecutionOrder(-50)]
    public class GameTurnManager : MonoBehaviour
    {
        public static GameTurnManager Instance { get; private set; }

        [Header("Configuración de Turno")]
        [SerializeField] private int startingDay = 1;
        [SerializeField] private int maxActionPoints = 3;

        // Estado interno
        public int CurrentDay { get; private set; }
        public int CurrentActionPoints { get; private set; }
        public bool IsProcessingTurn { get; private set; }

        // Eventos C# para desacoplar UI y otros sistemas
        public static event Action<int> OnDayChanged;
        public static event Action<int, int> OnActionPointsChanged; // (current, max)
        public static event Action OnTurnProcessingStarted;
        public static event Action OnTurnProcessingCompleted;

        // Eventos secuenciales del fin de día (GDD Sección 3)
        public static event Action OnProduceBuildingsPhase;
        public static event Action OnConsumeResourcesPhase;
        public static event Action OnEvaluateSurvivalPhase;
        public static event Action OnTriggerEventsPhase;

        private void Awake()
        {
            if (Instance != null && Instance != CheckDuplicate())
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitializeGameLoop();
        }

        private GameTurnManager CheckDuplicate() => Instance;

        public void InitializeGameLoop()
        {
            CurrentDay = startingDay;
            CurrentActionPoints = maxActionPoints;
            IsProcessingTurn = false;

            // Notificar estado inicial al HUD
            OnDayChanged?.Invoke(CurrentDay);
            OnActionPointsChanged?.Invoke(CurrentActionPoints, maxActionPoints);
        }

        /// <summary>
        /// Intenta descontar una cantidad de Puntos de Acción para ejecutar una tarea.
        /// </summary>
        public bool TrySpendActionPoints(int cost = 1)
        {
            if (IsProcessingTurn)
            {
                Debug.LogWarning("[GameTurnManager] No se pueden gastar PA mientras se procesa el fin de día.");
                return false;
            }

            if (CurrentActionPoints >= cost)
            {
                CurrentActionPoints -= cost;
                OnActionPointsChanged?.Invoke(CurrentActionPoints, maxActionPoints);
                return true;
            }

            Debug.LogWarning($"[GameTurnManager] PA insuficientes. Requeridos: {cost}, Disponibles: {CurrentActionPoints}");
            return false;
        }

        /// <summary>
        /// Disparador del botón de interfaz "Terminar Día".
        /// </summary>
        public void EndDay()
        {
            if (IsProcessingTurn) return;

            StartCoroutine(ProcessTurnSequenceCoroutine());
        }

        private IEnumerator ProcessTurnSequenceCoroutine()
        {
            IsProcessingTurn = true;
            OnTurnProcessingStarted?.Invoke();

            // 1. Fase de Producción de edificios
            OnProduceBuildingsPhase?.Invoke();
            yield return new WaitForSeconds(0.25f);

            // 2. Fase de Consumo de recursos por habitantes
            OnConsumeResourcesPhase?.Invoke();
            yield return new WaitForSeconds(0.25f);

            // 3. Fase de Verificación de escasez y muertes
            OnEvaluateSurvivalPhase?.Invoke();
            yield return new WaitForSeconds(0.25f);

            // 4. Disparador de eventos (si aplica para el día actual)
            OnTriggerEventsPhase?.Invoke();
            yield return new WaitForSeconds(0.25f);

            // 5. Cierre: Avanzar día y restaurar PA
            CurrentDay++;
            CurrentActionPoints = maxActionPoints;

            Debug.Log($"<color=green>[GameTurnManager] ¡Día {CurrentDay} iniciado! PA restablecidos a: {CurrentActionPoints}/{maxActionPoints}</color>");

            OnDayChanged?.Invoke(CurrentDay);
            OnActionPointsChanged?.Invoke(CurrentActionPoints, maxActionPoints);

            IsProcessingTurn = false;
            OnTurnProcessingCompleted?.Invoke();
        }

        [ContextMenu("Depuración: Gastar 1 PA")]
        public void DebugSpendOnePoint()
        {
            TrySpendActionPoints(1);
            Debug.Log($"[GameTurnManager] PA gastado. Restantes: {CurrentActionPoints}/{maxActionPoints}");
        }

        private void Update()
        {
            // Detección con el nuevo Input System de Unity
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                DebugSpendOnePoint();
            }
        }
    }
}


