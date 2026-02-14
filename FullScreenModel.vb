
Public Class FullScreenModel

    Private Sub CreateButtons()
        ' Create a panel to hold the buttons for the fullscreen model
        ModelPanelForFullScreenModel = New Panel()
        ModelPanelForFullScreenModel.Dock = DockStyle.Fill
        ModelPanelForFullScreenModel.BackColor = Color.Black
        Me.Controls.Add(ModelPanelForFullScreenModel)

        ' Create a button to toggle salt bridges for the model
        Dim SaltBridgeButton As New Button()
        SaltBridgeButton.Text = "Toggle Salt Bridge(s)"
        SaltBridgeButton.Location = New Point(10, 10)
        SaltBridgeButton.Size = New Size(130, 80)
        AddHandler SaltBridgeButton.Click, AddressOf ToggleSaltBridges
        SaltBridgeButton.BackColor = SystemColors.ControlDarkDark
        Me.Controls.Add(SaltBridgeButton)
        SaltBridgeButton.BringToFront()

        ' Create a button to toggle disulphide bridges for the model
        Dim DisulphideBridgeButton As New Button()
        DisulphideBridgeButton.Text = "Toggle Disulphide Bridge(s)"
        DisulphideBridgeButton.Location = New Point(150, 10)
        DisulphideBridgeButton.Size = New Size(130, 80)
        AddHandler DisulphideBridgeButton.Click, AddressOf ToggleDisulphideBridges
        DisulphideBridgeButton.BackColor = SystemColors.ControlDarkDark
        Me.Controls.Add(DisulphideBridgeButton)
        DisulphideBridgeButton.BringToFront()
    End Sub

    Private Sub ToggleSaltBridges(sender As Object, e As EventArgs)
        ' Toggle the visibility of salt bridges on the model
        ShowSaltBridgesForModel = Not ShowSaltBridgesForModel
        ModelPanelForFullScreenModel.Invalidate() ' Redraw the panel to update the model display
    End Sub

    Private Sub ToggleDisulphideBridges(sender As Object, e As EventArgs)
        ' Toggle the visibility of disulphide bridges on the model
        ShowDisulphideBridgesForModel = Not ShowDisulphideBridgesForModel
        ModelPanelForFullScreenModel.Invalidate() ' Redraw the panel to update the model display
    End Sub

    Private Sub FullScreenModel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Maximize the window when the form is loaded
        Me.WindowState = FormWindowState.Maximized

        ' Initialize the components of the form
        InitializeComponent()

        ' Create the toggle buttons for controlling salt and disulphide bridges
        CreateButtons()

        ' Call the ModelCreation function to populate the model in the panel
        ModelCreation(ModelPanelForFullScreenModel)
    End Sub


End Class