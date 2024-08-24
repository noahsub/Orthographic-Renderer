using System.Threading;
using NAudio.Wave;

namespace Orthographic.Renderer.Managers;

public class SoundManager
{
    public static void PlaySound(string path)
    {
        using(var audioFile = new AudioFileReader(path))
        using(var outputDevice = new WaveOutEvent())
        {
            outputDevice.Init(audioFile);
            outputDevice.Play();
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(250);
            }
        }
    }
}