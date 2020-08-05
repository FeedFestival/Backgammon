using UnityEngine;
using UnityEngine.UI;
using b = Assets.Scripts.Utils.style_base;

namespace Assets.Scripts.Utils
{
    public static class style_utils
    {
        public static float GetPercent(float value, float percent)
        {
            return (value / 100f) * percent;
        }

        public static float GetValuePercent(float value, float maxValue)
        {
            return (value * 100f) / maxValue;
        }

        public static float XPercent(float percent)
        {
            return (b.ScreenWidth / 100f) * percent;
        }
        public static float YPercent(float percent)
        {
            return (b.ScreenHeight / 100f) * percent;
        }

        public static void BootstrapSize(value width, value height, div div)
        {
            var size = b.getParentFreeWidth(div.parent);
            width = b.getValue(width, size);

            // setup the paddings
            div.padding.left().floating = b.getFValue(div.padding.left(), width.floating);
            div.padding.right().floating = b.getFValue(div.padding.right(), width.floating);

            size = b.getParentFreeHeight(div.parent);
            height = b.getValue(height, size);

            div.padding.top().floating = b.getFValue(div.padding.top(), height.floating);
            div.padding.bottom().floating = b.getFValue(div.padding.bottom(), height.floating);

            b.SetFixedSize(width.floating, height.floating, div.element);
        }

        public static void SetPosition(float x, float y, Button button)
        {
            b.SetPosition(x, y, button.GetComponent<RectTransform>());
        }

        public static void SetPosition(float x, float y, Image image)
        {
            b.SetPosition(x, y, image.GetComponent<RectTransform>());
        }

        public static void SetAnchor(AnchorType at, RectTransform rt)
        {
            b.SetAnchor(at, rt);
        }

        public static void SetAnchor(AnchorType at, Button button)
        {
            b.SetAnchor(at, button.GetComponent<RectTransform>());
        }

        public static void SetAnchor(AnchorType at, Image image)
        {
            b.SetAnchor(at, image.GetComponent<RectTransform>());
        }

        public static void getDivs(Transform parent, div div)
        {
            if (div.ignoreChildren)
                return;

            var i = 0;
            foreach (Transform child in parent)
            {
                // TODO: full functionality for borders
                if (child.gameObject.name == "border-top-2")
                {
                    div.border = child.GetComponent<RectTransform>();
                }

                var s = child.GetComponent<style>();
                if (s == null)
                    continue;

                s.Refresh();
                s.Div.childIndex = i;
                s.Div.parent = div;

                getDivs(child, s.Div);

                div.children.Add(s.Div);

                i++;
            }
        }

        public static void buildDiv(div div)
        {
            if (b.isBody(div.elementName) == false)
                build(div);
            if (div.children.Count > 0)
                foreach (div d in div.children)
                {
                    buildDiv(d);
                }
        }

        private static void build(div div)
        {
            if (div.type == divType.div)
            {
                BootstrapSize(div.width,
                    div.height,
                    div);

                setPosition(ref div);
            }
            else if (div.type == divType.label)
            {
                div.element.anchoredPosition = new Vector3(0f, 0f, 0f);
            }
            else if (div.type == divType.button)
            {
                BootstrapSize(div.width,
                    div.height,
                    div);

                div.element.anchoredPosition3D = new Vector3(0f, 0f, -2f);
                div.element.anchoredPosition = new Vector2(0f, 0f);
            }

            if (div.border != null)
            {
                // TODO: full functionality for borders
                div.border.sizeDelta = new Vector3(GetPercent(div.parent.element.sizeDelta.x, 100f), 24f);
                div.border.localPosition = new Vector2(0f, 2f);
            }
        }

        public static void setPosition(ref div div)
        {
            var parentFreeSize = b.getParentFreeWidth(div.parent);

            var elSize = div.element.sizeDelta.x + div.margin.horizontal();

            if (parentFreeSize >= elSize)
            {
                if (div.margin.left().auto && div.margin.right().auto) // it means we center this boot horizontally.
                {
                    var remainingSpace = parentFreeSize - elSize;
                    div.margin.left().floating = remainingSpace / 2;
                }
                else
                {
                    div.margin.left().floating = b.getFValue(div.margin.left(), parentFreeSize);
                }
            }

            var parentHeight = b.getParentFreeHeight(div.parent);
            var elHeight = div.element.sizeDelta.y + div.margin.vertical();

            if (parentHeight >= elHeight)
            {
                if (div.margin.top().auto && div.margin.bottom().auto)
                {
                    var remainingSpace = parentHeight - elHeight;
                    div.margin.top().floating = remainingSpace / 2;
                }
                else
                {
                    div.margin.top().floating = b.getFValue(div.margin.top(), parentHeight);
                }
            }

            float x = div.margin.left().floating;
            float y = div.margin.top().floating;

            x = x + div.parent.padding.left().floating;

            if (div.childIndex > 0)
            {
                float siblingY;
                float biggestSiblingHeight;
                var ocupiedSpace = getOcupiedSpace(div, out siblingY, out biggestSiblingHeight);

                siblingY = Mathf.Abs(siblingY);

                // check if this div fits in with the rest.
                var pS = (int)parentFreeSize;
                if (pS < (ocupiedSpace + elSize))   // div doesn't fit move down + the height of its biggest sibling.
                {
                    y = siblingY + y + biggestSiblingHeight;
                }
                else
                {
                    if (siblingY == 0)
                        y = y + div.parent.padding.top().floating;

                    x = ocupiedSpace + x;
                    y = siblingY + y;
                }
            }
            else
            {
                y = y + div.parent.padding.top().floating;
            }


            div.element.localPosition = new Vector3(x, -y, 0f);
        }

        private static float getOcupiedSpace(div div, out float siblingY, out float biggestSiblingHeight)
        {
            var siblingIndex = div.childIndex - 1;
            var sibling = div.parent.children;

            siblingY = sibling[siblingIndex].element.localPosition.y;

            biggestSiblingHeight = sibling[siblingIndex].element.sizeDelta.y + sibling[siblingIndex].margin.vertical();

            var space = 0f;
            space += sibling[siblingIndex].element.sizeDelta.x + sibling[siblingIndex].margin.horizontal();

            if (siblingIndex != 0)
            {
                for (var i = siblingIndex - 1; i >= 0; i--)
                {
                    if ((int)sibling[i].element.localPosition.y != (int)siblingY)
                        continue;

                    if (biggestSiblingHeight <
                        sibling[i].element.sizeDelta.y + sibling[siblingIndex].margin.vertical())
                        biggestSiblingHeight = sibling[i].element.sizeDelta.y +
                                               sibling[siblingIndex].margin.vertical();

                    space += sibling[i].element.sizeDelta.x + sibling[i].margin.horizontal();
                }
            }

            // getting it without the margin
            if (siblingY < 0)
                siblingY = siblingY + sibling[siblingIndex].margin.top().floating;
            else
                siblingY = siblingY - sibling[siblingIndex].margin.top().floating;

            return space;
        }
    }
}