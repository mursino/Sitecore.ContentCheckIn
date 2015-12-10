using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System;
using System.Reflection;

namespace Sitecore.ContentCheckIn.Framework.Commands
{
    [Serializable]
    public class CheckIn : Sitecore.Shell.Framework.Commands.CheckIn
    {
        private string UnlockerRole
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("Sitecore.ContentCheckIn.UnlockerRole", "sitecore\\Unlocker");
            }
        }

        private bool UserCanUnlock
        {
            get
            {
                return Context.User.IsInRole(UnlockerRole);
            }
        }
        
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
            {
                return CommandState.Hidden;
            }
            Item item = context.Items[0];
            
            if (UserCanUnlock)
            {
                if (!item.Locking.IsLocked())
                {
                    return CommandState.Hidden;
                }
                return CommandState.Enabled;
            }
            
            return base.QueryState(context);
        }

        protected new void Run(ClientPipelineArgs args)
        {
            // call base Run()
            
            Type t = typeof(Sitecore.Shell.Framework.Commands.CheckIn);
            var reflectedMethod = t.GetMethod("Run", BindingFlags.Instance | BindingFlags.NonPublic);

            if (reflectedMethod != null)
            {
                reflectedMethod.Invoke(this, new object[] { args });
            }

            // unlock if user is in the custom unlocker role

            if (!SheerResponse.CheckModified())
            {
                return;
            }
            string itemPath = args.Parameters["id"];
            string name = args.Parameters["language"];
            string value = args.Parameters["version"];
            Item itemNotNull = Client.GetItemNotNull(itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(value));

            if (itemNotNull.Locking.HasLock() || UserCanUnlock)
            {
                Log.Audit(this, "Check in: {0}", new string[] { AuditFormatter.FormatItem(itemNotNull) });

                itemNotNull.Editing.BeginEdit();
                using (new SecurityDisabler())
                {
                    itemNotNull.Locking.Unlock();
                }
                itemNotNull.Editing.EndEdit();

                Context.ClientPage.SendMessage(this, "item:checkedin");
            }
        }
    }
}
