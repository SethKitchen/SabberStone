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
using SabberStoneCoreGui.Meta;

namespace SabberStoneGui.Deck
{
	internal interface IDeck
	{
		string Name { get; set; }
		string Link { get; set; }
		FormatType FormatType { get; set; }
		List<string> CardIds { get; set; }
	}

	public class MetaDeck : IDeck
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Link { get; set; }
		public CardClass HeroClass { get; set; }
		public FormatType FormatType { get; set; }
		public Strategy Strategy { get; set; }
		public List<string> CardIds { get; set; }
	}
}
