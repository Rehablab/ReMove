# X-Controller Documentation

## Overview

A self-design application in order to create different dynamic situations and collect kinematic data of upper extremity on the commercial upper limb rehabilitation robot.

## About

Having a software to create the ideal dynamic environment is essential for further studies in biomechanics, motor control, and clinical research.

## Usage

Only windows10 operating systems are now supported.

### Environment Required

- [Unity Hub](https://unity.com/unity-hub)
- [Unity LTS Release 2019+](https://unity3d.com/unity/qa/lts-releases)
- [.NET Framework 4.7.1](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net471)

### CSV Configure

#### Noun interpretation

- `StartPoint` The starting coordinates during the motion of a two-dimensional plane.
- `EndPoint` The termination coordinates during the motion of the two-dimensional plane.
- `ForceControlMode` Simulated torque based on mass coefficient and damping coefficient.

#### Example

```bash
# example.csv
start_x,start_y,end_x,end_y,mass_x,mass_y,damping_x,damping_y
0.32,0.001,0.32,0.267,5,10,30,60
0.32,0.001,0.001,0.267,10,20,60,120
0.32,0.001,0.26,0.2,5,10,30,60
```

### RUN

A peer-to-peer motion experiment simulated with a mouse.When the motion reaches the `EndPoint`, a **1.2**-second countdown timer appears and the countdown begins.

![Trialing Picture](/Usage/Trialing_1.jpg "Trialing Picture")

## Test

1. Click on unity's menu bar in turn. `Windows > Generl > Test Runner`
2. Click the `Run All` button to start the global test

![Test X-Controller](/Usage/UnitTest_1.jpg "Test X-Controller")

## Upgrades and New Features

- The experimental trajectory can be designed independently.
- Experimenters can choose the path and script of the lab script.

## License

[MIT](./LICENSE)
