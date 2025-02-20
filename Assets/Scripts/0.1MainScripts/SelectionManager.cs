using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public RectTransform SelectionBox; // Ô chọn trong UI
    private Vector2 startPos;
    private Vector2 endPos;
    private bool isSelecting = false;
    private Camera cam;
    public List<Unit> selectedUnits = new List<Unit>();

    void Start()
    {
        cam = Camera.main;
        SelectionBox.gameObject.SetActive(false); // Ẩn ô chọn ban đầu
    }

    void Update()
    {
        // Nếu nhấn chuột trái và đang có đơn vị được chọn, thì bỏ chọn
        if (Input.GetMouseButtonDown(0) && selectedUnits.Count > 0)
        {
            selectedUnits.Clear();
            return; // Thoát khỏi Update để tránh xử lý thêm
        }

        if (Input.GetMouseButtonDown(1)) // Bắt đầu chọn
        {
            isSelecting = true;
            startPos = Input.mousePosition;
            SelectionBox.gameObject.SetActive(true);
            SelectionBox.anchoredPosition = startPos; // Cố định vị trí bắt đầu
            SelectionBox.sizeDelta = Vector2.zero; // Reset kích thước
        }

        if (isSelecting) // Trong khi kéo chuột
        {
            endPos = Input.mousePosition;
            DrawSelectionBox();
        }

        if (Input.GetMouseButtonUp(1)) // Khi thả chuột
        {
            isSelecting = false;
            SelectionBox.gameObject.SetActive(false);
            SelectUnits();
        }

        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0) // Di chuyển đơn vị
        {
            Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;

            float offsetX = 0;
            foreach (Unit unit in selectedUnits)
            {
                Vector3 newTarget = target + new Vector3(offsetX, 0, 0);
                unit.MoveTo(newTarget);
                offsetX += 0.5f; // Giãn cách các đơn vị
            }
        }
    }

    void DrawSelectionBox()
    {
        if (!SelectionBox.gameObject.activeInHierarchy)
            SelectionBox.gameObject.SetActive(true);

        Vector2 mousePos = Input.mousePosition;

        float width = Mathf.Abs(mousePos.x - startPos.x);
        float height = Mathf.Abs(mousePos.y - startPos.y);

        Vector2 pivotFix = new Vector2(
            (mousePos.x < startPos.x) ? 1 : 0,  // Nếu kéo ngược về bên trái, pivot x = 1
            (mousePos.y < startPos.y) ? 1 : 0   // Nếu kéo xuống, pivot y = 1
        );

        SelectionBox.pivot = pivotFix;
        SelectionBox.anchoredPosition = startPos;
        SelectionBox.sizeDelta = new Vector2(width, height);
    }

    void SelectUnits()
    {
        if (Vector2.Distance(startPos, endPos) > 10f) // Chỉ reset khi kéo chuột đủ xa
        {
            selectedUnits.Clear();
        }
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
