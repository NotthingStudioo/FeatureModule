<H1>How to Add a Unity Package Using the Package Manager and a Git URL</H1>
If you want to add a specific Unity package from a GitHub repository, you can do it easily using Unity's Package Manager. Below is a simple guide to help you add a package to your project.

Steps
1. Open the <a href="https://docs.unity3d.com/Manual/upm-ui.html" target="_blank">Unity Package Manager</a><br>
In Unity, go to the top menu and click on Window > Package Manager.
2. Add Package from Git URL
In the Package Manager window, locate the + button in the top left corner.
From the dropdown, select Add package from git URL....
3. Insert the Git URL with the Desired Package Path
A text field will appear where you can paste the Git URL of the specific package. For example, to add a package named `[Package-to-use]` from your GitHub repository, use the following URL:

```
https://github.com/NotthingStudioo/FeatureModule.git?path=UnityFeatureModule/Assets/GameModule/[Package-to-use]
```

Replace <code>[Package-to-use]</code> with the specific folder name of the package you'd like to include.

4. Confirm and Install
After pasting the URL, press the Enter key or click Add.
Unity will now download and install the package from the specified GitHub repository path into your project.
Example Usage
To use the `Daily Reward` package, for example, you would follow the same steps and insert the following URL:

```
https://github.com/NotthingStudioo/FeatureModule.git?path=UnityFeatureModule/Assets/GameModule/DailyReward
```

This will install the Daily Reward system into your project, ready for use!

<h2>Daily Reward System</h2>

<p><strong>1. Install the <code>DailyReward</code> to the Project Installer</strong></p>
<p>
To integrate the Daily Reward system into your project, you'll need to bind it using the <strong>Zenject</strong> framework (if you're using it) or manually initialize it. Follow these steps:
</p>

<p>
- Open your <code>GameProjectInstaller</code> class (which is a MonoInstaller).<br>
- Add the following code to the <code>InstallBindings</code> method to properly install the Daily Reward system:
</p>

``` Csharp
public class GameProjectInstaller : MonoInstaller
{
    public override void InstallBindings() 
    { 
            DailyRewardInstaller<DailyRewardPresenter>.Install(this.Container); // DailyRewardPresenter is an example for View
    }
}
```

<p><strong>2. Import Blueprint Data</strong></p>
<p>
The <code>DailyReward</code> system relies on blueprint data, typically stored in CSV files, to manage the reward configuration.
</p>
<p>
- <strong>Navigate</strong> to the folder <code>GameModule/DailyReward/Resources/BlueprintDataSample</code>.<br>
- <strong>Move</strong> all the <code>.csv</code> files from this folder to your own project's blueprint folder where other blueprints are stored.<br>
- <strong>Modify the CSV files</strong> to match your reward structure if necessary.
</p>

<p><strong>3. Customize the Daily Reward Slot View</strong></p>
<p>
The <code>DailyRewardSlotView</code> is the visual component that displays the daily rewards to the player. You can either use the default view provided or create your own.
</p>
<p>
- <strong>If you're using the default view</strong>:<br>
   - Move the <code>DailyRewardSlotView</code> to Unity's Addressables system.<br>
   - Optionally, simplify the name of the view.
</p>
<p>
- <strong>If you're creating your own view</strong>:<br>
   - Skip this step, and make sure your custom view adheres to the expected structure required by the Daily Reward system.
</p>

<p><strong>4. Modify the Default Screen Behavior</strong></p>
<p>
By default, the Daily Reward system will open on the <strong>Main Screen</strong> when the game starts. However, you can change this behavior:
</p>
<p>
- <strong>Open the <code>DailyRewardMiscParam</code></strong> file.<br>
- <strong>Find the <code>StartOnScreen</code> parameter</strong> and change its value to the screen name where you'd like the Daily Reward system to appear.
</p>

<p><strong>5. Configure the Reward Loop</strong></p>
<p>
The <code>TimeLoop</code> parameter represents the length of the reward cycle. You can customize it as follows:
</p>
<p>
- <code>7</code> for a 7-day reward cycle.<br>
- <code>30</code> for a 30-day reward cycle.<br>
- <code>365</code> for an annual reward cycle.
</p>

<p>You can now customize the Daily Reward system to fit your game's structure and needs.</p>
