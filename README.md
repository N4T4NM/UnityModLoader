# UnityModLoader
This tool allows you to load mods into unity games

# Install

* Move the UnityModLoader dll files to the game folder
* Move the UnityModLoader.Manager.exe to the game folder

# Add Mods

* Move mod files to the "Mods" folder
* Move dependencies to the "Dependencies" folder

# Mods Creation
* Create a new Class Library project
* Import the Assembly-CSharp.dll file
* Create a public class with the 'MainClass' attribute
* Create a public static method without any parameters, and add the 'EntryPoint' attribute.
* Check [ModSample.AutoLoader](https://github.com/N4T4NM/UnityModLoader/tree/master/ModSample.AutoLoader)
