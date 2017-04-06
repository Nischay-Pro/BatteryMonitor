Imports BatteryMonitor.IniFile
Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSettings()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim power As PowerStatus = SystemInformation.PowerStatus
        Dim percent As Single = power.BatteryLifePercent
        Label1.Text = "Percent battery life remaining: " & percent * 100
        Dim iSpan As TimeSpan = TimeSpan.FromSeconds(power.BatteryLifeRemaining)
        Label2.Text = "Battery Life Remaining: " & iSpan.Hours.ToString.PadLeft(2, "0"c) & ":" &
                        iSpan.Minutes.ToString.PadLeft(2, "0"c) & ":" &
                        iSpan.Seconds.ToString.PadLeft(2, "0"c)
    End Sub
    Private Function LoadSettings()
        Dim ini As New IniFile
        Try
            ini.Load(My.Application.Info.DirectoryPath & "\settings.ini")
            If ini.GetKeyValue("General", "Startup") = "True" Then
                CheckBox1.Checked = True
            End If
            If ini.GetKeyValue("General", "Threshold") = "True" Then
                CheckBox2.Checked = True
                NumericUpDown1.Enabled = True
                NumericUpDown2.Enabled = True
                NumericUpDown1.Value = ini.GetKeyValue("General", "Min")
                NumericUpDown2.Value = ini.GetKeyValue("General", "Max")
                Timer2.Start()
            End If
            If ini.GetKeyValue("General", "Overcharge") = "True" Then
                CheckBox3.Checked = True
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Dim ini As New IniFile
        Try
            ini.Load(My.Application.Info.DirectoryPath & "\settings.ini")
        Catch ex As Exception
        End Try
        ini.SetKeyValue("General", "Startup", CheckBox1.Checked)
        ini.Save(My.Application.Info.DirectoryPath & "\settings.ini")
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        Dim ini As New IniFile
        Try
            ini.Load(My.Application.Info.DirectoryPath & "\settings.ini")
        Catch ex As Exception
        End Try
        ini.SetKeyValue("General", "Threshold", CheckBox2.Checked)
        ini.SetKeyValue("General", "Min", NumericUpDown1.Value)
        ini.SetKeyValue("General", "Max", NumericUpDown2.Value)
        ini.Save(My.Application.Info.DirectoryPath & "\settings.ini")
        NumericUpDown1.Enabled = CheckBox2.Checked
        NumericUpDown2.Enabled = CheckBox2.Checked
        If CheckBox2.Checked = False Then
            Timer2.Stop()
        Else
            Timer2.Start()
        End If
    End Sub

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        Button2.Enabled = True
    End Sub

    Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged
        Button2.Enabled = True
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If NumericUpDown1.Value >= NumericUpDown2.Value Then
            MsgBox("Error Minimum value can't be greater than or equal to Maximum value", MsgBoxStyle.Information, "Error")
            NumericUpDown1.Value = NumericUpDown2.Value - 1
            Exit Sub
        End If
        If NumericUpDown2.Value <= NumericUpDown1.Value Then
            MsgBox("Error Maximum value can't be lesser than or equal to Minimum value", MsgBoxStyle.Information, "Error")
            NumericUpDown2.Value = NumericUpDown1.Value + 1
            Exit Sub
        End If
        Dim ini As New IniFile
        Try
            ini.Load(My.Application.Info.DirectoryPath & "\settings.ini")
        Catch ex As Exception
        End Try
        ini.SetKeyValue("General", "Threshold", CheckBox2.Checked)
        ini.SetKeyValue("General", "Min", NumericUpDown1.Value)
        ini.SetKeyValue("General", "Max", NumericUpDown2.Value)
        ini.Save(My.Application.Info.DirectoryPath & "\settings.ini")
        NumericUpDown1.Enabled = CheckBox2.Checked
        NumericUpDown2.Enabled = CheckBox2.Checked
        Button2.Enabled = False
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        If NumericUpDown1.Enabled = NumericUpDown2.Enabled = True Then
            Dim power As PowerStatus = SystemInformation.PowerStatus
            Dim percent As Single = power.BatteryLifePercent
            If percent * 100 <= NumericUpDown1.Value Then
                NotifyIcon1.BalloonTipText = "Battery Level is below threshold of " & NumericUpDown1.Value & ". We suggest you connect your laptop for charging."
                NotifyIcon1.BalloonTipTitle = "Battery Monitor"
                NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning
                NotifyIcon1.ShowBalloonTip(5000)
                Timer3.Start()
                Timer2.Stop()
            End If
            If percent * 100 >= NumericUpDown2.Value Then
                NotifyIcon1.BalloonTipText = "Battery Level is above threshold of " & NumericUpDown2.Value & ". We suggest you disconnect your laptop from charging."
                NotifyIcon1.BalloonTipTitle = "Battery Monitor"
                NotifyIcon1.BalloonTipIcon = ToolTipIcon.Warning
                NotifyIcon1.ShowBalloonTip(5000)
                Timer3.Start()
                Timer2.Stop()
            End If
        End If
    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        Dim power As PowerStatus = SystemInformation.PowerStatus
        Dim percent As Single = power.BatteryLifePercent * 100
        If power.BatteryChargeStatus = BatteryChargeStatus.Charging And percent > NumericUpDown1.Value And percent < NumericUpDown2.Value Then
            Timer2.Start()
            Timer3.Stop()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        Me.ShowInTaskbar = False
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
        Me.ShowInTaskbar = True
    End Sub
End Class
