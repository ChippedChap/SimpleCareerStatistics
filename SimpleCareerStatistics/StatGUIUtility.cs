using System;
using System.Reflection;
using UnityEngine;

namespace SimpleCareerStatistics
{
	static class StatGUIUtility
	{
		private static GUIContent testHolder = new GUIContent();

		public static readonly string alwaysShowSign = "+#;-#;0";

		public static GUISkin BaseSkin { get; private set; }

		public static GUIStyle DateText { get; private set; }

		public static GUIStyle DateTextField { get; private set; }

		public static GUIStyle PositiveText { get; private set; }

		public static GUIStyle NegativeText { get; private set; }

		public static GUIStyle PositiveTextField { get; private set; }

		public static GUIStyle NegativeTextField { get; private set; }

		public static void CreateStyles(GUISkin baseSkin)
		{
			BaseSkin = baseSkin;

			DateText = new GUIStyle(BaseSkin.label);
			DateText.alignment = TextAnchor.MiddleCenter;
			DateText.fontStyle = FontStyle.Bold;
			DateText.font = BaseSkin.textField.font;
			DateText.normal.textColor = Color.white;
			DateText.hover.textColor = Color.yellow;

			DateTextField = new GUIStyle(DateText);
			DateTextField.normal.background = BaseSkin.textField.normal.background;
			DateTextField.hover.background = BaseSkin.textField.hover.background;

			PositiveText = new GUIStyle(DateText);
			PositiveText.normal.textColor = Color.green;
			PositiveText.hover.textColor = Color.green;

			PositiveTextField = new GUIStyle(PositiveText);
			PositiveTextField.normal.background = BaseSkin.textField.normal.background;
			PositiveTextField.hover.background = BaseSkin.textField.hover.background;

			NegativeText = new GUIStyle(DateText);
			NegativeText.normal.textColor = Color.red;
			NegativeText.hover.textColor = Color.red;

			NegativeTextField = new GUIStyle(NegativeText);
			NegativeTextField.normal.background = BaseSkin.textField.normal.background;
			NegativeTextField.hover.background = BaseSkin.textField.hover.background;
		}

		public static Rect GetNormalizedRect(float x, float y, float w, float h, float anchorx = 0.5f, float anchory = 0.5f)
		{
			return new Rect(Screen.width * x - Screen.width * w * anchorx, 
				Screen.height * y - Screen.height * h * anchory, 
				Screen.width * w, 
				Screen.height * h
				);
		}

		public static void DrawDataColumn(int start, int end, 
			Func<int, string> label, Func<int, GUIStyle> style, Callback<int> onClick = null, 
			Callback<int> preRow = null, Callback<int> postRow = null)
		{
			GUILayout.BeginVertical();
			for (int i = start; i <= end; i++)
			{
				preRow?.Invoke(i);
				if (onClick != null)
				{
					if (GUILayout.Button(label(i), style(i), GUILayout.ExpandWidth(true))) onClick(i);
				} 
				else
				{
					GUILayout.Label(label(i), style(i), GUILayout.ExpandWidth(true));
				}
				postRow?.Invoke(i);
			}
			GUILayout.EndVertical();
		}

		public static GUIStyle PosNegStyle(float num, bool background = false)
		{
			if(background) return num < 0 ? NegativeTextField : PositiveTextField;
			return num < 0 ? NegativeText : PositiveText;
		}

		public static Vector2 GetTextSize(this GUIStyle style, string text)
		{
			testHolder.text = text;
			return style.CalcSize(testHolder);
		}

		public static string ToDate(this double t)
		{
			return KSPUtil.dateTimeFormatter.PrintDateCompact(t, true);
		}

		public static void TestSkins()
		{
			// For testing only
			PropertyInfo[] properties = typeof(GUISkin).GetProperties();
			for (int i = 0; i < properties.Length; i++)
				if (properties[i].PropertyType == typeof(GUIStyle))
					GUILayout.Label(properties[i].Name, (GUIStyle)properties[i].GetValue(HighLogic.Skin));
		}
	}
}
