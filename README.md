# 🚁 Unity NLP Drone Controller

![Drone Animation](https://img.icons8.com/color/96/000000/drone.png)

These scripts enable voice-controlled drone commands using natural language processing, allowing intuitive control through commands like "take off", "move left 50", or complex compound instructions.

## ✨ Key Features
- 🎙️ Natural voice command recognition
- 📝 Complex command parsing ("take off and move forward")
- ⚙️ Parameter handling (distance, rotation degrees)
- 🤖 Seamless Tello drone integration
- 🎮 VR controller support

## 🧩 Core Scripts

### 1. `AudioRecorder.cs`
**Purpose**: Handles microphone input and speech recognition workflow

#### 🔑 Key Features
- Records audio from microphone on button press
- Converts audio to WAV format
- Triggers speech-to-text processing
- Executes recognized commands

#### 🚀 Usage
- Press VR controller button to start recording

- Speak drone commands:
  - "take off"
  - "move left 50"
  - "rotate 90 degrees"
  - System executes commands after speech ends

### 2. `CommandParser.cs`
Purpose: Interprets text commands and controls the drone

#### 🔑 Key Features
- Maps natural language to drone actions
- Supports compound commands
- Handles distance/rotation parameters
- Interfaces with TelloController

#### 💡 Command Examples
- Command	Action
  - "take off and hover"	Takeoff + maintain position
  - "rotate 90 degrees clockwise"	90° right rotation
