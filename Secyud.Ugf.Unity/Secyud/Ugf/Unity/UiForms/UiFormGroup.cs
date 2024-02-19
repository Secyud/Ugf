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
            int count = Elements.Count;
            for (int i = 0; i < count; i++)
            {
                Element form = Elements[i];
                form.CreateInstance();
            }

            SetInFrontOfUi();
        }

        public void DestroyAll()
        {
            int count = Elements.Count;
            for (int i = 0; i < count; i++)
            {
                Element form = Elements[i];
                form.DestroyInstance();
            }
        }

        public void ShowAll()
        {
            int count = Elements.Count;
            for (int i = 0; i < count; i++)
            {
                Element form = Elements[i];
                form.SetFormVisible();
            }

            SetInFrontOfUi();
        }

        public void HideAll()
        {
            int count = Elements.Count;
            for (int i = 0; i < count; i++)
            {
                Element form = Elements[i];
                form.SetFormInvisible();
            }
        }

        public void SetInFrontOfUi()
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
                CreateInstance();
                SetFormVisible();
            }

            public void DestroyFrom()
            {
                DestroyInstance();
            }

            public void ShowForm()
            {
                SetFormVisible();
                SetInFrontOfUi();
                Group.SetInFrontOfUi();
            }

            public void HideForm()
            {
                SetFormInvisible();
            }

            private void SetInFrontOfUi()
            {
                Instance.transform.SetAsLastSibling();
            }

            internal void CreateInstance()
            {
                if (Instance) return;
                Instance = Prefab.Instantiate(Group._transform);
                Instance.OnShowing();
            }

            internal void DestroyInstance()
            {
                if (!Instance) return;
                Instance.OnHiding();
                Instance.DestroyGameObject();
            }

            internal void SetFormVisible()
            {
                if (!Instance || Instance.Visible) return;
                Instance.transform.localPosition = _position;
                Instance.Visible = true;
                Instance.OnShowing();
            }

            internal void SetFormInvisible()
            {
                if (!Instance || !Instance.Visible) return;
                Instance.transform.localPosition =
                    new Vector3(65536, 0, 0);
                Instance.Visible = false;
                Instance.OnHiding();
            }
        }
    }
}