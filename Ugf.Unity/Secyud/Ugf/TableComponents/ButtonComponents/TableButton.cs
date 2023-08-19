using System;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.ButtonComponents
{
    public class TableButton : TableComponentBase<TableButton, TableButtonDelegate>
    {
        public override string Name => nameof(TableButton);

        [SerializeField] private TableButtonGroup ButtonGroupTemplate;

        public TableButtonGroup Template => ButtonGroupTemplate;

    }
}