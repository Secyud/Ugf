#if UNITY_EDITOR

#region

using System.Linq;
using UnityEditor;
using UnityEngine.UI;

#endregion

namespace UnityEngine.Editor
{
	public static class UgfUnityLayoutExtension
	{

		[MenuItem("Layout/UseLayout")]
		public static void UseLayout()
		{
			if (Selection.activeGameObject)
			{
				SetLayout(Selection.activeGameObject.transform, true);
			}
		}

		[MenuItem("Layout/CancelLayout")]
		public static void CancelLayout()
		{
			if (Selection.activeGameObject)
			{
				SetLayout(Selection.activeGameObject.transform, false);
			}
		}


		private static void SetLayout(Transform t, bool value)
		{
			if (!t) return;

			if (t.TryGetComponent(out ContentSizeFitter sizeFitter))
				sizeFitter.enabled = value;

			if (t.TryGetComponent(out LayoutGroup layoutGroup))
				layoutGroup.enabled = value;

			for (int i = 0; i < t.transform.childCount; i++)
				SetLayout(t.GetChild(i), value);
		}

		[MenuItem("脚本处理/查找有missing脚本的物体")]
		static void FindMissingScriptObject()
		{
			var objs = Selection.gameObjects;
			Debug.Log($"选中的物体数量为：{objs.Length}");

			var allObjs = objs
				.SelectMany(obj => obj.GetComponentsInChildren<Transform>().Select(x => x.gameObject)).ToList();
			Debug.Log($"选中的物体及其子物体的数量为：{allObjs.Count()}");

			allObjs.ForEach(
				obj =>
				{
					//1、该物体是否有null的脚本
					var hasNullScript =
						obj.GetComponents<MonoBehaviour>()
							.Any(mono => mono == null); //注意:用【MonoBehaviour】而不是用【MonoScript】
					//Debug.Log($"是否有空脚本：{hasNullScript}，物体名字：【{obj.name}】");

					//2、Debug物体名字
					if (hasNullScript)
					{
						Debug.Log($"物体 【{obj.name}】 上有Missing的脚本");
					}
				}
			);
		}
	}
}
#endif