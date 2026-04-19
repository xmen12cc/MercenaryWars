# Tilemap Guide

## Layers Needed
- **Background:** Far walls, no collide
- **Platforms:** Ground, walls, player walk here
- **Hazards:** Spikes, acid, hurt player
- **Foreground:** Pipes, wires in front of player

## Level Pacing
- **Start:** Safe zone, teach jump
- **Middle:** Small gap, 1 weak enemy
- **Pickups:** Put health near hard jump
- **End:** Big enemy, exit door

## Colliders
- Add `Tilemap Collider 2D` to Platform and Hazard layers
- Add `Composite Collider 2D` (auto adds `Rigidbody 2D`)
- On Rigidbody 2D, set `Body Type` to `Static`
- On Tilemap Collider 2D, check `Used By Composite`
- This make one big smooth wall, player no get stuck on tile cracks
