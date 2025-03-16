using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Xml.Linq;

public class NetworkSpawner : MonoBehaviour
{
    public NetworkManager NetworkManager;

    public enum PlayMode
    {
        None,
        Host,
        Server,
        Client
    }

    public PlayMode playMode;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGame")
        {
            if(playMode == PlayMode.Host)
            {
                NetworkManager.Singleton.StartHost();
            }
            else if (playMode == PlayMode.Server)
            {
                NetworkManager.Singleton.StartServer();
            }
            else if (playMode == PlayMode.Client)
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
