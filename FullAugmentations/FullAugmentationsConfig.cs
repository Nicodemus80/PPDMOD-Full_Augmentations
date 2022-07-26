using PhoenixPoint.Modding;

namespace FullAugmentations
{
	/// <summary>
	/// ModConfig is mod settings that players can change from within the game.
	/// Config is only editable from players in main menu.
	/// Only one config can exist per mod assembly.
	/// Config is serialized on disk as json.
	/// </summary>
	
	

	public class FullAugmentationsConfig : ModConfig
	{
		public static int MaxAugmentations = 3;
	}
}
