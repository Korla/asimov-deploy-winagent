﻿namespace AsimovDeploy.WinAgent.Web.Contracts
{
	public class HeartbeatDTO
	{
		public string name;
		public string group;
		public string url;
		public string apiKey;
		public string version;
		public int configVersion;
		public LoadBalancerStateDTO loadBalancerState;
	    public string[] unitGroups;
	    public string[] unitTypes { get; set; }
	    public string[] tags { get; set; }
	}
}