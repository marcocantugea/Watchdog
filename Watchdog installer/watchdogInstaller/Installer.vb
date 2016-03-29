Public Class Installer

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        OpenFileDialog1.ShowDialog()
        TextBox1.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        ' Copy the watchdog to the install path and set the application to run
        Try
            CopyFilesToSystem()
        Catch ex As Exception
            MsgBox(ex.Message)
            EnableWatchDog()
        End Try

        ' change register on regedit
        ChangeRegister()
        ' run watchdog
        Process.Start(Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\watchdog.exe")
        ' close the desktop
        KillDesktop()

        'close the installer
        Application.Exit()
        End

    End Sub
    Private Sub ChangeRegister()
        Dim shell = My.Computer.Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows NT\CurrentVersion\Winlogon", True)
        Dim actualshell = shell.GetValue("Shell").ToString

        If Not actualshell.Contains("watchdog") Then
            'MsgBox(Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\watchdog.exe")
            shell.SetValue("Shell", Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\watchdog.exe")
            'MsgBox("Register ", MsgBoxStyle.Information, "Watchdog disbabler")
        Else
            MsgBox("Watchdog is alredy registered", MsgBoxStyle.Exclamation, "Watchdog installer")
        End If

    End Sub

    Private Sub CopyFilesToSystem()
        'make directory
        My.Computer.FileSystem.CreateDirectory(Configuration.ConfigurationSettings.AppSettings("InstallPath"))
        'get files to copy to the directory created and copy to the installation path
        Dim di As New IO.DirectoryInfo(Application.StartupPath & "\version")
        Dim Files As IO.FileInfo() = di.GetFiles

        For Each file As IO.FileInfo In Files
            If Not file.Name.Equals("watchdog.exe.config") Then
                Try
                    My.Computer.FileSystem.CopyFile(file.FullName, Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\" & file.Name)
                Catch ex As Exception
                    Throw New Exception("watchdog is alredy installed")
                End Try
            End If
        Next
        SetApptoRun()
    End Sub

    Private Sub SetApptoRun()
        Dim reader As New IO.StreamReader(Application.StartupPath & "\version\watchdog.exe.config")
        Dim writer As New IO.StreamWriter(Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\watchdog.exe.config")
        Dim line As String
        Do While reader.Peek <> -1
            line = reader.ReadLine
            If line.Contains("@apptorun") Then
                line = line.Replace("@apptorun", TextBox1.Text)
                writer.WriteLine(line)
            Else
                writer.WriteLine(line)
            End If
        Loop
        writer.Close()
        reader.Close()
        writer.Dispose()
        reader.Dispose()

    End Sub

    Private Sub KillDesktop()
        Try
            Dim PS() As Process = Process.GetProcessesByName("explorer")
            For Each item As Process In PS
                item.Kill()
            Next
            'MsgBox("Watchdog services stop", MsgBoxStyle.Information, "Watchdog disbabler")
        Catch ex As Exception
            MsgBox("Error trying to stop the desktop", MsgBoxStyle.Critical, "Watchdog installer")
        End Try
    End Sub

    Private Sub EnableWatchDog()
        Dim reader As New IO.StreamReader(Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\watchdog.exe.config")
        Dim writer As New IO.StreamWriter(Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\watchdog.exe.config.tmp")
        Dim line As String
        Do While reader.Peek <> -1
            line = reader.ReadLine
            If line.Contains("EnableWatchDog") Then
                line = line.Replace("false", "true")
                writer.WriteLine(line)
            Else
                writer.WriteLine(line)
            End If
        Loop
        writer.Close()
        reader.Close()
        writer.Dispose()
        reader.Dispose()

        My.Computer.FileSystem.DeleteFile(Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\watchdog.exe.config")
        My.Computer.FileSystem.CopyFile(Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\watchdog.exe.config.tmp", Configuration.ConfigurationSettings.AppSettings("InstallPath") & "\watchdog.exe.config")
    End Sub

End Class
