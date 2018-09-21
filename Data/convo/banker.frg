//Britannia Banker Fragment				    
//Notes:  This is used for general information pertaining to all Britannian minters
//Current Keyword List:
//   job
//   
//Additional Keywords:
//Revision Date:  4/14/97
//Author:  DAB

//!!Directions to mint
//!!Talk about gold as a resource

#Fragment Britannia, Job, Britannia_Banker 
{
	#Sophistication High 
	{
		#Key "*job*", "*work*", "*what*do*do*", "*profession*", "*occupation*" 
		{
			"I am a Banker, $milord/milady$.",
			"I handle the money transactions for _Town_, $milord/milady$.",
			"I am the local banker."
                }
		#Key "*Bank*" 
		{
			"Banking is a satisfactory experience.",
			"We have no history of bank robberies at this branch.",
			"My mother wanted me to adventure, but I find banking much more satisfying."
                }
		#Key "*rob*" "*steal*" "*hold up*" 
		{
			"You see, once a client's money.is deposited, no one can get to it. Not even me.",
			"A thief could kill me and I still couldn't give any money away.",
			"I can't give any money away unless a client specifically asks for a withdrawal."
                }
		#Key "*coin*", "*money*", "*currency*", "*deposit*", "*transaction*", "*account*"   
		{
		 	"Thou can certainly leave thy money with us. 'Tis safer than in thine own pockets, I assure thee.",
			"Thou art welcome to open an account at any time that we are open.",
			"Just give thy money to me, _Name_, and I shall take care of it for thee.",
			"No one shall take thy gold as long as it is in our hands,$milord/milady$."
		}
		#Key "*account*"
		{
			"If thou dost need to withdraw money from thy account, just say withdraw and the amount that thou dost need.",
			"To find thy balance, ask for a statement or for thy balance."
		}	 
		#Key "*gold*", "*silver*", "*copper*" 
		{
			"Gold is the only metal we use when minting our coins.",
			"Silver and copper just aren't suitable for coinage.",
			"Gold is the basis for nearly all of our exchanges."
		}
	}
        #Sophistication Medium 
	{
		#Key "*job*", "*work*", "*what*do*do*", "*profession*", "*occupation*" 
		{
			"I am a Banker, $milord/milady$.",
			"I handle the money transactions for _Town_, $milord/milady$.",
			"I am the local banker."
                }
		#Key "*Bank*" 
		{
			"Banking is a satisfactory experience.",
			"We have no history of bank robberies at this branch.",
			"My mother wanted me to adventure, but I find banking much more satisfying."
                }
		#Key "*rob*" "*steal*" "*hold up*" 
		{
			"You see, once a client's money.is deposited, no one can get to it. Not even me.",
			"A thief could kill me and I still couldn't give any money away.",
			"I can't give any money away unless a client specifically asks for a withdrawal."
                }
		#Key "*coin*", "*money*", "*currency*", "*deposit*", "*transaction*", "*account*"   
		{
		 	"Thou can certainly leave thy money with us. 'Tis safer than in thine own pockets, I assure thee.",
			"Thou art welcome to open an account at any time that we are open.",
			"Just give thy money to me, _Name_, and I shall take care of it for thee.",
			"No one shall take thy gold as long as it is in our hands,$milord/milady$."
		}
		#Key "*account*"
		{
			"If thou dost need some of thy money, just ask to withdraw whatever amount thou needs.",
			"To check how much money thou dost have in thy account, ask for a statement or for thy balance." 
		}
		#Key "*gold*", "*silver*", "*copper*" 
		{
			"Gold is the only metal we use when minting our coins.",
			"Silver and copper just aren't suitable for coinage.",
			"Gold is the basis for nearly all of our exchanges."
		}
	}
	#Sophistication Low 
	{
		#Key "*job*", "*work*", "*what*do*do*", "*profession*", "*occupation*" 
		{
			"I'm the Banker here, $milord/milady$.",
			"I handle the money money for _Town_, $milord/milady$.",
			"I'm the local banker."
                }
		#Key "*Bank*" 
		{
			"Keeping the money of others is my job.",
			"Never lost a coin from robberies.",
			"Yes, banking is my unfortunate lot in life."
                }
		#Key "*rob*" "*steal*" "*hold up*" 
		{
			"You see, once a client's money is deposited, no one can get to it. Not even me.",
			"A thief could kill me and I still couldn't give any money away.",
			"I can't give any money away unless a client directly asks for a withdrawal."
                }
		#Key "*coin*", "*money*", "*currency*", "*deposit*", "*transaction*", "*account*"  
		{
		 	"Thou can certainly leave thy money with us. 'Tis safer than in thine own pockets, I assure thee.",
			"Thou can only open an account when we're open.",
			"Just give thy money to me, _Name_. 'Tis safe. I promise.",
			"Dost thou not think me an honest man? No one will take thy gold as long as it's with us."
		}
		#Key "*account*"
		{
			"To get some money out of thy account, ask to withdraw X. X bein' how much thou needs.",
			"To check thy statement balance, ask for a statement or for thy balance. Easy as that."
		}
		#Key "*gold*", "*silver*", "*copper*" 
		{
			"Gold is the only metal to use when minting coins.",
			"Silver and copper just aren't suitable for coinage.",
			"Gold is the basis for all our exchanges."
		}
	}
}
