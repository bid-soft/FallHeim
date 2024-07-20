using HarmonyLib;

namespace FallHeim.Patches
{
	
	[HarmonyPatch(typeof(MusicMan), nameof(MusicMan.StartMusic), typeof(MusicMan.NamedMusic))]
	class MusicMan_StartMusic_patch
	{
		static void Prefix(ref MusicMan.NamedMusic music)
		{
			if (music!= null) music.m_fadeInTime = 1f;
		}
	}
	
	
}
