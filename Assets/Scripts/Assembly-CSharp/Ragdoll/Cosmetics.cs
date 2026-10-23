using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ragdoll
{
	[Serializable]
	public class Cosmetics
	{
		[Serializable]
		public class Item
		{
			public string name;

			public GameObject[] parts;
		}

		public static readonly string[] Slots = { "Head", "Torso", "Legs", "Face" };

		public const int Face = 3;

		public List<Item> head = new List<Item>();

		public List<Item> torso = new List<Item>();

		public List<Item> legs = new List<Item>();

		public List<Item> face = new List<Item>();

		[NonSerialized]
		public string[] worn = { "", "", "", "" };

		[NonSerialized]
		public Color color = Color.white;

		private Material skin;

		private const float clothShade = 0.85f;

		private MaterialPropertyBlock block;

		public List<Item> List(int slot)
		{
			switch (slot)
			{
				case 0:
					return head;
				case 1:
					return torso;
				case 2:
					return legs;
				default:
					return face;
			}
		}

		public bool Move(string[] outfit)
		{
			if (outfit[0] == "" || Has(0, outfit[0]))
			{
				return false;
			}
			if (!Has(Face, outfit[0]) || outfit[Face] != "")
			{
				return false;
			}
			outfit[Face] = outfit[0];
			outfit[0] = "";
			return true;
		}

		public bool Has(int slot, string name)
		{
			foreach (Item item in List(slot))
			{
				if (item.name == name)
				{
					return true;
				}
			}
			return false;
		}

		public void Wear(int slot, string name)
		{
			worn[slot] = Has(slot, name) ? name : "";
			foreach (Item item in List(slot))
			{
				bool on = item.name == worn[slot];
				foreach (GameObject part in item.parts)
				{
					if (part && part.activeSelf != on)
					{
						part.SetActive(on);
					}
				}
			}
		}

		public string Step(int slot, string from, int step)
		{
			List<Item> list = List(slot);
			int at = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].name == from)
				{
					at = i + 1;
				}
			}
			int count = list.Count + 1;
			int next = ((at + step) % count + count) % count;
			return next == 0 ? "" : list[next - 1].name;
		}

		public void Tint(Transform body, Color c)
		{
			color = c;
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			foreach (SkinnedMeshRenderer r in body.GetComponentsInChildren<SkinnedMeshRenderer>(true))
			{
				if (Owns(r.transform))
				{
					continue;
				}
				if (!skin)
				{
					skin = r.sharedMaterial;
				}
				if (r.sharedMaterial != skin)
				{
					continue;
				}
				r.GetPropertyBlock(block);
				block.SetColor("_BaseColor", c);
				r.SetPropertyBlock(block);
			}
			Color cloth = new Color(c.r * clothShade, c.g * clothShade, c.b * clothShade, c.a);
			for (int slot = 1; slot < 3; slot++)
			{
				foreach (Item item in List(slot))
				{
					foreach (GameObject part in item.parts)
					{
						if (!part)
						{
							continue;
						}
						foreach (Renderer r in part.GetComponentsInChildren<Renderer>(true))
						{
							Material[] mats = r.sharedMaterials;
							SkinnedMeshRenderer skinned = r as SkinnedMeshRenderer;
							bool gradient = skinned && skinned.sharedMesh && skinned.sharedMesh.HasVertexAttribute(VertexAttribute.TexCoord3);
							for (int i = 0; i < mats.Length; i++)
							{
								if (!mats[i] || !mats[i].HasProperty("_TopColor"))
								{
									continue;
								}
								if (mats[i].HasProperty("_KeepColor") && mats[i].GetFloat("_KeepColor") > 0.5f)
								{
									continue;
								}
								r.GetPropertyBlock(block, i);
								block.SetColor("_TopColor", cloth);
								block.SetColor("_BottomColor", cloth);
								block.SetFloat("_MeshGradient", gradient ? 1f : 0f);
								if (gradient)
								{
									block.SetFloat("_Bottom", 0f);
									block.SetFloat("_Top", 1f);
								}
								r.SetPropertyBlock(block, i);
							}
						}
					}
				}
			}
		}

		public void NearClip(float radius)
		{
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			for (int slot = 1; slot < 3; slot++)
			{
				foreach (Item item in List(slot))
				{
					foreach (GameObject part in item.parts)
					{
						if (!part)
						{
							continue;
						}
						foreach (Renderer r in part.GetComponentsInChildren<Renderer>(true))
						{
							Material[] mats = r.sharedMaterials;
							for (int i = 0; i < mats.Length; i++)
							{
								if (!mats[i] || !mats[i].HasProperty("_CamClip"))
								{
									continue;
								}
								r.GetPropertyBlock(block, i);
								block.SetFloat("_CamClip", radius);
								r.SetPropertyBlock(block, i);
							}
						}
					}
				}
			}
		}

		public bool Owns(Transform t)
		{
			for (int slot = 0; slot < Slots.Length; slot++)
			{
				foreach (Item item in List(slot))
				{
					foreach (GameObject part in item.parts)
					{
						if (part && t.IsChildOf(part.transform))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
