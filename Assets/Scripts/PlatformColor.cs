using Injection;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlatformColor : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    private int index = 0;

    [SerializeField] private List<Material> materialsList;
    public int MaterialCount => materialsList.Count;

    private void OnEnable()
    {
        signalBus.Subscribe<LevelInitializedEvent>(()=> index = 0);
    }

    public Material GetNextMaterial()
    {
        if (materialsList == null || materialsList.Count == 0)
        {
            Debug.LogWarning("Materials list is empty or not initialized.");
            return null; // Return null if the list is empty or not initialized
        }

        // Get the current material based on the index
        Material nextMaterial = materialsList[index];

        // Increment the index and loop back to the start if necessary
        index = (index + 1) % materialsList.Count;

        return nextMaterial;
    }


}
