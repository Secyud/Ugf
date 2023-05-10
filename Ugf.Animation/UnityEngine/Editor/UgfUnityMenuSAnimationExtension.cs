#if UNITY_EDITOR

#region

using Secyud.Ugf;
using Secyud.Ugf.Animation;
using System.IO;
using UnityEditor;

#endregion

namespace UnityEngine.Editor
{
	public class UgfUnityMenuSAnimationExtension : UnityEditor.Editor
	{
		[MenuItem("Secyud/ShowType")]
		private static void ShowType()
		{
			if (Selection.activeObject)
				Debug.Log(Selection.activeObject.GetType());
		}


		[MenuItem("Secyud/ConvertAnimation")]
		private static void ConvertAnimation()
		{
			var animator = Selection.activeGameObject.GetComponent<Animator>();

			if (!animator)
			{
				Debug.LogWarning("Can't find animator!");

				return;
			}

			foreach (var o in Selection.objects)
			{
				if (o is not AnimationClip clip) return;

				var path = Path.Combine(
					Og.AppPath, AssetDatabase.GetAssetPath(o.GetInstanceID())[7..] + "ation"
				);

				SaveAnimSequence(clip, animator, path);

				Debug.Log($"Convert {clip.name} finished!");
			}

			Debug.Log("All clips convert finished!");
		}


		public static void SaveAnimSequence(
			AnimationClip clip,
			Animator animator,
			string filePath)
		{
			AnimDataSequence sequence = new(clip, animator);

			var directoryName = Path.GetDirectoryName(filePath);

			if (!Directory.Exists(directoryName))
				Directory.CreateDirectory(directoryName);

			using BinaryWriter writer = new(File.Open(filePath, FileMode.Create));

			sequence.Save(writer);
		}
	}
}
#endif