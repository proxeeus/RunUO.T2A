using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Server.SkillHandlers
{
    public class AnimalLore
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.AnimalLore].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse(Mobile m)
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 500328 ); // What animal should I look at?

			return TimeSpan.FromSeconds( 1.0 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 8, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( !from.Alive )
				{
					from.SendLocalizedMessage( 500331 ); // The spirits of the dead are not the province of animal lore.
				}
				else if ( targeted is BaseCreature )
				{
					BaseCreature c = (BaseCreature)targeted;

					if ( c.Body.IsAnimal || c.Body.IsMonster || c.Body.IsSea )
					{
						if ( !c.Controlled && from.Skills[SkillName.AnimalLore].Value < 100.0 )
						{
							from.SendLocalizedMessage( 1049674 ); // At your skill level, you can only lore tamed creatures.
						}
						else if ( !c.Controlled && !c.Tamable && from.Skills[SkillName.AnimalLore].Value < 110.0 )
						{
							from.SendLocalizedMessage( 1049675 ); // At your skill level, you can only lore tamed or tameable creatures.
						}
						else if ( !from.CheckTargetSkill( SkillName.AnimalLore, c, 0.0, 120.0 ) )
						{
							from.SendLocalizedMessage( 500334 ); // You can't think of anything you know offhand.
						}
						else
						{
							from.CloseGump( typeof( AnimalLoreGump ) );
							from.SendGump( new AnimalLoreGump( c ) );
						}
					}
					else
					{
						from.SendLocalizedMessage( 500329 ); // That's not an animal!
					}
				}
				else
				{
					from.SendLocalizedMessage( 500329 ); // That's not an animal!
				}
			}
		}
	}

	public class AnimalLoreGump : Gump
	{
		private static string FormatSkill( BaseCreature c, SkillName name )
		{
			Skill skill = c.Skills[name];

			if ( skill.Base < 10.0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0:F1}</div>", skill.Value );
		}

		private static string FormatAttributes( int cur, int max )
		{
			if ( max == 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0}/{1}</div>", cur, max );
		}

		private static string FormatStat( int val )
		{
			if ( val == 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0}</div>", val );
		}

		private static string FormatDouble( double val )
		{
			if ( val == 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0:F1}</div>", val );
		}

		private static string FormatElement( int val )
		{
			if ( val <= 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0}%</div>", val );
		}

		#region Mondain's Legacy
		private static string FormatDamage( int min, int max )
		{
			if ( min <= 0 || max <= 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0}-{1}</div>", min, max );
		}
		#endregion

		private const int LabelColor = 0x24E5;

		public AnimalLoreGump( BaseCreature c ) : base( 250, 50 )
		{
			AddPage( 0 );

			AddImage( 100, 100, 2080 );
			AddImage( 118, 137, 2081 );
			AddImage( 118, 207, 2081 );
			AddImage( 118, 277, 2081 );
			AddImage( 118, 347, 2083 );

			AddHtml( 147, 108, 210, 18, String.Format( "<center><i>{0}</i></center>", c.Name ), false, false );

			AddButton( 240, 77, 2093, 2093, 2, GumpButtonType.Reply, 0 );

			AddImage( 140, 138, 2091 );
			AddImage( 140, 335, 2091 );

			int pages = 3;
			int page = 0;


			#region Attributes
			AddPage( ++page );

			AddImage( 128, 152, 2086 );
			AddHtmlLocalized( 147, 150, 160, 18, 1049593, 200, false, false ); // Attributes

			AddHtmlLocalized( 153, 168, 160, 18, 1049578, LabelColor, false, false ); // Hits
			AddHtml( 280, 168, 75, 18, FormatAttributes( c.Hits, c.HitsMax ), false, false );

			AddHtmlLocalized( 153, 186, 160, 18, 1049579, LabelColor, false, false ); // Stamina
			AddHtml( 280, 186, 75, 18, FormatAttributes( c.Stam, c.StamMax ), false, false );

			AddHtmlLocalized( 153, 204, 160, 18, 1049580, LabelColor, false, false ); // Mana
			AddHtml( 280, 204, 75, 18, FormatAttributes( c.Mana, c.ManaMax ), false, false );

			AddHtmlLocalized( 153, 222, 160, 18, 1028335, LabelColor, false, false ); // Strength
			AddHtml( 320, 222, 35, 18, FormatStat( c.Str ), false, false );

			AddHtmlLocalized( 153, 240, 160, 18, 3000113, LabelColor, false, false ); // Dexterity
			AddHtml( 320, 240, 35, 18, FormatStat( c.Dex ), false, false );

			AddHtmlLocalized( 153, 258, 160, 18, 3000112, LabelColor, false, false ); // Intelligence
			AddHtml( 320, 258, 35, 18, FormatStat( c.Int ), false, false );

			AddImage( 128, 278, 2086 );
			AddHtmlLocalized( 147, 276, 160, 18, 3001016, 200, false, false ); // Miscellaneous

			AddHtmlLocalized( 153, 294, 160, 18, 1049581, LabelColor, false, false ); // Armor Rating
			AddHtml( 320, 294, 35, 18, FormatStat( c.VirtualArmor ), false, false );

			AddButton( 340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1 );
			AddButton( 317, 358, 5603, 5607, 0, GumpButtonType.Page, pages );
			#endregion

			#region Skills
			AddPage( ++page );

			AddImage( 128, 152, 2086 );
			AddHtmlLocalized( 147, 150, 160, 18, 3001030, 200, false, false ); // Combat Ratings

			AddHtmlLocalized( 153, 168, 160, 18, 1044103, LabelColor, false, false ); // Wrestling
			AddHtml( 320, 168, 35, 18, FormatSkill( c, SkillName.Wrestling ), false, false );

			AddHtmlLocalized( 153, 186, 160, 18, 1044087, LabelColor, false, false ); // Tactics
			AddHtml( 320, 186, 35, 18, FormatSkill( c, SkillName.Tactics ), false, false );

			AddHtmlLocalized( 153, 204, 160, 18, 1044086, LabelColor, false, false ); // Magic Resistance
			AddHtml( 320, 204, 35, 18, FormatSkill( c, SkillName.MagicResist ), false, false );

			AddHtmlLocalized( 153, 222, 160, 18, 1044061, LabelColor, false, false ); // Anatomy
			AddHtml( 320, 222, 35, 18, FormatSkill( c, SkillName.Anatomy ), false, false );

			AddImage( 128, 260, 2086 );
			AddHtmlLocalized( 147, 258, 160, 18, 3001032, 200, false, false ); // Lore & Knowledge

			AddHtmlLocalized( 153, 276, 160, 18, 1044085, LabelColor, false, false ); // Magery
			AddHtml( 320, 276, 35, 18, FormatSkill( c, SkillName.Magery ), false, false );

			AddHtmlLocalized( 153, 294, 160, 18, 1044076, LabelColor, false, false ); // Evaluating Intelligence
			AddHtml( 320, 294, 35, 18,FormatSkill( c, SkillName.EvalInt ), false, false );

			AddHtmlLocalized( 153, 312, 160, 18, 1044106, LabelColor, false, false ); // Meditation
			AddHtml( 320, 312, 35, 18, FormatSkill( c, SkillName.Meditation ), false, false );

			AddButton( 340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1 );
			AddButton( 317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1 );
			#endregion

			#region Misc
			AddPage( ++page );

			AddImage( 128, 152, 2086 );
			AddHtmlLocalized( 147, 150, 160, 18, 1049563, 200, false, false ); // Preferred Foods

			int foodPref = 3000340;

			if ( (c.FavoriteFood & FoodType.FruitsAndVegies) != 0 )
				foodPref = 1049565; // Fruits and Vegetables
			else if ( (c.FavoriteFood & FoodType.GrainsAndHay) != 0 )
				foodPref = 1049566; // Grains and Hay
			else if ( (c.FavoriteFood & FoodType.Fish) != 0 )
				foodPref = 1049568; // Fish
			else if ( (c.FavoriteFood & FoodType.Meat) != 0 )
				foodPref = 1049564; // Meat
			else if ( (c.FavoriteFood & FoodType.Eggs) != 0 )
				foodPref = 1044477; // Eggs

			AddHtmlLocalized( 153, 168, 160, 18, foodPref, LabelColor, false, false );

			AddImage( 128, 224, 2086 );
			AddHtmlLocalized( 147, 222, 160, 18, 1049594, 200, false, false ); // Loyalty Rating

			AddHtmlLocalized( 153, 240, 160, 18, !c.Controlled || c.Loyalty == 0 ? 1061643 : 1049595 + c.Loyalty / 10, LabelColor, false, false );

			AddButton( 340, 358, 5601, 5605, 0, GumpButtonType.Page, 1 );
			AddButton( 317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1 );
			#endregion
		}
	}
}