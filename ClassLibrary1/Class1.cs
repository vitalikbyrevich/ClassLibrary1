using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using ItemManager;
using UnityEngine;

namespace Weapons
{
	[BepInPlugin(ModGUID, ModName, ModVersion)]
	public class Weapons : BaseUnityPlugin
	{
		private const string ModName = "Weapons";
		private const string ModVersion = "1.0";
		private const string ModGUID = "org.bepinex.plugins.weapons";
		private static Harmony harmony = new Harmony("Weapons");
		private static Weapons _thistype;
		private AssetBundle _embeddedResourceBundle;
		public GameObject bowVB;
		public GameObject swordVB;

        public void Awake()
		{
			Weapons._thistype = this;
			Weapons.harmony.PatchAll();
			this._embeddedResourceBundle = this.LoadAssetBundleFromResources("vbmod", Assembly.GetExecutingAssembly());
			Item item = new Item("vbmod", "BowVB", "assets");
			item.Name.English("Iron Fang Axe"); // You can use this to fix the display name in code
			item.Description.English("A sharp blade made of iron.");
			item.Crafting.Add("MyAmazingCraftingStation", 3); // Custom crafting stations can be specified as a string
			item.MaximumRequiredStationLevel = 5; // Limits the crafting station level required to upgrade or repair the item to 5
			item.RequiredItems.Add("Iron", 120);
			item.RequiredItems.Add("WolfFang", 20);
			item.RequiredItems.Add("Silver", 40);
			item.RequiredUpgradeItems.Add("Iron", 20); // Upgrade requirements are per item, even if you craft two at the same time
			item.RequiredUpgradeItems.Add("Silver", 10); // 10 Silver: You need 10 silver for level 2, 20 silver for level 3, 30 silver for level 4
			item.CraftAmount = 2; // We really want to dual wield these

			Item item2 = new Item("vbmod", "SwordVB", "assets");
			item2["My first recipe"].Crafting.Add(CraftingTable.Workbench, 1); // You can add multiple recipes for the same item, by giving the recipe a name
			item2["My first recipe"].RequiredItems.Add("Wood", 10);
			item2["My first recipe"].RequiredItems.Add("Flint", 5);
			item2["My first recipe"].RequiredUpgradeItems.Add("Wood", 5);
			item2["My alternate recipe"].Crafting.Add(CraftingTable.Forge, 1); // And this is our second recipe then
			item2["My alternate recipe"].RequiredItems.Add("Bronze", 2);
			item2["My alternate recipe"].RequiredUpgradeItems.Add("Bronze", 1);
			item2.Snapshot(); // I don't have an icon for this item in my asset bundle, so I will let the ItemManager generate one automatically
							  // The icon for the item will have the same rotation as the item in unity

		}
		private void LoadItems()
		{
			this.bowVB = this._embeddedResourceBundle.LoadAsset<GameObject>("BowVB");
			this.swordVB = this._embeddedResourceBundle.LoadAsset<GameObject>("SwordVB");
			this.Debug("All Items Loaded");
		}
		public AssetBundle LoadAssetBundleFromResources(string bundleName, Assembly resourceAssembly)
		{
			if (resourceAssembly == null)
			{
				throw new ArgumentNullException("Parameter resourceAssembly can not be null.");
			}
			string text = null;
			try
			{
				text = resourceAssembly.GetManifestResourceNames().Single((string str) => str.EndsWith(bundleName));
			}
			catch (Exception)
			{
			}
			if (text == null)
			{
				base.Logger.LogError("AssetBundle " + bundleName + " not found in assembly manifest");
				return null;
			}
			AssetBundle result;
			using (Stream manifestResourceStream = resourceAssembly.GetManifestResourceStream(text))
			{
				result = AssetBundle.LoadFromStream(manifestResourceStream);
			}
			return result;
		}

		public void Debug(string msg)
		{
			base.Logger.LogInfo(msg);
		}

		public void DebugError(string msg)
		{
			base.Logger.LogError(msg);
		}
	}
}
