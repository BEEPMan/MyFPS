using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_PopUp : MonoBehaviour
{
    protected CanvasGroup panel;
    protected RectTransform rectTransform;

    private void Awake()
    {
        panel = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if(panel != null)
        {
            panel.blocksRaycasts = false;
            panel.alpha = 0f;
        }
    }

    public virtual void ShowPanel()
    {
        rectTransform.SetAsLastSibling();
        panel.blocksRaycasts = true;
        panel.alpha = 1f;
        OnPopUp();
    }

    public virtual void HidePanel()
    {
        rectTransform.SetAsFirstSibling();
        panel.blocksRaycasts = false;
        panel.alpha = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }

    protected virtual void Init()
    {

    }
    protected virtual void OnUpdate()
    {

    }
    public virtual void OnPopUp()
    {

    }
}
