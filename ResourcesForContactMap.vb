Imports System.Windows.Forms.DataVisualization.Charting

Module ResourcesForContactMap

    ' Flags to toggle the visibility of salt and disulphide bridges in the contact map view
    Private _ShowSaltBridgesForContactMap As Boolean = True
    Public Property ShowSaltBridgesForContactMap As Boolean
        Get
            Return _ShowSaltBridgesForContactMap
        End Get
        Set(value As Boolean)
            _ShowSaltBridgesForContactMap = value
        End Set
    End Property

    Private _ShowDisulphideBridgesForContactMap As Boolean = True
    Public Property ShowDisulphideBridgesForContactMap As Boolean
        Get
            Return _ShowDisulphideBridgesForContactMap
        End Get
        Set(value As Boolean)
            _ShowDisulphideBridgesForContactMap = value
        End Set
    End Property

    Public Sub ContactMapCreation(ContactMapChart As Chart)
        ' Create a new series for the contact map points
        Dim series As New Series("Contact")
        series.ChartType = SeriesChartType.Point ' Points will be plotted in the chart
        ContactMapChart.Series.Add(series)

        ChartPlotter(series)
        ChartConfiguration(ContactMapChart)
    End Sub

    Private Sub ChartPlotter(series As Series)
        ' Loop through the globally accessible distance dictionary to add points
        For Each kvp In GloballyAccessibleDistDictionary
            ' Only consider entries where the distance is less than 5
            If kvp.Value < 5 Then
                ' Split the key into X and Y coordinates
                Dim parts = kvp.Key.Split("_"c)
                Dim XCoString = parts(0)
                Dim YCoString = parts(1)
                Dim AminoAcidNumberX = parts(0).Substring(3)
                Dim AminoAcidNumberY = parts(1).Substring(3)
                Dim XCo = Integer.Parse(AminoAcidNumberX)
                Dim YCo = Integer.Parse(AminoAcidNumberY)

                If Math.Abs(XCo - YCo) > 5 Then
                    Dim DataPoint As New DataPoint(XCo, YCo)

                    ' Check for disulphide bridges (CYS-CYS) and salt bridges
                    If XCoString.Contains("CYS") AndAlso YCoString.Contains("CYS") Then
                        DataPoint.Color = Color.Red
                        DataPoint.MarkerSize = 3
                        DisulphideBridgePoints.Add(DataPoint)
                    ElseIf (IsSaltBridge(XCoString, YCoString)) Then
                        DataPoint.Color = Color.Green
                        DataPoint.MarkerSize = 3
                        SaltBridgePoints.Add(DataPoint)
                    Else
                        DataPoint.Color = Color.Blue
                        DataPoint.MarkerSize = 1
                    End If

                    series.Points.Add(DataPoint)
                End If
            End If
        Next
    End Sub

    Private Sub ChartConfiguration(ContactMapChart As Chart)
        ' Configure the chart area for the contact map
        Dim chartArea As New ChartArea
        ContactMapChart.ChartAreas.Clear()
        ContactMapChart.ChartAreas.Add(chartArea)

        ' Set axis titles
        chartArea.AxisX.Title = "X Axis"
        chartArea.AxisY.Title = "Y Axis"

        ' Set grid line colors
        chartArea.AxisX.MajorGrid.LineColor = Color.LightGray
        chartArea.AxisY.MajorGrid.LineColor = Color.LightGray

        ' Set axis ranges and intervals
        chartArea.AxisX.Minimum = 0
        chartArea.AxisX.Maximum = GloballyAccessibleCACoordinates.Count
        chartArea.AxisY.Minimum = 0
        chartArea.AxisY.Maximum = GloballyAccessibleCACoordinates.Count
        chartArea.AxisX.Interval = 10
        chartArea.AxisY.Interval = 10

        ' Invalidate the chart to force it to refresh and display the updated points
        ContactMapChart.Invalidate()
    End Sub


End Module
