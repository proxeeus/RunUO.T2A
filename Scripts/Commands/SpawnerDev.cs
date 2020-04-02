using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server.Mobiles;
using Server.Commands;

// Version 0.8

namespace Server.Commands
{
    public class SpawnerDev
	{
		private static int m_Count;

		public static void Initialize()
		{
			CommandSystem.Register( "SpawnerDev", AccessLevel.Administrator, new CommandEventHandler( Generate_OnCommand ) );
		}

		[Usage( "SpawnerDev" )]
		[Description( "Dev version of the dynamic spawn tool." )]
		private static void Generate_OnCommand( CommandEventArgs e )
		{
			if( e.Length == 0 )
			{
				e.Mobile.SendMessage("You need to provide an action as well as a valid .map name.");
				return;
			}
			Parse( e.Mobile, e.GetString( 0 ).ToLower(), e.GetString( 1 ).ToLower() );
		}

		public static void Parse( Mobile from, string action, string spawnFile )
		{
			string vendor_path = Path.Combine( Core.BaseDirectory, string.Format("Data/{0}.map", spawnFile) );
			m_Count = 0;

			if ( File.Exists( vendor_path ) )
			{
				ArrayList list = new ArrayList();
				if(action == "add")
					from.SendMessage( "Generating Spawns..." );
				else if(action == "delete")
					from.SendMessage( "Deleting Spawns..." );

				using ( StreamReader ip = new StreamReader( vendor_path ) )
				{
					string line;

					while ( (line = ip.ReadLine()) != null )
					{
						if (line.StartsWith("//")) continue;

						if (line.StartsWith("-")) continue;

						var indexOf = line.IndexOf(':');
						var mobIndexOf = line.IndexOf("[");
						var mobLastIndexOf = line.IndexOf("]");
						if (indexOf == -1)  continue;

						var spawnIndexOf = line.IndexOf('+');
						if (spawnIndexOf == -1) continue;

						var sub = line.Substring(++indexOf).Trim();
						var spawnName = line.Substring(++spawnIndexOf, mobIndexOf - spawnIndexOf);

						var mobs = line.Substring(++mobIndexOf, mobLastIndexOf - mobIndexOf);
						var split = sub.Split(' ');
						var mobSplit = mobs.Split(',');
						var uniqueSpawn = true;
						if( mobSplit.Length == 1 && mobSplit[0].Contains("|"))
						{
							mobSplit = mobs.Split('|');
							if(mobSplit != null)
								uniqueSpawn = false;
						}

						var mobsList = new List<string>();
						foreach (var mob in mobSplit)
							mobsList.Add(mob.Trim());

						if (split.Length < 3)
							continue;
						if(action == "add")
							PlaceNPC( split[0], split[1], split[2], mobsList, split[3], split[4], split[5], double.Parse(split[6],System.Globalization.CultureInfo.InvariantCulture), double.Parse(split[7],System.Globalization.CultureInfo.InvariantCulture), split[8], uniqueSpawn);
						else if(action == "delete")
						{
							// TODO: need to account for both maps :/
							Map map;
							switch ( Utility.ToInt32(split[2]) )
							{	
								case 0://Trammel and Felucca
									map = Map.Felucca;
									break;
								case 1: // Felucca
									map = Map.Felucca;
									break;
								default:
									map = Map.Felucca;
									break;
							}
							ClearSpawners( Utility.ToInt32(split[0]), Utility.ToInt32(split[1]), GetSpawnerZ( Utility.ToInt32(split[0]), Utility.ToInt32(split[1]), map ), map );
						}	
					}
				}
				if(action == "add")
					from.SendMessage( "Done, added {0} spawners",m_Count );
				else if(action == "delete")
					from.SendMessage( "Done, all spawners have been removed.",m_Count );
			}
			else
			{
				from.SendMessage( "{0} not found!", vendor_path );
			}
		}

		public static void PlaceNPC( string sx, string sy, string sm, List<string> types, string NPCCount, string HomeRange, string BringToHome, double MinTime, double MaxTime, string Team, bool UniqueSpawn )
		{
			if ( types.Count == 0 )
				return;

			int x = Utility.ToInt32( sx );
			int y = Utility.ToInt32( sy );
			int map = Utility.ToInt32( sm );
			int npcCount = Utility.ToInt32( NPCCount );
			int homeRange = Utility.ToInt32( HomeRange );
			bool bringToHome = Convert.ToBoolean( BringToHome );
			TimeSpan minTime = TimeSpan.FromMinutes( MinTime );
			TimeSpan maxTime = TimeSpan.FromMinutes ( MaxTime );
			int team = Utility.ToInt32( Team );
			bool uniqueSpawn = UniqueSpawn;
			
			switch ( map )
			{
				case 0://Trammel and Felucca
					MakeSpawner( types, x, y, Map.Felucca, npcCount, homeRange, bringToHome, minTime, maxTime, team, uniqueSpawn );
					break;
				case 1://Felucca
					MakeSpawner( types, x, y, Map.Felucca, npcCount, homeRange, bringToHome, minTime, maxTime, team , uniqueSpawn );
					break;
				default:
					Console.WriteLine( "UOAM Vendor Parser: Warning, unknown map {0}", map );
					break;
			}
		}

		private static void MakeSpawner( List<string> types, int x, int y, Map map, int npcCount, int homeRange, bool bringToHome, TimeSpan minTime, TimeSpan maxTime, int team, bool uniqueSpawn )
		{
			if ( types.Count == 0 )
				return;

			int z = GetSpawnerZ( x, y, map );

			ClearSpawners( x, y, z, map );

			if(!uniqueSpawn)
			{
				bool isGuildmaster = types[0].EndsWith( "Guildmaster" );
				Spawner sp = new Spawner (  );
				
				if ( isGuildmaster )
					sp.Count = 1;
				else
					sp.Count = npcCount;
				
				foreach(var name in types)
					sp.SpawnNames.Add(name);
					
				sp.MinDelay = minTime;
				sp.MaxDelay = maxTime;
				sp.Team = team;
				sp.HomeRange = homeRange;

				sp.MoveToWorld( new Point3D( x, y, z ), map );

				sp.Respawn();
				if ( bringToHome )
				{
					sp.BringToHome();
				}
				++m_Count;
			}
			else
			{
				for ( int i = 0; i < types.Count; ++i )
				{
					bool isGuildmaster = types[i].EndsWith( "Guildmaster" );

					Spawner sp = new Spawner( types[i] );

					if ( isGuildmaster )
						sp.Count = 1;
					else
						sp.Count = npcCount;

					sp.MinDelay = minTime;
					sp.MaxDelay = maxTime;
					sp.Team = team;
					sp.HomeRange = homeRange;

					sp.MoveToWorld( new Point3D( x, y, z ), map );

					sp.Respawn();
					if ( bringToHome )
					{
						sp.BringToHome();
					}

					++m_Count;
				}
			}
		}
		
		public static int GetSpawnerZ( int x, int y, Map map )
		{
			int z = map.GetAverageZ( x, y );

			if ( map.CanFit( x, y, z, 16, false, false, true ) )
				return z;

			for ( int i = 1; i <= 20; ++i )
			{
				if ( map.CanFit( x, y, z + i, 16, false, false, true ) )
					return z + i;

				if ( map.CanFit( x, y, z - i, 16, false, false, true ) )
					return z - i;
			}

			return z;
		}

		private static Queue m_ToDelete = new Queue();

		public static void ClearSpawners( int x, int y, int z, Map map )
		{
			IPooledEnumerable eable = map.GetItemsInRange( new Point3D( x, y, z ), 0 );

			foreach ( Item item in eable )
			{
				if ( item is Spawner && item.Z == z )
					m_ToDelete.Enqueue( item );
			}

			eable.Free();

			while ( m_ToDelete.Count > 0 )
				((Item)m_ToDelete.Dequeue()).Delete();
		}
	}
}
