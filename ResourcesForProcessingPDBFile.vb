Imports System.IO
Imports System.Text.RegularExpressions

Public Module ResourcesForProcessingPDBFile

    ' Label to display the name of the polypeptide
    Private _NameOfPolypeptideLabel As New Label()
    Public Property NameOfPolypeptideLabel As Label
        Get
            Return _NameOfPolypeptideLabel
        End Get
        Set(value As Label)
            _NameOfPolypeptideLabel = value
        End Set
    End Property

    ' A globally accessible string variable to hold the filename of the current model
    Private _GloballyAccessibleFileName As String
    Public Property GloballyAccessibleFileName As String
        Get
            Return _GloballyAccessibleFileName
        End Get
        Set(value As String)
            _GloballyAccessibleFileName = value
        End Set
    End Property

    ' A dictionary to store distance data associated with a string key, 
    ' representing Distances between specific coordinates or atoms
    Private _GloballyAccessibleDistDictionary As New Dictionary(Of String, Decimal)
    Public Property GloballyAccessibleDistDictionary As Dictionary(Of String, Decimal)
        Get
            Return _GloballyAccessibleDistDictionary
        End Get
        Set(value As Dictionary(Of String, Decimal))
            _GloballyAccessibleDistDictionary = value
        End Set
    End Property

    ' A dictionary to store coordinates of alpha-carbon atoms, 
    ' with the string key representing an atom or residue identifier and a list of decimals as the coordinates
    Private _GloballyAccessibleCACoordinates As New Dictionary(Of String, List(Of Decimal))
    Public Property GloballyAccessibleCACoordinates As Dictionary(Of String, List(Of Decimal))
        Get
            Return _GloballyAccessibleCACoordinates
        End Get
        Set(value As Dictionary(Of String, List(Of Decimal)))
            _GloballyAccessibleCACoordinates = value
        End Set
    End Property

    ' A list of lists of decimals representing coordinates of all atoms (in the structure)
    Private _GloballyAccessibleCoordinates As New List(Of List(Of Decimal))
    Public Property GloballyAccessibleCoordinates As List(Of List(Of Decimal))
        Get
            Return _GloballyAccessibleCoordinates
        End Get
        Set(value As List(Of List(Of Decimal)))
            _GloballyAccessibleCoordinates = value
        End Set
    End Property

    ' An integer representing the length of the polypeptide chain (the amount of amino acids)
    Private _GloballyAccessibleLengthOfPolypeptide As Integer
    Public Property GloballyAccessibleLengthOfPolypeptide As Integer
        Get
            Return _GloballyAccessibleLengthOfPolypeptide
        End Get
        Set(value As Integer)
            _GloballyAccessibleLengthOfPolypeptide = value
        End Set
    End Property

    ' Label to display the length of the polypeptide
    Private _LengthOfPolypeptideLabel As New Label()
    Public Property LengthOfPolypeptideLabel As Label
        Get
            Return _LengthOfPolypeptideLabel
        End Get
        Set(value As Label)
            _LengthOfPolypeptideLabel = value
        End Set
    End Property

    Public Sub ProteinDataBankFileProcessor(fileName As String)
        ' Define the file path for storing the PDB file history
        Dim FilePath As String = "C:\Users\tjmes\OneDrive\CSCW\PDBFiles\PDBfileHistory.txt" ' Replace with your file path
        Dim PDBFilePath As String
        Dim PDBFileLinesList As New List(Of List(Of String)) ' 2D list to store lines and words from PDB file

        Try
            ' Check if the history file exists
            If System.IO.File.Exists(FilePath) Then
                ' Read the entire content of the file to check if the filename is already present
                Dim fileContent As String = System.IO.File.ReadAllText(FilePath)
                If Not fileContent.Contains(fileName) Then
                    ' Append the filename to the history file if not already present
                    Using writer As New System.IO.StreamWriter(FilePath, True)
                        writer.WriteLine(fileName)
                    End Using
                End If
            Else
                ' Notify the user if the history file does not exist
                MessageBox.Show("The history file does not exist: " & FilePath, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ' Create full path for the PDB file
            PDBFilePath = ("C:\Users\tjmes\OneDrive\CSCW\PDBFiles\" & fileName & ".pdb")

            ' Check if the PDB file exists before attempting to read it
            If Not System.IO.File.Exists(PDBFilePath) Then
                Throw New FileNotFoundException("The PDB file was not found at the specified path: " & PDBFilePath)
            End If

            ' Read all the lines from the PDB file
            Dim lines = System.IO.File.ReadAllLines(PDBFilePath)

            ' Loop through each line and process lines starting with "ATOM"
            For Each line In lines
                If line.StartsWith("ATOM") Then
                    ' Split the line into elements based on whitespace and add to PDBFileLinesList
                    Dim elements = Regex.Split(line, "\s+")
                    Dim row As New List(Of String)(elements)
                    PDBFileLinesList.Add(row)
                End If
            Next

        Catch ex As FileNotFoundException
            ' Handle file not found exception
            MessageBox.Show(ex.Message, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As IOException
            ' Handle other IO exceptions
            MessageBox.Show("An error occurred while reading or writing to the file: " & ex.Message, "IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            ' Handle any other unexpected errors
            MessageBox.Show("An unexpected error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        GloballyAccessibleFileName = fileName

        ' Pass extracted data for further processing
        ProcessAtomData(PDBFileLinesList)

    End Sub

    Private Sub ProcessAtomData(PDBFileLinesList As List(Of List(Of String)))

        Dim CarbonAlphas As New List(Of String) ' Stores the amino acid identifier for each CA atom
        Dim CACombinations As New List(Of String) ' Stores combination of Carbon Alpha residues
        Dim Distances As New List(Of Decimal) ' Stores Distances between amino acids
        Dim Coordinates As New List(Of List(Of Decimal)) ' Stores coordinates of each atom
        Dim CACoordinates As New Dictionary(Of String, List(Of Decimal)) ' Dictionary to store each CA atom's coordinates

        ProcessCoordinates(PDBFileLinesList, CarbonAlphas, Coordinates)
        CarbonAlphaProcessor(CarbonAlphas, Coordinates, CACoordinates)
        CreationOfCarbonAlphaPairs(CarbonAlphas, CACombinations)
        EuclideanDistanceCalculator(Distances, Coordinates)
        MapCarbonAlphaPairsWithDistances(CACombinations, Distances)

    End Sub

    Private Sub ProcessCoordinates(PDBFileLinesList As List(Of List(Of String)), CarbonAlphas As List(Of String), Coordinates As List(Of List(Of Decimal)))

        Dim Index1 As Integer
        Dim Index2 As Decimal
        Dim LengthOfPolypeptide As Integer

        ' Process each line in PDBFileLinesList to identify Carbon Alpha (CA) atoms
        For Each line In PDBFileLinesList
            If line.Contains("CA") Then
                LengthOfPolypeptide += 1
                Dim SingleCoordinates As New List(Of Decimal)

                ' Extract coordinates of the CA atom
                Index1 = 6
                For Index2 = 1 To 3
                    SingleCoordinates.Add(line(Index1)) ' Add the coordinates to the list
                    Index1 += 1
                Next
                Coordinates.Add(SingleCoordinates) ' Add the coordinates to the main list
                CarbonAlphas.Add(line(3) + line(5)) ' Stores identifier for the CA atom (e.g., chain and residue)
            End If
        Next

        GloballyAccessibleLengthOfPolypeptide = LengthOfPolypeptide
        GloballyAccessibleCoordinates = Coordinates

    End Sub

    Private Sub CarbonAlphaProcessor(CarbonAlphas As List(Of String), Coordinates As List(Of List(Of Decimal)), CACoordinates As Dictionary(Of String, List(Of Decimal)))

        For i = 0 To Coordinates.Count - 1
            Dim placeHolderList As New List(Of Decimal)
            For j = 0 To 2
                placeHolderList.Add(Coordinates(i)(j)) ' Add the 3D coordinates for each CA atom
            Next
            CACoordinates.Add(CarbonAlphas(i), placeHolderList) ' Add the atom to the dictionary
        Next
        CACoordinates = CACoordinates

        GloballyAccessibleCACoordinates = CACoordinates

    End Sub

    Private Sub CreationOfCarbonAlphaPairs(CarbonAlphas As List(Of String), CACombos As List(Of String))
        ' Variables for pairing CA atoms and calculating Distances
        Dim TripletDif As New List(Of Decimal)
        Dim CarbonAlphaPairKey As New List(Of String)
        Dim CACombo As String

        ' Create combinations of CA pairs
        For g = 0 To CarbonAlphas.Count - 1
            For i = 0 To CarbonAlphas.Count - 1
                CACombo = CarbonAlphas(g) + "_" + CarbonAlphas(i) ' Pair two CA atoms
                CACombos.Add(CACombo) ' Add the pair to the combo list
            Next
        Next
    End Sub

    Private Sub EuclideanDistanceCalculator(Distances As List(Of Decimal), Coordinates As List(Of List(Of Decimal)))
        ' Calculate Euclidean Distances between CA atoms and store results
        Dim placeHolderDecimal As Decimal
        Dim distance As Decimal
        For i = 0 To Coordinates.Count - 1
            For j = 0 To Coordinates.Count - 1
                ' Calculate the Euclidean distance between two atoms (x1, y1, z1) and (x2, y2, z2)
                Dim x1 = Coordinates(i)(0)
                Dim x2 = Coordinates(i)(1)
                Dim y1 = Coordinates(i)(2)
                Dim y2 = Coordinates(j)(0)
                Dim z1 = Coordinates(j)(1)
                Dim z2 = Coordinates(j)(2)
                placeHolderDecimal = (x1 - x2 + (y1 - y2) + (z1 - z2)) ^ 2 ' Squared difference in each coordinate
                distance = Math.Sqrt(placeHolderDecimal) ' Square root of the Total of squared differences
                Distances.Add(distance) ' Add the calculated distance to the list
            Next
        Next
    End Sub

    Private Sub MapCarbonAlphaPairsWithDistances(CACombos As List(Of String), Distances As List(Of Decimal))
        ' Dictionary to map CA pairs with calculated Distances
        Dim DistanceDictionary As New Dictionary(Of String, Decimal)
        For i = 0 To CACombos.Count - 1
            DistanceDictionary.Add(CACombos(i), Distances(i)) ' Map the pair with its distance
        Next

        GloballyAccessibleDistDictionary = DistanceDictionary

    End Sub

    Public Function IsSaltBridge(a As String, b As String) As Boolean
        ' Define a list of amino acid pairs that represent special interactions
        ' Each pair is defined using Tuple, ensuring bidirectional matching
        Dim pairs = New List(Of Tuple(Of String, String)) From {
        Tuple.Create("ASP", "LYS"), ' Interaction between aspartic acid (ASP) and lysine (LYS)
        Tuple.Create("GLU", "ARG"), ' Interaction between glutamic acid (GLU) and arginine (ARG)
        Tuple.Create("ASP", "ARG"), ' Interaction between aspartic acid (ASP) and arginine (ARG)
        Tuple.Create("GLU", "LYS")  ' Interaction between glutamic acid (GLU) and lysine (LYS)
    }

        ' Check if the input strings (a and b) match any of the defined pairs
        ' Uses the Any function to iterate through the pairs
        ' Ensures the interaction is bidirectional (e.g., "ASP-ARG" is equivalent to "ARG-ASP")
        Return pairs.Any(Function(pair) (a.Contains(pair.Item1) AndAlso b.Contains(pair.Item2)) OrElse
                                     (a.Contains(pair.Item2) AndAlso b.Contains(pair.Item1)))
    End Function

End Module