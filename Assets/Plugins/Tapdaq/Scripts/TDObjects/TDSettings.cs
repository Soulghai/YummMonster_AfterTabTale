using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Tapdaq {
	public class TDSettings : ScriptableObject {

		public const string pluginVersion = "unity_5.7.1";
		
		public string ios_applicationID = "";
		public string ios_clientKey = "";
		public string android_applicationID = "";
		public string android_clientKey = "";

		public bool isDebugMode = false;
		public bool autoReloadAds = false;

		public AdTags tags = new AdTags ();

		[SerializeField]
		public List<TestDevice> testDevices = new List<TestDevice>();
	}

	public enum TestDeviceType {
		Android,
		iOS
	}

	[Serializable]
	public class TestDevice {

		public string name;
		public TestDeviceType type;

		public string adMobId;
		public string facebookId;

		public TestDevice(string deviceName, TestDeviceType deviceType) {
			name = deviceName;
			type = deviceType;
		}
	}

	[Serializable]
	public class TestDevicesList {
	
		[SerializeField]
		public List<string> adMobDevices = new List<string>();
		[SerializeField]
		public List<string> facebookDevices = new List<string>();

		public TestDevicesList(List<TestDevice> devices, TestDeviceType deviceType) {

			foreach (var device in devices) {
				if (device.type == deviceType) {
					if (!string.IsNullOrEmpty (device.adMobId)) {
						adMobDevices.Add (device.adMobId);
					}
					if (!string.IsNullOrEmpty (device.facebookId)) {
						facebookDevices.Add (device.facebookId);
					}
				}
			}
		}

		public override string ToString ()
		{
			return JsonConvert.SerializeObject (this);
		}

		public string GetAdMobListJson() {
			return JsonConvert.SerializeObject (adMobDevices);
		}

		public string GetFacebookListJson() {
			return JsonConvert.SerializeObject (facebookDevices);
		}
	}

	[Serializable]
	public class AdTags {

		public static string DefaultTag = "default";

		[SerializeField]
		private string[] tags = new string[Enum.GetValues (typeof(TDAdType)).Length];

		public string this[int index]
		{
			get
			{
				if (index >= tags.Count()) {
					UpdateTagsLength ();
				}
				return tags [index];
			}
			set
			{
				if (index >= tags.Count()) {
					UpdateTagsLength ();
				}
				tags [index] = value;
			}
		}

		public int Length {
			get { return tags.Length; }
		}

		private void UpdateTagsLength() {
			if (tags.Count() < Enum.GetValues (typeof(TDAdType)).Length) {

				var newTags = new string[Enum.GetValues (typeof(TDAdType)).Length];
				for (int i = 0; i < tags.Count(); i++) {
					newTags [i] = tags [i];
				}
				tags = newTags;
			}
		}

		public Dictionary<TDAdType, string> GetTags () {

			var toReturn = new Dictionary<TDAdType, string> ();

			for (int i = 1; i < tags.Length; i++) {
				if(!string.IsNullOrEmpty(tags[i]) )
					toReturn.Add ((TDAdType)i, tags[i]);
			}

			return toReturn;
		}

		public string GetTagsJson() {
			var lists = GetTags().Select(pair => new TagsList(pair.Key, pair.Value)).ToArray();
			return JsonConvert.SerializeObject(lists);
		}
	}

	[Serializable]
	public class TagsList {

		public string ad_type;
		public List<string> placement_tags;

		public TagsList(TDAdType adType, string tags) {
			ad_type = adType.ToString ();
			placement_tags = tags.Split (new []{ "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}
	}
}