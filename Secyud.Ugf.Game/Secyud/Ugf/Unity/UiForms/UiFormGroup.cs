using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.UiForms
{
    public class UiFormGroup
    {
        public int GroupId { get; }
        public List<Element> Elements { get; } = new();
        
        private readonly Transform _transform;

        public UiFormGroup(int groupId)
        {
            GroupId = groupId;
            GameObject groupObject = new($"Ui Group {groupId}");
            _transform = groupObject.transform;
            _transform.SetParent(U.Canvas.transform);
            _transform.localPosition = Vector3.zero;
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
            _transform.SetSiblingIndex(_transform.parent.childCount - 1);
        }

        public class Element
        {
            private readonly Vector3 _position;
            public string Name { get; }
            public  UiFormGroup Group {get;}
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
            }

            public void DestroyFrom()
            {
                if (Instance)
                {
                    Instance.OnHiding();
                    Instance.Destroy();
                }
            }

            public void HideForm()
            {
                if (Instance && Instance.Visible)
                {
                    Instance.transform.localPosition =
                        new Vector3(65536, 0, 0);
                    Instance.Visible = false;
                    Instance.OnHiding();
                }
            }

            public void ShowForm()
            {
                if (Instance && !Instance.Visible)
                {
                    Instance.transform.localPosition = _position;
                    Instance.Visible = true;
                    Instance.OnShowing();
                }

                Instance.transform.SetSiblingIndex(
                    Group._transform.childCount - 1);
            }
        }
    }
}