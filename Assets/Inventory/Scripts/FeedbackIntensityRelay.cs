using UnityEngine;
using MoreMountains.Feedbacks;

public class FeedbackIntensityRelay : MonoBehaviour
{
    [SerializeField] private MMF_Player feedbackPlayer;

    // This is the method your Unity Event will see
    public void UpdateIntensity(float intensity)
    {
        if (feedbackPlayer != null)
        {
            // This updates the global intensity of the shake live
            feedbackPlayer.FeedbacksIntensity = intensity;
        }
    }
}