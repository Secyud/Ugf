using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.UiForms
{
    public class UiFormGroup
    {
        public int GroupId { get; }
        public List<Element> Elements { get; } = new();

        private readonly RectTransform _transform;

        public UiFormGroup(int groupId)
        {
            GroupId = groupId;
            GameObject groupObject = new($"Ui Group {groupId}");
            RectTransform canvasTransform = U.Canvas.transform as RectTransform;
            _transform = groupObject.AddComponent<RectTransform>();
            _transform.SetParent(canvasTransform);
            _transform.localPosition = Vector3.zero;
            _transform.sizeDelta = canvasTransform!.sizeDelta;
        }

        public void CreateAll()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].CreateForm();
            }

            SetInFrontOfUi();
        }

        public void DestroyAll()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].DestroyFrom();
            }
        }

        public void HideAll()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].HideForm();
            }
        }

        public void ShowAll()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].ShowForm();
            }

            SetInFrontOfUi();
        }

        private void SetInFrontOfUi()
        {
            _transform.SetAsLastSibling();
        }

        public class Element
        {
            private readonly Vector3 _position;
            public string Name { get; }
            public UiFormGroup Group { get; }
            public UiFormBase Prefab { get; }
            public UiFormBase Instance { get; private set; }

            public Element(string name, UiFormBase prefab, UiFormGroup group)
            {
                _position = prefab.transform.localPosition;
                Name = name;
                Group = group;
                Prefab = prefab;
            }

            public void CreateForm()
            {
                if (!Instance)
                {
                    Instance = Prefab.Instantiate(Group._transform);
                    Instance.OnShowing();
                }

                SetInFrontOfUi();
            }

            public void DestroyFrom()
            {
                if (!Instance) return;
                Instance.OnHiding();
                Instance.Destroy();
            }

            public void HideForm()
            {
                if (!Instance || !Instance.Visible) return;
                
                Instance.transform.localPosition =
                    new Vector3(65536, 0, 0);
                Instance.Visible = false;
                Instance.OnHiding();
            }

            public void ShowForm()
            {
                if (!Instance) return;
                if (!Instance.Visible)
                {
                    Instance.transform.localPosition = _position;
                    Instance.Visible = true;
                    Instance.OnShowing();
                }

                SetInFrontOfUi();
            }

            private void SetInFrontOfUi()
            {
                Instance.transform.SetAsLastSibling();
            }
        }
    }
}