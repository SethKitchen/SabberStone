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
using SabberStoneCore.Model.Entities;

namespace SabberStoneCore.Tasks.SimpleTasks
{
    public class ExtraTurnEffectTask : SimpleTask
    {
	    //private readonly bool _opposite;

	    public ExtraTurnEffectTask(/*bool opposite = false*/)
	    {
		    //_opposite = opposite;
	    }

	    public override TaskState Process()
	    {
		    Controller c = /*_opposite ? Controller.Opponent :*/ Controller;

		    if (c == Game.CurrentPlayer)
			    c.NumTurnsLeft++;
		    else
			    throw new NotImplementedException();

			return TaskState.COMPLETE;
	    }

	    public override ISimpleTask Clone()
	    {
		    return new ExtraTurnEffectTask(/*_opposite*/);
	    }
    }
}
