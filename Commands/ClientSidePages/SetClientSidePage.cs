﻿#if !ONPREMISES
using OfficeDevPnP.Core.Pages;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Base.PipeBinds;
using System;
using System.Management.Automation;

namespace SharePointPnP.PowerShell.Commands.ClientSidePages
{
    [Cmdlet(VerbsCommon.Set, "PnPClientSidePage")]
    [CmdletHelp("Sets parameters of a Client-Side Page",
      Category = CmdletHelpCategory.ClientSidePages, SupportedPlatform = CmdletSupportedPlatform.Online)]
    [CmdletExample(
        Code = @"PS:> Set-PnPClientSidePage -Identity ""MyPage"" -LayoutType Home",
        Remarks = "Updates the properties of the Client-Side page called 'MyPage'",
        SortOrder = 1)]
    public class SetClientSidePage : PnPWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "The name/identity of the page")]
        public ClientSidePagePipeBind Identity;

        [Parameter(Mandatory = false, HelpMessage = "Specifies the title of the page.")]
        public string Title = null;

        [Parameter(Mandatory = false, HelpMessage = "Sets the name of the page.")]
        public string Name = null;

        [Parameter(Mandatory = false, HelpMessage = "Sets the layout type of the page. (Default = Article)")]
        public ClientSidePageLayoutType LayoutType = ClientSidePageLayoutType.Article;

        [Parameter(Mandatory = false, HelpMessage = "Allows to promote the page for a specific purpose (HomePage | NewsPage)")]
        public ClientSidePagePromoteType PromoteAs = ClientSidePagePromoteType.None;

        [Parameter(Mandatory = false, HelpMessage = "Enables or Disables the comments on the page")]
        public bool? CommentsEnabled = null;

        [Parameter(Mandatory = false, HelpMessage = "Publishes the page once it is saved.")]
        public SwitchParameter Publish;

        [Parameter(Mandatory = false, HelpMessage = "Sets the message for publishing the page.")]
        public string PublishMessage = string.Empty;

        protected override void ExecuteCmdlet()
        {

            ClientSidePage clientSidePage = Identity?.GetPage(ClientContext);

            if (clientSidePage == null)
                // If the client side page object cannot be found
                throw new Exception($"Page {Identity?.Name} cannot be found.");

            // We need to have the page name, if not found, raise an error
            string name = ClientSidePageUtilities.EnsureCorrectPageName(Name ?? Identity?.Name);
            if (name == null)
                throw new Exception("Insufficient arguments to add a client side page");

            if (!string.IsNullOrEmpty(Title))
            {
                clientSidePage.PageTitle = Title;
            }

            clientSidePage.LayoutType = LayoutType;
            clientSidePage.Save(name);

            // If a specific promote type is specified, promote the page as Home or Article or ...
            switch (PromoteAs)
            {
                case ClientSidePagePromoteType.HomePage:
                    clientSidePage.PromoteAsHomePage();
                    break;
                case ClientSidePagePromoteType.NewsArticle:
                    clientSidePage.PromoteAsNewsArticle();
                    break;
                case ClientSidePagePromoteType.None:
                default:
                    break;
            }

            if (CommentsEnabled.HasValue)
            {
                if (CommentsEnabled.Value)
                {
                    clientSidePage.EnableComments();
                }
                else
                {
                    clientSidePage.DisableComments();
                }
            }

            if (Publish)
            {
                clientSidePage.Publish(PublishMessage);
            }

            WriteObject(clientSidePage);
        }
    }
}
#endif