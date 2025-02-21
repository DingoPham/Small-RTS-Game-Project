using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public float walkSpeed = 1f;  // Tốc độ đi bộ
    public float runSpeed = 3f;   // Tốc độ chạy
    private Vector3 targetPosition;
    private float speed;
    private int clickCount = 0; // Đếm số lần click
    private float lastClickTime = 0f;
    public float doubleClickThreshold = 0.5f; // Khoảng thời gian tối đa giữa 2 lần nhấn để tính là double click
    private bool isMale;
    private bool inLove = false;
    private Unit partner;
    private GameObject selectionOutline;
    private Coroutine resetToWalkCoroutine;

    void Start()
    {
        targetPosition = transform.position;
        speed = walkSpeed; // Ban đầu đi bộ

        // Tìm đối tượng con "SelectionOutline"
        selectionOutline = transform.Find("SelectionOutline")?.gameObject;
        if (selectionOutline != null)
        {
            selectionOutline.SetActive(false); // Ẩn viền ban đầu
        }
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void SetGender(bool male)
    {
        isMale = male;
        GetComponent<SpriteRenderer>().color = male ? Color.blue : Color.red; // Đổi màu theo giới tính
    }

    public void MoveTo(Vector3 position)
    {
        clickCount++;

        // Nếu là lần nhấp đầu tiên hoặc quá lâu từ lần trước, đặt lại bộ đếm
        if (Time.time - lastClickTime > doubleClickThreshold)
        {
            clickCount = 1;
        }

        lastClickTime = Time.time; // Cập nhật thời điểm nhấp cuối

        if (clickCount >= 2)
        {
            speed = runSpeed; // Chạy khi double click
        }
        else
        {
            speed = walkSpeed; // Đi bộ khi single click
        }

        // Nếu có coroutine đang chạy, dừng lại để đặt lại bộ đếm chính xác
        if (resetToWalkCoroutine != null)
        {
            StopCoroutine(resetToWalkCoroutine);
        }
        // Bắt đầu một coroutine để đặt lại trạng thái đi bộ sau một khoảng thời gian
        resetToWalkCoroutine = StartCoroutine(ResetToWalk());

        StartCoroutine(FindValidPosition(position));
    }
    IEnumerator ResetToWalk()
    {
        yield return new WaitForSeconds(2f); // Sau 2 giây không click, trở lại đi bộ
        speed = walkSpeed;
        clickCount = 0;
    }

    IEnumerator FindValidPosition(Vector3 position)
    {
        int maxAttempts = 10; // Giới hạn số lần kiểm tra
        int attempts = 0;

        while (IsPositionOccupied(position) && attempts < maxAttempts )
        {
            position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            attempts++;
            yield return null;
        }
        targetPosition = position;

        if (attempts < maxAttempts)
        {
            targetPosition = position;
        }
        else
        {
            Debug.LogWarning("không thể tìm thấy đường sau 10 lần thử!");
        }
    }

    private bool IsPositionOccupied(Vector3 pos)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 0.5f);
        return colliders.Length > 1; // Nếu có đơn vị khác tại đây
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionOutline != null)
        {
            selectionOutline.SetActive(isSelected);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Unit otherUnit = other.GetComponent<Unit>();
        if (otherUnit != null && !inLove && isMale != otherUnit.isMale)
        {
            inLove = true;
            partner = otherUnit;
            StartCoroutine(CreateNewUnit());
        }
    }

    private static int maxUnits = 50; // Giới hạn số lượng đơn vị

    IEnumerator CreateNewUnit()
    {
        if (FindObjectsOfType<Unit>().Length >= maxUnits)
        {
            Debug.LogWarning("Đã đạt giới hạn số lượng đơn vị!");
            yield break;
        }

        yield return new WaitForSeconds(5f); // Đợi 5 giây

        GameObject newUnitObj = Instantiate(gameObject, transform.position, Quaternion.identity);
        Unit newUnit = newUnitObj.GetComponent<Unit>();
        newUnit.SetGender(Random.value > 0.5f);
    }

}
