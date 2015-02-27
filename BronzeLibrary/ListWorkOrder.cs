using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BronzeLibrary
{
    public class ListWorkOrder
    {
        public string id { get; set; }
        public string idCustomerAlias { get; set; }
        public string idCustomerAlias2 { get; set; }
        public string idCustomerAlias3 { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string userAssignedTo { get; set; }
        public object userSignedOffBy { get; set; }
        public object additionalUsersAsString { get; set; }
        public string additionalUsersAssignedTo { get; set; }
        public object dateScheduled { get; set; }
        public object dateScheduledEnd { get; set; }
        public int customerId { get; set; }
        public string customerName { get; set; }
        public object siteContactFirstName { get; set; }
        public object siteContactLastName { get; set; }
        public object siteAddress { get; set; }
        public object sitePostCode { get; set; }
        public object companyTel { get; set; }
        public object notes { get; set; }
        public object engineerSignatureLocationLat { get; set; }
        public object customerSignatureLocationLong { get; set; }
        public object engineerSignature { get; set; }
        public object engineerSignatureLocationLong { get; set; }
        public object customerSignatureLocationLat { get; set; }
        public object customerSignature { get; set; }
        public object signOffDate { get; set; }
        public object downloadDate { get; set; }
        public int deleteState { get; set; }
        public int signOffState { get; set; }
        public string siteContactFullName { get; set; }
        public string siteAddressFormatted { get; set; }
        public bool signedOff { get; set; }
        public object dateScheduledOnly { get; set; }
        public string timeScheduled { get; set; }
        public object dateScheduledEndOnly { get; set; }
        public string timeScheduledEnd { get; set; }
        public List<Form> forms { get; set; }
    }
}
