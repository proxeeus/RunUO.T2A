using System.Collections.Generic;

namespace Server.Mobiles
{
    public class TavernKeeper : BaseVendor 
	{ 
		private List<SBInfo> m_SBInfos = new List<SBInfo>(); 
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } } 

		[Constructable]
		public TavernKeeper() : base( "the tavern keeper" ) 
		{
			Job = JobFragment.tavernkeeper;
		} 

		public override void InitSBInfo() 
		{ 
			m_SBInfos.Add( new SBTavernKeeper() ); 
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.HalfApron() );
		}

		public TavernKeeper( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 
		} 
	} 
}