using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.UI
{
	public class GUIHealtBar : MonoBehaviour
	{
		[SerializeField]
		private Vector3 offset;
		
		private const string HP_Canvas = "HPCanvas";
		
		private Slider slider;
		private Unit unit;

		private Transform cameraTransform;
		private void Awake()
		{
			slider = GetComponent<Slider>();
			unit = GetComponentInParent<Unit>();

			cameraTransform = Camera.main.transform;
			
			var canvas = GameObject.FindWithTag(HP_Canvas);
			if(canvas)
			{
				transform.SetParent(canvas.transform);
			}
		}

		private void Update()
		{
			if(!unit)
			{
				Destroy(gameObject);
				return;
			}

			slider.value = unit.HealthPercent;
			transform.position = unit.transform.position + offset;
			transform.LookAt(cameraTransform);

			var rotation = transform.localEulerAngles;
			rotation.y = 180;
			transform.localEulerAngles = rotation;

		}
	}
}

