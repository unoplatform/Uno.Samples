using Xamarin.Forms;

// Resolution group must match across shared project and platform specific projects
[assembly: ResolutionGroupName("UnoSamples.Effects")]

namespace UnoSamples.Effects
{
    public class NoUnderlineEffect : RoutingEffect
    {
        // unique id for effect with resolution group name and effect class name
        internal const string EffectId = "UnoSamples.Effects.NoUnderlineEffect";

        public NoUnderlineEffect() : base(EffectId) 
        {
            // ensure linker doesn't exclude native implementation
#if ANDROID
            _ = new UnoSamples.Effects.Droid.NoUnderlineEffect();
#endif
        }
    }
}
