using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text;
using System.IO;
using BuildLesson;

public static class Helper
{
    public const float DETA_TIME_AVARAGRE = 0.0001f;

    const int BlockSize_16Bit = 2;

    public static IEnumerator MoveObject(GameObject moveObject, Vector3 targetPosition)
    {
        float timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            moveObject.transform.position = Vector3.Lerp(moveObject.transform.position, targetPosition, timeSinceStarted);
            if (moveObject.transform.position == targetPosition)
            {
                yield break;
            }
            yield return null;
        }
    }

    public static GameObject GetChildOrganOnTouchByTag(Vector3 position)
    {
		var cam = Camera.main;
		if (cam==null) return null;
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit[] raycastHits;
        raycastHits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider.transform.root.gameObject.tag == TagConfig.ORGAN_TAG)
            {
                if (hit.collider.transform.parent == ObjectManager.Instance.CurrentObject.transform)
                {
                    return hit.collider.gameObject;
                }
		
            }
        }
        return null;
    }
	public static GameObject GetChildOrganOnTouchByTag2(Vector3 position)
    {
		var cam = Camera.main;
		if (cam==null) return null;
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit[] raycastHits;
        raycastHits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider.transform.root.gameObject.tag == TagConfig.ORGAN_TAG)
            {
                if (hit.collider.transform.parent == ObjectManagerBuildLesson.Instance.CurrentObject.transform)
                {
                    return hit.collider.gameObject;
                }
		
            }
        }
        return null;
    }
    
    // touchPosition, refactor parameter 
    public static GameObject GetAddLabelOnTouch(Vector3 localPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(localPosition);
        RaycastHit[] raycastHits; 
        raycastHits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in raycastHits)
        {
            if (hit.collider.transform.root.gameObject)
            {
                if (hit.collider.transform.parent == ObjectManager.Instance.CurrentObject.transform)
                {
                    return hit.collider.gameObject;
                }
            }
        }
        return null;
    }

    public static bool GetUIByTouch(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit[] raycastHits;
        raycastHits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider.transform.gameObject.tag == TagConfig.UI_TAG)
            {
                return true;
            }
        }
        return false;
    }

    public static Bounds GetRenderBounds(GameObject go)
    {
        var totalBounds = new Bounds();
        totalBounds.SetMinMax(Vector3.one * Mathf.Infinity, -Vector3.one * Mathf.Infinity);
        foreach (var renderer in go.GetComponentsInChildren<Renderer>())
        {
            var bounds = renderer.bounds;
            var totalMin = totalBounds.min;
            totalMin.x = Mathf.Min(totalMin.x, bounds.min.x);
            totalMin.y = Mathf.Min(totalMin.y, bounds.min.y);
            totalMin.z = Mathf.Min(totalMin.z, bounds.min.z);

            var totalMax = totalBounds.max;
            totalMax.x = Mathf.Max(totalMax.x, bounds.max.x);
            totalMax.y = Mathf.Max(totalMax.y, bounds.max.y);
            totalMax.z = Mathf.Max(totalMax.z, bounds.max.z);
            totalBounds.SetMinMax(totalMin, totalMax);
        }

        return totalBounds;
    }
	public static Bounds CalculateBounds(GameObject obj)
	{
		var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length <= 0)
        {
            return new Bounds(Vector3.zero, Vector3.zero);
        }
        Bounds bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; ++i)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds;
	}

    public static string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static void RemoveElementOfListFromIndexToEnd(List<GameObject> listOrigin, int indexStartDelete)
    {
        for (int index = indexStartDelete; index < listOrigin.Count; index++)
        {
            listOrigin.RemoveAt(index);
        }
    }

    public static void RemoveElementOfListInListObjectFromIndexToEnd(List<List<GameObject>> listOrigin, int indexStartDelete)
    {
        for (int index = indexStartDelete; index < listOrigin.Count; index++)
        {
            listOrigin.RemoveAt(index);
        }
    }
    public static void RemoveElementOfListInListVector3FromIndexToEnd(List<List<Vector3>> listOrigin, int indexStartDelete)
    {
        for (int index = indexStartDelete; index < listOrigin.Count; index++)
        {
            listOrigin.RemoveAt(index);
        }
    }
    public static void CopyToClipboard(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }

    public static List<Vector3> GetListchildrenOfOriginPosition(GameObject obj)
    {
        int childCount = obj.transform.childCount;
       
        List<Vector3> listchildrenOfOriginPosition = new List<Vector3>();

        for (int i = 0; i < childCount; i++)
        {
            if (obj.transform.GetChild(i).gameObject.tag != TagConfig.LABEL_TAG)
            {
                listchildrenOfOriginPosition.Add(obj.transform.GetChild(i).localPosition);
            }
        }

        return listchildrenOfOriginPosition;
    } 

    public static void ResetStatusFeature()
    {
        // Label
        LabelManager.Instance.IsShowingLabel = false;
        LabelManager.Instance.HandleLabelView(LabelManager.Instance.IsShowingLabel);
        // Separate
        SeparateManager.Instance.IsSeparating = false;
        SeparateManager.Instance.HandleSeparate(SeparateManager.Instance.IsSeparating);
    }

    public static IEnumerator EffectScaleObject(GameObject scaleObject, float timeScale, Vector3 targetLocalScale)
    {
        Vector3 startScale = targetLocalScale / 10f;
        scaleObject.transform.localScale = startScale;
        int numberFrame = Convert.ToInt32(timeScale / DETA_TIME_AVARAGRE);
        while (true)
        {
            scaleObject.transform.localScale += (targetLocalScale - startScale) / numberFrame;
            if (Vector3.SqrMagnitude(scaleObject.transform.localScale - targetLocalScale) < DETA_TIME_AVARAGRE)
            {
                yield break;
            }
            yield return null;
        }
    }
    // Reference: https://github.com/deadlyfingers/UnityWav/blame/master/WavUtility.cs

    public static AudioClip ToAudioClip (byte[] fileBytes, int offsetSamples = 0, string name = "wav")
	{
		//string riff = Encoding.ASCII.GetString (fileBytes, 0, 4);
		//string wave = Encoding.ASCII.GetString (fileBytes, 8, 4);
		int subchunk1 = BitConverter.ToInt32 (fileBytes, 16);
		UInt16 audioFormat = BitConverter.ToUInt16 (fileBytes, 20);
		// NB: Only uncompressed PCM wav files are supported.
		string formatCode = FormatCode (audioFormat);
		Debug.AssertFormat (audioFormat == 1 || audioFormat == 65534, "Detected format code '{0}' {1}, but only PCM and WaveFormatExtensable uncompressed formats are currently supported.", audioFormat, formatCode);
		UInt16 channels = BitConverter.ToUInt16 (fileBytes, 22);
		int sampleRate = BitConverter.ToInt32 (fileBytes, 24);
		//int byteRate = BitConverter.ToInt32 (fileBytes, 28);
		//UInt16 blockAlign = BitConverter.ToUInt16 (fileBytes, 32);
		UInt16 bitDepth = BitConverter.ToUInt16 (fileBytes, 34);
		int headerOffset = 16 + 4 + subchunk1 + 4;
		int subchunk2 = BitConverter.ToInt32 (fileBytes, headerOffset);
		//Debug.LogFormat ("riff={0} wave={1} subchunk1={2} format={3} channels={4} sampleRate={5} byteRate={6} blockAlign={7} bitDepth={8} headerOffset={9} subchunk2={10} filesize={11}", riff, wave, subchunk1, formatCode, channels, sampleRate, byteRate, blockAlign, bitDepth, headerOffset, subchunk2, fileBytes.Length);
		float[] data;
		switch (bitDepth) 
		{
			case 8:
				data = Convert8BitByteArrayToAudioClipData (fileBytes, headerOffset, subchunk2);
				break;
			case 16:
				data = Convert16BitByteArrayToAudioClipData (fileBytes, headerOffset, subchunk2);
				break;
			case 24:
				data = Convert24BitByteArrayToAudioClipData (fileBytes, headerOffset, subchunk2);
				break;
			case 32:
				data = Convert32BitByteArrayToAudioClipData (fileBytes, headerOffset, subchunk2);
				break;
			default:
				throw new Exception (bitDepth + " bit depth is not supported.");
		}
		AudioClip audioClip = AudioClip.Create (name, data.Length, (int)channels, sampleRate, false);
		audioClip.SetData (data, 0);
		return audioClip;
	}

    private static float[] Convert8BitByteArrayToAudioClipData (byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32 (source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat (wavSize > 0 && wavSize == dataSize, "Failed to get valid 8-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);
		float[] data = new float[wavSize];
		sbyte maxValue = sbyte.MaxValue;
		int i = 0;
		while (i < wavSize) 
		{
			data [i] = (float)source [i] / maxValue;
			++i;
		}
		return data;
	}

    private static float[] Convert16BitByteArrayToAudioClipData (byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32 (source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat (wavSize > 0 && wavSize == dataSize, "Failed to get valid 16-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);
		int x = sizeof(Int16); // block size = 2
		int convertedSize = wavSize / x;
		float[] data = new float[convertedSize];
		Int16 maxValue = Int16.MaxValue;
		int offset = 0;
		int i = 0;
		while (i < convertedSize)
		{
			offset = i * x + headerOffset;
			data [i] = (float)BitConverter.ToInt16 (source, offset) / maxValue;
			++i;
		}
		Debug.AssertFormat (data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);
		return data;
	}

	private static float[] Convert24BitByteArrayToAudioClipData (byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32 (source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat (wavSize > 0 && wavSize == dataSize, "Failed to get valid 24-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);
		int x = 3; // block size = 3
		int convertedSize = wavSize / x;
		int maxValue = Int32.MaxValue;
		float[] data = new float[convertedSize];
		byte[] block = new byte[sizeof(int)]; // using a 4 byte block for copying 3 bytes, then copy bytes with 1 offset
		int offset = 0;
		int i = 0;
		while (i < convertedSize) 
		{
			offset = i * x + headerOffset;
			Buffer.BlockCopy (source, offset, block, 1, x);
			data [i] = (float)BitConverter.ToInt32 (block, 0) / maxValue;
			++i;
		}
		Debug.AssertFormat (data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);
		return data;
	}

	private static float[] Convert32BitByteArrayToAudioClipData (byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32 (source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat (wavSize > 0 && wavSize == dataSize, "Failed to get valid 32-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);
		int x = sizeof(float); //  block size = 4
		int convertedSize = wavSize / x;
		Int32 maxValue = Int32.MaxValue;
		float[] data = new float[convertedSize];
		int offset = 0;
		int i = 0;
		while (i < convertedSize) 
		{
			offset = i * x + headerOffset;
			data [i] = (float)BitConverter.ToInt32 (source, offset) / maxValue;
			++i;
		}
		Debug.AssertFormat (data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);
		return data;
	}

    private static string FormatCode (UInt16 code)
	{
		switch (code) 
		{
			case 1:
				return "PCM";
			case 2:
				return "ADPCM";
			case 3:
				return "IEEE";
			case 7:
				return "μ-law";
			case 65534:
				return "WaveFormatExtensable";
			default:
				Debug.LogWarning ("Unknown wav code format:" + code);
				return "";
		}
	}

    private static byte[] ConvertAudioClipDataToInt16ByteArray (float[] data)
	{
		MemoryStream dataStream = new MemoryStream ();
		int x = sizeof(Int16);
		Int16 maxValue = Int16.MaxValue;
		int i = 0;
		while (i < data.Length) 
		{
			dataStream.Write (BitConverter.GetBytes (Convert.ToInt16 (data [i] * maxValue)), 0, x);
			++i;
		}
		byte[] bytes = dataStream.ToArray ();
		// Validate converted bytes
		Debug.AssertFormat (data.Length * x == bytes.Length, "Unexpected float[] to Int16 to byte[] size: {0} == {1}", data.Length * x, bytes.Length);
		dataStream.Dispose ();
		return bytes;
	}

    private static int BytesPerSample (UInt16 bitDepth)
	{
		return bitDepth / 8;
	}

    private static int WriteBytesToMemoryStream (ref MemoryStream stream, byte[] bytes, string tag = "")
	{
		int count = bytes.Length;
		stream.Write (bytes, 0, count);
		//Debug.LogFormat ("WAV:{0} wrote {1} bytes.", tag, count);
		return count;
	}

    private static int WriteFileHeader (ref MemoryStream stream, int fileSize)
	{
		int count = 0;
		int total = 12;
		// riff chunk id
		byte[] riff = Encoding.ASCII.GetBytes ("RIFF");
		count += WriteBytesToMemoryStream (ref stream, riff, "ID");
		// riff chunk size
		int chunkSize = fileSize - 8; // total size - 8 for the other two fields in the header
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (chunkSize), "CHUNK_SIZE");
		byte[] wave = Encoding.ASCII.GetBytes ("WAVE");
		count += WriteBytesToMemoryStream (ref stream, wave, "FORMAT");
		// Validate header
		Debug.AssertFormat (count == total, "Unexpected wav descriptor byte count: {0} == {1}", count, total);
		return count;
	}

	private static int WriteFileFormat (ref MemoryStream stream, int channels, int sampleRate, UInt16 bitDepth)
	{
		int count = 0;
		int total = 24;
		byte[] id = Encoding.ASCII.GetBytes ("fmt ");
		count += WriteBytesToMemoryStream (ref stream, id, "FMT_ID");
		int subchunk1Size = 16; // 24 - 8
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (subchunk1Size), "SUBCHUNK_SIZE");
		UInt16 audioFormat = 1;
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (audioFormat), "AUDIO_FORMAT");
		UInt16 numChannels = Convert.ToUInt16 (channels);
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (numChannels), "CHANNELS");
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (sampleRate), "SAMPLE_RATE");
		int byteRate = sampleRate * channels * BytesPerSample (bitDepth);
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (byteRate), "BYTE_RATE");
		UInt16 blockAlign = Convert.ToUInt16 (channels * BytesPerSample (bitDepth));
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (blockAlign), "BLOCK_ALIGN");
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (bitDepth), "BITS_PER_SAMPLE");
		// Validate format
		Debug.AssertFormat (count == total, "Unexpected wav fmt byte count: {0} == {1}", count, total);
		return count;
	}

	private static int WriteFileData (ref MemoryStream stream, AudioClip audioClip, UInt16 bitDepth)
	{
		int count = 0;
		int total = 8;
		// Copy float[] data from AudioClip
		float[] data = new float[audioClip.samples * audioClip.channels];
		audioClip.GetData (data, 0);
		byte[] bytes = ConvertAudioClipDataToInt16ByteArray (data);
		byte[] id = Encoding.ASCII.GetBytes ("data");
		count += WriteBytesToMemoryStream (ref stream, id, "DATA_ID");
		int subchunk2Size = Convert.ToInt32 (audioClip.samples * BlockSize_16Bit); // BlockSize (bitDepth)
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (subchunk2Size), "SAMPLES");
		// Validate header
		Debug.AssertFormat (count == total, "Unexpected wav data id byte count: {0} == {1}", count, total);
		// Write bytes to stream
		count += WriteBytesToMemoryStream (ref stream, bytes, "DATA");
		// Validate audio data
		Debug.AssertFormat (bytes.Length == subchunk2Size, "Unexpected AudioClip to wav subchunk2 size: {0} == {1}", bytes.Length, subchunk2Size);
		return count;
	}

    public static byte[] FromAudioClip (AudioClip audioClip)
	{
		string file;
		return FromAudioClip (audioClip, out file, false);
	}

	public static byte[] FromAudioClip (AudioClip audioClip, out string filepath, bool saveAsFile = true, string dirname = "recordings")
	{
		MemoryStream stream = new MemoryStream ();
		const int headerSize = 44;
		// get bit depth
		UInt16 bitDepth = 16; //BitDepth (audioClip);
		// NB: Only supports 16 bit
		// Debug.AssertFormat (bitDepth == 16, "Only converting 16 bit is currently supported. The audio clip data is {0} bit.", bitDepth);
		// total file size = 44 bytes for header format and audioClip.samples * factor due to float to Int16 / sbyte conversion
		int fileSize = audioClip.samples * BlockSize_16Bit + headerSize; // BlockSize (bitDepth)
		// chunk descriptor (riff)
		WriteFileHeader (ref stream, fileSize);
		// file header (fmt)
		WriteFileFormat (ref stream, audioClip.channels, audioClip.frequency, bitDepth);
		// data chunks (data)
		WriteFileData (ref stream, audioClip, bitDepth);
		byte[] bytes = stream.ToArray ();
		// Validate total bytes
		Debug.AssertFormat (bytes.Length == fileSize, "Unexpected AudioClip to wav format byte count: {0} == {1}", bytes.Length, fileSize);
		// Save file to persistant storage location
		if (saveAsFile) 
		{
			filepath = string.Format ("{0}/{1}/{2}.{3}", Application.persistentDataPath, dirname, DateTime.UtcNow.ToString ("yyMMdd-HHmmss-fff"), "wav");
			Directory.CreateDirectory (Path.GetDirectoryName (filepath));
			File.WriteAllBytes (filepath, bytes);
			//Debug.Log ("Auto-saved .wav file: " + filepath);
		} 
		else 
		{
			filepath = null;
		}
		stream.Dispose ();
		return bytes;
	}  
}
