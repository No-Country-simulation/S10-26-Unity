using System.Collections.Generic;
using UnityEngine;

public enum TipoNodo { Madera, Piedra, Agua, Construccion }

public class NodoRecurso : MonoBehaviour
{
    public static readonly List<NodoRecurso> Todos = new List<NodoRecurso>();

    public TipoNodo tipo;
    public Transform puntoAcceso;
    [HideInInspector] public bool ocupado;

    public Vector3 PosicionAcceso =>
        puntoAcceso != null ? puntoAcceso.position : transform.position;

    void OnEnable() => Todos.Add(this);
    void OnDisable() => Todos.Remove(this);
}