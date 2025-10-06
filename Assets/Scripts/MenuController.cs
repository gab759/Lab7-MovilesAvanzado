using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private UnityPlayerAuth auth;
    [SerializeField] private TMP_InputField inputNombreEditable;
    [SerializeField] private Button btnActualizarNombre;

    [Header("Info")]
    [SerializeField] private TMP_Text txtJugador;
    [SerializeField] private TMP_Text txtNivel;
    [SerializeField] private TMP_Text txtXP;
    [SerializeField] private TMP_Text txtPuntos;

    [Header("Stats")]
    [SerializeField] private TMP_Text txtFuerza;
    [SerializeField] private TMP_Text txtDefensa;
    [SerializeField] private TMP_Text txtAgilidad;
    [SerializeField] private Button btnFuerzaMas;
    [SerializeField] private Button btnDefensaMas;
    [SerializeField] private Button btnAgilidadMas;

    [Header("Click Config")]
    [SerializeField] private int xpPorClick = 1;

    private PlayerRepository repo;
    private PlayerProgression prog;
    private bool ready;

    private void Awake()
    {
        repo = new PlayerRepository();
    }

    private async void Start()
    {
        await InitDataAsync();
        HookUI();
        Redraw();
    }

    private async Task InitDataAsync()
    {
        var playerInfo = AuthenticationService.Instance.PlayerInfo;
        string playerId = playerInfo?.Id ?? "unknown";
        string playerName = await AuthenticationService.Instance.GetPlayerNameAsync();

        var data = await repo.LoadOrCreateAsync(playerId, playerName);
        prog = new PlayerProgression(data);
        ready = true;

        if (inputNombreEditable) inputNombreEditable.text = playerName;
    }

    private void HookUI()
    {
        if (btnActualizarNombre) btnActualizarNombre.onClick.AddListener(async () =>
        {
            if (!string.IsNullOrEmpty(inputNombreEditable.text))
            {
                await auth.UpdateName(inputNombreEditable.text);
                prog.Data.playerName = inputNombreEditable.text;
                await repo.SaveAsync(prog.Data);
                Redraw();
            }
        });

        if (btnFuerzaMas) btnFuerzaMas.onClick.AddListener(async () => await SpendPoint("fuerza"));
        if (btnDefensaMas) btnDefensaMas.onClick.AddListener(async () => await SpendPoint("defensa"));
        if (btnAgilidadMas) btnAgilidadMas.onClick.AddListener(async () => await SpendPoint("agilidad"));

        auth.OnUpdateName += (newName) =>
        {
            if (inputNombreEditable) inputNombreEditable.text = newName;
            prog.Data.playerName = newName;
            Redraw();
        };
    }
    private async Task SpendPoint(string stat)
    {
        if (!ready) return;
        if (prog.TrySpendPointOn(stat))
        {
            await repo.SaveAsync(prog.Data);
            Redraw();
        }
    }

    public async void OnScreenClick()
    {
        if (!ready) return;
        bool leveled = prog.AddXP(xpPorClick);
        if (leveled)
        {
            await repo.SaveAsync(prog.Data);
        }
        else
        {
            await repo.SaveAsync(prog.Data);
        }
        Redraw();
    }

    private void Redraw()
    {
        if (prog == null) return;

        if (txtJugador) txtJugador.text = string.IsNullOrEmpty(prog.Data.playerName)
            ? "Jugador"
            : prog.Data.playerName;

        if (txtNivel) txtNivel.text = $"Nivel: {prog.Data.nivel}";
        if (txtXP) txtXP.text = $"XP: {prog.Data.experiencia} / {prog.Data.XPRequeridaSiguienteNivel}";
        if (txtPuntos) txtPuntos.text = $"Puntos: {prog.Data.puntosHabilidad}";

        if (txtFuerza) txtFuerza.text = $"Fuerza: {prog.Data.stats.fuerza}";
        if (txtDefensa) txtDefensa.text = $"Defensa: {prog.Data.stats.defensa}";
        if (txtAgilidad) txtAgilidad.text = $"Agilidad: {prog.Data.stats.agilidad}";
    }
}
