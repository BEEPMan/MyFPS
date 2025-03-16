using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager
{
    public Dictionary<string, Scroll> Scrolls { get; private set; }

    public ScrollManager()
    {
        Scrolls = new Dictionary<string, Scroll>();
    }

    public void OnUpdate()
    {
        //float deltaTime = Time.deltaTime;
        //foreach (Scroll scroll in Scrolls.Values)
        //{
        //    scroll.UpdateEffect(deltaTime);
        //}
    }

    public void AddScroll(Scroll newScroll)
    {
        if (Scrolls.ContainsKey(newScroll.ItemName) == false)
        {
            Scrolls.Add(newScroll.ItemName, newScroll);
        }
    }

    public void AddScroll(string scrollName)
    {
        if (Scrolls.ContainsKey(scrollName) == false)
        {
            Scrolls.Add(scrollName, ItemManager.Instance.FindScroll(scrollName));
        }
    }

    public void RemoveScroll(string scrollName, out Scroll scroll)
    {
        Scroll removeScroll;
        if (Scrolls.TryGetValue(scrollName, out removeScroll) == true)
        {
            removeScroll.Drop(GameManager.Instance.Player);
            Scrolls.Remove(scrollName);
        }
        scroll = removeScroll;
    }

    public bool TryGetScroll(string name, out Scroll scroll)
    {
        bool result = Scrolls.ContainsKey(name);
        scroll = Scrolls[name];
        return result;
    }
}
