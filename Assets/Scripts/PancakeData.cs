using UnityEngine;

public class PancakeData : MonoBehaviour
{
    public int batter_units = 0; // this represents how big the pancake is. Every 2 seconds, the dispenser gives 1 unit of batter.
    public int state = 0; // 0 is raw, 1 is cooked, 2 is burnt

}
