# LadderApp - Repository Context & Guidelines

## 1. Project Overview

**LadderApp** is an open-source development environment, simulator, and compiler for developing ladder logic programs based on the **IEC 61131-3** standard. It enables users to graphically design ladder diagrams, simulate PLC execution in software, transpile ladder logic into embedded C code for microcontrollers (specifically the Texas Instruments MSP430), flash programs directly to hardware, and reverse-engineer (remount) ladder logic from binaries uploaded back from the microcontroller.

### Key Capabilities
- **Visual Ladder Editor**: Graphical rung-based editor supporting normally open (NO) contacts, normally closed (NC) contacts, output coils, timers (TON, TOF), counters (CTU, CTD), reset coils, and parallel branching.
- **Built-in PLC Simulation**: Interactive runtime simulation with bit toggling, pulse simulation, and accumulated time/counter tracking without hardware.
- **Embedded C Code Generation**: Automatic transpilation of ladder programs into C source files integrated with an MSP430 firmware template (`LadderApp/PLC/`).
- **Hardware Integration**: Toolchain integration with **MSPGCC** to compile generated firmware and **MSPJTAG** for bidirectional flashing via USB.
- **Round-Trip Logic Embedding**: Ability to embed ladder metadata/opcodes inside the compiled binary on the MCU, allowing LadderApp to read back and reconstruct the diagram on the canvas.

---

## 2. Technology Stack & Prerequisites

| Layer | Technology | Details |
| :--- | :--- | :--- |
| **GUI Framework** | Windows Forms (WinForms) | .NET Framework 4.6 (`v4.6`) |
| **Primary Language** | C# (C# 6.0 features) | Uses string interpolation, expression bodies, auto-properties |
| **Embedded Firmware**| C | Microcontroller runtime template for TI MSP430 |
| **Hardware Toolchain**| MSPGCC & MSPJTAG | External compilers and flasher utilities |
| **Project Solution** | `LadderApp.sln` | Visual Studio 2017/2019 Solution format |

### Build Tool Requirements
> [!IMPORTANT]
> Because the codebase utilizes C# 6.0 syntax features, it requires an MSBuild version backed by the Roslyn compiler (from Visual Studio 2017/2019+ or a Developer Command Prompt / PowerShell environment). Do **not** invoke legacy .NET Framework 4.0 MSBuild directly as it lacks Roslyn support and will fail on C# 6 syntax (CS1056 / CS1519).

---

## 3. Build & Run Commands

All commands are PowerShell/CMD-compatible (run within a Developer Command Prompt or with modern MSBuild in `PATH`).

### Build Solution (Debug)
```powershell
msbuild LadderApp.sln -v:m /p:Configuration=Debug
```

### Build Solution (Release)
```powershell
msbuild LadderApp.sln -v:m /p:Configuration=Release
```

### Run Application (Debug)
```powershell
.\LadderApp\Debug\bin\LadderApp.exe
```

### Clean Solution
```powershell
msbuild LadderApp.sln /t:Clean
```

---

## 4. Directory & Codebase Architecture

```
LadderApp/
├── LadderApp.sln                   # Visual Studio Solution file
├── README.md                       # Project overview and roadmap
├── examples/                       # Sample ladder project XML files
│   ├── more-complex-2-leds-3times.xml
│   └── seal-in-circuit.xml
├── images/                         # Project screenshots and demo GIFs
└── LadderApp/                      # Main C# WinForms Project
    ├── LadderApp.csproj            # Project configuration (.NET Framework 4.6)
    ├── Program.cs                  # Application entry point
    │
    ├── Model/                      # Domain Entities & Business Models
    │   ├── Address.cs              # Represents I/O, memory, timer, counter addresses
    │   ├── AddressTypeEnum.cs      # Input, Output, Memory, Timer, Counter address types
    │   ├── Device.cs & Pin.cs      # Microcontroller hardware pin & device models
    │   ├── DeviceFactory.cs        # Factory for configuring target devices
    │   ├── LadderProgram.cs        # In-memory representation of ladder rungs and lines
    │   ├── LadderProject.cs        # Overall project container (program + addressing + device)
    │   ├── Line.cs                 # Representation of a single rung/line in ladder
    │   ├── InstructionList.cs      # Ordered list of instructions on a rung
    │   └── Instructions/           # IEC 61131-3 Instruction model hierarchy
    │       ├── IInstruction.cs     # Base instruction interface
    │       ├── Instruction.cs      # Base abstract instruction implementation
    │       ├── NormallyOpenContact.cs, NormallyClosedContact.cs
    │       ├── OutputCoil.cs, ResetOutputInstruction.cs
    │       ├── TimerInstruction.cs, CounterInstruction.cs
    │       ├── InstructionFactory.cs
    │       └── OperationCode/      # OpCode enum mapping to binary bytecode
    │
    ├── Services/                   # Core Business Logic & Processors
    │   ├── AddressingServices.cs   # Manages and resolves symbol addresses (Singleton/DI)
    │   ├── ExecutableGeneratorServices.cs # Transpiles ladder program into C source
    │   ├── ExecutableReaderService.cs     # Parses MCU binary to rebuild ladder logic
    │   ├── LadderSimulatorServices.cs    # Evaluates rung states, power flow, and timers
    │   ├── LadderVerificationServices.cs # Checks ladder logic syntax and validity
    │   ├── LadderLineServices.cs         # Manipulates line contents and branch insertions
    │   ├── MicIntegrationServices.cs     # Spawns MSPGCC compiler and MSPJTAG flasher
    │   ├── OpCode2TextServices.cs        # Converts opcode binary to text format
    │   └── Text2OpCodeServices.cs        # Converts text format to opcode binary
    │
    ├── VisualComponents/           # Custom Drawing & Diagram Rendering
    │   ├── VisualProgram.cs        # Orchestrates drawing of the entire ladder program
    │   ├── VisualLine.cs           # Draws individual rungs, power rails, and connections
    │   └── VisualParallelBranch.cs # Renders OR-branches and convergence nodes
    │
    ├── UserControls/               # Custom UI Controls
    │   └── VisualInstructionUserControl.cs # Interactive element for instructions on a rung
    │
    ├── Forms/                      # Windows Forms UI
    │   ├── MainWindowForm.cs       # Primary IDE window, toolbars, and MDI container
    │   ├── LadderForm.cs           # Canvas view for editing and simulating ladder rungs
    │   ├── ProjectForm.cs          # Tree view of project components, addressing, and pins
    │   ├── AddressingForm.cs       # Table editor for digital and internal addresses
    │   ├── DeviceForm.cs           # Microcontroller device and pin configuration
    │   └── ChangeTimerCounterParametersForm.cs # Modal to adjust presets and time bases
    │
    ├── PLC/                        # Microcontroller C Template Files (MSP430)
    │   ├── main.c, hardwaresetup.c, interruption.c
    │   ├── ladderprogram.c & ladderprogram.h
    │   ├── functions.c, exectimer.*, execcounter.*
    │   └── definitions.h, addresses.h
    │
    └── Resources/                  # Static assets and embedded template resources
```

---

## 5. Architectural Patterns & Conventions

### Separation of Concerns
- **UI (`Forms/`, `UserControls/`, `VisualComponents/`)**: Handles mouse events, visual selection, and GDI+ rendering. Avoid embedding simulation or compilation logic directly in forms; delegate to `Services/`.
- **Domain (`Model/`)**: Pure state objects representing rungs, instructions, devices, and addresses.
- **Application Logic (`Services/`)**: Stateful operations (simulation, transpilation, parsing).

### Addressing System
- Addresses map to physical MCU pins (inputs/outputs) or internal PLC flags (internal relays, timers, counters).
- `AddressTypeEnum` values:
  - `DigitalInput` (physical input pins)
  - `DigitalOutput` (physical output pins)
  - `DigitalMemory` (internal relays / auxiliary flags)
  - `DigitalMemoryTimer` (timer coils and status bits)
  - `DigitalMemoryCounter` (counter coils and status bits)

### Compilation & Flash Pipeline
1. **User triggers download**: `MainWindowForm` -> `MicIntegrationServices`.
2. **C Generation**: `ExecutableGeneratorServices` reads `LadderProgram`, iterates rungs, and outputs `ladderprogram.c` containing the scan-cycle logic.
3. **Opcode Embedding**: If enabled, `Text2OpCodeServices` embeds serialized diagram bytecode in a designated binary segment.
4. **Compilation**: `MicIntegrationServices` runs `msp430-gcc` against `LadderApp/PLC/` sources.
5. **Flasher**: Binary hex is transferred to MCU using `msp430-jtag`.

### Simulator Engine
- `LadderSimulatorServices` runs evaluation passes matching standard PLC scan cycles:
  1. Read Inputs (mocked via user clicks on GUI)
  2. Execute Ladder Logic (evaluates power flow from left rail to right rail)
  3. Update Timers & Counters (tracks elapsed time and pulse edges)
  4. Write Outputs (updates visual coil energization states)

---

## 6. Coding & Contribution Rules

1. **Language & Naming**:
   - Write all new code, comments, documentation, and commit messages in **English** (legacy comments in Portuguese are being gradually modernized).
   - Use standard C# PascalCase for classes, methods, and properties; camelCase for local variables and private fields.
2. **Backwards Compatibility**:
   - Do not alter existing `OperationCode` enum integer values or XML schema tags in `examples/`, as this breaks compatibility with stored files and firmware reverse-engineering.
3. **Refactoring Guidelines**:
   - When modifying `Services`, favor dependency injection and unit testability over tightly coupled singletons.
   - Guard against `NullReferenceException` when traversing `Line.Instructions` and parallel branch structures.
4. **Preserving Visual Components**:
   - `VisualLine.cs` and `VisualInstructionUserControl.cs` manage custom GDI+ drawing coordinates and hit-testing. Ensure visual padding and bounding box calculations remain consistent across DPI settings.
