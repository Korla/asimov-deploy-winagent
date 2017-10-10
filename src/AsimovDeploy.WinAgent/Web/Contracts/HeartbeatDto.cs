﻿namespace AsimovDeploy.WinAgent.Web.Contracts
{
	public class HeartbeatDTO
	{
	    public string name;
	    public string osPlatform;
	    public string[] groups;
	    public string url;
	    public string apiKey;
	    public string version;
	    public int configVersion;
	    public LoadBalancerStateDTO loadBalancerState;
	}
}