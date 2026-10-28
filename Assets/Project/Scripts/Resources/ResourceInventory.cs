using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInventory : MonoBehaviour
{
    [Header("Resource Definitions")]
    [SerializeField] private List<ResourceDefinition> resourceDefinitions = new();

    private readonly Dictionary<ResourceType, int> currentAmounts = new();

    public event Action<ResourceType, int> OnResourceChanged;

    private void Awake()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        currentAmounts.Clear();

        foreach (ResourceDefinition definition in resourceDefinitions)
        {
            if (definition == null)
            {
                Debug.LogWarning(
                    "ResourceInventory contains a null resource definition.",
                    this
                );
                continue;
            }

            if (currentAmounts.ContainsKey(definition.Type))
            {
                Debug.LogWarning(
                    $"Duplicate resource definition: {definition.Type}.",
                    this
                );
                continue;
            }

            int initialAmount = Mathf.Clamp(
                definition.InitialAmount,
                0,
                definition.MaxAmount
            );

            currentAmounts.Add(definition.Type, initialAmount);
        }
    }

    public bool HasResource(ResourceType type, int amount)
    {
        if (amount < 0)
        {
            return false;
        }

        return currentAmounts.TryGetValue(type, out int currentAmount)
            && currentAmount >= amount;
    }

    public int GetAmount(ResourceType type)
    {
        return currentAmounts.TryGetValue(type, out int amount)
            ? amount
            : 0;
    }

    public bool AddResource(ResourceType type, int amount)
    {
        if (amount <= 0 ||
            !currentAmounts.TryGetValue(type, out int currentAmount))
        {
            return false;
        }

        ResourceDefinition definition = GetDefinition(type);

        if (definition == null)
        {
            return false;
        }

        int newAmount = (int)Mathf.Min(
            (long)currentAmount + amount,
            definition.MaxAmount
        );

        if (newAmount == currentAmount)
        {
            return false;
        }

        currentAmounts[type] = newAmount;
        OnResourceChanged?.Invoke(type, newAmount);

        return true;
    }

    public bool RemoveResource(ResourceType type, int amount)
    {
        if (amount <= 0 ||
            !currentAmounts.TryGetValue(type, out int currentAmount) ||
            currentAmount < amount)
        {
            return false;
        }

        int newAmount = currentAmount - amount;

        currentAmounts[type] = newAmount;
        OnResourceChanged?.Invoke(type, newAmount);

        return true;
    }

    private ResourceDefinition GetDefinition(ResourceType type)
    {
        return resourceDefinitions.Find(
            definition => definition != null && definition.Type == type
        );
    }
    [ContextMenu("Debug/Show Current Resources")]
    private void DebugCurrentResources()
    {
        foreach (KeyValuePair<ResourceType, int> resource in currentAmounts)
        {
            Debug.Log($"{resource.Key}: {resource.Value}", this);
        }
    }
}