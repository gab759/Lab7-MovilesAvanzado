using UnityEngine;

public class PlayerProgression
{
    public PlayerData Data { get; private set; }

    public PlayerProgression(PlayerData initial)
    {
        Data = initial;
    }

    public bool AddXP(int amount)
    {
        if (amount <= 0) return false;

        Data.experiencia += amount;
        bool leveledUp = false;
        while (Data.experiencia >= Data.XPRequeridaSiguienteNivel)
        {
            Data.experiencia -= Data.XPRequeridaSiguienteNivel;
            Data.nivel++;
            Data.puntosHabilidad += 1;
            leveledUp = true;
        }
        return leveledUp;
    }

    public bool TrySpendPointOn(string stat)
    {
        if (Data.puntosHabilidad <= 0) return false;
        switch (stat.ToLower())
        {
            case "fuerza": Data.stats.fuerza++; break;
            case "defensa": Data.stats.defensa++; break;
            case "agilidad": Data.stats.agilidad++; break;
            default: return false;
        }
        Data.puntosHabilidad--;
        return true;
    }
}
