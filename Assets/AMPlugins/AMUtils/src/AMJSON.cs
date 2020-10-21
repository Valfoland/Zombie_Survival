/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/how-do-i-write-my-own-parser- (for-json).html
 *
 * Simplified it so that it doesn't throw exceptions
 * and can be used in Unity iPhone with maximum code stripping.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish, 
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using AMLogging;

namespace AMUtils
{
	/// <summary>
	/// This class encodes and decodes JSON strings.
	/// Spec. details, see http://www.json.org/
	///
	/// JSON uses Arrays and Objects. These correspond here to the datatypes ArrayList and Hashtable.
	/// All numbers are parsed to doubles.
	/// </summary>
	public class AMJSON
	{
		public const int TOKEN_NONE = 0;
		public const int TOKEN_CURLY_OPEN = 1;
		public const int TOKEN_CURLY_CLOSE = 2;
		public const int TOKEN_SQUARED_OPEN = 3;
		public const int TOKEN_SQUARED_CLOSE = 4;
		public const int TOKEN_COLON = 5;
		public const int TOKEN_COMMA = 6;
		public const int TOKEN_STRING = 7;
		public const int TOKEN_NUMBER = 8;
		public const int TOKEN_TRUE = 9;
		public const int TOKEN_FALSE = 10;
		public const int TOKEN_NULL = 11;

		private const int BUILDER_CAPACITY = 2000;

		/// <summary>
		/// Parses the string json into a value
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
		public static object JsonDecode (string json)
		{
			bool success = true;

			return JsonDecode (json, ref success);
		}

		/// <summary>
		/// Parses the string json into a value; and fills 'success' with the successfullness of the parse.
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <param name="success">Successful parse?</param>
		/// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
		public static object JsonDecode (string json, ref bool success)
		{
			success = true;
			if (json != null) {
				char[] charArray = json.ToCharArray ();
				int index = 0;
				object value = ParseValue (charArray, ref index, ref success);
				return value;
			} else {
				return null;
			}
		}

		/// <summary>
		/// Converts a Hashtable / ArrayList object into a JSON string
		/// </summary>
		/// <param name="json">A Hashtable / ArrayList</param>
		/// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
		public static string JsonEncode (object json)
		{
			StringBuilder builder = new StringBuilder (BUILDER_CAPACITY);
			bool success = SerializeValue (json, builder);
			return (success ? builder.ToString () : null);
		}

		protected static Hashtable ParseObject (char[] json, ref int index, ref bool success)
		{
			Hashtable table = new Hashtable ();
			int token;

			// {
			NextToken (json, ref index);

			bool done = false;
			while (!done) {
				token = LookAhead (json, index);
				if (token == AMJSON.TOKEN_NONE) {
					success = false;
					return null;
				} else if (token == AMJSON.TOKEN_COMMA) {
					NextToken (json, ref index);
				} else if (token == AMJSON.TOKEN_CURLY_CLOSE) {
					NextToken (json, ref index);
					return table;
				} else {

					// name
					string name = ParseString (json, ref index, ref success);
					if (!success) {
						success = false;
						return null;
					}

					// :
					token = NextToken (json, ref index);
					if (token != AMJSON.TOKEN_COLON) {
						success = false;
						return null;
					}

					// value
					object value = ParseValue (json, ref index, ref success);
					if (!success) {
						success = false;
						return null;
					}

					table[name] = value;
				}
			}

			return table;
		}

		protected static ArrayList ParseArray (char[] json, ref int index, ref bool success)
		{
			ArrayList array = new ArrayList ();

			// [
			NextToken (json, ref index);

			bool done = false;
			while (!done) {
				int token = LookAhead (json, index);
				if (token == AMJSON.TOKEN_NONE) {
					success = false;
					return null;
				} else if (token == AMJSON.TOKEN_COMMA) {
					NextToken (json, ref index);
				} else if (token == AMJSON.TOKEN_SQUARED_CLOSE) {
					NextToken (json, ref index);
					break;
				} else {
					object value = ParseValue (json, ref index, ref success);
					if (!success) {
						return null;
					}

					array.Add (value);
				}
			}

			return array;
		}

		protected static object ParseValue (char[] json, ref int index, ref bool success)
		{
			switch (LookAhead (json, index)) {
			case AMJSON.TOKEN_STRING:
				return ParseString (json, ref index, ref success);
			case AMJSON.TOKEN_NUMBER:
				return ParseNumber (json, ref index, ref success);
			case AMJSON.TOKEN_CURLY_OPEN:
				return ParseObject (json, ref index, ref success);
			case AMJSON.TOKEN_SQUARED_OPEN:
				return ParseArray (json, ref index, ref success);
			case AMJSON.TOKEN_TRUE:
				NextToken (json, ref index);
				return true;
			case AMJSON.TOKEN_FALSE:
				NextToken (json, ref index);
				return false;
			case AMJSON.TOKEN_NULL:
				NextToken (json, ref index);
				return null;
			case AMJSON.TOKEN_NONE:
				break;
			}

			success = false;
			return null;
		}

		protected static string ParseString (char[] json, ref int index, ref bool success)
		{
			StringBuilder s = new StringBuilder (BUILDER_CAPACITY);
			char c;

			EatWhitespace (json, ref index);

			// "
			c = json[index++];

			bool complete = false;
			while (!complete) {

				if (index == json.Length) {
					break;
				}

				c = json[index++];
				if (c == '"') {
					complete = true;
					break;
				} else if (c == '\\') {

					if (index == json.Length) {
						break;
					}
					c = json[index++];
					if (c == '"') {
						s.Append ('"');
					} else if (c == '\\') {
						s.Append ('\\');
					} else if (c == '/') {
						s.Append ('/');
					} else if (c == 'b') {
						s.Append ('\b');
					} else if (c == 'f') {
						s.Append ('\f');
					} else if (c == 'n') {
						s.Append ('\n');
					} else if (c == 'r') {
						s.Append ('\r');
					} else if (c == 't') {
						s.Append ('\t');
					} else if (c == 'u') {
						int remainingLength = json.Length - index;
						if (remainingLength >= 4) {
							// parse the 32 bit hex into an integer codepoint
							uint codePoint;
							if (! (success = UInt32.TryParse (new string (json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint))) {
								return "";
							}
							// convert the integer codepoint to a unicode char and add to string
							s.Append (Char.ConvertFromUtf32 ((int)codePoint));
							// skip 4 chars
							index += 4;
						} else {
							break;
						}
					}

				} else {
					s.Append (c);
				}

			}

			if (!complete) {
				success = false;
				return null;
			}

			return s.ToString ();
		}

		protected static double ParseNumber (char[] json, ref int index, ref bool success)
		{
			EatWhitespace (json, ref index);

			int lastIndex = GetLastIndexOfNumber (json, index);
			int charLength = (lastIndex - index) + 1;

			double number;
			success = Double.TryParse (new string (json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);

			index = lastIndex + 1;
			return number;
		}

		protected static int GetLastIndexOfNumber (char[] json, int index)
		{
			int lastIndex;

			for (lastIndex = index; lastIndex < json.Length; lastIndex++) {
				if ("0123456789+-.eE".IndexOf (json[lastIndex]) == -1) {
					break;
				}
			}
			return lastIndex - 1;
		}

		protected static void EatWhitespace (char[] json, ref int index)
		{
			for (; index < json.Length; index++) {
				if (" \t\n\r".IndexOf (json[index]) == -1) {
					break;
				}
			}
		}

		protected static int LookAhead (char[] json, int index)
		{
			int saveIndex = index;
			return NextToken (json, ref saveIndex);
		}

		protected static int NextToken (char[] json, ref int index)
		{
			EatWhitespace (json, ref index);

			if (index == json.Length) {
				return AMJSON.TOKEN_NONE;
			}

			char c = json[index];
			index++;
			switch (c) {
			case '{':
				return AMJSON.TOKEN_CURLY_OPEN;
			case '}':
				return AMJSON.TOKEN_CURLY_CLOSE;
			case '[':
				return AMJSON.TOKEN_SQUARED_OPEN;
			case ']':
				return AMJSON.TOKEN_SQUARED_CLOSE;
			case ',':
				return AMJSON.TOKEN_COMMA;
			case '"':
				return AMJSON.TOKEN_STRING;
			case '0': case '1': case '2': case '3': case '4':
			case '5': case '6': case '7': case '8': case '9':
			case '-':
				return AMJSON.TOKEN_NUMBER;
			case ':':
				return AMJSON.TOKEN_COLON;
			}
			index--;

			int remainingLength = json.Length - index;

			// false
			if (remainingLength >= 5) {
				if (json[index] == 'f' &&
					json[index + 1] == 'a' &&
					json[index + 2] == 'l' &&
					json[index + 3] == 's' &&
					json[index + 4] == 'e') {
					index += 5;
					return AMJSON.TOKEN_FALSE;
				}
			}

			// true
			if (remainingLength >= 4) {
				if (json[index] == 't' &&
					json[index + 1] == 'r' &&
					json[index + 2] == 'u' &&
					json[index + 3] == 'e') {
					index += 4;
					return AMJSON.TOKEN_TRUE;
				}
			}

			// null
			if (remainingLength >= 4) {
				if (json[index] == 'n' &&
					json[index + 1] == 'u' &&
					json[index + 2] == 'l' &&
					json[index + 3] == 'l') {
					index += 4;
					return AMJSON.TOKEN_NULL;
				}
			}

			return AMJSON.TOKEN_NONE;
		}

		protected static bool SerializeValue (object value, StringBuilder builder)
		{
			bool success = true;

			if (value is string) {
				success = SerializeString ((string)value, builder);
			} else if (value is Hashtable) {
				success = SerializeObject ((Hashtable)value, builder);
			} else if (value is ArrayList) {
				success = SerializeArray ((ArrayList)value, builder);
			} else if ((value is Boolean) && ((Boolean)value == true)) {
				builder.Append ("true");
			} else if ((value is Boolean) && ((Boolean)value == false)) {
				builder.Append ("false");
			} else if (value is ValueType) {
				// thanks to ritchie for pointing out ValueType to me
				success = SerializeNumber (Convert.ToDouble (value), builder);
			} else if (value == null) {
				builder.Append ("null");
			} else {
				success = false;
			}
			return success;
		}

		protected static bool SerializeObject (Hashtable anObject, StringBuilder builder)
		{
			builder.Append ("{");

			IDictionaryEnumerator e = anObject.GetEnumerator ();
			bool first = true;
			while (e.MoveNext ()) {
				string key = e.Key.ToString ();
				object value = e.Value;

				if (!first) {
					builder.Append (", ");
				}

				SerializeString (key, builder);
				builder.Append (":");
				if (!SerializeValue (value, builder)) {
					return false;
				}

				first = false;
			}

			builder.Append ("}");
			return true;
		}

		protected static bool SerializeArray (ArrayList anArray, StringBuilder builder)
		{
			builder.Append ("[");

			bool first = true;
			for (int i = 0; i < anArray.Count; i++) {
				object value = anArray[i];

				if (!first) {
					builder.Append (", ");
				}

				if (!SerializeValue (value, builder)) {
					return false;
				}

				first = false;
			}

			builder.Append ("]");
			return true;
		}

		protected static bool SerializeString (string aString, StringBuilder builder)
		{
			builder.Append ("\"");

			char[] charArray = aString.ToCharArray ();
			for (int i = 0; i < charArray.Length; i++) {
				char c = charArray[i];
				if (c == '"') {
					builder.Append ("\\\"");
				} else if (c == '\\') {
					builder.Append ("\\\\");
				} else if (c == '\b') {
					builder.Append ("\\b");
				} else if (c == '\f') {
					builder.Append ("\\f");
				} else if (c == '\n') {
					builder.Append ("\\n");
				} else if (c == '\r') {
					builder.Append ("\\r");
				} else if (c == '\t') {
					builder.Append ("\\t");
				} else {
					builder.Append (c);
				}
			}

			builder.Append ("\"");
			return true;
		}

		protected static bool SerializeNumber (double number, StringBuilder builder)
		{
			builder.Append (Convert.ToString (number, CultureInfo.InvariantCulture));
			return true;
		}

		private const string INDENT_STRING = "    ";
		public static string FormatJson (string jsonString)
		{
			int indent = 0;
			bool isQuoted = false;
			StringBuilder stringBuilder = new StringBuilder ();
			jsonString = jsonString.Replace (",", ", ");
			jsonString = jsonString.Replace (",  ", ", ");
			for (var i = 0; i < jsonString.Length; i++)
			{
				var currentChar = jsonString[i];
				switch (currentChar)
				{
				case '{':
				case '[':
					stringBuilder.Append (currentChar);
					if (!isQuoted && (jsonString [i + 1] != ']' && jsonString [i + 1] != '}'))
					{
						stringBuilder.AppendLine ();
						Enumerable.Range (0, ++indent).ForEach (item => stringBuilder.Append (INDENT_STRING));
					}
					break;
				case '}':
				case ']':
					if (!isQuoted && (jsonString [i - 1] != '[' && jsonString [i - 1] != '{'))
					{
						stringBuilder.AppendLine ();
						Enumerable.Range (0, --indent).ForEach (item => stringBuilder.Append (INDENT_STRING));
					}
					stringBuilder.Append (currentChar);
					break;
				case '"':
					stringBuilder.Append (currentChar);
					bool escaped = false;
					var index = i;
					while (index > 0 && jsonString[--index] == '\\')
						escaped = !escaped;
					if (!escaped)
						isQuoted = !isQuoted;
					break;
				case ',':
					stringBuilder.Append (currentChar);
					if (!isQuoted)
					{
						stringBuilder.AppendLine ();
						Enumerable.Range (0, indent).ForEach (item => stringBuilder.Append (INDENT_STRING));
					}
					break;
				case ':':
					stringBuilder.Append (currentChar);
					if (!isQuoted)
						stringBuilder.Append (" ");
					break;
				default:
					stringBuilder.Append (currentChar);
					break;
				}
			}
			return stringBuilder.ToString ();
		}
	}

	static class Extensions
	{
		public static void ForEach<T> (this IEnumerable<T> ie, System.Action<T> action)
		{
			foreach (var i in ie)
			{
				action (i);
			}
		}
	}
}