using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
    public class BaseEscortable : BaseConvo
	{
		public static readonly TimeSpan EscortDelay = TimeSpan.FromMinutes( 5.0 );
		public static readonly TimeSpan AbandonDelay = TimeSpan.FromMinutes( 2.0 );
		public static readonly TimeSpan DeleteTime = TimeSpan.FromSeconds( 30 );

		private EDI m_Destination;
		private string m_DestinationString;

		private DateTime m_DeleteTime;
		private Timer m_DeleteTimer;

		private bool m_DeleteCorpse = false;

		public bool IsBeingDeleted
		{
			get { return ( m_DeleteTimer != null ); }
		}

		public override bool Commandable { get { return false; } } // Our master cannot boss us around!
		public override bool DeleteCorpseOnDeath { get { return m_DeleteCorpse; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Destination
		{
			get { return m_Destination == null ? null : m_Destination.Name; }
			set { m_DestinationString = value; m_Destination = EDI.Find(value); }
		}

		// Classic list
		// Used when: !MLQuestSystem.Enabled && !Core.ML
		private static string[] m_TownNames = new string[]
		{
			"Cove", "Britain", "Jhelom",
			"Minoc", "Ocllo", "Trinsic",
			"Vesper", "Yew", "Skara Brae",
			"Nujel'm", "Moonglow", "Magincia"
		};

		[Constructable]
		public BaseEscortable()
			: base(AIType.AI_Melee, FightMode.Aggressor, 22, 1, 0.2, 1.0)
		{
			InitBody();
			InitOutfit();

			Fame = 200;
			Karma = 4000;
		}

		public virtual void InitBody()
		{
			SetStr(90, 100);
			SetDex(90, 100);
			SetInt(15, 25);

			Hue = Utility.RandomSkinHue();

			if (Female = Utility.RandomBool())
			{
				Body = 401;
				Name = NameList.RandomName("female");
			}
			else
			{
				Body = 400;
				Name = NameList.RandomName("male");
			}
		}

		public virtual void InitOutfit()
		{
			AddItem(new FancyShirt(Utility.RandomNeutralHue()));
			AddItem(new ShortPants(Utility.RandomNeutralHue()));
			AddItem(new Boots(Utility.RandomNeutralHue()));

			Utility.AssignRandomHair(this);

			PackGold(200, 250);
		}

		public virtual bool SayDestinationTo(Mobile m)
		{
			EDI dest = GetDestination();

			if (dest == null || !m.Alive)
				return false;

			Mobile escorter = GetEscorter();

			if (escorter == null)
			{
				Say("I am looking to go to {0}, will you take me?", (dest.Name == "Ocllo" && m.Map == Map.Trammel) ? "Haven" : dest.Name);
				return true;
			}
			else if (escorter == m)
			{
				Say("Lead on! Payment will be made when we arrive in {0}.", (dest.Name == "Ocllo" && m.Map == Map.Trammel) ? "Haven" : dest.Name);
				return true;
			}

			return false;
		}

		private static Hashtable m_EscortTable = new Hashtable();

		public static Hashtable EscortTable
		{
			get { return m_EscortTable; }
		}

		public virtual bool AcceptEscorter(Mobile m)
		{
			EDI dest = GetDestination();

			if (dest == null)
				return false;

			Mobile escorter = GetEscorter();

			if (escorter != null || !m.Alive)
				return false;

			BaseEscortable escortable = (BaseEscortable)m_EscortTable[m];

			if (escortable != null && !escortable.Deleted && escortable.GetEscorter() == m)
			{
				Say("I see you already have an escort.");
				return false;
			}
			else if (m is PlayerMobile && (((PlayerMobile)m).LastEscortTime + EscortDelay) >= DateTime.Now)
			{
				int minutes = (int)Math.Ceiling(((((PlayerMobile)m).LastEscortTime + EscortDelay) - DateTime.Now).TotalMinutes);

				Say("You must rest {0} minute{1} before we set out on this journey.", minutes, minutes == 1 ? "" : "s");
				return false;
			}
			else if (SetControlMaster(m))
			{
				m_LastSeenEscorter = DateTime.Now;

				if (m is PlayerMobile)
					((PlayerMobile)m).LastEscortTime = DateTime.Now;

				Say("Lead on! Payment will be made when we arrive in {0}.", (dest.Name == "Ocllo" && m.Map == Map.Trammel) ? "Haven" : dest.Name);
				m_EscortTable[m] = this;
				StartFollow();
				return true;
			}

			return false;
		}

		public override bool HandlesOnSpeech(Mobile from)
		{
			if (from.InRange(this.Location, 3))
				return true;

			return base.HandlesOnSpeech(from);
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			base.OnSpeech(e);

			EDI dest = GetDestination();

			if (dest != null && !e.Handled && e.Mobile.InRange(this.Location, 3))
			{
				if (e.HasKeyword(0x1D)) // *destination*
					e.Handled = SayDestinationTo(e.Mobile);
				else if (e.HasKeyword(0x1E)) // *i will take thee*
					e.Handled = AcceptEscorter(e.Mobile);
			}
		}

		public override void OnAfterDelete()
		{
			if (m_DeleteTimer != null)
				m_DeleteTimer.Stop();

			m_DeleteTimer = null;

			base.OnAfterDelete();
		}

		public override void OnThink()
		{
			base.OnThink();
			CheckAtDestination();
		}

		protected override bool OnMove(Direction d)
		{
			if (!base.OnMove(d))
				return false;

			CheckAtDestination();

			return true;
		}

		// TODO: Pre-ML methods below, might be mergeable with the ML methods in EscortObjective

		public virtual void StartFollow()
		{
			StartFollow(GetEscorter());
		}

		public virtual void StartFollow(Mobile escorter)
		{
			if (escorter == null)
				return;

			ActiveSpeed = 0.1;
			PassiveSpeed = 0.2;

			ControlOrder = OrderType.Follow;
			ControlTarget = escorter;

			if ((IsPrisoner == true) && (CantWalk == true))
			{
				CantWalk = false;
			}
			CurrentSpeed = 0.1;
		}

		public virtual void StopFollow()
		{
			ActiveSpeed = 0.2;
			PassiveSpeed = 1.0;

			ControlOrder = OrderType.None;
			ControlTarget = null;

			CurrentSpeed = 1.0;
		}

		private DateTime m_LastSeenEscorter;

		public virtual Mobile GetEscorter()
		{
			if ( !Controlled )
				return null;

			Mobile master = ControlMaster;

			if (master.Deleted || master.Map != this.Map || !master.InRange(Location, 30) || !master.Alive)
			{
				StopFollow();

				TimeSpan lastSeenDelay = DateTime.Now - m_LastSeenEscorter;

				if (lastSeenDelay >= AbandonDelay)
				{
					master.SendLocalizedMessage(1042473); // You have lost the person you were escorting.
					Say(1005653); // Hmmm. I seem to have lost my master.

					SetControlMaster(null);
					m_EscortTable.Remove(master);

					Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));
					return null;
				}
				else
				{
					ControlOrder = OrderType.Stay;
					return master;
				}
			}

			if (ControlOrder != OrderType.Follow)
				StartFollow(master);

			m_LastSeenEscorter = DateTime.Now;
			return master;
		}

		public virtual void BeginDelete()
		{
			if (m_DeleteTimer != null)
				m_DeleteTimer.Stop();

			m_DeleteTime = DateTime.Now + DeleteTime;

			m_DeleteTimer = new DeleteTimer(this, m_DeleteTime - DateTime.Now);
			m_DeleteTimer.Start();
		}

		public virtual bool CheckAtDestination()
		{
			EDI dest = GetDestination();

			if (dest == null)
				return false;

			Mobile escorter = GetEscorter();

			if (escorter == null)
				return false;

			if (dest.Contains(Location))
			{
				Say(1042809, escorter.Name); // We have arrived! I thank thee, ~1_PLAYER_NAME~! I have no further need of thy services. Here is thy pay.

				// not going anywhere
				m_Destination = null;
				m_DestinationString = null;

				Container cont = escorter.Backpack;

				if (cont == null)
					cont = escorter.BankBox;

				Gold gold = new Gold(500, 1000);

				if (!cont.TryDropItem(escorter, gold, false))
					gold.MoveToWorld(escorter.Location, escorter.Map);

				StopFollow();
				SetControlMaster(null);
				m_EscortTable.Remove(escorter);
				BeginDelete();

				Misc.Titles.AwardFame(escorter, 10, true);

				return true;
			}

			return false;
		}

		public override bool OnBeforeDeath()
		{
			m_DeleteCorpse = ( Controlled || IsBeingDeleted );

			return base.OnBeforeDeath();
		}

		public BaseEscortable(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version

			EDI dest = GetDestination();

			writer.Write(dest != null);

			if (dest != null)
				writer.Write(dest.Name);

			writer.Write(m_DeleteTimer != null);

			if (m_DeleteTimer != null)
				writer.WriteDeltaTime(m_DeleteTime);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if (reader.ReadBool())
				m_DestinationString = reader.ReadString(); // NOTE: We cannot EDI.Find here, regions have not yet been loaded :-(

			if (reader.ReadBool())
			{
				m_DeleteTime = reader.ReadDeltaTime();
				m_DeleteTimer = new DeleteTimer(this, m_DeleteTime - DateTime.Now);
				m_DeleteTimer.Start();
			}
		}

		public override bool CanBeRenamedBy(Mobile from)
		{
			return (from.AccessLevel >= AccessLevel.GameMaster);
		}

		public virtual string[] GetPossibleDestinations()
		{
			return m_TownNames;
		}

		public virtual string PickRandomDestination()
		{
			if (Map.Felucca.Regions.Count == 0 || Map == null || Map == Map.Internal || Location == Point3D.Zero)
				return null; // Not yet fully initialized

			string[] possible = GetPossibleDestinations();
			string picked = null;

			while (picked == null)
			{
				picked = possible[Utility.Random(possible.Length)];
				EDI test = EDI.Find(picked);

				if (test != null && test.Contains(Location))
					picked = null;
			}

			return picked;
		}

		public EDI GetDestination()
		{
			if (m_DestinationString == null && m_DeleteTimer == null)
				m_DestinationString = PickRandomDestination();

			if (m_Destination != null && m_Destination.Name == m_DestinationString)
				return m_Destination;

			if (Map.Felucca.Regions.Count > 0)
				return (m_Destination = EDI.Find(m_DestinationString));

			return (m_Destination = null);
		}

		private class DeleteTimer : Timer
		{
			private Mobile m_Mobile;

			public DeleteTimer(Mobile m, TimeSpan delay)
				: base(delay)
			{
				m_Mobile = m;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Mobile.Delete();
			}
		}
	}

	public class EscortDestinationInfo
	{
		private string m_Name;
		private Region m_Region;
		//private Rectangle2D[] m_Bounds;

		public string Name
		{
			get { return m_Name; }
		}

		public Region Region
		{
			get { return m_Region; }
		}

		/*public Rectangle2D[] Bounds
		{
			get{ return m_Bounds; }
		}*/

		public bool Contains(Point3D p)
		{
			return m_Region.Contains(p);
		}

		public EscortDestinationInfo(string name, Region region)
		{
			m_Name = name;
			m_Region = region;
		}

		private static Hashtable m_Table;

		public static void LoadTable()
		{
			ICollection list = Map.Felucca.Regions.Values;

			if (list.Count == 0)
				return;

			m_Table = new Hashtable();

			foreach (Region r in list)
			{
				if (r.Name == null)
					continue;

				if (r is Regions.DungeonRegion || r is Regions.TownRegion)
					m_Table[r.Name] = new EscortDestinationInfo(r.Name, r);
			}
		}

		public static EDI Find(string name)
		{
			if (m_Table == null)
				LoadTable();

			if (name == null || m_Table == null)
				return null;

			return (EscortDestinationInfo)m_Table[name];
		}
	}
}