using UnityEngine;
using System.IO;
using System;

public static class WavConverter
{
    // This class provides functionality to convert an AudioClip to a WAV byte array.
    // It can be used to save audio recordings in a standard WAV format.
    // Converts an AudioClip to a byte array in WAV format.
    // The AudioClip must be in PCM format with 16-bit samples.
    public static byte[] ConvertToWav(AudioClip clip)
    {
        const int targetSampleRate = 16000;

        using (MemoryStream stream = new MemoryStream())
        {
            if (clip == null)
            {
                Debug.LogError("Cannot convert null AudioClip");
                return null;
            }



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

                // Simple downsampling
                if (clip.frequency > targetSampleRate)
                {
                    int stride = Mathf.RoundToInt((float)clip.frequency / targetSampleRate);
                    float[] downsampled = new float[Mathf.CeilToInt(samples.Length / (float)stride)];

                    for (int i = 0; i < downsampled.Length; i++)
                    {
                        downsampled[i] = samples[i * stride];
                    }

                    samples = downsampled;
                }

                foreach (float sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return stream.ToArray();
        }
    }
    
    // Add to WavConverter.cs
    public static byte[] TrimSilence(byte[] wavData, float threshold = 0.02f)
    {
        // Parse WAV header
        int dataIndex = 44; // WAV header size
        int sampleCount = (wavData.Length - dataIndex) / 2;
    
        // Find start
        int start = 0;
        for (int i = dataIndex; i < wavData.Length; i += 2)
        {
            short sample = (short)((wavData[i+1] << 8) | wavData[i]);
            if (Mathf.Abs(sample / 32768f) > threshold)
            {
                start = i;
                break;
            }
        }
    
        // Find end
        int end = wavData.Length - 1;
        for (int i = wavData.Length - 2; i > dataIndex; i -= 2)
        {
            short sample = (short)((wavData[i+1] << 8) | wavData[i]);
            if (Mathf.Abs(sample / 32768f) > threshold)
            {
                end = i + 2;
                break;
            }
        }
    
        // Create new buffer
        byte[] newData = new byte[44 + (end - start)];
    
        // Copy header
        Buffer.BlockCopy(wavData, 0, newData, 0, 44);
    
        // Update data size in header (bytes 40-43)
        int dataSize = end - start;
        newData[40] = (byte)(dataSize & 0xFF);
        newData[41] = (byte)((dataSize >> 8) & 0xFF);
        newData[42] = (byte)((dataSize >> 16) & 0xFF);
        newData[43] = (byte)((dataSize >> 24) & 0xFF);
    
        // Copy audio data
        Buffer.BlockCopy(wavData, start, newData, 44, dataSize);
    
        return newData;
    }
}
