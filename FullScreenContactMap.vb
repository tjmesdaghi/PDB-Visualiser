Imports System.Windows.Forms.DataVisualization.Charting

Public Class FullScreenContactMap

    Private ContactMapChart As New Chart

    Private Sub FullScreenContactMap_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Maximize the window when the form is loaded
        Me.WindowState = FormWindowState.Maximized

        ' Create a panel to host the chart and the buttons
        Dim ContactMapPanel As New Panel()
        ContactMapPanel.Dock = DockStyle.Fill ' Set the panel to fill the form
        Me.Controls.Add(ContactMapPanel) ' Add the panel to the form's controls

        ' Create and configure the chart dynamically
        ContactMapChart = New Chart()
        ContactMapChart.Dock = DockStyle.Fill ' Set the chart to fill the panel
        ContactMapPanel.Controls.Add(ContactMapChart) ' Add the chart to the panel
        ContactMapChart.Titles.Add("Contact Map For " & GloballyAccessibleFileName) ' Set the chart title

        ' Call the ContactMapCreation function to populate the chart with data
        ContactMapCreation(ContactMapChart)

        ' Create the toggle buttons on the panel
        CreateToggleButtons(ContactMapPanel)
    End Sub

    Private Sub CreateToggleButtons(Panel As Panel)
        ' Create a button to toggle salt bridges on the contact map
        Dim SaltBridgeButton As New Button()
        SaltBridgeButton.Text = "Toggle Salt Bridges" ' Set the button text
        SaltBridgeButton.Location = New Point(10, 10) ' Set the button position on the panel
        SaltBridgeButton.Size = New Size(130, 40) ' Set the button size
        AddHandler SaltBridgeButton.Click, AddressOf ToggleSaltBridgesForContactMap ' Attach event handler for the button click
        Panel.Controls.Add(SaltBridgeButton) ' Add the button to the panel
        SaltBridgeButton.BringToFront() ' Ensure the button is in front of other controls

        ' Create a button to toggle disulphide bridges on the contact map
        Dim DisulphideButton As New Button()
        DisulphideButton.Text = "Toggle Disulphide Bridges" ' Set the button text
        DisulphideButton.Location = New Point(150, 10) ' Set the button position on the panel
        DisulphideButton.Size = New Size(130, 40) ' Set the button size
        AddHandler DisulphideButton.Click, AddressOf ToggleDisulphideBridgesForContactMap ' Attach event handler for the button click
        Panel.Controls.Add(DisulphideButton) ' Add the button to the panel
        DisulphideButton.BringToFront() ' Ensure the button is in front of other controls
    End Sub

    Public Sub ToggleSaltBridgesForContactMap(sender As Object, e As EventArgs)
        ' Toggle the visibility of salt bridges on the contact map
        ShowSaltBridgesForContactMap = Not ShowSaltBridgesForContactMap
        Dim contactSeries As Series = ContactMapChart.Series("Contact") ' Get the series for the contact map

        ' If salt bridges should be shown, add the points to the series
        If ShowSaltBridgesForContactMap Then
            For Each point As DataPoint In SaltBridgePoints
                contactSeries.Points.Add(point) ' Add each salt bridge point to the series
            Next
        Else
            ' If salt bridges should be hidden, remove the points that are colored green (representing salt bridges)
            For i As Integer = contactSeries.Points.Count - 1 To 0 Step -1
                If contactSeries.Points(i).Color = Color.Green Then
                    contactSeries.Points.RemoveAt(i) ' Remove the salt bridge points
                End If
            Next
        End If

        ' Refresh the chart to reflect the changes
        ContactMapChart.Invalidate()
    End Sub

    Public Sub ToggleDisulphideBridgesForContactMap(sender As Object, e As EventArgs)
        ' Toggle the visibility of disulphide bridges on the contact map
        ShowDisulphideBridgesForContactMap = Not ShowDisulphideBridgesForContactMap
        Dim contactSeries As Series = ContactMapChart.Series("Contact") ' Get the series for the contact map

        ' If disulphide bridges should be shown, add the points to the series
        If ShowDisulphideBridgesForContactMap Then
            For Each point As DataPoint In DisulphideBridgePoints
                contactSeries.Points.Add(point) ' Add each disulphide bridge point to the series
            Next
        Else
            ' If disulphide bridges should be hidden, remove the points that are colored red (representing disulphide bridges)
            For i As Integer = contactSeries.Points.Count - 1 To 0 Step -1
                If contactSeries.Points(i).Color = Color.Red Then
                    contactSeries.Points.RemoveAt(i) ' Remove the disulphide bridge points
                End If
            Next
        End If

        ' Refresh the chart to reflect the changes
        ContactMapChart.Invalidate()
    End Sub


End Class