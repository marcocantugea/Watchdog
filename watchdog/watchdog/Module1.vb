Module Module1
    Public Sub Main()
        Application.Run(new Watchdog)
    End Sub

End Module

Public Class Watchdog
    Inherits ApplicationContext

    Private AppToWatch As String
    Private FullPath As String = Configuration.ConfigurationSettings.AppSettings("Application")
    Private enableWatchDog As Boolean = Boolean.Parse(Configuration.ConfigurationSettings.AppSettings("EnableWatchDog"))

    Private WithEvents P As Process

    Public Sub New()
        If enableWatchDog Then
            AppToWatch = System.IO.Path.GetFileNameWithoutExtension(FullPath)
            Dim PS() As Process = Process.GetProcessesByName(AppToWatch)
            If PS.Length = 0 Then
                StartIt()
            Else
                P = PS(0)
                P.EnableRaisingEvents = True
            End If
        Else
            Application.Exit()
            End
        End If
    End Sub

    Private Sub P_Exited(ByVal sender As Object, ByVal e As EventArgs) Handles P.Exited
        StartIt()
    End Sub

    Private Sub StartIt()
        P = Process.Start(FullPath)
        P.EnableRaisingEvents = True
    End Sub

End Class