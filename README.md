<img width="617" height="275" alt="easyCMSLogo" src="https://github.com/user-attachments/assets/f18e8d27-e5bf-49e1-aaea-fd2a1b026c8b" />

# :wrench: EasyCMS
EasyCMS is a toolset created to simplify porting Automation creations exported to BeamNG to Car Mechanic Simulator 2021. The tool automates porting BeamNG materials to Unity HDRP materials and manages Asset Bundle creation and exporting to CMS
# :stop_sign: Limitations
Before we proceed to the installation and usage guides, let us go over things EasyCMS **cannot** do:
* EasyCMS does not manage the models outside of generating configs based on imported materials. The models have to be set up manually to the requirements of CMS
* EasyCMS does not fix textures unsupported by Unity
* Materials created by EasyCMS do not utilize CMS material features such as painting, rust, dust, dents or light functions
* Glass parts do not get cut out fully, some glass reflection effects remain on parts of glass that are supposed to be cut out
* Because EasyCMS materials are mostly transparent to support Automation cutouts, some objects may appear through these materials
# :spiral_notepad: Prerequisites
* An installed instance of the Unity 2020.3 editor
* A reference of the CMS21 modding PDF Guide `Car Mechanic Simulator 2021\ModdingTools\PDFGuides\Car Mechanic Simulator 2021 Car Modding Guide.pdf`
* 3D editing software that supports Collada (.dae) and FBX (.fbx) files (Blender recommended)
## Also recommended
* QoL Mod for CMS21
* An image editor with support for DDS (.dds) files
# :building_construction: Setup
1. Create a Unity 3D project and open it
2. In the top tool panel, go into `Window > Package Manager` 

<img width="960" height="516" alt="image" src="https://github.com/user-attachments/assets/23f5cd53-3a99-4ed3-b385-ccfbdcd561d1" />

3. In the Package Manager window, find the `Packages` selection at the top and change it to `Unity Registry` 

<img width="800" height="567" alt="image" src="https://github.com/user-attachments/assets/1e2a8a8b-edc6-4ef2-b00b-cab993ffac6f" />

4. Find `High Definition RP` and install it. Once installed, it will prompt you to set it up, click through all the buttons with warnings next to them 

<img width="898" height="638" alt="image" src="https://github.com/user-attachments/assets/2897b0f2-3c95-4668-9cff-1f5822e1bf64" /> <img width="500" height="567" alt="image" src="https://github.com/user-attachments/assets/fd7a9a1d-30ec-4d1b-99fb-c440dc068a2a" />

5. Click the plus button in the top left corner of the Package Manager and select `Add package from git URL...` 

<img width="800" height="567" alt="image" src="https://github.com/user-attachments/assets/448e01e4-d981-4ed8-93f2-9ea70b8f7e89" />

6. Put `com.unity.nuget.newtonsoft-json` into the created box and click add

<img width="800" height="567" alt="image" src="https://github.com/user-attachments/assets/4b3a6920-48e3-4844-846e-e8328714db0d" />


7. Download the latest EasyCMS release from the `Releases` section in this GitHub repository
8. Merge the `Assets` folder from the EasyCMS download with the `Assets` folder in your Unity project's directory
# :blue_car: Usage
## Car setup
First, let's prepare the Automation car itself. Make sure your car does not use modded materials that do not support exporting
* Remove the steering wheel and the seats. Car Mechanic Simulator adds its own
* Avoid fixtures going over the body's panel seams. This will not be an issue unless you want to separate the panels to work in CMS properly. Regardless, not all bodies are made the same and some of them will be a hassle to separate panels from
* Proceed to the export menu and select BeamNG.drive. **Make sure to uncheck `Zip-Pack Mod`**, it is also **highly** recommended to check `Merge All Fixtures` to simplify working with the exported models. Now export the car to BeamNG
## Set up the car project inside the Unity project
* In the Unity Editor's filesystem at the bottom, go to `Assets/Cars` and create a new folder for your car (`Right-click > Create > Folder`). The name of the folder will be the internal name of your car
* Right-click inside this folder and select `Create > EasyCMS > Car Manager` (do not mistake the `EasyCMS` option for `Create > EasyCMS`). This will create the core manager for your car, you can name this file anything you want

<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/4947de38-c1d7-4b24-afe9-51c009ed0473" />

* Select the newly created `Car Manager` file, fill out the `CMS Executable` field with the path to your CMS's .exe file (you can click the `...` button next to it to open a file dialog). You should only need to do this once, any Car Managers you create after will look for other Car Managers and copy this path on creation
* Now fill out the `BeamNG Materials` path with the path to your car's `[UID].materials.json` file. By default, this is located in `C:\Users\[USER]\AppData\Local\BeamNG\BeamNG.drive\current\mods\unpacked\[MOD NAME]\vehicles\[CAR NAME]`

<img width="366" height="530" alt="image" src="https://github.com/user-attachments/assets/e1c93039-4d68-4d58-acb1-645ceb4df74a" />


* Once both paths are set up, click through all of the buttons in the `Import` section in order. This will copy the materials file and the textures related to it from your car and then build Unity HDRP materials from them

> [!NOTE]
> When you import the textures, you will likely see a lot of import errors. A lot of textures used by the Autobeam cars are not using a format/compression that Unity supports. This is not critical - the car will work without them, however the textures will not. If you want the textures to work - you will have to fix them manually with image editing software. You can find information about doing so [further](#fixing-textures) in the guide

* Once materials are built, you will see `Paint_[X]` assets created in your car's folder. These files are created for each of the paint slots found in the materials file. You can use these to change the paint of the car. This cannot be done in the game, so this is where you decide on the color and the paint parameters. In the `Materials` folder you will also find all materials imported form the materials file, you can edit each one separately here if you so desire

<img width="365" height="273" alt="image" src="https://github.com/user-attachments/assets/20bbe452-861a-4301-a88b-16bff21015f8" />


## Car model setup
* Open your 3D editing software of choice and import the car's model file from the exported files. By default this will be in `C:\Users\[USER]\AppData\Local\BeamNG\BeamNG.drive\current\mods\unpacked\[MOD NAME]\vehicles\[CAR NAME]\[UID]\[UID]_bodymesh.dae`. In Blender, make sure not to import the same car multiple times in one project as this will mess up the material names
* As per CMS requirements, there need to be at least 2 model files: `model.fbx` - containing all the models of the car excluding alternate body parts, and `collider.fbx` - containing just one model for the collider. Let's start with the `model.fbx` file
* CMS requires that `model.fbx` contains at least 2 models: one named `body` and one named `details`. The official documentation lists all valid object names, other names must not be present here. For a quick working example we can select the main body model and rename it to `body` and select the chassis model and rename it to `details`. All other objects should now we deleted. Select all remaining objects and apply all their transforms (in Blender, hit `Ctrl+A` and select `All Transforms`)
* We also want a way into the driver's seat, so if the body is enclosed it's a good idea to separate the driver's window (in Blender, go into `Edit Mod` (default hotkey: `Tab`), then into face mode (hotkey: `3`) and press `L` when hovering over the window, then press `P` and select `Selection`), name the new object `window_door_front_left` or `window_door_front_right` depending on the side of the car. We will talk about openable panels in a [separate section](#openable-panels)
* This is enough for a basic car mesh, export the models as FBX into your project at path `Assets/Cars/[CAR NAME]/model.fbx`. Make sure to export only the required models (in Blender, use `Selected Objects` or `Active Collection` in the export settings)
* Now let's create the collider model. In Blender, it is a good idea to create a separate collection for it. For a quick and simple collider we can simply create a cube and size it to the car's dimensions. Remember to apply transforms on it again if you sized it in object mode. The name of the new object must be `collider`
* Export the collider model to `Assets/Cars/[CAR NAME]/collider.fbx`. Once again make sure to export only the collider, not the entire car with it
* Now in Unity, you can drag the car out of the filesystem into the scene to make sure the model exported correctly and that materials correctly applied to it. This is also where you can find if any materials are wrong and can adjust them with a good reference

Your car folder should now look something like this:
<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/f991cefe-e79d-4745-802f-aebfddf48547" />

## Generating configs
* Navigate to the `Car Manager` object for your car and click through the buttons in the `Templates` section. This will generate all config files required and used by CMS with EasyCMS templates that you can edit later. Instead of generating the `Car Config` (`config.txt`), you can grab one from the game files off a car that closer matches the car you are  porting. CMS car files can be found in `Car Mechanic Simulator 2021\Car Mechanic Simulator 2021_Data\StreamingAssets\Cars`. We will talk about what each config file does in a [further section](#config-files)
* Later you will likely edit these inside the game files, the `Updating Configs` section of the `Car Manager` can bring your project's config files up to date with the ones in your game files (exporting the car to the game files will also prompt you for this if applicable)
## Exporting and gameside edits
* Once ready to export the car to CMS, first click `Build Asset Bundle`. This will create the package that contains your car models so they can be loaded into CMS
* Now click `Copy Car Assets To Game`. This will take your car package and config files associated with it and copy them over to your game's directory so they are ready to be used in-game
* Open the CMS Car Editor and select your car there. Here you can properly set up mostly everything about the car itself. Here, the QoL Mod is very useful as it extends the editor features and provides you with a lot of useful information. Make sure to periodically click `Save` in the bottom left corner as you make changes - **there is no autosave**
* In the bottom right corner you will find buttons to generate images for the car thumbnail and the car parts. Click them both

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/b0bc6928-5749-4f09-93da-2977c81c82e0" />

* Now you are ready to open the game itself and find your car here. For testing the QoL Mod comes to the rescue again, allowing you to spawn the last edited car right in the garage



# :oncoming_automobile: Extras
## Fixing textures
Simply speaking, you will encounter 2 types of textures: color and monochrome

To fix DDS color textures all you need to do is import the texture into image editing software that supports various DDS compressions and re-export the texture with DXT5 compression, overwriting the original file. Preferably do this inside the Unity project so you don't have to repeat the import process

Monochrome textures are similar but require an intermediate step. These use color for transparency instead of making use of the actual alpha channel. We need to remove everything in the color black from the image so there is transparency in its place before exporting the texture to DXT5
## Config files
There is a total of 4 config files that we are using
1. `name.txt` - This is simply the display name of the car, including the brand name but excluding the configuration name (the latter is part of the `config.txt` file)
2. `config.txt` - This is the main file of the car that manages most of the technical stuff. You mostly edit it via the CMS Car Editor. The vanilla Car Editor does not expose all its parameters, so QoL Mod once again comes to the rescue
3. `bodyconfig.txt` - This file manages links between body parts. The `unmount_with` section defines which parts are attached to other parts, which means the attached parts will be removed when the part they are attached to gets removed, and this also manages part attachment to openable parts. This file also manages alternate positions of license plates that are used if you have alternate parts license plates attach to. In the config generated by EasyCMS the front license plate attaches to the front bumper and the rear license plate attaches to the trunk by default. EasyCMS also automatically generates links between doors, door windows and mirrors
4. `parts.txt` - This file defines prices for the car's body parts. To be able to appear in the in-game part shop the part must be defined here and must have a price above 0. EasyCMS generates a price of 300 credits for each valid part found in the model file

## Openable panels
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/d755cda7-0517-42be-8228-9a45a4ab89ea" />

Openable panels are separate objects set up in a slightly different way from other body parts. Here, working in Blender will be referenced specifically, working in other software may be different.
To make openable doors, hoods, trunks etc. one must first separate these panels from the body. Liberate use of the `L` key when in face edit mode in Blender helps quickly select large surfaces, however be careful that the parts of the panels that go inside panel gaps aren't usually picked up by this. With other more complex objects the X-Ray mode (default hotkey: `Alt-Z` or can be found as a button in the top right corner) paired with box or circle select (`B` or `C` hotkeys respectively) can help. Temporarily hiding faces with `H` can help get other faces out of the way so you don't accidentally select them along with the faces you want, you can unhide them again with `Alt-H`

Making the panels open correctly gets a bit weird. First, you need to set the object's origin point to the point around which the panel will rotate. The fastest (although inaccurate) way to do this is to `Shift-Right Click` on the point where you want the origin to be, this will set the 3D cursor to that point, then bring up the search menu (default hotkey: `F3`) and find and click `Origin to 3D Cursor` with the object selected. But we are not done yet, the axis needs to be set up in an unintuitive way

To set up the axis, first navigate to the top menu and change `Transformation Orientation` to `Local`. Now select the object you want to set up. You will now have to do different things depending on what panel this is:
* Hood: Rotate on the `Y` axis `-90 degrees` to open the hood forwards or `90 degrees` for backwards. Hit `Ctrl+A`, then `Rotation`. Rotate on the `Y` axis `90 degrees` or `-90 degrees` depending on what you did earlier respectively. Ensure that rotating the object on the `Z` axis to **negative** values opens the panel the correct way
* Trunk: Rotate on the `Y` axis `90 degrees` to open the trunk forwards or `-90 degrees` for backwards. Hit `Ctrl+A`, then `Rotation`. Rotate on the `Y` axis `-90 degrees` or `90 degrees` depending on what you did earlier respectively. Ensure that rotating the object on the `Z` axis to **positive** values opens the trunk the correct way
* Left Doors: Rotate on the `X` axis `-90 degrees`. Hit `Ctrl+A`, then `Rotation`. Rotate on the `X` axis `90 degrees`. Ensure that rotating the object on the `Y` axis to **negative** values opens the panel the correct way
* Right Doors: Rotate on the `X` axis `90 degrees`. Hit `Ctrl+A`, then `Rotation`. Rotate on the `X` axis `-90 degrees`. Ensure that rotating the object on the `Y` axis to **negative** values opens the panel the correct way
