﻿#region copyright
// Copyright (C) 2017-2019 SabberStone Team, darkfriend77 & rnilva
//
// SabberStone is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License,
// or any later version.
// SabberStone is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
#endregion
using System.Collections.Generic;
using SabberStoneCore.Enums;
using SabberStoneCore.Tasks.SimpleTasks;

namespace SabberStoneCore.Model.Entities
{
	/// <summary>
	/// Entity which is a character that can reside in the <see cref="Controller.BoardZone"/> and perform
	/// certain actions (provided through <see cref="Character{Minion}"/>.
	/// </summary>
	/// <seealso cref="Character{Minion}" />
	public partial class Minion : Character<Minion>
	{
		/// <summary>Initializes a new instance of the <see cref="Minion"/> class.</summary>
		/// <param name="controller">Owner of the character; not specifically limited to players.</param>
		/// <param name="card">The card which this character embodies.</param>
		/// <param name="tags">Properties of this entity.</param>
		/// <autogeneratedoc />
		public Minion(Controller controller, Card card, IDictionary<GameTag, int> tags)
			: base(controller, card, tags)
		{
			Game.Log(LogLevel.VERBOSE, BlockType.PLAY, "Minion", !Game.Logging? "":$"{this} ({Card.Class}) was created.");
		}

		/// <summary>
		/// A copy constructor.
		/// </summary>
		/// <param name="controller">The target <see cref="Controller"/> instance.</param>
		/// <param name="minion">The source <see cref="Minion"/>.</param>
		protected Minion(Controller controller, Minion minion) : base(controller, minion) { }

		/// <summary>Character can attack.</summary>
		/// <autogeneratedoc />
		public override bool CanAttack => /*ChargeBuffed() &&*/ base.CanAttack && AttackDamage > 0;

		//private bool ChargeBuffed()
		//{
		//	if (NumAttacksThisTurn == 0 && HasCharge && IsExhausted)
		//	{
		//		IsExhausted = false;
		//	}
		//	return true;
		//}

		/// <summary>Disables all special effects on this minion.
		/// It's not possible to undo a silence!
		/// </summary>
		public void Silence()
		{
			HasTaunt = false;
			IsFrozen = false;
			IsEnraged = false;
			HasCharge = false;
			HasWindfury = false;
			HasDivineShield = false;
			HasStealth = false;
			HasDeathrattle = false;
			HasBattleCry = false;
			HasInspire = false;
			HasLifeSteal = false;
			CantBeTargetedByHeroPowers = false;
			CantBeTargetedBySpells = false;
			IsImmune = false;

			int sp = this[GameTag.SPELLPOWER];
			if (sp > 0)
			{
				Controller.CurrentSpellPower -= sp;
				this[GameTag.SPELLPOWER] = 0;
			}

			OngoingEffect?.Remove();
			Game.OneTurnEffects.RemoveAll(p => p.entityId == Id);
			ActivatedTrigger?.Remove();
			//Controller.BoardZone.Auras.ForEach(aura => aura.EntityRemoved(this));

			if (AppliedEnchantments != null)
				for (int i = AppliedEnchantments.Count - 1; i >= 0; i--)
				{
					if (AppliedEnchantments[i].Creator.Power?.Aura != null)
						continue;
					AppliedEnchantments[i].Remove();
				}

			AttackDamage = Card[GameTag.ATK];
			if (Health > Card[GameTag.HEALTH])
			{
				Health = Card[GameTag.HEALTH];
			}
			else
			{
				int cardBaseHealth = Card[GameTag.HEALTH];
				int delta = BaseHealth - cardBaseHealth;
				if (delta > 0)
					Damage -= delta;
				this[GameTag.HEALTH] = Card[GameTag.HEALTH];
			}

			if (_data.Tags.TryGetValue(GameTag.CONTROLLER_CHANGED_THIS_TURN, out int v) && v > 0)
			{
				Game.TaskQueue.Execute(new ControlTask(EntityType.SOURCE, true), Controller, this, null);
				this[GameTag.CONTROLLER_CHANGED_THIS_TURN] = 0;
			}

			if (_history && Card[GameTag.TRIGGER_VISUAL] == 1) this[GameTag.TRIGGER_VISUAL] = 0;

			IsSilenced = true;

			Game.Log(LogLevel.INFO, BlockType.PLAY, "Minion", !Game.Logging? "":$"{this} got silenced!");
		}

		public override void Reset()
		{
			base.Reset();
			OngoingEffect?.Remove();
			Game.OneTurnEffects.RemoveAll(p => p.entityId == Id);
			if (ToBeDestroyed)
			{
				Game.DeadMinions.Remove(OrderOfPlay);
				ToBeDestroyed = false;
			}
		}

		/// <summary>
		/// Gets the Minions adjacent to this Minion in order from left to right.
		/// </summary>
		/// <param name="includeUntouchables">true if the result should contain Untouchable entities.</param>
		public Minion[] GetAdjacentMinions(bool includeUntouchables = false)
		{
			int pos = ZonePosition;

			if (includeUntouchables)
			{
				if (pos > 0)
				{
					if (pos < Controller.BoardZone.Count - 1)
					{
						var arr = new Minion[2];
						arr[0] = Controller.BoardZone[pos - 1];
						arr[1] = Controller.BoardZone[pos + 1];
						return arr;
					}
					return new[] { Controller.BoardZone[pos - 1] };
				}
				return pos < Controller.BoardZone.Count - 1 ?
					new[] { Controller.BoardZone[pos + 1] } :
					new Minion[0];
			}


			if (pos > 0)
			{
				Minion left;
				Minion right;
				if (pos < Controller.BoardZone.Count - 1)
				{
					left = Controller.BoardZone[pos - 1];
					right = Controller.BoardZone[pos + 1];
					return left.Untouchable
						? (right.Untouchable ? new Minion[0] : new[] {right})
						: (right.Untouchable ? new[] {left} : new[] {left, right});
				}

				left = Controller.BoardZone[pos - 1];
				return left.Untouchable ? new Minion[0] : new [] {left};
			}

			if (pos < Controller.BoardZone.Count - 1)
			{
				Minion r = Controller.BoardZone[pos + 1];
				return r.Untouchable ? new Minion[0] : new[] {r};
			}

			return new Minion[0];
		}

		/// <summary>
		/// Gets a value indicating whether this entity is playable by the player. Some entities require specific
		/// requirements before they can be played. This method will process the requirements and produce
		/// a result for the current state of the game.
		/// </summary>
		/// <value><c>true</c> if this entity is playable; otherwise, <c>false</c>.</value>
		public override bool IsPlayableByPlayer
		{
			get
			{
				// check if we got a slot on board for minions
				if (Controller.BoardZone.IsFull)
				{
					Game.Log(LogLevel.VERBOSE, BlockType.PLAY, "Playable",
						!Game.Logging? "":$"{this} isn't playable, because not enough place on board.");
					return false;
				}

				return base.IsPlayableByPlayer;
			}
		}

		public override IPlayable Clone(Controller controller)
		{
			return new Minion(controller, this);
		}
	}

	public partial class Minion
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	{
		public bool HasCharge
		{
			get => this[GameTag.CHARGE] >= 1;
			set
			{
				if (value)
				{
					if (IsExhausted && NumAttacksThisTurn == 0)
						IsExhausted = false;
					this[GameTag.CHARGE] = 1;
					return;
				}

				this[GameTag.CHARGE] = 0;
			}
		}

		public bool HasDivineShield
		{
			get { return this[GameTag.DIVINE_SHIELD] == 1; }
			set
			{
				if (!value)
				{
					if (this[GameTag.DIVINE_SHIELD] == 1)
						Game.TriggerManager.OnLoseDivineShield(this);
					this[GameTag.DIVINE_SHIELD] = 0;
					return;
				}

				this[GameTag.DIVINE_SHIELD] = 1;
			}
		}

		public bool HasBattleCry
		{
			get { return Card[GameTag.BATTLECRY] != 0; }
			set { this[GameTag.BATTLECRY] = value ? 1 : 0; }
		}

		public bool HasInspire
		{
			get { return this[GameTag.INSPIRE] == 1; }
			set { this[GameTag.INSPIRE] = value ? 1 : 0; }
		}

		public bool IsEnraged
		{
			get { return this[GameTag.ENRAGED] == 1; }
			set { this[GameTag.ENRAGED] = value ? 1 : 0; }
		}

		public bool Freeze
		{
			get { return this[GameTag.FREEZE] == 1; }
			set { this[GameTag.FREEZE] = value ? 1 : 0; }
		}

		public bool Poisonous
		{
			get { return this[GameTag.POISONOUS] == 1; }
			set { this[GameTag.POISONOUS] = value ? 1 : 0; }
		}

		public bool Untouchable => Card.Untouchable;

		public bool HasRush => this[GameTag.RUSH] == 1;

		public bool AttackableByRush
		{
			get => _data.Tags.Contains(new KeyValuePair<GameTag, int>(GameTag.ATTACKABLE_BY_RUSH, 1));
			set => this[GameTag.ATTACKABLE_BY_RUSH] = value ? 1 : 0;
		} 

		public int LastBoardPosition
		{
			get { return GetNativeGameTag(GameTag.TAG_LAST_KNOWN_POSITION_ON_BOARD); }
			set { this[GameTag.TAG_LAST_KNOWN_POSITION_ON_BOARD] = value; }
		}

		public override bool ToBeDestroyed
		{
			get => base.ToBeDestroyed;

			set
			{
				if (value == base.ToBeDestroyed)
					return;
				base.ToBeDestroyed = value;
				if (value)
				{
					Game.DeadMinions.Add(OrderOfPlay, this);
				}
			} 
		}
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
