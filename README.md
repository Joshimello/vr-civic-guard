# VR-CivicGuard

**VR-CivicGuard: Enhancing Campus Civil Defense and Air Defense Preparedness through Immersive Virtual Reality Training**

Published at **IEEE VR 2026**

> Hung-En Hsieh, Jeng Wen Joshua Lean, Jin-Wei Chang, Yu-Ning Chen, Pin-Yue Wang, Tsun-Hung Tsai, Min-Chun Hu
> National Tsing Hua University, Taiwan

---

## Overview

VR-CivicGuard is an immersive VR training system for civilian air raid preparedness. Unlike traditional passive media (videos, brochures), it builds procedural memory through embodied interaction — training users in expert-informed standard operating procedures (SOPs) for surviving air raids.

## Evaluation Results

A between-subjects user study (n=26) demonstrated:

| Metric                       | Result                                        |
| ---------------------------- | --------------------------------------------- |
| System Usability Scale (SUS) | **85.19** ("Excellent")                       |
| Spatial Presence (IPQ)       | **4.12 / 5**                                  |
| VR Group knowledge score     | **87.95%** vs. Control **66.41%** (p < 0.001) |

## Technical Stack

| Component            | Technology                                                      |
| -------------------- | --------------------------------------------------------------- |
| Engine               | Unity 6 (URP), Unity 6000.2.2f1                                 |
| Target Platform      | Meta Quest 3 (OpenXR)                                           |
| XR SDK               | XR Interaction Toolkit 3.2.1, Meta XR Core SDK                  |
| Scene Reconstruction | 3D Gaussian Splatting → Blender → Unity                         |
| AI Navigation        | Unity NavMesh + FSM (cat companion)                             |
| Rendering            | Baked lightmaps (static) + real-time lighting (dynamic hazards) |
| Destruction          | OpenFracture (dynamic mesh shattering)                          |

## Getting Started

**Requirements:**

- Unity 6000.2.2f1
- Meta Quest 3 (or compatible OpenXR headset)
- Android Build Support module installed in Unity Hub

**Setup:**

1. Clone this repository
2. Open the project in Unity 6000.2.2f1
3. Open `Assets/Scenes/Classroom01.unity` as the main scene
4. Build for Android via _File → Build Settings_ with the Android platform selected

## Citation

If you use this work, please cite:

```bibtex
@inproceedings{hsieh2026vrcivicguard,
  title     = {VR-CivicGuard: Enhancing Campus Civil Defense and Air Defense Preparedness through Immersive Virtual Reality Training},
  author    = {Hsieh, Hung-En and Lean, Jeng Wen Joshua and Chang, Jin-Wei and Chen, Yu-Ning and Wang, Pin-Yue and Tsai, Tsun-Hung and Hu, Min-Chun},
  booktitle = {IEEE Conference on Virtual Reality and 3D User Interfaces (IEEE VR)},
  year      = {2026}
}
```

## Acknowledgements

This research was supported by the National Science and Technology Council, Taiwan.
