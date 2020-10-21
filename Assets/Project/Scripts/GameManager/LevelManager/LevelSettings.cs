using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    public static int IsAutoAIM;
    public LevelMode LevelMode;

    private void Awake()
    {
        SetModeLevel(CarouselController.NumberLvl);

    }

    private void SetModeLevel(int idLevel)
    {
        switch (idLevel)
        {
            case 1:
                LevelMode = new EasyMode();
                break;
            case 2:
                LevelMode = new HardMode();
                break;
        }
    }
}

public abstract class LevelMode
{
    public static bool IsHardMode
    {
        get;
        protected set;
    }
}

public class EasyMode : LevelMode
{
    public EasyMode()
    {
        IsHardMode = false;
    }
}

public class HardMode : LevelMode
{
    public HardMode()
    {
        IsHardMode = true;
    }
}
