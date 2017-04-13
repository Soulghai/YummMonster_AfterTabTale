using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.Plugins.PSDK {
	/// <summary>
	/// Psdk Splash Interface
	/// </summary>
	/// 
	/// 

	public class AnalyticsTargets{
		public const long ANALYTICS_TARGET_FLURRY = 1;
		public const long ANALYTICS_TARGET_TT_ANALYTICS = 1 << 1;
		public const long ANALYTICS_TARGET_DELTA_DNA = 1 << 2;
		public const long ANALYTICS_PSDK_INTERNAL_TARGETS = ANALYTICS_TARGET_FLURRY | ANALYTICS_TARGET_TT_ANALYTICS;
	}

	public interface IPsdkAnalytics  : IPsdkService {

		void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed); 
		
		void EndLogEvent(string eventName, IDictionary<string,object> eventParams);
	
		//backwards compatibility
		void LogEvent(string eventName, IDictionary<string,string> eventParams, bool timed); 
	
		void EndLogEvent(string eventName, IDictionary<string,string> eventParams);
	
		void LogComplexEvent(string eventName, IDictionary<string, object> eventParams);

		void ReportPurchase(string price, string currency, string productId);

		/// <summary>
		/// Gets the iPhone/Android implementation.
		/// </summary>
		/// <returns>The implementation.</returns>
		IPsdkAnalytics GetImplementation();
	}
}
