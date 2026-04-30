using System;
using System.Collections.Generic;
using SibGameJam2026.Characters.Components;
using SibGameJam2026.Services;
using Zenject;

namespace SibGameJam2026.Characters {
	public class CharacterFactory : IFactory<ECharacterType, ACharacter> {
		private readonly CharactersDatabase _charactersDatabase;
		private readonly IInputService _inputService;

		public CharacterFactory(CharactersDatabase charactersDatabase, IInputService inputService) {
			_charactersDatabase = charactersDatabase;
			_inputService = inputService;
		}

		public ACharacter Create(ECharacterType eCharacterType) {
			var entry = _charactersDatabase.GetEntry(eCharacterType);
			var prefabInstance = entry.CharacterPrefab.InstantiateAsync().WaitForCompletion();
			if (prefabInstance == null) {
				throw new InvalidOperationException($"Failed to instantiate prefab for character type {eCharacterType}");
			}

			var character = prefabInstance.GetComponent<ACharacter>();
			if (character == null) {
				throw new InvalidOperationException($"Prefab for character type {eCharacterType} does not contain {nameof(ACharacter)}");
			}

			character.Initialize(CreateComponents(entry, character));
			return character;
		}

		private IReadOnlyDictionary<Type, ICharacterComponent> CreateComponents(CharacterEntry entry, ACharacter character) {
			return entry.ECharacterType switch {
				ECharacterType.Player => new Dictionary<Type, ICharacterComponent> {
					{ typeof(IHealthCharacterComponent), new HealthCharacterComponent(character, entry.Health) },
					{ typeof(IMovementCharacterComponent), new MovementCharacterComponent(character, entry.MoveSpeed) },
					{ typeof(IInputCharacterComponent), new InputCharacterComponent(character, _inputService) }
				},
				_ => throw new ArgumentOutOfRangeException(nameof(entry.ECharacterType), entry.ECharacterType, "Unknown character type")
			};
		}
	}
}
