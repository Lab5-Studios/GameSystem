# Unity GameSystem Package

This package provides a way for registering and managing custom game systems within Unity's player loop. It allows you to create modular and organized updates across different phases of the game loop.

## Features

- Define custom game systems that hook into different update phases.
- Use attributes to mark and register custom game systems.
- Automatically integrate custom game systems into Unity's player loop.
- Handles initialization, update, and cleanup of game systems.

## Installation

To install this package, follow these steps:

1. Open your Unity project.
2. Go to `Window > Package Manager`.
3. Click the `+` button and select `Add package from git URL`.
4. Enter the URL of this repository and click `Add`.

## Usage

### Defining a Custom Game System

1. Create a new class and implement one or more of the provided interfaces (`IInitialization`, `IEarlyUpdate`, `IFixedUpdate`, `IPostLateUpdate`, `IPreLateUpdate`, `IPreUpdate`, `ITimeUpdate` or `IUpdate`).
2. Mark the class with the `GameSystemAttribute`.

Example:
```csharp
using UnityEngine;

[GameSystem]
public class MyCustomSystem : IUpdate
{
    public void Update()
    {
        // Custom update logic
    }
}
```

### Custom Update Phases

- `IInitialization`: Initialization phase.
- `IEarlyUpdate`: Early update phase.
- `IFixedUpdate`: Fixed update phase.
- `IPostLateUpdate`: Post-late update phase.
- `IPreLateUpdate`: Pre-late update phase.
- `IPreUpdate`: Pre-update phase.
- `IUpdate`: Regular update phase.

### Integration

The `GameSystemRegister` class handles the registration and integration of custom game systems. It automatically collects, initializes, and adds them to the relevant phases of Unity's player loop.

### Editor Integration

In the Unity Editor, the player loop is reset to the default state upon exiting play mode to ensure a clean state.

## Contributing

If you encounter any issues or have suggestions for improvements, please open an issue or submit a pull request.

## License

This project is licensed under the MIT License - see the [License](License.md) file for details.

## Acknowledgements

This package was inspired by the need for a modular and organized approach to managing game systems in Unity.