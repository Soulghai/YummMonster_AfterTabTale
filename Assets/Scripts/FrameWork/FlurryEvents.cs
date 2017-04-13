using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using TabTale.Plugins.PSDK;

public class FlurryEvents : MonoBehaviour 
{
	private static IPsdkAnalytics _analytics;
	public static IPsdkAnalytics Analytics
	{
		get
		{
			if(_analytics == null)
				_analytics = PSDKMgr.Instance.GetAnalyticsService();

			return _analytics;
		}
	}

	public static void LogEvent(FlurryEvent @event)
	{
		Analytics.LogEvent(@event.targets, @event.eventName, @event.Parameters, @event.timed);

		D.Log ("LogEvent " + @event.eventName + " : " + PrintDictionary (@event.Parameters));
	}

	public static void EndLogEvent(FlurryEndEvent @event)
	{
		Analytics.EndLogEvent(@event.eventName, @event.Parameters);

		D.Log ("EndLogEvent " + @event.eventName + " : " + PrintDictionary (@event.Parameters));
	}

	static string PrintDictionary (IDictionary<string, object> dictionary)
	{
		StringBuilder sb = new StringBuilder();
		foreach (var d in dictionary)
		{
			sb.Append(d.Key + " = " + d.Value.ToString() + " ");
		}

		return sb.ToString();
	}
}



public class FlurryEvent
{
	public string eventName {get;private set;}
	public long targets {get;private set;}
	public bool timed {get;private set;}

	public Dictionary<string, object> Parameters {get;private set;}

	public FlurryEvent(string eventName, bool timed = false) : this(AnalyticsTargets.ANALYTICS_TARGET_FLURRY, eventName, timed)
	{
	}

	public FlurryEvent(long targets, string eventName, bool timed)
	{
		this.eventName = eventName;
		this.targets = targets;
		this.timed = timed;

		Parameters = new Dictionary<string, object>();
	}

	public void AddParameter(string parameterName, object value)
	{
		if(Parameters.ContainsKey(parameterName))
			Parameters[parameterName] = value;
		else 
			Parameters.Add(parameterName, value);
	}
}



public class FlurryEndEvent
{
	public string eventName {get;private set;}

	public Dictionary<string, object> Parameters {get;private set;}

	public FlurryEndEvent(string eventName)
	{
		this.eventName = eventName;

		Parameters = new Dictionary<string, object>();
	}

	public void AddParameter(string parameterName, object value)
	{
		if(Parameters.ContainsKey(parameterName))
			Parameters[parameterName] = value;
		else 
			Parameters.Add(parameterName, value);
	}
}