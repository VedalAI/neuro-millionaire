using HarmonyLib;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class ChangeAskTheAudienceMusic
{
    [HarmonyPatch(typeof(MusicDirector), nameof(MusicDirector.PlayMusicTrack), typeof(string), typeof(bool))]
    [HarmonyPatch(typeof(MusicDirector), nameof(MusicDirector.PlayMusicTrack), typeof(string), typeof(float))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static void Prefix(string MusicTrackName)
    {
        if (MusicTrackName != "AK_Event_Mus_Bonus_Public_Keyboard") return;

        MusicDirector.MusicTrack[] playlist = MusicDirector.instance.mMusicPlaylist;
        for (int i = 0; i < playlist.Length; i++)
        {
            if (playlist[i].mMusicName == MusicTrackName) playlist[i].mMusicClip = LONGER_AUDIENCE_CLIP;
        }
    }
}
