using UnityEngine;

public class GestorOrdenes : MonoBehaviour
{
    [Tooltip("Si está activo, todos los ciudadanos libres van a la vez")]
    public bool enviarATodos = false;

    public void Ordenar(TipoNodo tipo)
    {
        if (enviarATodos)
        {
            foreach (var npc in NPCController.Todos)
            {
                if (!npc.Libre) continue;
                var nodo = NodoLibreMasCercano(tipo, npc.transform.position);
                if (nodo != null) npc.IrANodo(nodo);
            }
            return;
        }

        // Un solo ciudadano: el par (NPC libre, nodo libre) más cercano
        NPCController mejorNpc = null;
        NodoRecurso mejorNodo = null;
        float mejorDist = float.MaxValue;

        foreach (var npc in NPCController.Todos)
        {
            if (!npc.Libre) continue;
            var nodo = NodoLibreMasCercano(tipo, npc.transform.position);
            if (nodo == null) continue;

            float d = (nodo.transform.position - npc.transform.position).sqrMagnitude;
            if (d < mejorDist) { mejorDist = d; mejorNpc = npc; mejorNodo = nodo; }
        }

        if (mejorNpc != null) mejorNpc.IrANodo(mejorNodo);
        else Debug.Log("No hay ciudadano libre o nodo disponible de ese tipo.");
    }

    NodoRecurso NodoLibreMasCercano(TipoNodo tipo, Vector3 desde)
    {
        NodoRecurso mejor = null;
        float mejorDist = float.MaxValue;

        foreach (var n in NodoRecurso.Todos)
        {
            if (n.tipo != tipo || n.ocupado) continue;
            float d = (n.transform.position - desde).sqrMagnitude;
            if (d < mejorDist) { mejorDist = d; mejor = n; }
        }
        return mejor;
    }
}