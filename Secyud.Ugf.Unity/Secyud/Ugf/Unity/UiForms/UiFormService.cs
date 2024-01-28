using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Unity.AssetLoading;

namespace Secyud.Ugf.Unity.UiForms
{
    public sealed class UiFormService : IRegistry
    {
        public PrefabContainer<UiFormCollection> FormPrefabs { get; set; }

        private readonly Dictionary<int, UiFormGroup> _uiFormGroups = new();

        public UiFormGroup GetGroup(int groupId)
        {
            if (!_uiFormGroups.TryGetValue(groupId, out UiFormGroup group))
            {
                group = new UiFormGroup(groupId);
                _uiFormGroups[groupId] = group;
            }

            return group;
        }

        public UiFormGroup.Element AddForm(UiFormBase prefab)
        {
            UiFormGroup group = GetGroup(prefab.GroupId);
            string name = prefab.GetType().Name;
            UiFormGroup.Element element = group.Elements
                .FirstOrDefault(u => u.Name == name);
            if (element is null)
            {
                element = new UiFormGroup.Element(name,prefab,group );
                group.Elements.Add(element);
            }

            return element;
        }

        public void GetFormPrefabAsync(Action<UiFormCollection> callback)
        {
            if (FormPrefabs is null)
            {
                throw new UgfNotRegisteredException(
                    nameof(UiFormService),
                    nameof(FormPrefabs));
            }

            FormPrefabs.GetValueAsync(callback);
        }
    }
}