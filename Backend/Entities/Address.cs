using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Backend.Entities
{
    public class Address
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string? Id;

        public string Name { get; set; }

        public string Company { get; set; }

        public string PhoneNumber { get; set; }

        public string AdditionalInfo1 { get; set; }

        public string AdditionalInfo2 { get; set; }

        public string Street { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        
        public string Country { get; set; }


        public string ToString()
        {
            return Name + "\r\n"
                + (Company == null || String.IsNullOrWhiteSpace(Company) ? "" : Company + "\r\n")
                + Street + "\r\n"
                + (String.IsNullOrWhiteSpace(AdditionalInfo1) ? "" : AdditionalInfo1 + "\r\n")
                + (String.IsNullOrWhiteSpace(AdditionalInfo2) ? "" : AdditionalInfo2 + "\r\n")
                + ZipCode + " " + City+"\r\n"
                + Country;
        }


        

    }
}
