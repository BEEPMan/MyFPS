using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PopUpType
{
    None,
    Scroll,
    Weapon,
    Peddler,
    Craftsman,
}

public class UIManager : MonoBehaviour
{
    private static UIManager _instance = null;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null) return null;
            return _instance;
        }
    }

    private PlayerUI _inGame;
    private UI_Inventory_Scroll _scrollInventory;
    private UI_Inventory_Weapon _weaponInventory;
    private UI_Peddler _peddler;

    public GameObject InGameUI;
    public GameObject PopUpUI;

    private Dictionary<PopUpType, UI_PopUp> popUps = new();
    public PopUpType currentPopUp;

    public PlayerUI InGame { get { return _inGame; } }
    public UI_Inventory_Scroll ScrollInventory { get { return (UI_Inventory_Scroll)popUps[PopUpType.Scroll]; } }
    public UI_Inventory_Weapon WeaponInventory { get { return (UI_Inventory_Weapon)popUps[PopUpType.Weapon]; } }
    public UI_Peddler Peddler {  get { return (UI_Peddler)popUps[PopUpType.Peddler]; } }

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _inGame = InGameUI.GetComponent<PlayerUI>();
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < PopUpUI.transform.childCount; i++)
        {
            if (PopUpUI.transform.GetChild(i).GetComponent<UI_Inventory_Scroll>() != null)
            {
                popUps.Add(PopUpType.Scroll, PopUpUI.transform.GetChild(i).GetComponent<UI_Inventory_Scroll>());
            }
            if(PopUpUI.transform.GetChild(i).GetComponent<UI_Inventory_Weapon>() != null)
            {
                popUps.Add(PopUpType.Weapon, PopUpUI.transform.GetChild(i).GetComponent<UI_Inventory_Weapon>());
            }
            if (PopUpUI.transform.GetChild(i).GetComponent<UI_Peddler>() != null)
            {
                popUps.Add(PopUpType.Peddler, PopUpUI.transform.GetChild(i).GetComponent<UI_Peddler>());
            }
        }
    }

    void Update()
    {
        
    }

    public void OpenPopUp(PopUpType popUpName)
    {
        InGameUI.SetActive(false);
        popUps[popUpName].gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Player.Instance.Input.InGame.Disable();
        Player.Instance.Input.UIInput.Enable();
        popUps[popUpName].OnPopUp();
        currentPopUp = popUpName;
    }

    public void ClosePopUp()
    {
        InGameUI.SetActive(true);
        foreach(UI_PopUp item in popUps.Values)
        {
            if (item == null) continue;
            item.gameObject.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Player.Instance.Input.InGame.Enable();
        Player.Instance.Input.UIInput.Disable();
        currentPopUp = PopUpType.None;
    }
}
