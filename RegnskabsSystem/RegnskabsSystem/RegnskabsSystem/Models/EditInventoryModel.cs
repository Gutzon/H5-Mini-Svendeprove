using ServerSideData.Models;

namespace RegnskabsSystem.Models
{
    public class EditInventoryModel
    {
        public Inventory oldInventory { get; set; }
        public Inventory newInventory { get; set; }
    }
}

