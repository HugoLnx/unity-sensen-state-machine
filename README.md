[![npm package](https://img.shields.io/npm/v/com.sensengames.unity-sensen-state-machine)](https://www.npmjs.com/package/com.sensengames.unity-sensen-state-machine)
[![openupm](https://img.shields.io/npm/v/com.sensengames.unity-sensen-state-machine?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.sensengames.unity-sensen-state-machine/)
![Tests](https://github.com/sensengames/unity-sensen-state-machine/workflows/Tests/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

# Sensen State Machine

Solution for creating state machine with minimum integration with Visual Scripting package.

- [How to use](#how-to-use)
- [Install](#install)
  - [via npm](#via-npm)
  - [via OpenUPM](#via-openupm)
  - [via Git URL](#via-git-url)
  - [Tests](#tests)
- [Configuration](#configuration)

<!-- toc -->

## How to use

*Work In Progress*

## Install

### via npm

Open `Packages/manifest.json` with your favorite text editor. Add a [scoped registry](https://docs.unity3d.com/Manual/upm-scoped.html) and following line to dependencies block:
```json
{
  "scopedRegistries": [
    {
      "name": "npmjs",
      "url": "https://registry.npmjs.org/",
      "scopes": [
        "com.sensengames"
      ]
    }
  ],
  "dependencies": {
    "com.sensengames.unity-sensen-state-machine": "1.0.0"
  }
}
```
Package should now appear in package manager.

### via OpenUPM

The package is also available on the [openupm registry](https://openupm.com/packages/com.sensengames.unity-sensen-state-machine). You can install it eg. via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.sensengames.unity-sensen-state-machine
```

### via Git URL

Open `Packages/manifest.json` with your favorite text editor. Add following line to the dependencies block:
```json
{
  "dependencies": {
    "com.sensengames.unity-sensen-state-machine": "https://github.com/sensengames/unity-sensen-state-machine.git"
  }
}
```

### Tests

The package can optionally be set as *testable*.
In practice this means that tests in the package will be visible in the [Unity Test Runner](https://docs.unity3d.com/2017.4/Documentation/Manual/testing-editortestsrunner.html).

Open `Packages/manifest.json` with your favorite text editor. Add following line **after** the dependencies block:
```json
{
  "dependencies": {
  },
  "testables": [ "com.sensengames.unity-sensen-state-machine" ]
}
```

## Configuration

*Work In Progress*

## License

MIT License

Copyright © 2023 Sensen Games
