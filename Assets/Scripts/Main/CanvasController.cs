using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Assets.Scripts.Utils;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    private Main _main;

    private div _body;
    
    private int index = 0;

    public void Init(Main main)
    {
        _main = main;

        FillScope();
    }

    public int InspectorScreenWidth;
    public int InspectorScreenHeight;
    public string InspectorScreenName;

    public void Build(bool inspector)
    {
        if (inspector)
            GetBasicElements();

        style_utils.buildDiv(_body);
    }

    public void GetBasicElements()
    {
        _body = new div
        {
            Id = index.ToString(),
            elementName = transform.gameObject.name,
            children = new List<div>(),
            type = divType.body,
            element = transform.GetComponent<RectTransform>()
        };
        style_utils.SetAnchor(AnchorType.TopLeft, _body.element);
        _body.element.localPosition = new Vector3(-(_body.element.sizeDelta.x / 2), (_body.element.sizeDelta.y / 2), 0);

        style_utils.getDivs(transform, _body);
    }

    public void FillScope()
    {
        _main.Game.scope = new Dictionary<string, GameObject>();

        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.name.Contains("View") && child.gameObject.name != "Viewport")
            {
                var s = child.gameObject.name.Split('{', '}');
                _main.Game.scope.Add(s[1], child.gameObject);
            }

            if (child.gameObject.name.Contains("Dropdown"))
            {
                _main.Game.scope.Add(child.gameObject.name, child.gameObject);
            }

            var obj = child.GetComponent<Button>();
            if (obj != null)
            {
                _main.Game.scope.Add(obj.transform.gameObject.name, obj.gameObject);
            }
            var objText = child.GetComponent<Text>();
            if (objText != null && (objText.name != "Text" && objText.name != "Placeholder"))
            {
                if (child.gameObject.name.Contains("Text}"))
                {
                    var s = child.gameObject.name.Split('{', '}');
                    _main.Game.scope.Add(s[1], objText.gameObject);
                }
                else
                    _main.Game.scope.Add(objText.transform.gameObject.name, objText.gameObject);
            }
        }
    }
}