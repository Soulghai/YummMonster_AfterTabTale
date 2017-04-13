using System.Linq;
using System.Text;
using UnityEngine;

public class D
{

	/// <summary>
	/// Author: WeslomPo
	/// InspiredBy: Ant.Karlov
	/// </summary>

	public static void Log(params object[] aArgs) {
#if DEBUG || UNITY_EDITOR
		Plain(aArgs).Log();
#endif
	}

	public static void Warning(params object[] aArgs) {
#if DEBUG || UNITY_EDITOR
		Plain(aArgs).Warning();
#endif
	}

	public static void Error(params object[] aArgs) {
#if DEBUG || UNITY_EDITOR
		Plain(aArgs).Error();
#endif
	}
	public static void LogFormat(params object[] aArgs) {
#if DEBUG || UNITY_EDITOR
		Format(aArgs).Log();
#endif
	}

	public static void WarningFormat(params object[] aArgs) {
#if DEBUG || UNITY_EDITOR
		Format(aArgs).Warning();
#endif
	}

	public static void ErrorFormat(params object[] aArgs) {
#if DEBUG || UNITY_EDITOR
		Format(aArgs).Error();
#endif
	}

	public static LogPrettifier Plain(params object[] arguments) {
		return LogPrettifier.Plain(arguments);
	}

	public static LogPrettifier Format(params object[] arguments) {
		return LogPrettifier.Format(arguments);
	}

	public class LogPrettifier
	{
		static private StringBuilder _builder = null;

		public static LogPrettifier Plain(params object[] arguments) {
			return new LogPrettifier().ToPlain(arguments);
		}

		public static LogPrettifier Format(params object[] arguments) {
			return new LogPrettifier().ToFormat(arguments);
		}

		public void Log() {
#if DEBUG || UNITY_EDITOR
			Debug.logger.Log(LogType.Log, _builder.ToString());
#endif
		}

		public void Warning() {
#if DEBUG || UNITY_EDITOR
			Debug.logger.Log(LogType.Warning, _builder.ToString());
#endif
		}

		public void Error() {
#if DEBUG || UNITY_EDITOR
			Debug.logger.Log(LogType.Error, _builder.ToString());
#endif
		}

		public LogPrettifier Bold {
			get { return Brackets("<b>", "</b>"); }
		}

		public LogPrettifier Italic {
			get { return Brackets("<i>", "</i>"); }
		}

		public LogPrettifier Red {
			get { return Color("red"); }
		}

		public LogPrettifier Yellow {
			get { return Color("yellow"); }
		}

		public LogPrettifier Blue {
			get { return Color("blue"); }
		}

		public LogPrettifier Green {
			get { return Color("green"); }
		}

		public LogPrettifier Size(string size) {
			_builder.Insert(0, ">").Insert(0, size).Insert(0, "<size=").Append("</size>");
			return this;
		}

		public LogPrettifier Color(string color) {
			_builder.Insert(0, ">").Insert(0, color).Insert(0, "<color=").Append("</color>");
			return this;
		}

		public LogPrettifier Brackets(string before, string after) {
			_builder.Insert(0, before).Append(after);
			return this;
		}

		public override string ToString() {
			return _builder.ToString();
		}

		private LogPrettifier ToPlain(params object[] arguments) {
			if (_builder == null) _builder = new StringBuilder(); else
				_builder.Length = 0;
			
			for (int index = 0, length = arguments.Length; index < length; index++)
				if (arguments[index] == null)
					_builder.Append("Null ");
				else
					_builder.Append(arguments[index]).Append(" ");
			return this;
		}

		private LogPrettifier ToFormat(params object[] arguments) {
			if (_builder == null) _builder = new StringBuilder(); else
				_builder.Length = 0;
			
			string format = arguments[0] as string;
			if (format == null || BracketCount(format) != arguments.Length - 1)
				return Plain(arguments);
			object[] result = new object[arguments.Length - 1];
			for (int i = 1, length = arguments.Length; i < length; i++)
				result[i - 1] = arguments[i] == null ? "Null" : arguments[i];
			_builder.Append(string.Format(format, result));
			return this;
		}

		private static int BracketCount(string value) {
			int count = 0;
			bool opened = false;
			for (int i = 0, n = value.Length; i < n; i++) {
				if (value[i] == '{') opened = true;
				else if (value[i] == '}') {
					if (opened) {
						count++;
						opened = false;
					}
				}
			}
			return count;
		}

	}

}
