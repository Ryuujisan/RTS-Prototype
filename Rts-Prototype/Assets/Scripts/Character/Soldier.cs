using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Soldier : Unit, ISelectable
{
	public void SetSeleted(bool seleted)
	{
		hpBar.gameObject.SetActive(seleted);
	}
}
