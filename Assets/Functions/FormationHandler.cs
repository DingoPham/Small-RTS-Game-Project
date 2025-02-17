using System.Collections.Generic;
using UnityEngine;

public class FormationHandler
{
    private float spacing = 1.5f; // Khoảng cách giữa các đơn vị
    private int maxAttempts = 10; // Số lần thử tìm vị trí trống
    public void MoveUnitsInFormation(List<UnitController> units, Vector3 targetPosition)
    {
        int unitCount = units.Count;
        if (unitCount == 0) return;

        int columns = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
        float spacing = 1.5f;

        int row = 0, col = 0;

        foreach (UnitController unit in units)
        {
            float offsetX = (col - (columns / 2)) * spacing;
            float offsetY = (row - (unitCount / columns / 2)) * spacing;

            Vector3 newPosition = targetPosition + new Vector3(offsetX, offsetY, 0);

            newPosition = FindNearestFreePosition(newPosition);

            unit.MoveTo(newPosition);

            col++;
            if (col >= columns)
            {
                col = 0;
                row++;
            }
        }
    }
    private Vector3 FindNearestFreePosition(Vector3 target)
    {
        Collider2D hit = Physics2D.OverlapCircle(target, spacing / 2);
        if (hit == null)
        {
            return target; // Không có va chạm, trả về vị trí ban đầu
        }

        // Nếu có đơn vị khác, thử các vị trí xung quanh theo vòng tròn
        for (int i = 1; i <= maxAttempts; i++)
        {
            Vector3 offset = Random.insideUnitCircle * spacing;
            Vector3 newPos = target + new Vector3(offset.x, offset.y, 0);

            if (Physics2D.OverlapCircle(newPos, spacing / 2) == null)
            {
                return newPos;
            }
        }

        return target; // Nếu không tìm được vị trí trống, giữ nguyên vị trí ban đầu
    }
}
