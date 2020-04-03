using Smod2;
using Smod2.API;
using Smod2.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCP008PLUGIN
{
	/// <summary>
	/// The <see cref="SCP008"/> <see cref="Plugin"/>!
	/// </summary>
	[PluginDetails(
		author = "Evan",
		name = "SCP008",
		description = "Plugin that replicates SCP008 behaviour",
		id = "rnen.scp.008",
		version = assemblyVersion + "-0",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
		)]
	public partial class SCP008 : Plugin
	{
		/// <summary>
		/// The current <see cref="SCP008"/> plugin version
		/// </summary>
		public const string assemblyVersion = "1.5";

		internal static List<string> playersToDamage = new List<string>();
		internal static int roundCount = 0;

		static SCP008 plugin;


        #region ConfigKeys
        internal static readonly string
            enableConfigKey088 = "scp088_enabled",
            damageAmountConfigKey088 = "scp088_damage_amount",
            damageIntervalConfigKey088 = "scp088_damage_interval",
            swingDamageConfigKey088 = "scp088_swing_damage",
            infectChanceConfigKey088 = "scp088_infect_chance",
            infectKillChanceConfigKey088 = "scp088_killinfect_chance",

            cureChanceConfigKey088 = "scp088_cure_chance",
            ranksAllowedConfigKey088 = "scp088_ranklist_commands",
            rolesCanBeInfectedConfigKey088 = "scp088_roles_caninfect",
            canHitTutConfigKey088 = "scp088_canhit_tutorial",
            announementsenabled088 = "scp088_announcement_enabled",
            announceRequire088ConfigKey088 = "scp088_announcement_count088",
            broudcastmessageConfigKey088 = "Выводит текст в broudcast.",
            consolemessageConfigKey088 = "Выводит текст в консоль",
            broudcastmessageConfigKey088_2 = " ",
            consolemessageConfigKey088_2 = " ",
            durationbroudcastmessageConfigKey088 = "Время, после которого будет изчезать текст из broudcast.";


        #endregion

        public override void OnDisable()
		{
			//Info(Details.name + " has been disabled.");
		}

		public override void OnEnable()
		{
			//this.Info(this.Details.name + " loaded successfully!");
			SCP008.plugin = this;
		}

		public override void Register()
		{
			#region EventRegister
			this.AddEventHandlers(new EventHandlers(this),Smod2.Events.Priority.Low);
			#endregion

			#region CommandRegister
			this.AddCommands(new string[] { "scp088", "scp88",  }, new Command.EnableDisableCommand(this));
			this.AddCommands(new string[] { "infect" }, new Command.InfectCommand(this));
			this.AddCommands(new string[] { "088help", "scp088help", "scp88help" }, new Command.HelpCommand(this));
			#endregion

			#region ConfigRegister
			this.AddConfig(new Smod2.Config.ConfigSetting(enableConfigKey088, true, true, "Enable/Disable plugin"));
			this.AddConfig(new Smod2.Config.ConfigSetting(canHitTutConfigKey088, true, true, "If zombies can hit TUTORIAL players or not"));
			this.AddConfig(new Smod2.Config.ConfigSetting(announementsenabled088, false, true, "If server announcements are enabled or not"));
			this.AddConfig(new Smod2.Config.ConfigSetting(announceRequire088ConfigKey088, false, true, "If server require 049 to be dead for announcement"));
            this.AddConfig(new Smod2.Config.ConfigSetting(broudcastmessageConfigKey088, "Вы SCP-088. Подробности в консоли.", true, "Show broadcast message at scp-088"));
            this.AddConfig(new Smod2.Config.ConfigSetting(consolemessageConfigKey088, "Вы SCP-088. Ваша задача заражать живых людей с помощью укуса, но убить вы их не можете. После смерти от инфекции зараженный становится SCP-088-2, а именно SCP-939-89 с пониженным здоровьем и возможностью заражать других людей", true, "Show console message at scp-088"));
            this.AddConfig(new Smod2.Config.ConfigSetting(broudcastmessageConfigKey088_2, "Вы SCP-088-2. Подробности в консоли.", true, "Show broadcast message at scp-088"));
            this.AddConfig(new Smod2.Config.ConfigSetting(consolemessageConfigKey088_2, "Вы SCP-088-2. Ваша задача заражать живых людей с помощью укуса, но убить вы их не можете. После смерти от инфекции зараженный становится SCP-088-2, а именно SCP-939-89 с пониженным здоровьем и возможностью заражать других людей", true, "Show console message at scp-088"));
            this.AddConfig(new Smod2.Config.ConfigSetting(ranksAllowedConfigKey088, new string[0], true, "What ranks are allowed to run the commands of the plugin"));
			this.AddConfig(new Smod2.Config.ConfigSetting(rolesCanBeInfectedConfigKey088, new int[] { -1 }, true, "What roles can be infected"));
            this.AddConfig(new Smod2.Config.ConfigSetting(durationbroudcastmessageConfigKey088, 5, true, "Время, после которого будет изчезать текст из broudcast."));
            this.AddConfig(new Smod2.Config.ConfigSetting(damageAmountConfigKey088, 1, true, "Amount of damage per interval."));
			this.AddConfig(new Smod2.Config.ConfigSetting(damageIntervalConfigKey088, 2, true, "The interval at which to apply damage."));
			this.AddConfig(new Smod2.Config.ConfigSetting(swingDamageConfigKey088, 0, true, "The damage applied on swing."));

			this.AddConfig(new Smod2.Config.ConfigSetting(infectKillChanceConfigKey088, 100, true, "Infection Chance on zombie kill"));
			this.AddConfig(new Smod2.Config.ConfigSetting(infectChanceConfigKey088, 100, true, "Infection Chance"));
			this.AddConfig(new Smod2.Config.ConfigSetting(cureChanceConfigKey088, 100, true, "Cure chance of medpacks"));

			this.AddConfig(new Smod2.Config.ConfigSetting("scp008_spawn_room", string.Empty, true, "The room ID that scp008 will spawn."));
			#endregion
		}
	}
}