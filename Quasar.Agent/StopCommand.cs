using System;
using System.Threading.Tasks;
using PluginSdk;
using PluginSdk.Commands;

namespace Quasar.Agent
{
    [CommandRoot("stop", "Quasar", "Save the world then shut the server down")]
    public sealed class StopCommand : CommandModule
    {
        internal static Action AdminStopRequested { get; set; }

        [Command("", "Save the world then shut the server down")]
        public void Stop()
        {
            Context.Respond("Saving world and shutting the server down...");
            Task.Run(() =>
            {
                TryNotifyAdminStopRequested();
                ServerControl.SaveAndQuit();
            });
        }

        private static void TryNotifyAdminStopRequested()
        {
            try
            {
                AdminStopRequested?.Invoke();
            }
            catch
            {
            }
        }
    }

    [CommandRoot("restart", "Quasar", "Save the world then restart the server")]
    public sealed class RestartCommand : CommandModule
    {
        internal static Action AdminRestartRequested { get; set; }

        [Command("", "Save the world then restart the server")]
        public void Restart()
        {
            Context.Respond("Saving world and restarting the server...");
            Task.Run(() =>
            {
                TryNotifyAdminRestartRequested();
                ServerControl.SaveAndQuit();
            });
        }

        private static void TryNotifyAdminRestartRequested()
        {
            try
            {
                AdminRestartRequested?.Invoke();
            }
            catch
            {
            }
        }
    }

    [CommandRoot("quit", "Quasar", "Quit the server immediately without saving")]
    public sealed class QuitCommand : CommandModule
    {
        internal static Action AdminStopRequested { get; set; }

        [Command("", "Quit the server immediately without saving")]
        public void Quit()
        {
            Context.Respond("Quitting without saving...");
            Task.Run(() =>
            {
                TryNotifyAdminStopRequested();
                ServerControl.QuitWithoutSaving();
            });
        }

        private static void TryNotifyAdminStopRequested()
        {
            try
            {
                AdminStopRequested?.Invoke();
            }
            catch
            {
            }
        }
    }
}
