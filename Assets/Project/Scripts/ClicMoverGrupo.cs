using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ClicMoverGrupo : MonoBehaviour
{
    public float separacion = 1.2f; // distancia entre ciudadanos al llegar
    Camera cam;

    void Awake() => cam = Camera.main;

    void Update()
    {
        if (!HuboClic(out Vector2 pantalla)) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray rayo = cam.ScreenPointToRay(pantalla);
        if (!Physics.Raycast(rayo, out RaycastHit hit, 500f)) return;

        var lista = NPCController.Todos;
        for (int i = 0; i < lista.Count; i++)
        {
            // Reparte a los ciudadanos en círculo alrededor del punto
            float ang = (360f / lista.Count) * i * Mathf.Deg2Rad;
            Vector3 destino = hit.point + new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)) * separacion;

            if (NavMesh.SamplePosition(destino, out NavMeshHit nav, 3f, NavMesh.AllAreas))
                lista[i].Mover(nav.position);
        }
    }

    bool HuboClic(out Vector2 pos)
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        { pos = Mouse.current.position.ReadValue(); return true; }
#else
        if (Input.GetMouseButtonDown(0))
        { pos = Input.mousePosition; return true; }
#endif
        pos = default;
        return false;
    }
}