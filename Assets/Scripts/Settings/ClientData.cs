using System;
using UnityEngine;
using SibGameJam2026.Characters;

namespace SibGameJam2026.Settings {
	[Serializable]
	public class ClientData {
		[field: SerializeField] public ECharacterType ECharacterType { get; private set; }
		[field: SerializeField] public int ItemId { get; private set; }
	}
}
