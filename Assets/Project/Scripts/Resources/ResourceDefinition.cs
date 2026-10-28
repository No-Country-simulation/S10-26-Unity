using UnityEngine;

public enum ResourceType
{
    Water,
    Food,
    Energy,
    Materials
}

[CreateAssetMenu(
    fileName = "NewResource",
    menuName = "EcoSurvive/Resource Definition"
)]
public class ResourceDefinition : ScriptableObject
{
    [Header("Resource Information")]
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private string resourceName;
    [SerializeField] private Sprite icon;

    [Header("Resource Limits")]
    [SerializeField, Min(0)] private int initialAmount;
    [SerializeField, Min(0)] private int maxAmount = 100;

    public ResourceType Type => resourceType;
    public string ResourceName => resourceName;
    public Sprite Icon => icon;
    public int InitialAmount => initialAmount;
    public int MaxAmount => maxAmount;
}