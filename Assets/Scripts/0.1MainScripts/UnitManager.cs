using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject unitPrefab;
    public int unitCount = 10;
    public float spacing = 2f;
    private List<Unit> units = new List<Unit>();
    public Transform startPosition;

    void Start()
    {
        unitPrefab.SetActive(false);
        SpawnUnits();
        ArrangeUnitsInTwoRows();
    }

    void SpawnUnits()
    {
        int rowCount = unitCount / 2;
        int maleCount = Mathf.RoundToInt(unitCount * 0.5f); // 50/50 hoặc 60/40
        int femaleCount = unitCount - maleCount;

        for (int i = 0; i < unitCount; i++)
        {
            Vector3 position = new Vector3((i % rowCount) * spacing, (i / rowCount) * -spacing, 0);
            GameObject unitObj = Instantiate(unitPrefab, position, Quaternion.identity);
            unitObj.SetActive(true);
            Unit unit = unitObj.GetComponent<Unit>();

            // Xác định giới tính
            if (maleCount > 0 && (Random.value > 0.4f || femaleCount == 0))
            {
                unit.SetGender(true); // Nam
                maleCount--;
            }
            else
            {
                unit.SetGender(false); // Nữ
                femaleCount--;
            }

            units.Add(unit);
        }
    }
    void ArrangeUnitsInTwoRows()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        int rowCount = Mathf.CeilToInt(allUnits.Length / 2f);
        float spacing = 1.5f;

        for (int i = 0; i < allUnits.Length; i++)
        {
            int row = i % 2;
            int col = i / 2;
            Vector3 position = startPosition.position + new Vector3(col * spacing, row * -spacing, 0);
            allUnits[i].transform.position = position;
        }
    }
}
