using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 15f;  // Tốc độ di chuyển khi đẩy chuột vào mép màn hình
    public float edgeThreshold = 10f; // Khoảng cách mép màn hình để di chuyển
    public float dragSpeed = 10f;  // Tốc độ kéo khi giữ chuột trái
    private Vector3 lastMousePosition;
    private bool isDragging = false;

    void Update()
    {
        MoveByScreenEdge();
        MoveByMouseDrag();
    }

    // Cách 1: Di chuyển khi đưa chuột vào mép màn hình
    private void MoveByScreenEdge()
    {
        Vector3 direction = Vector3.zero;
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x >= Screen.width - edgeThreshold) direction.x += 1;
        if (mousePosition.x <= edgeThreshold) direction.x -= 1;
        if (mousePosition.y >= Screen.height - edgeThreshold) direction.y += 1;
        if (mousePosition.y <= edgeThreshold) direction.y -= 1;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    // Cách 2: Giữ chuột trái để kéo màn hình
    private void MoveByMouseDrag()
    {
        if (Input.GetMouseButtonDown(0)) // Giữ chuột trái thay vì chuột phải
        {
            lastMousePosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x, -delta.y, 0) * dragSpeed * Time.deltaTime;
            transform.position += move;
            lastMousePosition = Input.mousePosition;
        }
    }
}
