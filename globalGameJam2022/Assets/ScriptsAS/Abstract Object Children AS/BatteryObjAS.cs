using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryObjAS : AbstractObjectAS
{
    private float batteryLife = 50f;

    public float GetBatteryLife()
    {
        return batteryLife;
    }

    public void SetBatteryLife(float val)
    {
        batteryLife = val;
    }
}
