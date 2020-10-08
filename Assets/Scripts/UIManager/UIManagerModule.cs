using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class UIManagerModule {

    public Dictionary<Object, bool> menuElements;

    public List<(Object, UIConditions.isValid)> objects;


    public UIManagerModule(params (Object, UIConditions.isValid)[] ui) {
        menuElements = new Dictionary<Object, bool>();
        objects = new List<(Object, UIConditions.isValid)>();

        foreach((Object, UIConditions.isValid) tuple in ui) {
            menuElements.Add(tuple.Item1, false);
            objects.Add(tuple);
        }
    }

    public void OnGameStateChanged(SharedGameStates state, int role) {
        foreach ((Object, UIConditions.isValid) tuple in objects) {
            menuElements[tuple.Item1] = tuple.Item2(state, role);
        }
    }

    public bool this[Object key] {
        get => menuElements[key];
    }
}
