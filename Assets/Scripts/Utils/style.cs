using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;

public class style : MonoBehaviour
{
    private div _div;
    public div Div
    {
        get
        {
            if (_div == null)
                _div = new div
                {
                    Id = Name,
                    elementName = gameObject.name,
                    type = divType.div,
                    element = GetComponent<RectTransform>(),
                    children = new List<div>()
                };
            return _div;
        }
    }

    public string Name;

    public string Class;

    public value Height
    {
        get
        {
            return Div.height;
        }
    }

    public value[] Margin;

    public void Calculate()
    {
        var pTransform = transform.parent.transform;
        var _parent = new div
        {
            Id = 0.ToString(),
            elementName = pTransform.gameObject.name,
            children = new List<div>(),
            type = pTransform.gameObject.name.Contains("<body") ? divType.body : divType.div,
            element = pTransform.GetComponent<RectTransform>()
        };

        //if (_parent.type == divType.body)
        //{
        //    style_utils.SetAnchor(AnchorType.TopLeft, _parent.element);
        //    _parent.element.localPosition = new Vector3(-(_parent.element.sizeDelta.x / 2), (_parent.element.sizeDelta.y / 2), 0);
        //}

        style_utils.getDivs(pTransform, _parent);

        //if (_parent.type == divType.body)
        //    style_utils.buildDiv(_parent);
        //else
        style_utils.buildDiv(_div);
    }

    public void Refresh()
    {
        _div = new div
        {
            Id = Name,
            elementName = gameObject.name,
            type = divType.div,
            element = GetComponent<RectTransform>(),
            children = new List<div>()
        };
    }
}