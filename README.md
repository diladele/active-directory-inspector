# active-directory-inspector
Active Directory inspection tool to be used in IP to User Name mapping in Squid's Pseudo AD Authenticator

build
-----

1. Open solution in Visual Studio 2013.
2. Select Debug configuration.
3. Build Diladele.ActiveDirectory.Service project (all dependencies will be build automatically)
4. Run build.bat in the root folder accordingly

install
-------

1. Install MSI
2. Open Services / Active Directory Inspector / Properties / LogOn
3. Select Other Account / Administrator and type admin's password
4. Save
5. Restart Service

If we run the service as LocalSystem it always get E_ACCESSDENIED when trying to run WMI queries
on remote machines.

