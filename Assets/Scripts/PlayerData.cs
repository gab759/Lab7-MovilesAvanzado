using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public int fuerza = 1;
    public int defensa = 1;
    public int agilidad = 1;
}

[Serializable]
public class PlayerData
{
    public string playerId;
    public string playerName;
    public int nivel = 1;
    public int experiencia = 0;
    public int puntosHabilidad = 0;
    public PlayerStats stats = new PlayerStats();

    public int XPRequeridaSiguienteNivel => GetXPRequerida(nivel);

    public static int GetXPRequerida(int nivel)
    {
        return 100 + 50 * (nivel - 1);
    }
}
