using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public enum PopUpType
{
    None,
    Scroll,
    Weapon,
    Peddler,
    Craftsman,
}

public class UIManager : Singleton<UIManager>
{
    [HideInInspector]
    public Transform Canvas;

    public Dictionary<string, UI_PopUp> PopUpsInScene;
    public Stack<UI_PopUp> CurrentPopUp;

    public UI_InGame InGame { get; set; }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected override void Awake()
    {
        base.Awake();
        PopUpsInScene = new Dictionary<string, UI_PopUp>();
        CurrentPopUp = new Stack<UI_PopUp>();
        Canvas = GameObject.Find("Canvas").transform;
        UI_PopUp[] popUps = Canvas.GetComponentsInChildren<UI_PopUp>();
        foreach (UI_PopUp popUp in popUps)
        {
            if (!PopUpsInScene.ContainsKey(popUp.name))
                PopUpsInScene.Add(popUp.name, popUp);
        }

        InGame = Canvas.GetComponentInChildren<UI_InGame>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "InGame")
        {
            if(GameManager.Instance.Player != null)
            {
                //GameManager.Instance.Player.InitUI();
            }
        }
    }

    public void ShowPanel(string name)
    {
        PopUpsInScene.TryGetValue(name, out UI_PopUp popUp);
        if(popUp != null)
        {
            if (CurrentPopUp.Count == 0) GameManager.Instance.Input.SetUIMode();
            CurrentPopUp.Push(popUp);
            popUp.ShowPanel();
        }
    }

    public void HidePanel(string name)
    {
        if (CurrentPopUp.Count == 0) return;
        if (CurrentPopUp.Peek().gameObject.name == name)
        {
            UI_PopUp popUp = CurrentPopUp.Pop();
            popUp.HidePanel();
            if (CurrentPopUp.Count == 0) GameManager.Instance.Input.SetInGameMode();
        }
    }

    public void HideTopPanel()
    {
        if (CurrentPopUp.Count == 0) return;
        UI_PopUp popUp = CurrentPopUp.Pop();
        popUp.HidePanel();
        if (CurrentPopUp.Count == 0) GameManager.Instance.Input.SetInGameMode();
    }

    public void HideAllPanel()
    {
        CurrentPopUp.Clear();
        GameManager.Instance.Input.SetInGameMode();
    }
}
