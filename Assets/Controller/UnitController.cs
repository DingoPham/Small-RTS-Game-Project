using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public enum Gender { Male, Female }

    public Gender gender;
    public bool isInLove = false;
    public UnitController partner;
    private float loveTime = 5f;
    private Vector3 targetPosition;
    public float speed = 0.5f;

    private GameManagerController gameManager;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        targetPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Đặt màu sắc hoặc hình dạng tương ứng giới tính
        if (gender == Gender.Male)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);  // Vuông
            spriteRenderer.color = Color.blue;  // Màu xanh cho nam
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);  // Tròn
            spriteRenderer.color = Color.red;  // Màu đỏ cho nữ
        }
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void Initialize(Gender gender, GameManagerController manager)
    {
        this.gender = gender;
        this.gameManager = manager;
    }

    public void CheckForLove(UnitController other)
    {
        if (gender != other.gender && partner == null && other.partner == null)
        {
            partner = other;
            other.partner = this;
            StartCoroutine(FallInLove());
        }
    }

    IEnumerator FallInLove()
    {
        isInLove = true;
        yield return new WaitForSeconds(loveTime);

        // Sinh đơn vị con sau thời gian yêu nhau
        gameManager.CreateUnit(GetRandomGender());

        // Reset trạng thái để có thể yêu người khác
        isInLove = false;
        partner.isInLove = false;
        partner.partner = null;
        partner = null;
    }

    private Gender GetRandomGender()
    {
        return Random.value > 0.5f ? Gender.Male : Gender.Female;
    }

    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
    }
}
