# ComponentTitlebarGUI
Unity3d script that add callback to draw MGUI (without CustomEditor) in Inspector upon:
- on any Component Titlebar
- on GameObject Icon
- on Material title preview



![image_2024-10-24_04-19-10](https://github.com/user-attachments/assets/76c0ca0e-75ea-40b6-bb56-6c5729a23b0b)
![image](https://github.com/user-attachments/assets/301a6d27-8bc5-4b67-8790-37d1046eeeac)

# Usage Example
```
[InitializeOnLoadMethod]
public static void Init()
{
  ComponentTitlebarGUI.OnTitlebarGUI += TestGUI;
}

private static void TestGUI(Rect rect, Object target)
{
  if (target is not MonoBehaviour) return;  
  GUI.Label(rect, EditorGUIUtility.IconContent("console.erroricon.sml"));
}
```
