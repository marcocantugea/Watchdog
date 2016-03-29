
Imports System.Configuration
Public Class Main

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ' Chage the winlogon register
        ChangeRegister()
        ' Disable the watchdog avoid to init
        DisableWatchDog()
        ' kill the watchdog process
        KillWatchDogProcess()
        ' Start windows desktop
        StartDesktop()
    End Sub

    Private Sub ChangeRegister()
        Dim shell = My.Computer.Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows NT\CurrentVersion\Winlogon", True)
        Dim actualshell = shell.GetValue("Shell").ToString

        If actualshell.Contains("watchdog") Then
            shell.SetValue("Shell", "Explorer.exe")
            MsgBox("Register was reset", MsgBoxStyle.Information, "Watchdog disbabler")
        Else
            MsgBox("watchdog not found on register", MsgBoxStyle.Exclamation, "Watchdog disbabler")
        End If

    End Sub

    Private Sub DisableWatchDog()
        'copy the configuration file to the root of the app
        'MsgBox(Application.StartupPath)

        Try
            If My.Computer.FileSystem.FileExists(Application.StartupPath & "\WatchdogDisabler.exe.config") Then
                My.Computer.FileSystem.DeleteFile(Application.StartupPath & "\WatchdogDisabler.exe.config")
            End If

            My.Computer.FileSystem.CopyFile("C:\watchdog\watchdog.exe.config", Application.StartupPath & "\WatchdogDisabler.exe.config")
            Dim config As System.Configuration.Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            'MsgBox(config.AppSettings.Settings("EnableWatchDog").Value)
            config.AppSettings.Settings("EnableWatchDog").Value = "false"
            config.Save()
            MsgBox(config.AppSettings.Settings("EnableWatchDog").Value)
            My.Computer.FileSystem.DeleteFile("C:\watchdog\watchdog.exe.config")
            My.Computer.FileSystem.CopyFile(Application.StartupPath & "\WatchdogDisabler.exe.config", "C:\watchdog\watchdog.exe.config")
            MsgBox("Watchdog is disable to run.", MsgBoxStyle.Information, "Watchdog disbabler")
        Catch ex As Exception
            MsgBox("Error trying to disable watchdog", MsgBoxStyle.Information, "Watchdog disbabler")
            MsgBox(ex.Message)

        End Try



    End Sub

    Private Sub KillWatchDogProcess()
        Try
            Dim PS() As Process = Process.GetProcessesByName("watchdog")
            For Each item As Process In PS
                item.Kill()
            Next
            MsgBox("Watchdog services stop", MsgBoxStyle.Information, "Watchdog disbabler")
        Catch ex As Exception
            MsgBox("Error trying to stop watchdog", MsgBoxStyle.Critical, "Watchdog disbabler")
        End Try
    End Sub

    Private Sub StartDesktop()
        Try
            Dim PS() As Process = Process.GetProcessesByName("explorer")
            If PS.Length = 0 Then
                Process.Start("explorer.exe")
            End If
            MsgBox("Windows Desktop Enabled", MsgBoxStyle.Information, "Watchdog disbabler")
        Catch ex As Exception
            MsgBox("Error trying to stop watchdog", MsgBoxStyle.Critical, "Watchdog disbabler")
        End Try
    End Sub

End Class
