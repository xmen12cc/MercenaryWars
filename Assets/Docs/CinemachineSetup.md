# Cinemachine Setup

## 1. Add Virtual Camera
- Go Unity top menu `GameObject` -> `Cinemachine` -> `Virtual Camera`
- Name it `PlayerCam`
- Drag player object to `Follow` slot in Inspector

## 2. Dead Zone & Damping
- In `PlayerCam` Inspector, open `Body` section
- Set `Dead Zone Width` to 0.1
- Set `Dead Zone Height` to 0.1
- Set `X Damping` to 1.5
- Set `Y Damping` to 1.5
- Camera now follow smooth

## 3. Camera Shake Impulse
- Add `Cinemachine Impulse Listener` extension to `PlayerCam`
- On player object, add `Cinemachine Impulse Source` component
- Set `Raw Signal` to Noise Settings profile
- In player damage script, call `GetComponent<CinemachineImpulseSource>().GenerateImpulse()` when hit
