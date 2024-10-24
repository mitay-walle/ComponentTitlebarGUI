using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UnityEditor.Custom
{
	public static class ComponentTitlebarGUI
	{
		public static Action<Rect, Object> OnTitlebarGUI;

#if UNITY_EDITOR
		private static Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
		private static Type type2 = typeof(EditorWindow).Assembly.GetType("UnityEditor.PropertyEditor");
		private static Type type3 = typeof(EditorWindow).Assembly.GetType("UnityEditor.UIElements.EditorElement");
		private static FieldInfo field = type2.GetField("m_EditorsElement", BindingFlags.NonPublic | BindingFlags.Instance);
		private static FieldInfo field2 = type3.GetField("m_Header", BindingFlags.NonPublic | BindingFlags.Instance);
		private static FieldInfo field3 = type3.GetField("m_EditorTarget", BindingFlags.NonPublic | BindingFlags.Instance);
		private static VisualElement m_EditorsElement;
		private static VisualElement editorsElement => m_EditorsElement ??= GetEditorVisualElement();
		private static Dictionary<VisualElement, Action> _callbacks = new();

		private static VisualElement GetEditorVisualElement()
		{
			EditorWindow window = EditorWindow.GetWindow(type);
			if (window)
			{
				return field.GetValue(window) as VisualElement;
			}

			return null;
		}

		[InitializeOnLoadMethod]
		public static void Init()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
			Selection.selectionChanged -= OnSelectionChanged;
			Selection.selectionChanged += OnSelectionChanged;
		}

		private static void OnSelectionChanged() => _callbacks.Clear();

		private static void OnUpdate()
		{
			VisualElement inspectorRoot = editorsElement;
			if (inspectorRoot == null) return;

			var foundAll = editorsElement.Children();
			foreach (VisualElement element in foundAll)
			{
				if (element.GetType() != type3) continue;

				var localTarget = field3.GetValue(element) as Object;
				if (localTarget)
				{
					IMGUIContainer value2 = field2.GetValue(element) as IMGUIContainer;
					Action callback = null;
					if (_callbacks.TryGetValue(element, out var found))
					{
						callback = found;
					}
					else
					{
						callback = _callbacks[element] = MyLocalCallback;
					}

					if (value2 != null)
					{
						value2.onGUIHandler -= callback;
						value2.onGUIHandler += callback;
					}

					void MyLocalCallback()
					{
						try
						{
							OnTitlebarGUI?.Invoke(GUILayoutUtility.GetLastRect(), localTarget);
						}
						catch (Exception e)
						{
							Debug.LogException(e);
						}
						finally { }
					}
				}
			}
		}

		//private static GUIContent content = EditorGUIUtility.IconContent("console.erroricon.sml");
		// [InitializeOnLoadMethod]
		// public static void InitTest()
		// {
		// 	ComponentTitlebarGUI.OnTitlebarGUI -= TestGUI;
		// 	ComponentTitlebarGUI.OnTitlebarGUI += TestGUI;
		// }
		//
		// private static void TestGUI(Rect rect, Object target)
		// {
		// 	if (target is not MonoBehaviour) return;
		//
		// 	GUI.Label(rect, content);
		// }
#endif
	}
}
