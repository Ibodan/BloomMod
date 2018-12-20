# BloomMod
BeatSaber bloom effect modification plugin

Customize internal bloom effect parameters of each cameras in-game.
It is mainly for applying proper or enhanced bloom on CameraPlus's 3rd person camera and MultiView's extra camera.
But you can also modify bloom effect of your HMD view.

### Install

put dll into Plugins directory

### Intialize config file with default value

First, start BeatSaber and **play any map for seconds** and quit.

### Modify config file

Open `UserData/modprefs.ini`. There you can see some `[BloomMod!...]` sections.
You can modify values and saving the file immediately takes effect in game.

### Hint

Only on `TextureHeight`, `TextureWidth`, value < 1.0 is used to compute texture size by scaling render target width/height rather raw count of pixels. 
For observing mod effects quick, try set both `TextureHeight/Width` to 0.25 in section `[BloomMod!Camera Plus]`.

### Restore defaults

Set `Reset=1` in section `[BloomMod]` and restart BeatSaber and play any map for seconds and quit.
