using UnityEngine;
using System.Collections.Generic;

public class GameManagerController : MonoBehaviour
{
    public GameObject unitPrefab;
    public int totalUnits = 10;
    private List<UnitController> units = new List<UnitController>();

    void Start()
    {
        SpawnUnits();
    }

    void SpawnUnits()
    {
        for (int i = 0; i < totalUnits; i++)
        {
            CreateUnit(i % 2 == 0 ? UnitController.Gender.Male : UnitController.Gender.Female);
        }
    }

    public void CreateUnit(UnitController.Gender gender)
    {
        GameObject newUnit = Instantiate(unitPrefab, GetRandomPosition(), Quaternion.identity);
        UnitController controller = newUnit.GetComponent<UnitController>();
        controller.gender = gender;
        units.Add(controller);

        // Thay đổi màu theo giới tính
        SpriteRenderer spriteRenderer = newUnit.GetComponent<SpriteRenderer>();
        spriteRenderer.color = gender == UnitController.Gender.Male ? Color.blue : Color.red;
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
    }
}
