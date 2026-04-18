using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourseManager : MonoBehaviour
{
    public static ResourseManager Instance { get; private set; }

    public static event Action<ResourceType, int> OnResourseAmountChanged;

    private Dictionary<ResourceType, int> resourses = new Dictionary<ResourceType, int>();


    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        resourses[ResourceType.Wood] = 200;
        resourses[ResourceType.Stone] = 200;
    }

    private void Start()
    {
        foreach (var resourse in resourses)
        {
            OnResourseAmountChanged?.Invoke(resourse.Key, resourse.Value);
        }
    }
        

    public void Add(ResourceType type, int Amount)
    {
        resourses[type] += Amount;
        OnResourseAmountChanged?.Invoke(type, resourses[type]);
    }

    public bool TrySpend(List<ResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            if (!resourses.ContainsKey(cost.type) || resourses[cost.type] < cost.amount)
            {
                return false;
            }   
        }

        foreach (var cost in costs)
        {
            resourses[cost.type] -= cost.amount;
            OnResourseAmountChanged?.Invoke(cost.type, resourses[cost.type]);
        }
        return true;
    }
}
