using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManagerController : MonoBehaviour
{
    public RectTransform selectionBox;
    private Vector2 startPos;
    private Vector2 endPos;
    private bool isSelecting = false;
    private Camera cam;

    private SelectionHandler selectionHandler;
    private FormationHandler formationHandler;

    void Start()
    {
        cam = Camera.main;
        selectionBox.gameObject.SetActive(false);

        selectionHandler = new SelectionHandler(cam);
        formationHandler = new FormationHandler();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            startPos = Input.mousePosition;
            selectionBox.gameObject.SetActive(true);
        }

        if (isSelecting)
        {
            endPos = Input.mousePosition;
            DrawSelectionBox();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            selectionBox.gameObject.SetActive(false);
            selectionHandler.SelectUnits(startPos, endPos);
        }

        if (Input.GetMouseButtonDown(1) && selectionHandler.selectedUnits.Count > 0)
        {
            Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;
            formationHandler.MoveUnitsInFormation(selectionHandler.selectedUnits, target);
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
}
