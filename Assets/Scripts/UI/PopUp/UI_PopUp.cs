using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_PopUp : MonoBehaviour
{
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
