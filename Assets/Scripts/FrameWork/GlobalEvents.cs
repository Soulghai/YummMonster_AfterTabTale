using System;
using System.Collections.Generic;

public abstract class GlobalEvents{}

public class GlobalEvents<T> : GlobalEvents
{
	private Action<T> m_eventBackingDelegate;

	private static GlobalEvents<T> s_eventSystemInstance;

	private static GlobalEvents<T> EventSystemInstance
	{
		get { return s_eventSystemInstance ?? (s_eventSystemInstance = new GlobalEvents<T>()); }
	}

	private GlobalEvents()
	{
	}

	public static event Action<T> Happened
	{
		add
		{
			if (EventSystemInstance.m_eventBackingDelegate == null)
			{
				GlobalEventzMaitenance.RegisterNewEventSystem(EventSystemInstance);
			}

			EventSystemInstance.m_eventBackingDelegate += value;
		}
		remove
		{
			EventSystemInstance.m_eventBackingDelegate -= value;
		}
	}

	public static void Call(T eventData)
	{
		EventSystemInstance.SafeCall(eventData);
	}

	private void SafeCall(T eventData)
	{
		if (m_eventBackingDelegate != null)
		{
			m_eventBackingDelegate(eventData);
		}
	}
}

public static class GlobalEventzMaitenance
{
	private static readonly List<GlobalEvents> globalEvents = new List<GlobalEvents>();

	public static void RegisterNewEventSystem(GlobalEvents eventSystem)
	{
		if (!globalEvents.Contains(eventSystem))
		{
			globalEvents.Add(eventSystem);
		}
	}
}