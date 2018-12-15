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
using System.Linq;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Model.Entities;

namespace SabberStoneCore.Tasks.SimpleTasks
{
	public class ReplaceTask : SimpleTask
	{
		private ReplaceTask(EntityType type, Rarity rarity, Card card)
		{
			Type = type;
			Rarity = rarity;
			Card = card;
		}

		public ReplaceTask(EntityType type, Rarity rarity)
		{
			Type = type;
			Rarity = rarity;
			Card = null;
		}

		public ReplaceTask(EntityType type, string cardId)
		{
			Type = type;
			Rarity = Rarity.INVALID;
			Card = Cards.FromId(cardId);
		}

		public EntityType Type { get; set; }

		public Rarity Rarity { get; set; }

		public Card Card { get; set; }

		public override TaskState Process()
		{
			//List<IPlayable> entities = IncludeTask.GetEntities(Type, Controller, Source, Target, Playables);

			List<Card> cards = Card == null
				? Cards.All.Where(p => p.Collectible && p.Rarity == Rarity).ToList()
				: new List<Card> { Card };

			foreach (IPlayable p in IncludeTask.GetEntities(Type, Controller, Source, Target, Playables))
			{
				Model.Zones.IZone zone = p.Zone;
				Controller.SetasideZone.Add(zone.Remove(p));
				zone.Add(Entity.FromCard(Controller, cards.Count > 1 ? Util.Choose(cards) : cards.First()));
			};

			return TaskState.COMPLETE;
		}

		public override ISimpleTask Clone()
		{
			var clone = new ReplaceTask(Type, Rarity, Card);
			clone.Copy(this);
			return clone;
		}
	}

}
