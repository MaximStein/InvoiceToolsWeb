using MongoDB.Bson;

namespace Backend.Entities.DataTransferObjects
{

    public class ShopDto
    {
        public string? Id { get; set; }
        public string? Label { get; set; }

        //public string? OAuthAccessShopApiProvider { get; set; }
        public string? CurrentApiProvider{ get; set; }        

        public long OrdersCount { get; set; } = 0;

        public int? LastImportOrdersCreated { get; set; }

        public int? LastImportOrdersUpdated { get; set; }

        public DateTime? LastImportTime { get; set; }

        public bool? LastImportSuccessful { get; set; }

        // public string? OAuthAccessIsActive { get; set; }
    }
}
