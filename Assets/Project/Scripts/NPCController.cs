using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    public static readonly List<NPCController> Todos = new List<NPCController>();

    public float tiempoEnNodo = 1.5f; // placeholder hasta la US de recolección

    NavMeshAgent agente;
    NodoRecurso nodoActual;
    bool yendo;

    public bool Libre => nodoActual == null;

    void Awake() => agente = GetComponent<NavMeshAgent>();
    void OnEnable() => Todos.Add(this);
    void OnDisable() => Todos.Remove(this);

    // Orden por botón: ir a un nodo (árbol, roca, agua, ranura)
    public bool IrANodo(NodoRecurso nodo)
    {
        if (!Libre || nodo == null || nodo.ocupado) return false;

        if (!NavMesh.SamplePosition(nodo.PosicionAcceso, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            Debug.LogWarning($"{nodo.name}: su PuntoAcceso no está sobre el NavMesh. ¿Hiciste Bake?");
            return false;
        }

        nodoActual = nodo;
        nodo.ocupado = true;
        yendo = true;
        agente.isStopped = false;
        agente.SetDestination(hit.position);
        return true;
    }

    // Orden por clic en el suelo: ir a un punto libre (explorar)
    public void Mover(Vector3 punto)
    {
        StopAllCoroutines();   // cancela lo que estuviera haciendo
        yendo = false;
        LiberarNodo();
        agente.isStopped = false;
        agente.SetDestination(punto);
    }

    void Update()
    {
        if (!yendo || agente.pathPending) return;

        if (agente.remainingDistance <= agente.stoppingDistance + 0.05f)
        {
            yendo = false;
            StartCoroutine(LlegarYLiberar());
        }
    }

    IEnumerator LlegarYLiberar()
    {
        Debug.Log($"{name} llegó a {nodoActual.name}");
        // Aquí irá después: talar / recoger / construir
        yield return new WaitForSeconds(tiempoEnNodo);
        LiberarNodo();
    }

    void LiberarNodo()
    {
        if (nodoActual != null)
        {
            nodoActual.ocupado = false;
            nodoActual = null;
        }
    }
}