using Fusion;
using UnityEngine;

namespace Ragdoll
{
	public struct NetPress : INetworkStruct
	{
		public NetworkId avatar;

		public int bone;

		public Vector3 local;

		public Vector3 force;

		public Vector3 target;

		public NetworkBool grab;

		public NetworkBool first;

		public Vector3 root;

		public int turn;
	}
}
