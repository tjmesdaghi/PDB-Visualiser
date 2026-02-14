Imports System.Windows.Forms.DataVisualization.Charting

Module ResourcesForModel

    ' A Panel objects to represent the main model display
    Private _ModelPanelForPrimaryDisplayForm As New Panel
    Public Property ModelPanelForPrimaryDisplayForm As Panel
        Get
            Return _ModelPanelForPrimaryDisplayForm
        End Get
        Set(value As Panel)
            _ModelPanelForPrimaryDisplayForm = value
        End Set
    End Property

    Private _ModelPanelForFullScreenModel As New Panel
    Public Property ModelPanelForFullScreenModel As Panel
        Get
            Return _ModelPanelForFullScreenModel
        End Get
        Set(value As Panel)
            _ModelPanelForFullScreenModel = value
        End Set
    End Property

    ' Rotation angles (in degrees or radians) for rotating the model around the X and Y axes
    Private _RotationX As Single = 0
    Private Property RotationX As Single
        Get
            Return _RotationX
        End Get
        Set(value As Single)
            _RotationX = value
        End Set
    End Property

    Private _RotationY As Single = 0
    Public Property RotationY As Single
        Get
            Return _RotationY
        End Get
        Set(value As Single)
            _RotationY = value
        End Set
    End Property

    ' Variable to store the last mouse position, used to drag and rotate controls
    Private _LastMousePos As Point
    Public Property LastMousePos As Point
        Get
            Return _LastMousePos
        End Get
        Set(value As Point)
            _LastMousePos = value
        End Set
    End Property

    ' Flag to toggle the visibility of salt bridges in the 3D model
    Private _ShowSaltBridgesForModel As Boolean = True
    Public Property ShowSaltBridgesForModel As Boolean
        Get
            Return _ShowSaltBridgesForModel
        End Get
        Set(value As Boolean)
            _ShowSaltBridgesForModel = value
        End Set
    End Property

    ' Flag to toggle the visibility of disulphide bridges in the 3D model
    Private _ShowDisulphideBridgesForModel As Boolean = True
    Public Property ShowDisulphideBridgesForModel As Boolean
        Get
            Return _ShowDisulphideBridgesForModel
        End Get
        Set(value As Boolean)
            _ShowDisulphideBridgesForModel = value
        End Set
    End Property

    ' A list of rotated coordinates for the model, used for updating the view after applying rotations
    Private _RotatedCoords As List(Of Point3D)
    Public Property RotatedCoords As List(Of Point3D)
        Get
            Return _RotatedCoords
        End Get
        Set(value As List(Of Point3D))
            _RotatedCoords = value
        End Set
    End Property

    ' Lists to store data points representing salt bridges and disulphide bridges for visual representation
    Private _SaltBridgePoints As New List(Of DataPoint)
    Public Property SaltBridgePoints As List(Of DataPoint)
        Get
            Return _SaltBridgePoints
        End Get
        Set(value As List(Of DataPoint))
            _SaltBridgePoints = value
        End Set
    End Property

    Private _DisulphideBridgePoints As New List(Of DataPoint)
    Public Property DisulphideBridgePoints As List(Of DataPoint)
        Get
            Return _DisulphideBridgePoints
        End Get
        Set(value As List(Of DataPoint))
            _DisulphideBridgePoints = value
        End Set
    End Property

    ' A list of 3D points representing the coordinates of the alpha-carbon atoms in the polypeptide
    Private _CarbonAlphaCoords As List(Of Point3D)
    Public Property CarbonAlphaCoords As List(Of Point3D)
        Get
            Return _CarbonAlphaCoords
        End Get
        Set(value As List(Of Point3D))
            _CarbonAlphaCoords = value
        End Set
    End Property


    Public Sub ModelCreation(ModelPanel As Panel)

        ' Convert global 3D coordinate data into a list of Point3D objects for the model
        CarbonAlphaCoords = New List(Of Point3D)() ' Initialize the list to hold 3D coordinates
        For Each coord In GloballyAccessibleCoordinates
            If coord.Count = 3 Then ' Ensure the coordinate is valid (has three dimensions)
                CarbonAlphaCoords.Add(New Point3D(coord(0), coord(1), coord(2))) ' Create a new Point3D and add it to the list
            End If
        Next

        ' Initialize RotatedCoords to match the original CarbonAlphaCoords
        ' This will store the rotated coordinates for rendering after transformations
        RotatedCoords = New List(Of Point3D)(CarbonAlphaCoords)

        ' Add event handlers to enable interactivity for the 3D model in ModelPanel
        AddHandler ModelPanel.MouseDown, AddressOf Panel1_MouseDown ' Triggered when the mouse is clicked
        AddHandler ModelPanel.MouseMove, AddressOf Panel1_MouseMove ' Triggered when the mouse is moved
        AddHandler ModelPanel.Paint, AddressOf Panel1_Paint ' Triggered when the panel needs to be repainted
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs)
        ' Render the 3D model on the panel
        ' Uses the panel's dimensions for scaling and centering
        If IsFormOpen("FullScreenModel") Then
            Draw3DModel(e.Graphics, ModelPanelForFullScreenModel.Width, ModelPanelForFullScreenModel.Height)
        Else
            Draw3DModel(e.Graphics, ModelPanelForPrimaryDisplayForm.Width, ModelPanelForPrimaryDisplayForm.Height)
        End If
    End Sub

    Private Function IsFormOpen(ByVal formName As String)
        ' Functin to check which form is open and will display the correlated modelpanel
        For Each frm As Form In Application.OpenForms
            If frm.Name = formName Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub Draw3DModel(g As Graphics, width As Integer, height As Integer)
        ' Scale factor for converting 3D coordinates to 2D space
        Dim scale As Single = 500.0F
        ' Field of View (FOV) for perspective projection
        Dim fov As Single = 100.0F
        ' Radius for rendering individual points
        Dim radius As Integer = 5

        ' Enable anti-aliasing for smoother rendering
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        ' Loop through each 3D point in the rotated coordinates
        For i As Integer = 0 To RotatedCoords.Count - 1
            Dim p As Point3D = RotatedCoords(i)

            ' Apply perspective projection:
            ' Objects farther away (larger Z) appear smaller due to division by (Z + fov).
            Dim x As Single = (p.X * scale) / (p.Z + fov) + width / 2
            Dim y As Single = (p.Y * scale) / (p.Z + fov) + height / 2

            ' Draw the current point as a circle
            g.FillEllipse(Brushes.Purple, x - radius, y - radius, radius * 2, radius * 2)

            ' Draw a line connecting the current point to the next point
            If i < RotatedCoords.Count - 1 Then
                Dim nextP As Point3D = RotatedCoords(i + 1)

                ' Apply perspective projection for the next point
                Dim x2 As Single = (nextP.X * scale) / (nextP.Z + fov) + width / 2
                Dim y2 As Single = (nextP.Y * scale) / (nextP.Z + fov) + height / 2

                ' Draw a line connecting the two points to form the structure
                g.DrawLine(Pens.White, x, y, x2, y2)
            End If
        Next

        InteractionIdentifierAndPlotter(scale, fov, width, height, g)

    End Sub

    Private Sub InteractionIdentifierAndPlotter(scale As Single, fov As Single, width As Integer, height As Integer, g As Graphics)
        ' Draw special connections based on the distance dictionary
        For Each kvp As KeyValuePair(Of String, Decimal) In GloballyAccessibleDistDictionary
            If kvp.Value < 5 Then ' Only consider distances below 5 units
                ' Parse the connection keys to extract coordinate indices
                Dim parts() As String = kvp.Key.Split("_"c)
                Dim XCo As Integer = Integer.Parse(parts(0).Substring(3))
                Dim YCo As Integer = Integer.Parse(parts(1).Substring(3))

                ' Ensure indices are valid and represent points far enough apart
                If Math.Abs(XCo - YCo) > 5 AndAlso XCo < RotatedCoords.Count AndAlso YCo < RotatedCoords.Count Then
                    ' Retrieve the two points involved in the connection
                    Dim p1 As Point3D = RotatedCoords(XCo)
                    Dim p2 As Point3D = RotatedCoords(YCo)

                    ' Apply perspective projection to both points
                    Dim x1 As Single = (p1.X * scale) / (p1.Z + fov) + width / 2
                    Dim y1 As Single = (p1.Y * scale) / (p1.Z + fov) + height / 2
                    Dim x2 As Single = (p2.X * scale) / (p2.Z + fov) + width / 2
                    Dim y2 As Single = (p2.Y * scale) / (p2.Z + fov) + height / 2

                    ' If the points represent specific interactions, draw connections
                    ' Red lines for "CYS" (cysteine connections)
                    If ShowDisulphideBridgesForModel AndAlso parts(0).Contains("CYS") AndAlso parts(1).Contains("CYS") Then
                        g.DrawLine(Pens.Red, x1, y1, x2, y2)
                        ' Green lines for other special interactions
                    ElseIf ShowSaltBridgesForModel AndAlso IsSaltBridge(parts(0), parts(1)) Then
                        g.DrawLine(Pens.Green, x1, y1, x2, y2)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs)
        ' Capture the current mouse position when the mouse button is pressed
        ' The position is stored in LastMousePos, which is used for tracking drag movements
        LastMousePos = e.Location
    End Sub

    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs)
        ' Check if the left mouse button is held down during the mouse move event
        If e.Button = MouseButtons.Left Then
            ' Calculate the change in mouse position since the last recorded position
            Dim deltaX As Integer = e.X - LastMousePos.X
            Dim deltaY As Integer = e.Y - LastMousePos.Y

            ' Adjust the rotation angles based on the mouse movement
            ' Vertical movement (deltaY) affects RotationX (rotation around the X-axis)
            ' Horizontal movement (deltaX) affects RotationY (rotation around the Y-axis)
            ' The scaling factor (0.5F) smooths the rotation to make it more manageable
            RotationX += deltaY * 0.5F
            RotationY += deltaX * 0.5F

            ' Update the last recorded mouse position
            LastMousePos = e.Location

            ' Apply the updated rotations to the 3D model
            UpdateRotation()

            ' Request the panel to repaint itself to reflect the changes
            ModelPanelForPrimaryDisplayForm.Invalidate()
            ModelPanelForFullScreenModel.Invalidate()
        End If
    End Sub

    Private Sub UpdateRotation()
        ' Iterate through each point in the original CarbonAlphaCoordinates list
        For i As Integer = 0 To CarbonAlphaCoords.Count - 1
            ' Apply a rotation transformation to each point
            ' The rotation angles (RotationX and RotationY) represent rotations around the X and Y axes
            RotatedCoords(i) = RotatePoint(CarbonAlphaCoords(i), RotationX, RotationY)
        Next
    End Sub

    Private Function RotatePoint(p As Point3D, angleX As Single, angleY As Single) As Point3D
        ' Convert the X-axis rotation angle from degrees to radians
        Dim radX As Single = Math.PI * angleX / 180.0F
        ' Convert the Y-axis rotation angle from degrees to radians
        Dim radY As Single = Math.PI * angleY / 180.0F

        ' Precompute the sine and cosine values for the X-axis rotation
        Dim cosX As Single = Math.Cos(radX)
        Dim sinX As Single = Math.Sin(radX)

        ' Precompute the sine and cosine values for the Y-axis rotation
        Dim cosY As Single = Math.Cos(radY)
        Dim sinY As Single = Math.Sin(radY)

        ' Step 1: Rotate around the X-axis
        ' Using the X-axis rotation matrix:
        ' [ 1    0      0   ]
        ' [ 0  cosX  -sinX  ]
        ' [ 0  sinX   cosX  ]
        ' This updates the Y and Z coordinates while keeping X unchanged
        Dim y As Single = p.Y * cosX - p.Z * sinX
        Dim z As Single = p.Y * sinX + p.Z * cosX

        ' Step 2: Rotate around the Y-axis
        ' Using the Y-axis rotation matrix:
        ' [  cosY  0  sinY  ]
        ' [   0    1   0    ]
        ' [ -sinY  0  cosY  ]
        ' This updates the X and Z coordinates while keeping Y unchanged
        Dim x As Single = p.X * cosY + z * sinY
        z = -p.X * sinY + z * cosY

        ' Return the new rotated point with updated X, Y, and Z coordinates
        Return New Point3D(x, y, z)
    End Function

    Public Class Point3D
        ' Public properties to store the X, Y, and Z coordinates of a point in 3D space
        Public X As Single
        Public Y As Single
        Public Z As Single

        ' Constructor to initialize a Point3D instance with given X, Y, and Z values
        Public Sub New(x As Single, y As Single, z As Single)
            Me.X = x   ' Set the X-coordinate
            Me.Y = y   ' Set the Y-coordinate
            Me.Z = z   ' Set the Z-coordinate
        End Sub
    End Class


End Module
