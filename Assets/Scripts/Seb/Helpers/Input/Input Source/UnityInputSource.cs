using System.Text;
using UnityEngine;

namespace Seb.Helpers.InputHandling
{
	public class UnityInputSource : IInputSource
	{
		static readonly StringBuilder guiChars = new();
		static string cachedString = string.Empty;
		static int cachedFrame = -1;
		static int skipGuiFromFrame = -1;
		static bool guiShiftHeld;
		static bool guiCapsLockOn;

		public Vector2 MousePosition => Input.mousePosition;

		public bool IsKeyDownThisFrame(KeyCode key) => Input.GetKeyDown(key);
		public bool IsKeyUpThisFrame(KeyCode key) => Input.GetKeyUp(key);
		public bool IsKeyHeld(KeyCode key) => Input.GetKey(key);
		public bool AnyKeyOrMouseDownThisFrame => Input.anyKeyDown;
		public bool AnyKeyOrMouseHeldThisFrame => Input.anyKey;
		public string InputString => GetInputString();
		public Vector2 MouseScrollDelta => Input.mouseScrollDelta;
		public bool IsMouseDownThisFrame(MouseButton button) => Input.GetMouseButtonDown((int)button);

		public static void ProcessGuiEvent(Event e)
		{
			if (e == null) return;

			guiShiftHeld = e.shift;
			guiCapsLockOn = e.capsLock;

			if (e.type != EventType.KeyDown) return;
			if (skipGuiFromFrame == Time.frameCount) return;

			char c = e.character;
			if (c == '\0' || char.IsControl(c)) return;

			bool upper = e.shift ^ e.capsLock;
			if (c >= 'a' && c <= 'z' && upper) c = (char)(c - 32);
			else if (c >= 'A' && c <= 'Z' && !upper) c = (char)(c + 32);

			guiChars.Append(c);
		}

		public static void ResetTextBuffer()
		{
			guiChars.Clear();
			cachedString = string.Empty;
			cachedFrame = -1;
			skipGuiFromFrame = -1;
			guiShiftHeld = false;
			guiCapsLockOn = false;
		}

		static string GetInputString()
		{
			if (cachedFrame == Time.frameCount) return cachedString;
			cachedFrame = Time.frameCount;

			// IMGUI characters already include Shift/Caps Lock.
			if (guiChars.Length > 0)
			{
				cachedString = guiChars.ToString();
				guiChars.Clear();
				return cachedString;
			}

			string unityString = Input.inputString ?? string.Empty;
			bool shiftHeld = guiShiftHeld || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			bool capsOn = guiCapsLockOn;

			if (HasPrintableChar(unityString))
			{
				// If modifiers are active, Input.inputString is often still lowercase on Linux/Unity 6.
				// Wait for OnGUI in that case so we get the real character.
				if (shiftHeld || capsOn)
				{
					cachedString = ApplyLetterCase(unityString, shiftHeld, capsOn);
					skipGuiFromFrame = Time.frameCount;
					guiChars.Clear();
					return cachedString;
				}

				skipGuiFromFrame = Time.frameCount;
				guiChars.Clear();
				cachedString = unityString;
				return cachedString;
			}

			cachedString = unityString;
			return cachedString;
		}

		static string ApplyLetterCase(string s, bool shiftHeld, bool capsOn)
		{
			char[] chars = s.ToCharArray();
			bool upper = shiftHeld ^ capsOn;
			for (int i = 0; i < chars.Length; i++)
			{
				if (chars[i] >= 'a' && chars[i] <= 'z' && upper)
				{
					chars[i] = (char)(chars[i] - 32);
				}
				else if (chars[i] >= 'A' && chars[i] <= 'Z' && !upper)
				{
					chars[i] = (char)(chars[i] + 32);
				}
			}

			return new string(chars);
		}

		static bool HasPrintableChar(string s)
		{
			if (string.IsNullOrEmpty(s)) return false;
			foreach (char c in s)
			{
				if (!char.IsControl(c)) return true;
			}

			return false;
		}
	}
}
