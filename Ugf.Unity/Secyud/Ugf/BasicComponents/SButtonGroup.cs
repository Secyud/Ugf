#region

using System;
using System.Collections.Generic;
using Secyud.Ugf.LayoutComponents;
using Secyud.Ugf.TableComponents.ButtonComponents;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    public class SButtonGroup : LayoutGroupTrigger
    {
        public static SButtonGroup Instance;
        [SerializeField] private SLabelButton ButtonTemplate;

        public void Clear()
        {
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
        }

        public void OnInitialize<TItem>(TItem target,
            IEnumerable<ButtonDescriptor<TItem>> buttons)
        {
            Clear();

            foreach (ButtonDescriptor<TItem> button in buttons.SelectVisibleFor(target))
            {
                button.Target = target;
                SLabelButton b = ButtonTemplate.Instantiate(transform);
                b.Bind(button.Invoke);
                if (Float)
                    b.onClick.AddListener(() => Destroy(gameObject));
                b.Text = U.T[button.ShowName];
                button.SetButton(b);
            }

            if (Float && LayoutElement is GridLayoutGroup group)
            {
                group.constraintCount = (int)Math.Ceiling(RectTransform.rect.height / Screen.height);
                RectTransform.SetRectPosition(
                    UgfUnityExtensions.GetMousePosition() - new Vector2(8, 8),
                    new Vector2(0, -0.5f)
                );
            }

            enabled = true;
        }

        public SButtonGroup Create<TItem>(TItem target,
            IEnumerable<ButtonDescriptor<TItem>> buttons)
        {
            SButtonGroup group = Instantiate(this, U.Canvas.transform);
            group.OnInitialize(target, buttons);
            group.Replace(ref Instance);
            return group;
        }
    }
}