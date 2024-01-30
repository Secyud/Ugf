﻿using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents
{
    /// <summary>
    /// 数据分页方式，实现此类以自定义分页方式。
    /// 可结合<see cref="TableContent"/>
    /// 使用。
    /// </summary>
    public abstract class TablePager:MonoBehaviour
    {
        public Table Table { get; private set; }
        public List<object> PagedData { get; } = new();
        protected virtual void Awake()
        {
            Table = GetComponent<Table>();
        }
        public abstract void Apply();
    }
}