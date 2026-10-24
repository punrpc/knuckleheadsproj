using UnityEngine.Networking;

public class GzipDownloadHandler : DownloadHandlerScript
{
	private byte[] _data;

	public byte[] Data => null;

	protected override byte[] GetData()
	{
		return null;
	}

	protected override bool ReceiveData(byte[] data, int dataLength)
	{
		return false;
	}

	protected override void CompleteContent()
	{
	}
}
