namespace HermitMod.Utility;

public static class HermitSfx
{
    public const string Gun1 = "hermit_gun";
    public const string Gun2 = "hermit_gun2";
    public const string Gun3 = "hermit_gun3";
    public const string Spin = "hermit_spin";
    public const string Reload = "hermit_reload";
    public const float DefaultDb = 5f;
    public const float SpinPitchVariation = 0.15f;
    public const float GunPitchVariation = 0.1f;

    public static void PlayGun1(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
        => ModAudio.PlaySfx(Gun1, volumeDb, pitchVariation);

    public static void PlayGun2(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
        => ModAudio.PlaySfx(Gun2, volumeDb, pitchVariation);

    public static void PlayGun3(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
        => ModAudio.PlaySfx(Gun3, volumeDb, pitchVariation);

    public static void PlaySpin(float volumeDb = DefaultDb, float pitchVariation = SpinPitchVariation)
        => ModAudio.PlaySfx(Spin, volumeDb, pitchVariation);

    public static void PlayReload(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
        => ModAudio.PlaySfx(Reload, volumeDb, pitchVariation);
}
