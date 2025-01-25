# BrickBreaker

## Important Note
This repository contains only the original source code I've written & some free packages. The required Sounds Effects & Background Images are not included due to license restrictions. 

You can play the complete game with my own SFXs & Background [here](https://play.unity.com/en/games/da648cc4-0b29-486f-920c-1dab22288477/brick-breaker)

## Technical Implementation

### Design Patterns
- **Singleton Pattern**: Global access to managers (Audio, Game, Data)
- **Factory Pattern**: Brick creation and variations
- **Observer Pattern**: Event-driven communication between components


### State Management
- Game state system handling idle, gameplay, level transitions, and game over states
- Event-driven state changes with observer subscription

### Audio System
- Multi-layer sound management for background music and effects
- Dynamic pitch variation for sound diversity
- Playlist system with random track selection
- State-aware audio transitions

## Technical Architecture

### Core Systems
- **AudioManager**: Handles all game audio with multiple audio sources
- **GameManager**: Controls game state and flow
- **DataManager**: Manages data persistence and highscores
- **LevelManager**: Controls level progression and brick layouts
- **InputHandler**: Manages player input across game states

### Debug Features
- Comprehensive debug logging system
- Visual debugging tools for brick placement
- Desktop log export functionality

## Dependencies

### External Assets
1. **SharpUI**
   - Used a UI sprite for menu and dialogue interfaces
2. **Cartoon FX Remaster Free**
   - Particle VFX for brick explosions
3. **TextMeshPro**
   - Advanced text rendering and styling
   - Used for dialogue system and UI elements

## Code Structure

### Key Components
- **Brick System**: Abstract base class with specialized implementations
- **Ball Physics**: Custom physics calculations for gameplay feel
- **Dialogue System**: Terminal-style narrative introduction
- **Score System**: Real-time score tracking with persistence

### Design Principles
- SOLID principles adherence
- Clean code practices with comprehensive documentation
- Modular component design for maintainability
- Event-driven architecture for loose coupling

## Setup Instructions
1. Clone repository
2. Install required Unity version
3. Open project in Unity Editor

## Development Environment
- Unity 2022.3 LTS
- C# scripting
- Visual Studio 2022/VS Code for editing
