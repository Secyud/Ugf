﻿using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 数据源 本地数据源样板见<see cref="LocalComponents.LocalTableSource"/>
    /// </summary>
    public abstract class TableSource : MonoBehaviour
    {
        public Table Table { get; internal set; }
        public abstract void Apply();
    }
}