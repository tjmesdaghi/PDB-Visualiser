Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting

Public Class PrimaryDisplayForm

    Private ContactMapChart As New Chart
    Private Sub InputPDBFile_Click(sender As Object, e As EventArgs)
        Dim FileName As String
        FileName = InputBox("Please enter file name")
        'prompts the user to enter the disired file name with an input box where they will type in the name of the file
        ProteinDataBankFileProcessor(FileName)
    End Sub

    Private Sub Generate3DModel_Click(sender As Object, e As EventArgs)


        ' Clear ModelPanel and remove existing components that may interfere with the rendering
        ModelPanelForPrimaryDisplayForm.Controls.Clear() ' Remove any child controls from ModelPanel

        ' Remove existing charts from the main form
        ' Iterate through all controls on the form to find charts and remove them
        For Each ctrl As Control In Me.Controls
            If TypeOf ctrl Is DataVisualization.Charting.Chart Then
                Me.Controls.Remove(ctrl) ' Remove the chart from the form
                Exit For ' Exit loop after removing a chart (assumes only one chart at a time)
            End If
        Next

        ' Update the label displaying the length of the polypeptide
        LengthOfPolypeptideLabel.Text = "Amino Acid Count:" & Environment.NewLine & GloballyAccessibleLengthOfPolypeptide
        LengthOfPolypeptideLabel.ForeColor = Color.White ' Set text color to white for visibility
        LengthOfPolypeptideLabel.Size = New Size(1000, 1000) ' Set the size of the label (unusually large for debugging)
        LengthOfPolypeptideLabel.Location = New Point(515, 420) ' Position the label at the desired location
        Controls.Add(LengthOfPolypeptideLabel) ' Add the label to the form

        ' Update the label displaying the name of the polypeptide
        NameOfPolypeptideLabel.Text = GloballyAccessibleFileName ' Set text to the global file name
        NameOfPolypeptideLabel.ForeColor = Color.White ' Set text color to white for visibility
        NameOfPolypeptideLabel.Size = New Size(250, 21) ' Set size to fit the text content
        NameOfPolypeptideLabel.Location = New Point(12, 9) ' Position the label in the top-left corner
        Controls.Add(NameOfPolypeptideLabel) ' Add the label to the form
        NameOfPolypeptideLabel.BringToFront() ' Ensure the label is on top of other controls

        ' Configure the ModelPanel to serve as a container for the 3D rendering
        ModelPanelForPrimaryDisplayForm.Size = New Size(480, 440) ' Set the size of the panel
        ModelPanelForPrimaryDisplayForm.Location = New Point(10, 10) ' Position the panel at the top-left corner
        Controls.Add(ModelPanelForPrimaryDisplayForm) ' Add the panel to the form

        ModelCreation(ModelPanelForPrimaryDisplayForm)

    End Sub

    Private Sub SaltBridge_Click(sender As Object, e As EventArgs)
        ToggleSaltBridge()
    End Sub

    Private Sub DisulphideBridge_Click(sender As Object, e As EventArgs)
        ToggleDisulphideBridge()
    End Sub

    Public Sub ToggleSaltBridge()
        ' Iterate through all controls on the form
        For Each ctrl As Control In Me.Controls
            ' Check if the control is a Chart (from DataVisualization.Charting namespace)
            If TypeOf ctrl Is DataVisualization.Charting.Chart Then
                ' Toggle the state of ShowSaltBridgesForContactMap (used to control salt bridge visibility)
                ShowSaltBridgesForContactMap = Not ShowSaltBridgesForContactMap

                ' Get the series named "Contact" from the chart
                Dim contactSeries As Series = ContactMapChart.Series("Contact")

                If ShowSaltBridgesForContactMap Then
                    ' If ShowSaltBridgesForContactMap is True, add salt bridge points to the series
                    For Each point As DataPoint In SaltBridgePoints
                        contactSeries.Points.Add(point)
                    Next
                Else
                    ' If ShowSaltBridgesForContactMap is False, remove all points with Color.Green from the series
                    For i As Integer = contactSeries.Points.Count - 1 To 0 Step -1
                        If contactSeries.Points(i).Color = Color.Green Then
                            contactSeries.Points.RemoveAt(i)
                        End If
                    Next
                End If

                ' Redraw the chart to reflect the updated points
                ContactMapChart.Invalidate()

                ' Exit the loop after processing the first chart (assumes only one chart needs to be handled at a time)
                Exit For
            Else
                ' If the control is not a chart, toggle the visibility of green lines (used to represent salt bridges or specific interactions)
                ShowSaltBridgesForModel = Not ShowSaltBridgesForModel

                ' Request the ModelPanel control to be redrawn to reflect the updated visibility of green lines
                ModelPanelForPrimaryDisplayForm.Invalidate()
            End If
        Next
    End Sub

    Public Sub ToggleDisulphideBridge()
        ' Iterate through all controls on the form
        For Each ctrl As Control In Me.Controls
            ' Check if the control is a Chart (from DataVisualization.Charting namespace)
            If TypeOf ctrl Is DataVisualization.Charting.Chart Then
                ' Toggle the state of ShowDisulphideBridgesForContactMap (used to control disulphide bridge visibility)
                ShowDisulphideBridgesForContactMap = Not ShowDisulphideBridgesForContactMap

                ' Get the series named "Contact" from the chart
                Dim contactSeries As Series = ContactMapChart.Series("Contact")

                If ShowDisulphideBridgesForContactMap Then
                    ' If ShowDisulphideBridgesForContactMap is True, add disulphide bridge points to the series
                    For Each point As DataPoint In DisulphideBridgePoints
                        contactSeries.Points.Add(point)
                    Next
                Else
                    ' If ShowDisulphideBridgesForContactMap is False, remove all points with Color.Red from the series
                    For i As Integer = contactSeries.Points.Count - 1 To 0 Step -1
                        If contactSeries.Points(i).Color = Color.Red Then
                            contactSeries.Points.RemoveAt(i)
                        End If
                    Next
                End If

                ' Redraw the chart to reflect the updated points
                ContactMapChart.Invalidate()

                ' Exit the loop after processing the first chart (assumes only one chart needs to be handled at a time)
                Exit For
            End If
        Next

        ' Toggle the visibility of the "red lines" (used to represent disulphide bridges or specific interactions)
        ShowDisulphideBridgesForModel = Not ShowDisulphideBridgesForModel

        ' Request the ModelPanel control to be redrawn to reflect the updated visibility of red lines
        ModelPanelForPrimaryDisplayForm.Invalidate()
    End Sub

    Private Sub GenerateContactMap_Click(sender As Object, e As EventArgs)
        ' Remove the first Panel control from the form (if any exists)
        For Each ctrl As Control In Me.Controls
            ' Check if the control is of type Panel
            If TypeOf ctrl Is Panel Then
                Me.Controls.Remove(ctrl) ' Remove the panel from the form
                Exit For ' Exit the loop once the panel is removed
            End If
        Next

        ' Update the label displaying the length of the polypeptide
        LengthOfPolypeptideLabel.Text = "amino acid count:" & Environment.NewLine & GloballyAccessibleLengthOfPolypeptide
        LengthOfPolypeptideLabel.ForeColor = Color.White
        LengthOfPolypeptideLabel.Size = New Size(1000, 1000)
        LengthOfPolypeptideLabel.Location = New Point(515, 420)
        Controls.Add(LengthOfPolypeptideLabel)

        ' Update the label displaying the name of the polypeptidejuz
        NameOfPolypeptideLabel.Text = GloballyAccessibleFileName
        NameOfPolypeptideLabel.ForeColor = Color.White
        NameOfPolypeptideLabel.Size = New Size(250, 21)
        NameOfPolypeptideLabel.Location = New Point(12, 9)
        Controls.Add(NameOfPolypeptideLabel)
        NameOfPolypeptideLabel.BringToFront()

        ' Set up and add the Contact Map chart to the form
        ContactMapChart.Size = New Size(480, 410)
        ContactMapChart.Location = New Point(10, 30)
        Controls.Add(ContactMapChart)

        ' Clear any existing data series in the chart
        ContactMapChart.Series.Clear()

        ContactMapCreation(ContactMapChart)
    End Sub

    Private Sub FullScreen3DModel_Click(sender As Object, e As EventArgs)
        ' Opens full screen version of the model
        FullScreenModel.Show()
    End Sub

    Private Sub FullScreenContactMap_Click(sender As Object, e As EventArgs)
        ' Opens full screen version of the model
        FullScreenContactMap.Show()
    End Sub

    Private Sub ProteinDataBankFileHistory_Click(sender As Object, e As EventArgs)
        Dim filePath = "C:\Users\tjmes\OneDrive\CSCW\PDBFiles\PDBfileHistory.txt" ' Replace with your file path

        ' Check if the file exists
        If Not File.Exists(filePath) Then
            MessageBox.Show("The file does not exist.")
            Return
        End If

        ' Read all lines from the file
        Dim lines = File.ReadLines(filePath).ToList

        ' Display the lines in a MessageBox or a new Form
        Dim PDBFileHistoryForm As New Form ' Create a new form to display history
        Dim historyListBox As New ListBox ' Create a ListBox to hold the history
        Dim okButton As New Button ' Button to select the PDB file

        ' Set up the ListBox
        historyListBox.Items.AddRange(lines.ToArray) ' Add lines to the ListBox
        historyListBox.Dock = DockStyle.Top ' Dock the ListBox to the top of the form
        historyListBox.Height = 200 ' Set height for the ListBox

        ' Set up the OK Button
        okButton.Text = "Select PDB File"
        okButton.Dock = DockStyle.Bottom ' Dock the button to the bottom of the form

        Dim FileName As String

        ' Add handler for the OK Button click
        AddHandler okButton.Click, Sub(s, ev)
                                       ' Check if an item is selected in the ListBox
                                       If historyListBox.SelectedItem IsNot Nothing Then
                                           Dim selectedPDBFile = historyListBox.SelectedItem.ToString
                                           FileName = selectedPDBFile
                                           ' Perform any action with the selectedPDBFile here
                                       Else
                                           MessageBox.Show("Please select a file from the history.")
                                       End If
                                       ' Close the form after selection
                                       PDBFileHistoryForm.Close()
                                   End Sub

        ' Add controls to the history form
        PDBFileHistoryForm.Controls.Add(historyListBox)
        PDBFileHistoryForm.Controls.Add(okButton)

        ' Set up the history form properties
        PDBFileHistoryForm.Text = "PDB History"
        PDBFileHistoryForm.Size = New Size(300, 300) ' Set size of the history form
        PDBFileHistoryForm.StartPosition = FormStartPosition.CenterParent ' Center the form

        ' Show the history form
        PDBFileHistoryForm.ShowDialog(Me)
        ProteinDataBankFileProcessor(FileName)
    End Sub

    Private Sub CreateButtons()
        ' Create and configure the "Input PDB File" button
        Dim InputPDBFile As New Button()
        InputPDBFile.Name = "InputPDBFile"
        InputPDBFile.Text = "Input PDB File"
        InputPDBFile.BackColor = SystemColors.ControlDarkDark
        InputPDBFile.Size = New Size(304, 68)
        InputPDBFile.Location = New Point(513, 24) ' Set position manually
        ' Add click event handler for InputPDBFile button
        AddHandler InputPDBFile.Click, AddressOf InputPDBFile_Click
        ' Add the button to the form
        Me.Controls.Add(InputPDBFile)

        ' Create and configure the "Generate Contact Map" button
        Dim GenerateContactMap As New Button()
        GenerateContactMap.Name = "GenerateContactMap"
        GenerateContactMap.Text = "Generate Contact Map"
        GenerateContactMap.BackColor = SystemColors.ControlDarkDark
        GenerateContactMap.Size = New Size(304, 68)
        GenerateContactMap.Location = New Point(513, 114)
        ' Add click event handler for GenerateContactMap button
        AddHandler GenerateContactMap.Click, AddressOf GenerateContactMap_Click
        ' Add the button to the form
        Me.Controls.Add(GenerateContactMap)

        ' Create and configure the "Generate 3D Model" button
        Dim Generate3DModel As New Button()
        Generate3DModel.Name = "Generate3DModel"
        Generate3DModel.Text = "Generate 3D Model"
        Generate3DModel.BackColor = SystemColors.ControlDarkDark
        Generate3DModel.Size = New Size(304, 68)
        Generate3DModel.Location = New Point(513, 205)
        ' Add click event handler for Generate3DModel button
        AddHandler Generate3DModel.Click, AddressOf Generate3DModel_Click
        ' Add the button to the form
        Me.Controls.Add(Generate3DModel)

        ' Create and configure the "Toggle Disulphide Bridge" button
        Dim ToggleDisulphideBridge As New Button()
        ToggleDisulphideBridge.Name = "ToggleDisulphideBridge"
        ToggleDisulphideBridge.Text = "Toggle Disulphide Bridge(s)"
        ToggleDisulphideBridge.BackColor = SystemColors.ControlDarkDark
        ToggleDisulphideBridge.Size = New Size(141, 52)
        ToggleDisulphideBridge.Location = New Point(513, 298)
        ' Add click event handler for ToggleDisulphideBridge button
        AddHandler ToggleDisulphideBridge.Click, AddressOf DisulphideBridge_Click
        ' Add the button to the form
        Me.Controls.Add(ToggleDisulphideBridge)

        ' Create and configure the "Toggle Salt Bridge" button
        Dim ToggleSaltBridge As New Button()
        ToggleSaltBridge.Name = "ToggleSaltBridge"
        ToggleSaltBridge.Text = "Toggle Salt Bridge(s)"
        ToggleSaltBridge.BackColor = SystemColors.ControlDarkDark
        ToggleSaltBridge.Size = New Size(141, 52)
        ToggleSaltBridge.Location = New Point(680, 298)
        ' Add click event handler for ToggleSaltBridge button
        AddHandler ToggleSaltBridge.Click, AddressOf SaltBridge_Click
        ' Add the button to the form
        Me.Controls.Add(ToggleSaltBridge)

        ' Create and configure the "Full Screen 3D Model" button
        Dim FullScreen3DModel As New Button()
        FullScreen3DModel.Name = "FullScreen3DModel"
        FullScreen3DModel.Text = "Full Screen 3D Model"
        FullScreen3DModel.BackColor = SystemColors.ControlDarkDark
        FullScreen3DModel.Size = New Size(141, 52)
        FullScreen3DModel.Location = New Point(514, 356)
        ' Add click event handler for FullScreen3DModel button
        AddHandler FullScreen3DModel.Click, AddressOf FullScreen3DModel_Click
        ' Add the button to the form
        Me.Controls.Add(FullScreen3DModel)

        ' Create and configure the "Full Screen Contact Map" button
        Dim FullScreenContactMap As New Button()
        FullScreenContactMap.Name = "FullScreenContactMap"
        FullScreenContactMap.Text = "Full Screen Contact Map"
        FullScreenContactMap.BackColor = SystemColors.ControlDarkDark
        FullScreenContactMap.Size = New Size(141, 52)
        FullScreenContactMap.Location = New Point(680, 356)
        ' Add click event handler for FullScreenContactMap button
        AddHandler FullScreenContactMap.Click, AddressOf FullScreenContactMap_Click
        ' Add the button to the form
        Me.Controls.Add(FullScreenContactMap)

        ' Create and configure the "PDB File History" button
        Dim PDBFileHistory As New Button()
        PDBFileHistory.Name = "PDBFileHistory"
        PDBFileHistory.Text = "PDB File History"
        PDBFileHistory.BackColor = SystemColors.ControlDarkDark
        PDBFileHistory.Size = New Size(140, 23)
        PDBFileHistory.Location = New Point(680, 424)
        ' Add click event handler for PDBFileHistory button
        AddHandler PDBFileHistory.Click, AddressOf ProteinDataBankFileHistory_Click
        ' Add the button to the form
        Me.Controls.Add(PDBFileHistory)

    End Sub

    Private Sub PrimaryDisplayForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.Black
        ' Call the CreateButtons method to create and add buttons to the form.
        CreateButtons()
    End Sub

End Class