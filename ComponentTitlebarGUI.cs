using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UnityEditor.Custom
{
	public static class ComponentTitlebarGUI
	{
		private static GUIContent content = EditorGUIUtility.IconContent("console.erroricon.sml");
		private static Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
		private static Type type2 = typeof(EditorWindow).Assembly.GetType("UnityEditor.PropertyEditor");
		private static Type type3 = typeof(EditorWindow).Assembly.GetType("UnityEditor.UIElements.EditorElement");
		private static FieldInfo field = type2.GetField("m_EditorsElement", BindingFlags.NonPublic | BindingFlags.Instance);
		private static FieldInfo field2 = type3.GetField("m_Header", BindingFlags.NonPublic | BindingFlags.Instance);
		private static FieldInfo field3 = type3.GetField("m_EditorTarget", BindingFlags.NonPublic | BindingFlags.Instance);

		private static VisualElement m_EditorsElement;
		private static VisualElement editorsElement => m_EditorsElement ??= FindVisualElementInTreeByClassName("unity-inspector-editors-list");

		public static Action<Rect, Object> OnTitlebarGUI;

		private static VisualElement FindVisualElementInTreeByClassName(string elementClassName)
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
		}

		public static void OnUpdate()
		{
			VisualElement value = editorsElement;
			IMGUIContainer value2 = null;
			if (value != null)
			{
				var foundAll = editorsElement.Children().Where(element => element.GetType().GetInterfaces().Any(interfaceType => interfaceType.Name == "IEditorElement"));
				foreach (VisualElement element in foundAll)
				{
					var localTarget = field3.GetValue(element) as Object;
					if (localTarget)
					{
						value2 = field2.GetValue(element) as IMGUIContainer;
						if (value2 != null)
						{
							value2.onGUIHandler -= Action;
							value2.onGUIHandler += Action;
						}

						void Action()
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
		}

		//[InitializeOnLoadMethod]
		public static void InitTest()
		{
			ComponentTitlebarGUI.OnTitlebarGUI -= TestGUI;
			ComponentTitlebarGUI.OnTitlebarGUI += TestGUI;
		}

		private static void TestGUI(Rect rect, Object target)
		{
			if (target is not MonoBehaviour) return;
			rect.x = rect.width - 80;
			
			GUI.Label(rect, content);
			//GUILayout.Label(content);
		}
	}
}
