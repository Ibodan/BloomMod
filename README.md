# BloomMod
BeatSaber bloom effect modification plugin

Modifies internal bloom effect parameters of each cameras in-game.
It is mainly for appling proper or enhanced bloom on CameraPlus's 3rd person camera and MultiView's extra camera.
But you can also modify bloom effect in HMD.

### Install

put dll into Plugins directory

### Intialize config file with default value

First, start BeatSaber and play any map for seconds and quit.

### Modify config file

Open UserData/modprefs.ini. There you can see some [BloomMod!...] sections.
You can modify values and saving the file immediately takes effect in game.

### Hint

Only on <key>TextureHeight/Width</key>, < 1.0 value used to compute texture size by scaling render target width/height rather raw count of pixels. 
For section [BloomMod!Camera Plus], just set both <b>TextureHeight/Width</b> to 0.25.

### Restore defaults

Set [BloomMod] Reset=1 and restart BeatSaber and play any map for seconds and quit.
