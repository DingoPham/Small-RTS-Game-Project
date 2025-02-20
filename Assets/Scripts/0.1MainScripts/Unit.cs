using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 targetPosition;
    private bool isMale;
    private bool inLove = false;
    private Unit partner;

    void Start()
    {
        targetPosition = transform.position;
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
        StartCoroutine(FindValidPosition(position));
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
