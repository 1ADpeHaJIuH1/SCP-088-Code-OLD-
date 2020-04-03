using Smod2;
using Smod2.API;
using Smod2.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCP008PLUGIN
{
    public partial class SCP008
    {
        
        DateTime _next088Check = new DateTime();
		bool _is088required;
		bool Get049Required()
		{
			if (_next088Check < DateTime.Now)
			{
                
                    _is088required = this.GetConfigBool(SCP008.announceRequire088ConfigKey088);
				_next088Check = DateTime.Now.AddSeconds(15);
			}
			return _is088required;
		}
		/// <summary>
		/// Checks if all sources of SCP-008 has been exterminated
		/// </summary>
		internal bool SCP008Dead()
		{
			bool scp049alive = (Get049Required()) ?
				plugin.Server.GetPlayers(Role.SCP_939_89).Count > 0 : false;
			bool scp008alive = SCP008.playersToDamage.Count < 1 &&
				scp049alive || this.Server.GetPlayers(Role.SCP_939_89).Count > 0;
			return !scp008alive;
		}

		private bool CanAnnounce = false;
		private DateTime _lastCanAnnounceConfigCheck = new DateTime();
		private bool _lastConfigCanAnnounce;
		internal bool GetCanAnnounce()
		{
			if (_lastCanAnnounceConfigCheck == null || _lastCanAnnounceConfigCheck < DateTime.Now)
			{
				_lastConfigCanAnnounce = this.GetConfigBool(announementsenabled088);
				_lastCanAnnounceConfigCheck = DateTime.Now.AddSeconds(15);
			}
			return _lastConfigCanAnnounce && CanAnnounce;
		}
		internal void SetCanAnnounce(bool can_announce)
		{
			plugin.Debug("CanAnnounce set to: " + can_announce);
			if (!this.GetConfigBool(announementsenabled088))
				plugin.Debug("Announcement(s) disabled in config!");
			CanAnnounce = can_announce;
		}

		#region General_Plugin_Configs
		private static bool _changedEnabledState = false;

		private static DateTime nextCheck = new DateTime();
		private static bool IsEnabled = true;

		public bool GetIsEnabled()
		{
			if (nextCheck < DateTime.Now && !_changedEnabledState)
			{
				nextCheck = DateTime.Now.AddSeconds(30);
				IsEnabled = this.GetConfigBool(enableConfigKey088);
			}
			return IsEnabled;
		}
		/// <summary>
		/// Sets the current state of <see cref="SCP008"/>
		/// <para>Also disables further config checks for this</para>
		/// </summary>
		public void SetIsEnabled(bool value)
		{
			IsEnabled = value;
			_changedEnabledState = true;
		}
		#endregion
	}
}
