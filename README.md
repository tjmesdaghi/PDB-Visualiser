
# Protein Folding & Polypeptide Visualisation Application

A Visual Basic 2022 application for generating interactive 3D models and contact maps of polypeptides using PDB files.

---

## Project Overview

This application allows users to input a Protein Data Bank (.pdb) file and generate:

- An interactive 3D model of the polypeptide
- A contact map visualisation
- Highlighted disulphide bridges
- Highlighted salt bridges
- A history system for previously loaded PDB files
- Full screen visualisation modes

---

## Primary Display Interface

![PDF]PDF.png

Main interface features:

- Input PDB File
- Generate 3D Model
- Generate Contact Map
- Toggle Disulphide Bridges
- Toggle Salt Bridges
- Full Screen 3D Model
- Full Screen Contact Map
- PDB File History

---

## 3D Polypeptide Model

[INSERT IMAGE HERE – Screenshot of 3D model (no interactions enabled)]

The 3D model is generated using:

- Perspective projection
- X-axis and Y-axis rotation matrices
- Euclidean distance calculations
- Interactive mouse-driven rotation

### Interactive Rotation

- Click + drag mouse to rotate
- Vertical movement → Rotation around X-axis
- Horizontal movement → Rotation around Y-axis

[INSERT IMAGE HERE – Screenshot of rotated model]

---

## Disulphide Bridges

Detected when:

CYS – CYS  
Distance < 5Å

Rendered as:
- Red lines in 3D model
- Red points in contact map

[INSERT IMAGE HERE – Screenshot highlighting disulphide bridges]

---

## Salt Bridges

Detected using residue pairing logic:

- ASP ↔ LYS
- GLU ↔ ARG
- ASP ↔ ARG
- GLU ↔ LYS

Rendered as:
- Green lines in 3D model
- Green points in contact map

[INSERT IMAGE HERE – Screenshot highlighting salt bridges]

---

## Contact Map

[INSERT IMAGE HERE – Screenshot of contact map]

The contact map:

- Plots amino acid number vs amino acid number
- Displays interactions under 5Å
- Colour-coded interactions:
  - Red: Disulphide bridges
  - Green: Salt bridges
  - Black/Blue: Other contacts

---

## Full Screen Modes

### Full Screen 3D Model

[INSERT IMAGE HERE – Screenshot of FullScreenModel form]

- Maximised viewing area
- Rotation capability
- Toggle interaction buttons

### Full Screen Contact Map

[INSERT IMAGE HERE – Screenshot of FullScreenContactMap form]

- Enlarged contact map
- Toggle interaction buttons

---

## PDB File History System

[INSERT IMAGE HERE – Screenshot of PDB File History window]

- Stores previously input PDB filenames
- Saves them in PDBFileHistory.txt
- Allows reloading with one click
- Prevents duplicate entries

---

## Technical Implementation

### PDB File Processing

The program:

1. Extracts lines beginning with "ATOM"
2. Filters "CA" (Carbon Alpha) residues
3. Extracts coordinates (x, y, z)
4. Stores residues in structured lists & dictionaries
5. Calculates all pairwise Euclidean distances

Distance formula:

Distance = √((x₂ - x₁)² + (y₂ - y₁)² + (z₂ - z₁)²)

---

## Built With

- Visual Basic 2022
- Windows Forms 2D Graphics Engine
- .NET Chart Controls
- Regex for PDB parsing

---

## Future Improvements

- Highlight enzyme active sites
- Display alpha helices and beta sheets
- Predict possible substrates
- Visualise post-Golgi chain cleavage (e.g., insulin)

---

## Author

Tristan Mesdaghi  
Sir John Deane’s Sixth Form College  
A-Level Computer Science Coursework
