
# 🏗️ BIMVR: Immersive BIM Visualization in VR

BIMVR is an interactive Unity-based virtual reality system for visualizing Building Information Modeling (BIM) models. It supports real-time navigation, layer control, and dual support for HTC Vive and multi-screen CAVE systems.

## 📽️ Demo

[▶️ Watch Demo Video](./assets/demo.mp4)

> The video file is stored locally in the `assets/` folder. Click the link above to watch the walkthrough.

## ✨ Features

- 🧱 Multi-layer building model viewer
- 🎮 VR navigation (teleport, fly, and gravity-based walk)
- 🔍 Toggle and isolate specific BIM layers
- 🛠️ Automatic collider assignment for building elements
- 🌐 Hybrid deployment: HTC Vive & CAVE visualization
- 🔄 Remote multi-user support *(in development)*

## 🚀 Getting Started

### Requirements

- Unity 2019.4 or later
- Windows 10/11
- HTC Vive or SteamVR-compatible headset
- BIM model exported from Revit (.fbx recommended)

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/BIMVR.git
   ```

2. Open the project in Unity.

3. Import your building model into `Assets/BIMModels/`.

4. Open the appropriate scene (`Scenes/VRScene.unity` or `Scenes/CAVEScene.unity`).

5. Press Play to start the experience.

## 🎮 Controls (HTC Vive)

| Action           | Control Input             |
|------------------|----------------------------|
| Move             | Touchpad up/down           |
| Toggle gravity   | Grip button                |
| Interact         | Trigger                    |
| Open UI          | Menu button                |

## 📁 Project Structure

```
Assets/
├── BIMModels/        # FBX/OBJ files
├── Scripts/          # Custom C# scripts
├── Resources/        # UI elements, prefabs
├── Scenes/           # Unity scenes
├── assets/           # Local video file (demo.mp4)
```

## 📄 License

MIT License. See `LICENSE` file for details.

## 👥 Acknowledgments

- Institute of Construction Informatics, TU Dresden
- VR CAVE Lab team
- Open-source BIM and XR contributors

---

*For academic collaboration or inquiries, contact [hubuzhou@gmail.com](mailto:your.email@domain.com).*
