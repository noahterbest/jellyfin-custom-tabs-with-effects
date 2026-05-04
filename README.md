<h1 align="center">Custom Tabs — Glow Edition</h1>
<h2 align="center">A Jellyfin Plugin Fork</h2>
<p align="center">
    <img alt="Logo" src="https://raw.githubusercontent.com/noahterbest/jellyfin-custom-tabs-with-effects/main/src/logo.jpg" />
    <br />
 	<br />
 	<a href="https://github.com/noahterbest/jellyfin-custom-tabs-with-effects">
 		<img alt="GPL 3.0 License" src="https://img.shields.io/github/license/noahterbest/jellyfin-custom-tabs-with-effects.svg" />
 	</a>
</p>

## About

This is a fork of [IAmParadox27/jellyfin-plugin-custom-tabs](https://github.com/IAmParadox27/jellyfin-plugin-custom-tabs) that adds fun visual effects to custom tab buttons — glow, heartbeat pulse, and RGB cycling animations. The original plugin lets you add custom tabs to the top navigation bar of the Jellyfin web client.

## Prerequisites

- Jellyfin Version `10.11.2`
- [File Transformation](https://github.com/IAmParadox27/jellyfin-plugin-file-transformation) plugin at least v2.2.1.0

## Installation

1. Download the latest `.zip` release from the [Releases](https://github.com/noahterbest/jellyfin-custom-tabs-with-effects/releases) page.
2. In Jellyfin admin dashboard, go to **Plugins** → **Catalogue** → **Add repository** and point it to the zip file.
3. Install the plugin and restart Jellyfin.
4. Navigate to **My Plugins** → **Custom Tabs** to configure your tabs and enable visual effects.
5. Save and force refresh your browser.

## Visual Effects

Each tab button can have the following effects, all configurable from the plugin settings:

- **Glow** — A static multi-layered box-shadow. Configurable color and intensity (0–50px).
- **Heartbeat** — Pulses the glow by scaling blur radii up to 1.5×. Requires Glow. Configurable speed (200–3000ms).
- **RGB Cycle** — Smooth rainbow color cycling through the full hue spectrum. Requires Glow. Configurable speed (200–3000ms).

All effects are disabled by default and fully backward compatible.

## Issues

Open an issue on GitHub if you find a bug. Check existing issues first before creating a new one.

## AI Generation Disclosure

This project contains modifications generated with the assistance of artificial intelligence tools. AI was used to help implement and refine the visual effects features (glow, heartbeat animation, and RGB cycling) described above. All code has been reviewed and tested before being committed.
