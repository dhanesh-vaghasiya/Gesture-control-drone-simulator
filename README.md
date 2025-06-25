# 🛸 Gesture-Controlled Drone Simulator

A Unity-based 3D drone simulator controlled via real-time hand gestures using a webcam. Built with Unity and powered by Python's MediaPipe for gesture tracking.

---

## 📸 Features

- Control the drone using **hand gestures**
- Option to switch to **keyboard mode**
- Realistic drone physics and camera movement
- Python server streams hand tracking data to Unity via socket
- Modular and clean architecture

---

## 🧰 Requirements

### 🧠 Unity
- **Unity Version:** `2021.3 LTS` or above (tested on 2021.3.30f1)

> ⚠️ Make sure you install Unity with **.NET support** and **Windows build support**.

### 🐍 Python (for Gesture Control)

You need **Python installed locally** to run the hand-tracking module.

- **Python Version:** `3.9.x` recommended (tested on 3.9.13)
- Do **NOT** use Python 3.11+ with MediaPipe—it may cause runtime errors.

#### 📦 Python Dependencies

Install these packages in a virtual environment or globally:

```bash
pip install opencv-python mediapipe
You can also use requirements.txt if included:

bash
Copy
Edit
pip install -r requirements.txt
🗂 Project Structure
bash
Copy
Edit
Gesture-control-drone-simulator/
├── Assets/
│   └── External/
│       └── Python/
│           └── main.py         # Python hand tracking server
├── Scripts/
│   └── DroneController.cs      # Unity drone logic
├── Scenes/
│   └── Main.unity              # Main scene with drone
├── README.md
└── ...
🚀 How to Run
🖥️ Gesture Control Mode (Hand Tracking)
Open a terminal and navigate to the Python script location:

bash
Copy
Edit
cd Assets/External/Python
python main.py
Now run the game from Unity:

Press Play in Unity Editor

OR build the game and launch the .exe

In the UI, select "Hand Mode" or toggle it in settings.

✅ Your drone is now controlled via thumb and index finger gestures!

🎮 Keyboard Control Mode
W/S – Move forward/backward

A/D – Roll left/right

Mouse – Look around

Space/Ctrl – Ascend/descend

You can switch control mode in the UI before the simulation starts.

📦 Build Instructions (Windows)
In Unity, go to File > Build Settings

Select Target Platform: Windows

Click Build

After building, make sure to:

Copy the Python/main.py script to the correct relative path

Ensure Python is installed on the target machine

🌐 Optional: Run Python Automatically (via .bat)
If you want to auto-launch the Python server with the game, create a .bat file:

bat
Copy
Edit
@echo off
start python "Assets/External/Python/main.py"
start "" "YourGameBuild.exe"
❗ Notes
The game won't run in gesture mode without Python & webcam access.

Python script uses socket at port 5000, ensure it's not blocked.

Do not run multiple instances; this can cause WinError 10048.

👤 Author
Made with ♥ by Dhanesh Vaghasiya

