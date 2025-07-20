# 🚈 Gesture-Controlled Drone Simulator

A real-time Unity-powered drone simulator controlled via hand gestures using Python, MediaPipe, and OpenCV.

---

## 📌 Project Overview

This project bridges **Python-based gesture recognition** with a **Unity-based 3D drone simulation**. Users can control a drone using **hand gestures** detected through a webcam, powered by **MediaPipe's hand tracking**.

Python handles gesture tracking and sends data to Unity using **sockets**. Unity reads this data in real-time to control the drone's movement in a simulated environment.

---

## ✨ Features

* 🎮 Real-time drone control using hand gestures
* 🖐️ Hand tracking using MediaPipe
* 🔄 Seamless integration between Python and Unity via TCP sockets
* 🧠 Support for both gesture and keyboard control
* 🕹️ Smooth, physics-based drone movement
* 🔁 Auto-launch of Python script from Unity using `.bat` file

---

## 🔧 Technologies Used

### 🐍 Python

* **Python 3.9.13** (only this version recommended for MediaPipe compatibility)
* [MediaPipe](https://google.github.io/mediapipe/) (v0.10.21)
* [OpenCV](https://opencv.org/) (`cv2`)
* `socket`, `math`, `threading`, `json` (standard libraries)

### 🎮 Unity

* **Unity Editor 2022.3 LTS** (recommended)
* Unity C# scripting for movement and socket communication
* `.bat` file to auto-run Python gesture script

---

## 🖥️ System Requirements

* **Operating System**: Windows 10/11
* **Python**: 3.9.13
* **Unity**: Unity Hub + Unity Editor 2022.3 LTS
* **Webcam**: Functional webcam required
* **Git** (for cloning project)

---

## ⚙️ Installation

### 1. Clone the Repository

```bash
      git clone https://github.com/your-username/Gesture-control-drone-simulator.git
      cd Gesture-control-drone-simulator
```

### 2. Set Up Python Environment

```bash
      # Create virtual environment
      python -m venv mediapipe_env

      # Activate it (Windows)
      mediapipe_env\Scripts\activate

      # Install dependencies
      pip install -r requirements.txt
```

`requirements.txt` should contain:

```txt
      mediapipe==0.10.21
      opencv-python
```

### 3. Open Unity Project

* Open **Unity Hub** → Click **"Open"**
* Navigate to this cloned folder
* Wait for Unity to index and compile the project

## ▶️ How to Run the Project

1. Press **Play** in Unity Editor to start the simulation
2. Unity executes a `.bat` file that starts `main.py` (gesture tracker)

### 🔄 Python Side

* Accesses your webcam
* Detects hand landmarks via MediaPipe
* Sends real-time gesture data to Unity over socket

### 🎮 Unity Side

* Receives this data
* Updates the drone's motion accordingly

## 🎮 Controls

| Action            | Gesture / Input             |
| ----------------- | --------------------------- |
| Move Forward      | Right Index Finger Extended |
| Move Backward     | Left Index Finger Extended  |
| Turn Left / Right | Move Left Thumb (X-axis)    |
| Ascend / Descend  | Move Right Thumb (Y-axis)   |
| Roll              | Keyboard A / D              |
| Keyboard Movement | W / S / Space / Left Ctrl   |

## 🧠 How It Works

### 🎯 Gesture Detection

* MediaPipe tracks both hands with 21 landmarks
* Uses thumb and index finger position for gesture interpretation

### 🌐 Data Communication

* Python acts as a TCP server (`localhost:5000`)
* Unity connects to this server and reads gesture JSON frames
* Unity C# parses input and applies movement to drone using physics

### ⚙️ Automation

* `.bat` file is used to start Python script automatically when Unity scene starts

## 🛠️ Troubleshooting & FAQ

### ❓ Python script doesn't start

* Ensure `.bat` file path is correct and points to `main.py`
* Test `main.py` by running manually:

```bash
    python main.py
```

### ❓ Unity fails to connect to Python

* Make sure `main.py` is running
* Check if port `5000` is already in use
* Avoid running multiple instances of Unity or Python together

### ❓ Gesture input not detected

* Ensure webcam is accessible
* Check lighting conditions
* Verify correct Python version (`3.9.13`)

---

## 📂 Project Structure

```bash
  Gesture-control-drone-simulator/
  ├── Assets/
  │   ├── External/              # main.py and .bat file
  │   ├── Scripts/               # Unity C# scripts
  │   └── Scenes/
  ├── mediapipe_env/             # Python venv
  ├── requirements.txt
  ├── README.md
  └── .gitignore
```

## 📸 Screenshots / Demo

  <img width="480" height="300" alt="Drone Simulator with Hand Control" src="https://github.com/user-attachments/assets/97209a75-d436-4752-b4dc-1b2e5720a9e7" />
  <img width="480" height="300" alt="Hand Gesture Tracking" src="https://github.com/user-attachments/assets/d5f61839-3dbf-4841-8cbd-6666472566a7" />

**Assets used in the Unity project:**

* [Japanese Otaku City](https://assetstore.unity.com/packages/3d/environments/urban/japanese-otaku-city-20359) — for the city environment
* [Simple Drone](https://assetstore.unity.com/packages/3d/vehicles/air/simple-drone-190684) — for the drone model

## 🤝 Contributing

Contributions are welcome! You can:

* Submit bug reports
* Improve gesture detection accuracy
* Enhance UI/UX in Unity

---

## 📬 Contact

**Dhanesh Vaghasiya**
GitHub: [@dhanesh-vaghasiya](https://github.com/dhanesh-vaghasiya)
Email: [dhaneshvaghasiya999@gmail.com](mailto:dhaneshvaghasiya999@gmail.com)
