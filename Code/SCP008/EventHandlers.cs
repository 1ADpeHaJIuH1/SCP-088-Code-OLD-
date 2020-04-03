using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SCP008PLUGIN
{       
    
	class EventHandlers : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers,
		IEventHandlerPlayerHurt, IEventHandlerPlayerDie, IEventHandlerMedkitUse, IEventHandlerUpdate, IEventHandlerCheckEscape
	{
        List<int> list;
        Player player;
        
        bool enabled_1 = false;

        bool using_ = true;
        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            
            if (using_ == true)
            { 
                    if (ev.Player.TeamRole.Role == Role.SCP_939_89)
                    {
                    int s = new Random().Next(0, 100);
                    if (s <= 50)
                    {
                        player = ev.Player;
                        ev.Player.SetRank("lime", "SCP-088");
                        ev.Player.PersonalBroadcast(30, plugin.GetConfigString(SCP008.broudcastmessageConfigKey088), true);
                        ev.Player.SendConsoleMessage(plugin.GetConfigString(SCP008.consolemessageConfigKey088), "green");
                        using_ = false;
                        enabled_1 = true;
                    }
                   
                    
                }
            }
           
        }

        private SCP008 plugin;
		private Server Server => PluginManager.Manager.Server;

		bool IsEnabled => plugin.GetIsEnabled();

		int damageAmount = 2, 
			damageInterval = 1;

		List<int> rolesCanBecomeInfected = new List<int>();

		public EventHandlers(SCP008 plugin)
		{
			this.plugin = plugin;
		}

		#region PlayerSpecific

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (ev.Attacker.PlayerId == ev.Player.PlayerId || !IsEnabled) return;

			int damageAmount = (ev.Attacker.TeamRole.Role == Role.SCP_939_89) ? plugin.GetConfigInt(SCP008.swingDamageConfigKey088) : 0;
			int infectChance = (ev.Attacker.TeamRole.Role == Role.SCP_939_89) ? plugin.GetConfigInt(SCP008.infectChanceConfigKey088) : 0;
			int infectOnKillChance = (ev.Attacker.TeamRole.Role == Role.SCP_939_89 && ev.Damage >= ev.Player.GetHealth()) ? plugin.GetConfigInt(SCP008.infectKillChanceConfigKey088) : 0;

			if (ev.Player.TeamRole.Team == Smod2.API.Team.TUTORIAL && ev.Attacker.TeamRole.Role == Role.SCP_939_53
				&& !plugin.GetConfigBool(SCP008.canHitTutConfigKey088) || enabled_1)
			{ ev.Damage = 0f; return; }

			//Sets damage to config amount if above 0
			if (ev.Attacker.TeamRole.Role == Role.SCP_939_89 && damageAmount > 0 || enabled_1)
				ev.Damage = damageAmount;
			
			//When a zombie damages a player, adds them to list of infected players to damage
			if (IsEnabled && ev.Attacker.TeamRole.Role == Role.SCP_939_89
                && !SCP008.playersToDamage.Contains(ev.Player.SteamId )
				&& infectChance > 0
				&& new Random().Next(1, 100) <= infectChance || enabled_1)
			{
				if( (rolesCanBecomeInfected == null || rolesCanBecomeInfected.Count == 0 || rolesCanBecomeInfected.FirstOrDefault() == -1) 
					|| 
					(rolesCanBecomeInfected.Count > 0 && rolesCanBecomeInfected.Contains((int)ev.Player.TeamRole.Role)))
					SCP008.playersToDamage.Add(ev.Player.SteamId);
			}

			if (ev.Attacker.TeamRole.Role == Role.SCP_939_89
                && ev.Damage >= ev.Player.GetHealth()
				&& (infectOnKillChance > 99 || new Random().Next(1, 100) <= infectOnKillChance) || enabled_1)
			{
				ev.Damage = 0f;
				ChangeToSCP088(ev.Player);
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			//If player dies, removes them from infected list
			if (SCP008.playersToDamage.Contains(ev.Player.SteamId))
				SCP008.playersToDamage.Remove(ev.Player.SteamId);
            if(ev.Player.TeamRole.Role == Role.SCP_939_89)
            {
                ev.Player.SetRank("white", " ");

                if(ev.Player.GetRankName() == "SCP-088")
                {
                    for(int i = 0; i<list.Count; i++)
                    {   
                        Player player = Server.GetPlayer(list[i]);
                        player.Kill();               
                    }
                    list.Clear();
                }
            }

            
		}

		public void OnMedkitUse(PlayerMedkitUseEvent ev)
		{
			if (!IsEnabled) return;
			int cureChance = plugin.GetConfigInt(SCP008.cureChanceConfigKey088);
			//If its enabled in config and infected list contains player and cure chance is more than, cure.
			if (SCP008.playersToDamage.Contains(ev.Player.SteamId) 
				&& cureChance > 0 
				&& cureChance >= new Random().Next(1,100))
				SCP008.playersToDamage.Remove(ev.Player.SteamId);
		}

		#endregion

		#region RoundHandlers

		public void OnRoundEnd(RoundEndEvent ev)
		{
			//Empties infected list
			SCP008.playersToDamage.Clear();
			//Duh.
			SCP008.roundCount++;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
            list.Clear();
            if (!IsEnabled) return;
			//Empties infected list
			SCP008.playersToDamage.Clear();

			if (!plugin.SCP008Dead())
				plugin.SetCanAnnounce(true);
			/* Poof's untested code
			string RoomID = plugin.GetConfigString("scp008_spawn_room");
			if (!string.IsNullOrEmpty(RoomID))
				foreach(Player p in ev.Server.GetPlayers().Where(p => p.TeamRole.Role == Role.SCP_049_2))
					plugin.pluginManager.CommandManager.CallCommand(server, "tproom", new string[] { p.PlayerId.ToString(), RoomID });
			*/
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			//Reload theese configs on each round restart
			this.damageAmount = plugin.GetConfigInt(SCP008.damageAmountConfigKey088);
			this.damageInterval = plugin.GetConfigInt(SCP008.damageIntervalConfigKey088);
			this.rolesCanBecomeInfected = plugin.GetConfigIntList(SCP008.rolesCanBeInfectedConfigKey088).ToList();
		}

		#endregion


		DateTime updateTimer = DateTime.Now;
		DateTime announementTimer = DateTime.Now;

		public void OnUpdate(UpdateEvent ev)
		{
			if (!IsEnabled) return;
			if(announementTimer < DateTime.Now)
			{
				announementTimer = plugin.GetCanAnnounce() ? DateTime.Now.AddSeconds(5) : DateTime.Now.AddSeconds(30);
				if(plugin.GetCanAnnounce() && plugin.SCP008Dead())
				{
					plugin.Debug("Before announce: \n" + "  - Can Announce: " + plugin.GetCanAnnounce() + "\n" + "  - 008 Exterminated: " + plugin.SCP008Dead());
					if(plugin.GetConfigBool(SCP008.announementsenabled088))
						plugin.Server.Map.AnnounceScpKill("008");
					plugin.SetCanAnnounce(false);
					plugin.Debug("After announce: \n" + "  - Can Announce: " + plugin.GetCanAnnounce() + "\n" + "  - 008 Exterminated: " + plugin.SCP008Dead());
				}
			}

			if (updateTimer < DateTime.Now)
			{
				//Sets when the next time this code will run
				updateTimer = DateTime.Now.AddSeconds(damageInterval);

				//If the server isnt empty, run code on all players
				if (Server.GetPlayers().Count > 0)
					foreach(Player p in Server.GetPlayers())
					{
						//If the victim is human and the player is in the infected list
						if ((p.TeamRole.Team != Smod2.API.Team.SCP && p.TeamRole.Team != Smod2.API.Team.SPECTATOR) && SCP008.playersToDamage.Contains(p.SteamId))
						{
							//If the damage doesnt kill, deal the damage
							if (damageAmount < p.GetHealth())
								p.Damage(damageAmount, DamageType.SCP_939);
							else if (damageAmount >= p.GetHealth())
							{
								//If the damage kills the human, transform
								ChangeToSCP088(p);
							}
						}
					}
			}
		}

		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (SCP008.playersToDamage.Contains(ev.Player.SteamId))
				SCP008.playersToDamage.Remove(ev.Player.SteamId);
		}

		internal void ChangeToSCP088(Player player)
		{
            if (enabled_1)
            {
                if (SCP008.playersToDamage.Contains(player.SteamId))
                    SCP008.playersToDamage.Remove(player.SteamId);
                Vector pos = player.GetPosition();
                player.ChangeRole(Role.SCP_939_89, spawnTeleport: false);
                player.Teleport(pos);
                player.SetHealth(1000);
                player.SetRank("lime", "SCP-088-2");
                player.PersonalBroadcast(30, plugin.GetConfigString(SCP008.broudcastmessageConfigKey088_2), false);
                player.SendConsoleMessage(plugin.GetConfigString(SCP008.consolemessageConfigKey088_2), "green");
                plugin.SetCanAnnounce(true);
                plugin.Debug("Changed " + player.Name + " to SCP-939-89");
                list.Add(player.PlayerId);
            }
		}
        

	}
}