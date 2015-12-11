# The Sitecore *Content CheckIn* Module
The purpose of this module is to allow a role-based approach to allowing non admin users the ability to see and use the Check In / Unlock feature of workflow. This is useful if there are non-admin super users in Sitecore that need to be able to unlock items other authors have locked (e.g. before they went on vacation).

## Download and Usage Instructions

1. Download the Sitecore package from the [Sitecore Marketplace](https://marketplace.sitecore.net/Modules/C/Content_CheckIn.aspx?sc_lang=en) site
1. Install the Sitecore package (deploys a DLL and a patch config). ***Note***: this only needs to be on the **content 
authoring** instance.
1. Next, setup the role that will allow members to unlock content. Either:
 1. Create a role called **sitecore\Unlocker**
 1. Or, configure the `<setting name="Sitecore.ContentCheckIn.UnlockerRole" ... />` in the patch config with the respective role you want to give unlock capability to
`

## Source Code
The source code is compiled against the **Sitecore 8.1 rev. 151003** kernel, however you can change the version and re-compile yourself if you need to.
