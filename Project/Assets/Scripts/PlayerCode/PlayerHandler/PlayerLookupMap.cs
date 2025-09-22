using PlayerCode.BaseCode;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerLookupMap {
    private static Dictionary<Collider, BasePlayerController> players = new Dictionary<Collider, BasePlayerController>();
    
    public static BasePlayerController GetPlayer(Collider coliider) {
        return players.GetValueOrDefault(coliider, null);
    }

    public static void AddPlayer(Collider collider, BasePlayerController player) {
        players[collider] = player;
    }

    public static void RemovePlayer(Collider collider) {
        players.Remove(collider);
    }
}
