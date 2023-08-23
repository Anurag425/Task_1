using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _label;
    [SerializeField]
    private TextMeshProUGUI _point;
    [SerializeField]
    private int _id;

    public void UpdateValues(string label, int point, int id)
    {
        _label.text = label;
        _point.text = point.ToString();
        _id = id;
    }

    public void DestroyBlock()
    {
        Destroy(this.gameObject);
    }

    public void OnViewClient()
    {
        Manager.instance.ShowClientInfoPopup(_id);
    }



}
