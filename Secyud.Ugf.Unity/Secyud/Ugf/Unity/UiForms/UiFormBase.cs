using System;
using UnityEngine;

namespace Secyud.Ugf.Unity.UiForms
{
    public abstract class UiFormBase : MonoBehaviour
    {
        public bool Visible { get; internal set; }

        public virtual int GroupId => 0;

        public UiFormGroup.Element GroupElement { get;  internal set; }

        protected internal virtual void OnShowing()
        {
        }

        protected internal virtual void OnHiding()
        {
        }
    }

    /// <summary>
    /// TPanel is this panel.
    /// </summary>
    /// <typeparam name="TForm"> this form type </typeparam>
    public abstract class UiFormBase<TForm> : UiFormBase where TForm : UiFormBase
    {
        private static TForm _prefab;


        protected static void ActionWithPrefab(
            Action<UiFormGroup.Element> action)
        {
            if (_prefab)
            {
                action?.Invoke(_prefab.GroupElement);
                return;
            }

            UiFormService service = U.Get<UiFormService>();
            service.GetFormPrefabAsync(FormCollectionCallback);
            return;

            void FormCollectionCallback(
                UiFormCollection c)
            {
                TForm form = c.GetForm<TForm>();

                if (!form)
                    throw new UgfNotRegisteredException(
                        nameof(UiFormCollection),
                        typeof(TForm).Name);

                _prefab = form;
                form.GroupElement = service.AddForm(form);
                action?.Invoke(_prefab.GroupElement);
            }
        }

        public static void CreateGroup()
        {
            ActionWithPrefab(e => e.Group.CreateAll());
        }

        public static void DestroyGroup()
        {
            ActionWithPrefab(e => e.Group.DestroyAll());
        }

        public static void ShowGroup()
        {
            ActionWithPrefab(e => e.Group.ShowAll());
        }

        public static void HideGroup()
        {
            ActionWithPrefab(e => e.Group.HideAll());
        }

        public static void CreateForm()
        {
            ActionWithPrefab(e => e.CreateForm());
        }

        public static void DestroyFrom()
        {
            ActionWithPrefab(e => e.DestroyFrom());
        }

        public static void ShowForm()
        {
            ActionWithPrefab(e => e.ShowForm());
        }

        public static void HideForm()
        {
            ActionWithPrefab(e => e.HideForm());
        }

        public static TForm GetForm()
        {
            return _prefab.GroupElement.Instance as TForm;
        }
    }
}