using System;
using System.Text;
using ConsoleLib.Console;
using XRL.Core;
using XRL.Rules;
using XRL.UI;
using XRL.World.Capabilities; //for AddPlayerMessage ( I think )

using XRL;
using XRL.Messages;
using System.Collections.Generic;
using XRL.World.Parts.Effects;


namespace XRL.World.Parts
{
	// Token: 0x020008A6 RID: 2214
	[Serializable]
	public class egocarib_bioplastifier : IPart
	{
		// Token: 0x06004383 RID: 17283 RVA: 0x001E55CB File Offset: 0x001E39CB
		public egocarib_bioplastifier()
		{
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		// Token: 0x06004384 RID: 17284 RVA: 0x001E55DE File Offset: 0x001E39DE
		public override bool SameAs(IPart p)
		{
			return false;
		}

		public bool Bioplastify(GameObject who, bool FromDialog)
		{
			if (this.ParentObject.InInventory != null) {

                if (this.ParentObject.InInventory.HasPart("Brain")) { //I think this should work for player or dominated creature. I don't think it's possible for a thing-with-brain to activate an object in a different thing-with-brain's inventory, right?
                    Popup.Show("It seems a bit unwieldy. Maybe you should set it down first.");
				} else {
					Popup.Show("It's difficult to make it do anything inside " + this.ParentObject.InInventory.the + this.ParentObject.InInventory.DisplayName + ".");
				}

			} else if (this.ParentObject.Equipped != null) {
				Popup.Show("It seems a bit unwieldy. Maybe you should set it down first.");

			} else if (who.HasEffect("Prone", "Sitting")) {
				Popup.Show("You should probably stand up before you mess with that thing.");

			} else if (this.ParentObject.HasEffect("Broken", "Rusted")) {
				Popup.Show("The " + this.ParentObject.DisplayName + " trundles wheezily but nothing happens.");

			} else {
				Popup.Show("The " + this.ParentObject.DisplayName + " unfolds into a glorious mess of gears and needles.\r\n\r\nMechanical appendages wrap, wind, and insert as the gears crank noisily...");

                //get all subtype (mutant/Truekin/custom) sprites
				List<SubtypeEntry> allSubtypes = new List<SubtypeEntry>();
				foreach (SubtypeClass subtypeClass in SubtypeFactory.Classes) {
					allSubtypes.AddRange(subtypeClass.GetAllSubtypes());
				}
				//pick random sprite until we find one that doesn't match the user's current sprite
				SubtypeEntry se;
				do { se = allSubtypes[UnityEngine.Random.Range(0, allSubtypes.Count)]; } while (se.Tile == who.pRender.Tile);
                //change the user's sprite to a random other sprite
                who.pRender.Tile = se.Tile;

				int outcome = UnityEngine.Random.Range(0, 100);
				if (outcome < 60) {
					who.ApplyEffect(new Prone()); //knock the user prone
				}

				if (outcome < 18) {
					Popup.Show("You feel like you've been trampled by a Rhinox.\r\n\r\nThe " + this.ParentObject.DisplayName + " spews sparks and sputters into a busted heap.");
					this.ParentObject.ApplyEffect(new Broken(1));
				} else if (outcome < 38) {
					Popup.Show("Your head hits the ground hard. Ow.\r\n\r\nThe " + this.ParentObject.DisplayName + " clanks and crumples grumpily.");
				} else if (outcome < 58) {
					Popup.Show("Your skin smacks and shivers with rubbery chagrin.\r\n\r\nThe " + this.ParentObject.DisplayName + " buzzes.");
				} else if (outcome < 78) {
					Popup.Show("You black out. Eventually, the world comes back into focus.\r\n\r\nThe " + this.ParentObject.DisplayName + " slaps its gears cantankerously.");
				} else if (outcome < 90) {
					Popup.Show("Ugggghhhh... Are your limbs still all accounted for?\r\n\r\nThe " + this.ParentObject.DisplayName + " precariously winds itself back into the box from which it emerged.");
				} else {
					Popup.Show("You feel... good? Yes. Probably.\r\n\r\nThe " + this.ParentObject.DisplayName + " chirps and flashes green numeric bio-statistics.");
				}
			}
			return true;
		}

		// Token: 0x06004385 RID: 17285 RVA: 0x001E55E1 File Offset: 0x001E39E1
		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "CanSmartUse");
			Object.RegisterPartEvent(this, "CommandSmartUse");
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandActivate");
			base.Register(Object);
		}

		// Token: 0x06004386 RID: 17286 RVA: 0x001E5604 File Offset: 0x001E3A04
		public override bool FireEvent(Event E)
		{
			// MessageQueue.AddPlayerMessage(E.ID);


			if (E.ID == "CanSmartUse")
			{
				return false;
			}
			if (E.ID == "CommandSmartUse")
			{
				this.Bioplastify(E.GetGameObjectParameter("User"), false);
			}

			if (E.ID == "GetInventoryActions")
			{
				EventParameterGetInventoryActions eventParameterGetInventoryActions = E.GetParameter("Actions") as EventParameterGetInventoryActions;
				eventParameterGetInventoryActions.AddAction("Activate", 'a', false, "&Wa&yctivate", "InvCommandActivate", 0, false, false, false, false);
				return true;
			}
			if (E.ID == "InvCommandActivate")
			{
				this.Bioplastify(XRLCore.Core.Game.Player.Body, true);
				E.RequestInterfaceExit();
				return true;
			}
			return base.FireEvent(E);
		}
	}
}
