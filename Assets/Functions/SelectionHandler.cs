using System.Collections.Generic;
using UnityEngine;

public class SelectionHandler
{
    private Camera cam;
    public List<UnitController> selectedUnits = new List<UnitController>();

    public SelectionHandler(Camera camera)
    {
        cam = camera;
    }

    public void SelectUnits(Vector2 startPos, Vector2 endPos)
    {
        selectedUnits.Clear();
        Vector3 startWorld = cam.ScreenToWorldPoint(new Vector3(startPos.x, startPos.y, cam.nearClipPlane));
        Vector3 endWorld = cam.ScreenToWorldPoint(new Vector3(endPos.x, endPos.y, cam.nearClipPlane));

        Vector2 min = new Vector2(Mathf.Min(startWorld.x, endWorld.x), Mathf.Min(startWorld.y, endWorld.y));
        Vector2 max = new Vector2(Mathf.Max(startWorld.x, endWorld.x), Mathf.Max(startWorld.y, endWorld.y));

        Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Unit"))
            {
                selectedUnits.Add(col.GetComponent<UnitController>());
            }
        }
    }
}
