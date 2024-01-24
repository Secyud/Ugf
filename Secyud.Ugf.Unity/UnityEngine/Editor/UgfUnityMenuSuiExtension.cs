#if UNITY_EDITOR

using UnityEditor;

namespace UnityEngine.Editor
{
	public static class UgfUnityMenuSuiExtension
	{
		private const string RootPath = "Assets/Ugf/Ugf.Unity";


		[MenuItem("GameObject/Secyud/SText")]
		public static void CreateSText()
		{
			Create("Basic", "SText");
		}

		[MenuItem("GameObject/Secyud/SImage")]
		public static void CreateSImage()
		{
			Create("Basic", "SImage");
		}

		[MenuItem("GameObject/Secyud/SButton")]
		public static void CreateSButton()
		{
			Create("Basic", "SButton");
		}

		[MenuItem("GameObject/Secyud/SDropdown")]
		public static void CreateSDropdown()
		{
			Create("Basic", "SDropdown");
		}

		[MenuItem("GameObject/Secyud/SInputField")]
		public static void CreateSInputField()
		{
			Create("Basic", "SInputField");
		}

		[MenuItem("GameObject/Secyud/SSlider")]
		public static void CreateSSlider()
		{
			Create("Basic", "SSlider");
		}

		[MenuItem("GameObject/Secyud/SToggle")]
		public static void CreateSToggle()
		{
			Create("Basic", "SToggle");
		}

		[MenuItem("GameObject/Secyud/SFaceSlider")]
		public static void CreateSFaceSlider()
		{
			Create("Basic", "SFaceSlider");
		}

		private static void Create(string path, string name)
		{
			path = $"{RootPath}/UnityEngine/Prefabs/{path}/{name}.prefab";

			GameObject obj = AssetDatabase
				.LoadAssetAtPath<GameObject>(path);
			obj = Object.Instantiate(obj);

			obj.name = name;

			RectTransform transform = obj.GetComponent<RectTransform>();

			Transform sel = Selection.activeTransform;
			if (sel is not null)
				transform.SetParent(sel);
		}
	}
}
#endif