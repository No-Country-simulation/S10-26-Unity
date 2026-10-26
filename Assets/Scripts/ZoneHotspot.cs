using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ZoneHotspot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string zoneId;                       
    public GameObject highlight;               
    public UnityEvent<string> onZoneClicked;    

    void Start() { if (highlight) highlight.SetActive(false); }

    public bool mostrarLogs = true;             

    public void OnPointerClick(PointerEventData e)
    {
        if (mostrarLogs) Debug.Log($"[Mapa] Clic en la zona: {zoneId}");
        onZoneClicked?.Invoke(zoneId);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (mostrarLogs) Debug.Log($"[Mapa] Mouse sobre: {zoneId}");
        if (highlight) highlight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData e) { if (highlight) highlight.SetActive(false); }
}