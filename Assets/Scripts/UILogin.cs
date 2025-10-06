using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject userPanel;
    [Header("Botones Login")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button updateNameBtn;
    [SerializeField] private Button btnInvitado;
    [Header("Textos")]
    [SerializeField] private TMP_Text playerIDTxt;
    [SerializeField] private TMP_Text playerNameTxt;

    [SerializeField] private TMP_InputField UpdateNameIF;

    [SerializeField] private UnityPlayerAuth unityPlayerAuth;
    void Start()
    {
        loginPanel.gameObject.SetActive(true);
        userPanel.gameObject.SetActive(false);
    }
    private void Awake()
    {
        SessionMode.IsGuest = false;

        if (btnInvitado != null)
            btnInvitado.onClick.AddListener(EnterAsGuest);
    }
    private void OnEnable()
    {
        loginButton?.onClick.AddListener(LoginButton);
        unityPlayerAuth.OnSingedIn += UnityPlayerOnSignedIn;

        updateNameBtn.onClick.AddListener(UpdateName);
        unityPlayerAuth.OnUpdateName += UpdateNameVisual;
    }

    private void EnterAsGuest()
    {
        // No inicializamos UGS ni autenticamos
        SessionMode.IsGuest = true;

        // Opcional: UI
        if (loginPanel) loginPanel.SetActive(false);
        if (userPanel) userPanel.SetActive(false);

        // Ir directo al Menú
        SceneManager.LoadScene("Menu");
    }

    private async void UpdateName()
    {
        await unityPlayerAuth.UpdateName(UpdateNameIF.text);
    }
    private void UpdateNameVisual(string newName)
    {
        playerNameTxt.text = newName;
    }
    private void UnityPlayerOnSignedIn(PlayerInfo playerInfo, string PlayerName)
    {
        loginPanel.gameObject.SetActive(false);
        userPanel.gameObject.SetActive(true);

        playerIDTxt.text = "ID: " + playerInfo.Id;
        playerNameTxt.text = PlayerName;
        SceneManager.LoadScene("Menu");
    }

    private async void LoginButton()
    {
        await unityPlayerAuth.InitSignIn();
    }

    private void OnDisable()
    {
        loginButton?.onClick.RemoveListener(LoginButton);
        unityPlayerAuth.OnSingedIn -= UnityPlayerOnSignedIn;
    }
}
