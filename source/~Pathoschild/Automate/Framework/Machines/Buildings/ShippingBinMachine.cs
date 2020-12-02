/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.Automate.Framework.Machines.Buildings
{
    /// <summary>A shipping bin that accepts input and provides output.</summary>
    internal class ShippingBinMachine : BaseMachine
    {
        /*********
        ** Fields
        *********/
        /// <summary>The farm to automate.</summary>
        private readonly Farm Farm;

        /// <summary>The constructed shipping bin, if applicable.</summary>
        private readonly ShippingBin Bin;


        /*********
        ** Accessors
        *********/
        /// <summary>Get the unique ID for the shipping bin machine.</summary>
        internal static string ShippingBinId { get; } = BaseMachine.GetDefaultMachineId(typeof(ShippingBinMachine));


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="farm">The farm containing the shipping bin.</param>
        /// <param name="tileArea">The tile area covered by the machine.</param>
        public ShippingBinMachine(Farm farm, Rectangle tileArea)
            : base(farm, tileArea, ShippingBinMachine.ShippingBinId)
        {
            this.Farm = farm;
            this.Bin = null;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="bin">The constructed shipping bin.</param>
        /// <param name="location">The location which contains the machine.</param>
        /// <param name="farm">The farm which has the shipping bin data.</param>
        public ShippingBinMachine(ShippingBin bin, GameLocation location, Farm farm)
            : base(location, BaseMachine.GetTileAreaFor(bin))
        {
            this.Farm = farm;
            this.Bin = bin;
        }

        /// <summary>Get the machine's processing state.</summary>
        public override MachineState GetState()
        {
            if (this.Bin?.isUnderConstruction() == true)
                return MachineState.Disabled;

            return MachineState.Empty; // always accepts items
        }

        /// <summary>Get the output item.</summary>
        public override ITrackedStack GetOutput()
        {
            return null; // no output
        }

        /// <summary>Provide input to the machine.</summary>
        /// <param name="input">The available items.</param>
        /// <returns>Returns whether the machine started processing an item.</returns>
        public override bool SetInput(IStorage input)
        {
            foreach (ITrackedStack tracker in input.GetItems().Where(p => p.Sample is SObject obj && obj.canBeShipped()))
            {
                SObject added = (SObject)tracker.Take(tracker.Count);

                Utility.addItemToThisInventoryList(
                    i: added,
                    list: this.Farm.getShippingBin(Game1.MasterPlayer),
                    listMaxSpace: int.MaxValue
                );

                if (this.Bin != null)
                    this.Bin.showShipment(added, false);
                else
                    this.Farm.showShipment(added, false);

                return true;
            }

            return false;
        }
    }
}
