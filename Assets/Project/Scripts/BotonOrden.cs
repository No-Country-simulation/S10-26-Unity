using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BotonOrden : MonoBehaviour
{
    public TipoNodo tipo;
    public GestorOrdenes gestor;

    void Awake() =>
        GetComponent<Button>().onClick.AddListener(() => gestor.Ordenar(tipo));
}