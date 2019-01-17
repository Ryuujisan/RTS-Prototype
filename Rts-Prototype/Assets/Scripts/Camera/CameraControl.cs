using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{   
	public float cameraSpeed;
	public float zoomSpeed;
	public float graundHight;
	
	public Vector2 cameraHeightMinMax;
	public Vector2 cameraRotationMinMax;

	[Range(0, 1)]
	public float zoomLerp = .1f;

	[Range(0, 0.2f)]
	public float cursorTreshold;
	
	private RectTransform selectionBox;
	private new Camera camera;

	private Vector2 mousePos;
	private Vector2 mousePosScreen;
	private Vector2 keyBoardInput;
	private Vector2 mouseScroll;
	
	private bool isCursorInGameScreen;
	
	private Rect seletionRect;
	private Rect boxRect;

	private List<Unit> selectedUnits = new List<Unit>();
	
	private void Awake()
	{
		selectionBox = GetComponentInChildren<Image>(true).transform as RectTransform;
		camera = GetComponent<Camera>();
		
		selectionBox.gameObject.SetActive(false);
	}

	
	// Update is called once per frame
	void Update () 
	{
		UpdateMovemnent();
		UpdateZoom();
		UpdateClicks();	
	}

	private void UpdateMovemnent()
	{
		keyBoardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		mousePos = Input.mousePosition;
		
		mousePosScreen = camera.ScreenToViewportPoint(mousePos);

		isCursorInGameScreen = mousePosScreen.x >= 0 && mousePosScreen.x <= 1 &&
		                             mousePosScreen.y >= 0 && mousePosScreen.y <= 1;

		Vector2 movmentDirection = keyBoardInput;

		if(isCursorInGameScreen)
		{
			if(mousePosScreen.x < cursorTreshold) movmentDirection.x -= 1 - mousePosScreen.x / cursorTreshold;
			if(mousePosScreen.x > 1 - cursorTreshold) movmentDirection.x += 1- (1 - mousePosScreen.x) / cursorTreshold;
			
			if(mousePosScreen.y < cursorTreshold) movmentDirection.y -= 1 - mousePosScreen.y / cursorTreshold;
			if(mousePosScreen.y > 1 - cursorTreshold) movmentDirection.y += 1- (1 - mousePosScreen.y) / cursorTreshold;
		}
		
		var deltaPos = new Vector3(movmentDirection.x, 0 , movmentDirection.y) ;
		deltaPos *= cameraSpeed * Time.deltaTime;
		transform.position += deltaPos;
	}

	private void UpdateZoom()
	{
		 mouseScroll = Input.mouseScrollDelta;
		 float deltaZoom = mouseScroll.y * zoomSpeed * Time.deltaTime;

		 zoomLerp = Mathf.Clamp01(zoomLerp + deltaZoom);

		 var position = transform.localPosition;
		 position.y = Mathf.Lerp(cameraHeightMinMax.y, cameraHeightMinMax.x, zoomLerp) + graundHight;
		 camera.transform.position = position;
		 
		 var rotation = transform.localEulerAngles;
		 rotation.x = Mathf.Lerp(cameraRotationMinMax.y, cameraRotationMinMax.x, zoomLerp);
		 camera.transform.localEulerAngles = rotation;
	}
	
	private void UpdateClicks()
	{
		
		if(Input.GetMouseButtonDown(0))
		{
			selectionBox.gameObject.SetActive(true);
			seletionRect.position = mousePos;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			selectionBox.gameObject.SetActive(false);
		}

		if(Input.GetMouseButton(0))
		{
			seletionRect.size = mousePos - seletionRect.position;
			boxRect = AbsRect(seletionRect);
			
			selectionBox.anchoredPosition = boxRect.position;
			selectionBox.sizeDelta = boxRect.size;
			UpdateSelecting();
		}
	}
	
	private void UpdateSelecting()
	{
		selectedUnits.Clear();
		
		foreach(Unit unit
			in Unit.selectablesUnit)
		{
			if(unit == null) continue;
			var pos       = unit.transform.position;
			var posScrean = camera.WorldToScreenPoint(pos);

			bool inRect = IsPointInRect(boxRect, posScrean);
			(unit as ISelectable).SetSeleted(inRect);

			if(inRect)
			{
				selectedUnits.Add(unit);
			}
		}
	}

	private bool IsPointInRect(Rect rect, Vector2 point)
	{
		return point.x >= rect.position.x && point.x <= (rect.position.x + rect.size.x) &&
		       point.y >= rect.position.y && point.y <= (rect.position.y + rect.size.y);
	}
	
	private Rect AbsRect(Rect rect)
	{
		if(rect.width < 0)
		{
			rect.x += rect.width;
			rect.width *= -1;
		}
		
		if(rect.height < 0)
		{
			rect.y     += rect.height;
			rect.height *= -1;
		}

		return rect;
	}
}
