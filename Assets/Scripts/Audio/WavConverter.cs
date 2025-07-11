using UnityEngine;
using System.IO;

public static class WavConverter 
{
    // This class provides functionality to convert an AudioClip to a WAV byte array.
    // It can be used to save audio recordings in a standard WAV format.
    // Converts an AudioClip to a byte array in WAV format.
    // The AudioClip must be in PCM format with 16-bit samples.
    public static byte[] ConvertToWav(AudioClip clip)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Write WAV header
                // "RIFF" header
                // "WAVE" format
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + clip.samples * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)clip.channels);
                writer.Write(clip.frequency);
                writer.Write(clip.frequency * clip.channels * 2);
                writer.Write((ushort)(clip.channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(clip.samples * clip.channels * 2);

                // Audio data
                // Get the audio data from the AudioClip
                float[] samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);
                foreach (float sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return stream.ToArray();
        }
    }
}
