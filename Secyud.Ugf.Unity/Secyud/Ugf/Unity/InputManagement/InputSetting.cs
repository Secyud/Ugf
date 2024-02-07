using System.Collections.Generic;
using Secyud.Ugf.Logging;
using Secyud.Ugf.Unity.EditorComponents;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Secyud.Ugf.Unity.InputManagement
{
    public class InputSetting : EditorBase<IInputEvent>, IHasId<int>
    {
        [field: SerializeField] public int Id { get; private set; }
        [SerializeField] private TextMeshProUGUI _showEditText;
        [SerializeField] private TextMeshProUGUI _labelText;

        private void Awake()
        {
            InputService service = U.Get<InputService>();
            var inputEvent = service.AllEvents.GetValueOrDefault(Id);
            if (inputEvent is null)
            {
                UgfLogger.LogError($"Cannot get event for id {Id}!");
                gameObject.SetActive(false);
            }
            else
            {
                Bind(inputEvent);
            }

            //ensure disabled while awake.
            enabled = false;
        }

        private void Update()
        {
            FunctionKey functionKey = InputService.GetFunctionKey();
            KeyCode code = 0;
            for (KeyCode i = KeyCode.Alpha0; i < KeyCode.Alpha9; i++)
            {
                if (Input.GetKeyUp(i))
                {
                    code = i;
                    break;
                }
            }

            if (code == 0)
            {
                for (KeyCode i = KeyCode.A; i < KeyCode.Z; i++)
                {
                    if (Input.GetKeyUp(i))
                    {
                        code = i;
                        break;
                    }
                }
            }

            Property.FunctionKey = functionKey;
            Property.KeyCode = code;

            SetText(code, functionKey);


            if (code != 0)
            {
                enabled = false;
            }
        }

        private void SetText(KeyCode code, FunctionKey key)
        {
            List<KeyCode> codes = ListPool<KeyCode>.Get();

            if (key.HasFlag(FunctionKey.LeftShift)) codes.Add(KeyCode.LeftShift);
            if (key.HasFlag(FunctionKey.RightShift)) codes.Add(KeyCode.RightShift);
            if (key.HasFlag(FunctionKey.LeftControl)) codes.Add(KeyCode.LeftControl);
            if (key.HasFlag(FunctionKey.RightControl)) codes.Add(KeyCode.RightControl);
            if (key.HasFlag(FunctionKey.LeftAlt)) codes.Add(KeyCode.LeftAlt);
            if (key.HasFlag(FunctionKey.RightAlt)) codes.Add(KeyCode.RightAlt);
            if (code != 0) codes.Add(code);

            _showEditText.text = string.Join(" + ", codes);
            ListPool<KeyCode>.Release(codes);
        }

        public override void Bind(IInputEvent property)
        {
            base.Bind(property);
            SetText(property.KeyCode, property.FunctionKey);
            _labelText.text = U.T[property.Name];
        }
    }
}