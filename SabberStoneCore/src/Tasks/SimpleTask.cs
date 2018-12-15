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
using System;
using System.Collections.Generic;
using SabberStoneCore.Model;
using SabberStoneCore.Model.Entities;

namespace SabberStoneCore.Tasks
{
	public interface ISimpleTask
	{
		TaskState State { get; set; }
		//ISimpleTask CurrentTask { get; }

		Game Game { get; set; }
		Controller Controller { get; set; }
		IEntity Source { get; set; }
		IEntity Target { get; set; }

		List<IPlayable> Playables { get; set; }
		//List<string> CardIds { get; set; }
		bool Flag { get; set; }
		int Number { get; set; }
		int Number1 { get; set; }
		int Number2 { get; set; }
		int Number3 { get; set; }
		int Number4 { get; set; }

		//List<Game> Splits { get; set; }
		//IEnumerable<IEnumerable<IPlayable>> Sets { get; set; }

		ISimpleTask Clone();

		TaskState Process();

		//void ResetState();

		bool IsTrigger { get; set; }
	}

	public abstract class SimpleTask : ISimpleTask
	{
		internal static Random Random => Util.Random;

		public TaskState State { get; set; } = TaskState.READY;

		//public ISimpleTask CurrentTask => this;

		public Game Game { get; set; }
		private int _controllerId;
		public Controller Controller
		{
			get { return Game.ControllerById(_controllerId); }
			set { _controllerId = value.Id; }
		}

		private int _sourceId;
		public IEntity Source
		{
			get { return Game.IdEntityDic[_sourceId]; }
			set { _sourceId = value.Id; }
		}

		private int _targetId;
		public IEntity Target
		{
			get { return _targetId > -1 ? Game.IdEntityDic[_targetId] : null; }
			set { _targetId = value?.Id ?? -1; }
		}

		//public List<IPlayable> Playables { get; set; }
		public List<IPlayable> Playables
		{
			get { return Game.TaskStack.Playables; }
			set { Game.TaskStack.Playables = value; }
		}
		//public List<string> CardIds
		//{
		//	get { return Game.TaskStack.CardIds; }
		//	set { Game.TaskStack.CardIds = value; }
		//}
		//public bool Flag { get; set; }
		public bool Flag
		{
			get { return Game.TaskStack.Flag; }
			set { Game.TaskStack.Flag = value; }
		}
		//public int Number { get; set; }
		public int Number
		{
			get { return Game.TaskStack.Numbers[0]; }
			set { Game.TaskStack.Numbers[0] = value; }
		}
		public int Number1
		{
			get { return Game.TaskStack.Numbers[1]; }
			set { Game.TaskStack.Numbers[1] = value; }
		}
		public int Number2
		{
			get { return Game.TaskStack.Numbers[2]; }
			set { Game.TaskStack.Numbers[2] = value; }
		}
		public int Number3
		{
			get { return Game.TaskStack.Numbers[3]; }
			set { Game.TaskStack.Numbers[3] = value; }
		}
		public int Number4
		{
			get { return Game.TaskStack.Numbers[4]; }
			set { Game.TaskStack.Numbers[4] = value; }
		}
		public abstract TaskState Process();
		//{
		//    return TaskState.COMPLETE;
		//}

		public abstract ISimpleTask Clone();

		public void Copy(SimpleTask task)
		{
			State = task.State;

			if (task.Game == null)
				return;

			Game = task.Game;
			Controller = task.Controller;
			Source = task.Source;
			Target = task.Target;

			//Playables = task.Playables;
			//CardIds = task.CardIds;
			//Flag = task.Flag;
			//Number = task.Number;
			//Number1 = task.Number1;
			//Number2 = task.Number2;
			//Number3 = task.Number3;
			//Number4 = task.Number4;

			//Splits = task.Splits;
			//Sets = task.Sets;
		}

		public void ResetState()
		{
			State = TaskState.READY;
		}

		public bool IsTrigger { get; set; }

		public override string ToString()
		{
			return GetType().Name;
		}
	}
}
