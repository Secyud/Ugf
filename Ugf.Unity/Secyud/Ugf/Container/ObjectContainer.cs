﻿using System;

namespace Secyud.Ugf.Container
{
	public abstract class ObjectContainer<TObject> : IObjectAccessor<TObject>
	{
		protected TObject CurrentInstance;

		protected ObjectContainer()
		{
		}

		protected abstract TObject GetObject();

		public virtual TObject Value => CurrentInstance ??= GetObject();

		public virtual void Release()
		{
			CurrentInstance = default;
		}

	}
}