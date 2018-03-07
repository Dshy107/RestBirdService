using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Services;

namespace BirdObservationRESTService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        // TODO birds by id, birds by userID, birds by nameFragment

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "birds")]
        List<Bird> GetBirds();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "observations?userid={userid}")]
        // TODO ?userId, order By date, birds name
        List<BirdObservationFull> GetObservations(string userid = null);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "observations")]
        int AddObservation(BirdObservation obseration);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "observations/{id}")]
        int RemoveObservation(string id);

        /*   [OperationContract]
           [WebInvoke(Method = "PUT",
               RequestFormat = WebMessageFormat.Json,
               ResponseFormat = WebMessageFormat.Json,
               UriTemplate = "observations/{id}")]
           int UpdateObservation(string id, BirdObservation observation);
       */
    }

    [DataContract]
    public class Bird
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string NameEnglish { get; set; }
        [DataMember]
        public string NameDanish { get; set; }
        [DataMember]
        public string PhotoUrl { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public DateTime? Created { get; set; }
    }

    [DataContract]
    public class BirdObservation
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int BirdId { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public DateTime? Created { get; set; }

        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public string Placename { get; set; }
        [DataMember]
        public int Population { get; set; }
        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class BirdObservationFull : BirdObservation
    {
        [DataMember]
        public string NameEnglish { get; set; }
        [DataMember]
        public string NameDanish { get; set; }
    }
}
