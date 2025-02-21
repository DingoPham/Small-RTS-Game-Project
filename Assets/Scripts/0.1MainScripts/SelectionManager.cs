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
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider == null || !hit.collider.CompareTag("Unit"))
            {
                DeselectAllUnits();
            }
            
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

            float radius = 1.5f; // Bán kính tối đa của cụm đơn vị
            int unitCount = selectedUnits.Count;
            float angleStep = 360f / unitCount; // Chia đều góc để không quá xa nhau

            for (int i = 0; i < unitCount; i++)
            {
                // Tạo vị trí ngẫu nhiên trong bán kính
                float angle = i * angleStep + Random.Range(-15f, 15f); // Có chút ngẫu nhiên để không quá đều
                float randomRadius = Random.Range(0.3f, radius); // Giữ khoảng cách hợp lý

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * randomRadius,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * randomRadius,
                    0
                );

                Vector3 newTarget = target + offset;
                selectedUnits[i].MoveTo(newTarget);
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
        if (isSelecting) // Chỉ bỏ chọn khi thực sự kéo bôi đen lần mới
        {
            DeselectAllUnits();
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
                Unit unit = col.GetComponent<Unit>();
                selectedUnits.Add(unit);
                unit.SetSelected(true); // Hiển thị viền chọn
            }
        }
    }

    void DeselectAllUnits()
    {
        foreach (Unit unit in selectedUnits)
        {
            unit.SetSelected(false); // Ẩn viền chọn
        }
        selectedUnits.Clear();
    }
}
