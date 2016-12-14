﻿using System;
using System.Collections.Generic;
using UnityEngine;
using ResourcesPickups = SRResources.Game.Pickups;
using System.Linq;

public class Level : MonoBehaviour
{
    public Level Initialize(Action onLevelCleared)
    {
        this.onLevelCleared = onLevelCleared;
        int index = 0;
        Dictionary<TypeSafe.PrefabResource, int> pickups = GetPickups();
        this.blocks = GetComponentsInChildren<Block>().Select(block =>
        {
            block.Initialize(index, (id) => OnBlockDestroyed(id));
            TypeSafe.PrefabResource choosen = Utils.RandomWeightedChooser(pickups);
            if (choosen.Name != "EmptyPickup")
            {
                GameObject pickUP = choosen.Instantiate();
                block.SetItemOnHit(pickUP);
            }
            index++;
            return block.gameObject;
        }).ToArray();
        return this;
    }

    public Level Destroy()
    {
        Destroy(this.gameObject);
        return this;
    }

    private Level OnBlockDestroyed(int blockId)
    {
        int index = 0;
        bool finished = Array.TrueForAll(this.blocks, block => {
            bool isNotActive = !block.activeInHierarchy || blockId == index;
            index++;
            return isNotActive;
        });
        if (!finished)
        {
            return this;
        }
        this.onLevelCleared();
        return this;
    }

    private Dictionary<TypeSafe.PrefabResource, int> GetPickups()
    {
        Dictionary<TypeSafe.PrefabResource, int> pickups = new Dictionary<TypeSafe.PrefabResource, int>();
        pickups.Add(ResourcesPickups.Heart, 3);
        pickups.Add(ResourcesPickups.Clock, 3);
        pickups.Add(ResourcesPickups.Star, 3);
        pickups.Add(ResourcesPickups.EmptyPickup, 91);
        return pickups;
    }

    private GameObject[] blocks;
    private Action onLevelCleared;
}