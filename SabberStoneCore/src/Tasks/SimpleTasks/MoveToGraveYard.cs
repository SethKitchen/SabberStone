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
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Enums;

namespace SabberStoneCore.Tasks.SimpleTasks
{
	public class MoveToGraveYard : SimpleTask
	{
		public MoveToGraveYard(EntityType type)
		{
			Type = type;
		}

		public EntityType Type { get; set; }

		public override TaskState Process()
		{
			//List<IPlayable> entities = IncludeTask.GetEntities(Type, Controller, Source, Target, Playables);
			//entities.ForEach(p =>
			foreach (IPlayable p in IncludeTask.GetEntities(Type, Controller, Source, Target, Playables))
			{
				p.Controller.GraveyardZone.Add(p.Zone.Remove(p));
				if (p.Card.IsSecret && p[GameTag.REVEALED] == 1)
					Game.TriggerManager.OnSecretRevealedTrigger(p);
			};
			return TaskState.COMPLETE;
		}

		public override ISimpleTask Clone()
		{
			var clone = new MoveToGraveYard(Type);
			clone.Copy(this);
			return clone;
		}
	}
}
