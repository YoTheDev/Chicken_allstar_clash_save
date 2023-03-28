using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_manager : ScriptableObject
{
    public abstract void Do(UI ui);
    public abstract void Back(UI ui);
}
