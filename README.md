# Bus Mapping Unity Project

This project is currently an in-progress Anchorage People Mover GTFS visualizer deployable through WebGL. It is intended to potentially provide the framework for a more fully functional web or mobile app in the future. 

Presently this project will estimate bus positions based on the current time and the bus time tables within the GTFS data. It also provides functionality to scroll time forward via the bottom bar and to automatically fast forward via the button in the upper right. 


# Running the Unity Project

This project was build with Unity 5.5.1 but has been confirmed to work within the editor (within Unity) in 5.6.3 and 2017.1. To try it, simply download Unity from here: https://unity3d.com/get-unity/download/archive and open the project folder once cloned from github. 


# Creating a WebGL build

To create a WebGL build simply go to File > Build Settings from within Unity, make sure WebGL is the target platform (that the Unity logo is next to the WebGL option, if not select WebGL and click "Switch Platform") and then hit "Build" to just save it or "Build and Run" to also automatically open it in a web browser. This will generate the index.html and all required files to run a build within a browser. 

A test build is currently up at https://jazonburnell.github.io/. Note that this build has been modified after being built from Unity to add hi-dpi display support by following the information at http://addcomponent.com/unity-webgl-retina-fix/.
