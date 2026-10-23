using System;
using System.Collections; 
using UnityEngine;

// Orquestador del bucle por turnos que coordina la secuencia de fases 
// tales como: Producción -> Consumo -> Escasez/Supervivencia -> Eventos -> Nuevo Día

namespace EcoSurvive.Core //evita colisiones de nombres con plugins o scripts de terceros
{
    [DefaultExecutionOrder(-50)] //ejecutar método Awake de este script antes que cualquier script
    public class GameTurnManager : MonoBehaviour
    {
        public static GameTurnManager Instance { get; private set; } //singleton

        [Header("Configuración de Turno")]
        [SerializeField] private int startingDay = 1;
        [SerializeField] private int maxActionPoints = 3;

        // Estado interno

        //variables de solo lectura externa y modificacion interna
        public int CurrentDay { get; private set; } 
        public int CurrentActionPoints { get; private set; } 
        public bool IsProcessingTurn { get; private set; } //true -> bloquea acciones del jugador al terminar el dia

        // Eventos estaticos: desacople UI y otros sistemas (Patron Observer)
        public static event Action<int> OnDayChanged; //envia entero con el nuevo numero de dia
        public static event Action<int, int> OnActionPointsChanged; // envia PA restantes y PA limite max
        public static event Action OnTurnProcessingStarted; // Indica cuando encender o apagar controles de interfaz
        public static event Action OnTurnProcessingCompleted;

        // Eventos secuenciales del fin de día (GDD Sección 3)
        public static event Action OnProduceBuildingsPhase;
        public static event Action OnConsumeResourcesPhase;
        public static event Action OnEvaluateSurvivalPhase;
        public static event Action OnTriggerEventsPhase;

        //Controla que solo exista unica instancia GameTurnManager en la escena 
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // destruye instancia duplicada no deseada
                return; //corta ejecucion obj duplicado
            }
            // Instancia activa, scripts pueden acceder -> GameTurnManager.Instance
            Instance = this; 
        }

        private void Start()
        {
            InitializeGameLoop();
        }

        private GameTurnManager CheckDuplicate() => Instance;

        //Asigna los valores iniciales y utiliza el operador de propagación nula ?.Invoke
        // ?. evalua, si no hay scripts escuchando, no hay error y dispara evento
        public void InitializeGameLoop()
        {
            CurrentDay = startingDay;
            CurrentActionPoints = maxActionPoints;
            IsProcessingTurn = false;

            // Notificar estado inicial al HUD
            OnDayChanged?.Invoke(CurrentDay);
            OnActionPointsChanged?.Invoke(CurrentActionPoints, maxActionPoints);
        }

        // Intenta descontar una cantidad de PA para ejecutar una tarea.

        public bool TrySpendActionPoints(int cost = 1) //Cost es enviado por InteractableNode
        {
            if (IsProcessingTurn) //si juego esta cambiando de dia, Aborta
            {
                Debug.LogWarning("No se pueden gastar PA mientras se procesa el fin de día.");
                return false; 
            }

            if (CurrentActionPoints >= cost) //SI PA>1 descuenta puntos PA y notifica a la UI
            {
                CurrentActionPoints -= cost;
                OnActionPointsChanged?.Invoke(CurrentActionPoints, maxActionPoints); //envia los datos
                return true; //autoriza la accion
            }
            // Se muestra este mensaje cuando el juego no esta cambiando de dia y PA<1
            Debug.LogWarning($"PA insuficientes. Requeridos: {cost}, Disponibles: {CurrentActionPoints}");
            return false; //Aborta accion
        }

        // Disparador del botón de interfaz "Terminar Día" si esta procesando un turno lo ignora
        public void EndDay()
        {
            if (IsProcessingTurn) return;

            StartCoroutine(ProcessTurnSequenceCoroutine()); //inicia corrutina si no procesa turno
        }

        //Corrutina da un margen de 250 ms * 4 = 1s para mostrar anim o # en UI. Incrementa día, 
        //reinicia PA a 3, notifica al HUD y desbloquea el semáforo
        private IEnumerator ProcessTurnSequenceCoroutine()
        {
            //activa semaforo de bloqueo
            //Dispara inicio, la UI lo recibe y ejecuta endDayButton.interactable = false apagando boton terminar dia
            IsProcessingTurn = true;
            OnTurnProcessingStarted?.Invoke();

            // **FASE DE PRODUCCION DE EDIFICIOS**
            //emite señal de recolecta recursos escuchados por script economia (huerto y recolector de agua) y suma recursos generados 
            OnProduceBuildingsPhase?.Invoke(); 
            yield return new WaitForSeconds(0.25f); //pausa

            // **FASE DE CONSUMO DE RECURSOS POR HABITANTE**
            //Emite señal gasto biologico por ciudadano vivo y descuenta a reservas globales al gestor de recursos
            OnConsumeResourcesPhase?.Invoke(); 
            yield return new WaitForSeconds(0.25f);

            // **FASE DE VERIFICACION DE ESCACEZ Y MUERTES**
            //evalua recursos necesarios minimos y de poblacion
            OnEvaluateSurvivalPhase?.Invoke(); 
            yield return new WaitForSeconds(0.25f);

            // **DISPARADOR DE EVENTOS (SI APLICA PARA EL DIA ACTUAL)**
            //notifica al gestor de eventos si habra tormenta, sequia, que dias y decisiones (A/B)
            OnTriggerEventsPhase?.Invoke();     
            yield return new WaitForSeconds(0.25f);

            // **CIERRE: AVANZAR DIA Y RESTAURAR PA**
            //aumenta dia actual
            //restablece PA
            CurrentDay++; 
            CurrentActionPoints = maxActionPoints; 

            Debug.Log($"<color=green>¡Día {CurrentDay} iniciado! PA restablecidos a: {CurrentActionPoints}/{maxActionPoints}</color>");
            //Envia dato y TurnUIController toma el nuevo dia y actualiza texto de dia (DIA 1, DIA 2...)
            OnDayChanged?.Invoke(CurrentDay); 
            // envia datos y TurnUI recorre imgs de slots y pinta las 3 ranuras de cian
            OnActionPointsChanged?.Invoke(CurrentActionPoints, maxActionPoints); 
            //Apaga semaforo y el jugador puede gastar PA
            IsProcessingTurn = false; 
            //Dispara notificacion de finalizacion TurnUI reactiva boton 'terminar dia' 
            OnTurnProcessingCompleted?.Invoke(); 
        }
    }
}


