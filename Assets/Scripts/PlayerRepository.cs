using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

public class PlayerRepository
{
    private const string KEY_PLAYER_DATA = "PLAYER_DATA_JSON";

    public async Task SaveAsync(PlayerData data)
    {
        if (SessionMode.IsGuest)
        {
            Debug.Log("[CloudSave] Invitado: Save ignorado.");
            return;
        }

        string json = JsonUtility.ToJson(data);
        var dict = new Dictionary<string, object> { { KEY_PLAYER_DATA, json } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(dict);
    }

    public async Task<PlayerData> LoadOrCreateAsync(string playerId, string playerNameUGS)
    {
        if (SessionMode.IsGuest)
        {
            return new PlayerData
            {
                playerId = "guest",
                playerName = string.IsNullOrEmpty(playerNameUGS) ? "Invitado" : playerNameUGS,
                nivel = 1,
                experiencia = 0,
                puntosHabilidad = 0,
                stats = new PlayerStats { fuerza = 1, defensa = 1, agilidad = 1 }
            };
        }
        try
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { KEY_PLAYER_DATA });

            if (result != null && result.TryGetValue(KEY_PLAYER_DATA, out var saved))
            {
                string json = saved.Value.GetAs<string>();
                var data = JsonUtility.FromJson<PlayerData>(json);
                if (string.IsNullOrEmpty(data.playerId)) data.playerId = playerId;
                if (string.IsNullOrEmpty(data.playerName)) data.playerName = playerNameUGS;
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Cloud Save Load failed: {e.Message}. Se crearán datos por defecto.");
        }

        var fresh = new PlayerData
        {
            playerId = playerId,
            playerName = playerNameUGS,
            nivel = 1,
            experiencia = 0,
            puntosHabilidad = 0,
            stats = new PlayerStats { fuerza = 1, defensa = 1, agilidad = 1 }
        };

        await SaveAsync(fresh);
        return fresh;
    }
}
