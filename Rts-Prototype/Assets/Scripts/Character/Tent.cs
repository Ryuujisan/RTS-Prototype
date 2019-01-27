using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tent : MonoBehaviour, ISelectable
{
    [SerializeField]
    private GameObject soldierPrefab;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private Transform flags;
    
    [SerializeField]
    private GameObject hpBarPrefab;
    
    protected GUIHealtBar hpBar;
    
    private void Start()
    {
        Unit.selectablesUnit.Add(this);
        hpBar = Instantiate(hpBarPrefab, transform).GetComponent<GUIHealtBar>();
    }

    public void SetSeleted(bool seleted)
    {
        flags.gameObject.SetActive(false);
        hpBar.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Unit.selectablesUnit.Remove(this);
    }

    void SpawnUnit(GameObject prefab)
    {
        var unit = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        unit.SendMessage("Command", flags.position);
    }
}
