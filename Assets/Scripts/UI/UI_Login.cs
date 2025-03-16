using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Login : MonoBehaviour
{
    public NetworkSpawner NetworkSpawner;

    public void OnHostButtonClicked()
    {
        NetworkSpawner.playMode = NetworkSpawner.PlayMode.Host;
        SceneManager.LoadScene("InGame");
    }

    public void OnServerButtonClicked()
    {
        NetworkSpawner.playMode = NetworkSpawner.PlayMode.Server;
        SceneManager.LoadScene("InGame");
    }

    public void OnClientButtonClicked()
    {
        NetworkSpawner.playMode = NetworkSpawner.PlayMode.Client;
        SceneManager.LoadScene("InGame");
    }
}
