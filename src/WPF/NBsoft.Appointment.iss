;IsX64 = Define se cria o instalador 64bits ou 32bits
#define IsX64 true
[Setup]
AppId = 9B2BBE12-2D6C-403C-9788-5CD4A8FF4F3C
AppName=NBsoft.Appointment
AppVerName=NBsoft.Appointment
AppPublisher=Nuno Benjamim Araujo
AppPublisherURL=geral@nbsoft.pt
AppSupportURL=www.nbsoft.pt
#if IsX64
  ArchitecturesInstallIn64BitMode = x64 
  ArchitecturesAllowed = x64
  OutputBaseFilename=NBsoft.Appointment.1.0_x64
#else
  ArchitecturesAllowed = x86 x64
  OutputBaseFilename=NBsoft.Appointment.1.0_AnyCPU
#endif
AllowNoIcons=no
WizardImageFile=.\Install\WizModernImage-IS.bmp
WizardSmallImageFile=.\Install\WizModernSmallImage-IS.bmp
DefaultGroupName="NBsoft.Appointment"
DefaultDirName={pf}\NBsoft\Appointment1.0
UsePreviousAppDir = yes
AppCopyright=Nuno Benjamim Araújo - nunobaraujo@hotmail.com
PrivilegesRequired=Admin
MinVersion=0.0,5.0
LicenseFile=.\Documents\Software_Disclaimer_Appointment.rtf
UninstallDisplayIcon={app}\NBsoft.Appointment.WPF.exe
AppVersion=1.1.0.22
VersionInfoVersion=1.1.0.22
;OutputDir=D:\Share\Output\InvXPress\2017-01-12_1.0.0.167
OutputDir=T:\NBsoft\Appointment\2017-06-05_1.1.0.22

[Tasks]
Name: desktopicon; Description: "Create &Desktop Shortcuts"; GroupDescription: "Additional Shortcuts:"; 
;Name: frontofficestartup; Description: "Automatic Start FrontOffice on Startup"; GroupDescription: "Additional Shortcuts:"; Components: FrontOffice; Flags: unchecked

;[Types]
;Name: "full"; Description: "Full installation"
;Name: "custom"; Description: "Custom installation"; Flags: iscustom

;[Components]
;Name: "Server"; Description: "Server"; Types: full serv custom;
;Name: "Backoffice"; Description: "Back-Office"; Types: full back custom;
;Name: "Frontoffice"; Description: "Front-Office "; Types: full frnt;
;Types:  full compact custom;

[DIRS]
Name: {commonappdata}\NBsoft; Permissions: everyone-modify
Name: {commonappdata}\NBsoft\Appointment1.0
              
[Files]      
; <<========= Sem Arquitectura =========>>
; License
Source: .\Documents\Software_Disclaimer_Appointment.rtf; DestDir: {app}; Flags:  ignoreversion
Source: .\Documents\THIRDPARTYLICENSEREADME.txt; DestDir: {app}; Flags:  ignoreversion

; Manual                                                         
;Source: "..\Server\Documents\Manual1.0.pdf"; DestDir: {app}\Manual; Flags:  ignoreversion

; Common
;Source: .\Install\DirectShowLib-2005.dll; DestDir: {app}; Flags:  ignoreversion
Source: .\Dictionary\*.xaml; DestDir: {app}\Dictionary; Flags:  ignoreversion; 
Source: .\Dictionary\*.png;  DestDir: {app}\Dictionary; Flags:  ignoreversion;

; Database Query
;Source: ..\DAL\Query\MySql\*.sql; DestDir: {app}\Query\MySql; Flags:  ignoreversion; Components: Server
Source: ..\DAL\Query\*.sql; DestDir: {app}\Query; Flags:  ignoreversion;

;SQLite
Source: .\bin\Release\x64\SQLite.Interop.dll; DestDir: {app}\x64; Flags:  ignoreversion;
Source: .\bin\Release\x86\SQLite.Interop.dll; DestDir: {app}\x86; Flags:  ignoreversion;

;Server
Source: .\bin\Release\NBsoft.Appointment.WPF.exe.config; DestDir: {app}; Flags:  ignoreversion; 
#if IsX64
  ; <<========= Arquitectura x64 =========>>

  ; Dependencies
  Source: .\Install\vcredist_x64.exe; DestDir: {app}\Deploy; Flags:  ignoreversion;    
  
  ; Common  
  Source: .\bin\x64\Release\EntityFramework.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\x64\Release\EntityFramework.SqlServer.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\x64\Release\FirstFloor.ModernUI.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\x64\Release\NBsoft.Appointment.DAL.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\x64\Release\NBsoft.Appointment.WPF.exe; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\x64\Release\System.Data.SQLite.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\x64\Release\System.Data.SQLite.EF6.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\x64\Release\System.Data.SQLite.Linq.dll; DestDir: {app}; Flags:  ignoreversion

#else
  ; <<========= Arquitectura x86 =========>>

  ; Dependencies
    Source: .\Install\vcredist_x86.exe; DestDir: {app}\Deploy; Flags:  ignoreversion;    
    
  ; Common  
  Source: .\bin\Release\EntityFramework.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\Release\EntityFramework.SqlServer.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\Release\FirstFloor.ModernUI.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\Release\NBsoft.Appointment.DAL.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\Release\NBsoft.Appointment.WPF.exe; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\Release\System.Data.SQLite.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\Release\System.Data.SQLite.EF6.dll; DestDir: {app}; Flags:  ignoreversion
  Source: .\bin\Release\System.Data.SQLite.Linq.dll; DestDir: {app}; Flags:  ignoreversion

#endif

[Icons]
Name: {group}\NBsoft Appointment;               Filename: {app}\NBsoft.Appointment.WPF.exe;           WorkingDir: {app};
Name: {group}\License Agreement;                Filename: {app}\Software_Disclaimer_Appointment.rtf;  WorkingDir: {app}
Name: {group}\Uninstall NBsoft Appointment ;    Filename: {uninstallexe};                             WorkingDir: {app}
Name: {userdesktop}\NBsoft Appointment;         Filename: {app}\NBsoft.Appointment.WPF.exe;           WorkingDir: {app}; Tasks: desktopicon; 


[Registry]
Root: HKLM; Subkey: "Software\NBsoft\Appointment1.0"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\NBsoft\Appointment1.0\Settings"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"
Root: HKLM; Subkey: "Software\NBsoft\Appointment1.0\Settings"; ValueType: string; ValueName: "InstallDate"; ValueData: "{code:DateTime}"; Flags: createvalueifdoesntexist


[Run]
#if IsX64
  Filename: "{app}\Deploy\vcredist_x64.exe"; Parameters:"/passive /norestart"; Description: "Visual C++ Files";  Flags: skipifdoesntexist waituntilterminated  
  ;Filename: "msiexec.exe"; Parameters: "/i ""{app}\Deploy\SyncSDK-v2.1-x64-ENU.msi"" /passive "; Flags: skipifdoesntexist waituntilterminated; Components: Server  
#else
  Filename: "{app}\Deploy\vcredist_x86.exe"; Parameters:"/passive /norestart"; Description: "Visual C++ Files";  Flags: skipifdoesntexist waituntilterminated  
  ;Filename: "msiexec.exe"; Parameters: "/i ""{app}\Deploy\SyncSDK-v2.1-x86-ENU.msi"" /passive "; Flags: skipifdoesntexist waituntilterminated; Components: Server  
#endif
;Filename: "{app}\NBsoft.InvXPress.Admin.exe"; Parameters:"-i"; Flags: nowait runascurrentuser;  Components: Server

[UninstallRun]
;Filename: "{app}\NBsoft.InvXPress.Admin.exe"; Parameters:"-u"; Flags: waituntilterminated runascurrentuser;  Components: Server

[UninstallDelete]
Type: filesandordirs; Name: {app};  

[Languages]
;Name: Portugues; MessagesFile: "compiler:\languages\Portuguese.isl"

[Code]
function DateTime(Param: String) : string;
begin
  Result := GetDateTimeString('yyyy/mm/dd', '-', ':');
end;

