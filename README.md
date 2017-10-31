# active-directory-inspector
Active Directory inspection tool to be used in IP to User Name mapping in Squid's Pseudo AD Authenticator

build
-----

1. Ensure you have Visual Studio 2013.
2. For production builds, change:
	- tools\builder\shared.xml 
	- src\Diladele.ActiveDirectory* \Properties\AssemblyInfo.cs to match the ones in shared.xml 
3. Run build_debug.bat to get bin\Debug\active-directory-inspector.msi 
   or build_release.bat to get bin\Release\active-directory-inspector.msi 

or download the one we built
----------------------------

http://packages.diladele.com/adi/1.0.0.0/debug/active-directory-inspector.msi

install
-------

1. Install MSI
2. Open Services / Active Directory Inspector / Properties / LogOn
3. Select Other Account / Administrator and type admin's password
4. Save
5. Restart Service

If we run the service as LocalSystem it always get E_ACCESSDENIED when trying to run WMI queries
on remote machines.


-------------------------------------------------------------------------------
Active Directory Inspector
Copyright (C) 2017 Diladele B.V.

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

Contact us at support@diladele.com