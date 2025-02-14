using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public RectTransform selectionBox; // Ô chọn trong UI
    private Vector2 startPos;
    private Vector2 endPos;
    private bool isSelecting = false;
    private Camera cam;
    public List<Unit> selectedUnits = new List<Unit>();

    void Start()
    {
        cam = Camera.main;
        selectionBox.gameObject.SetActive(false); // Ẩn ô chọn ban đầu
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Bắt đầu chọn
        {
            isSelecting = true;
            startPos = Input.mousePosition;
            selectionBox.gameObject.SetActive(true);
        }

        if (isSelecting) // Trong khi kéo chuột
        {
            endPos = Input.mousePosition;
            DrawSelectionBox();
        }

        if (Input.GetMouseButtonUp(0)) // Khi thả chuột
        {
            isSelecting = false;
            selectionBox.gameObject.SetActive(false);
            SelectUnits();
        }

        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0) // Di chuyển đơn vị
        {
            Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;
            foreach (Unit unit in selectedUnits)
            {
                unit.MoveTo(target);
            }
        }
    }

    void DrawSelectionBox()
    {
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        Vector2 currentMousePos = Input.mousePosition;
        Vector2 boxStart = startPos;
        Vector2 boxSize = currentMousePos - startPos;

        selectionBox.anchoredPosition = boxStart + boxSize / 2;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(boxSize.x), Mathf.Abs(boxSize.y));
    }

    void SelectUnits()
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
                selectedUnits.Add(col.GetComponent<Unit>());
            }
        }
    }
}
