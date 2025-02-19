using System.Collections.Generic;
using UnityEngine;

public class LoveSystem : MonoBehaviour
{
    public float loveRange = 1.5f;  // Khoảng cách yêu nhau

    void Update()
    {
        List<UnitController> units = new List<UnitController>(FindObjectsOfType<UnitController>());

        foreach (UnitController unit in units)
        {
            foreach (UnitController other in units)
            {
                if (unit != other && Vector3.Distance(unit.transform.position, other.transform.position) < loveRange)
                {
                    unit.CheckForLove(other);
                }
            }
        }
    }
}
