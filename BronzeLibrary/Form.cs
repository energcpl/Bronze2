using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BronzeLibrary
{
    public class Form
    {
        public string id { get; set; }
        public string formTitle { get; set; }
        public string fileName { get; set; }
        public string formId { get; set; }
        public string formVersionId { get; set; }
        public string workOrderId { get; set; }
        public object engineerSignatureLocationLat { get; set; }
        public object engineerSignatureLocationLong { get; set; }
        public object engineerSignature { get; set; }
        public object engineerSignatureDate { get; set; }
        public object customerSignatureLocationLat { get; set; }
        public object customerSignatureLocationLong { get; set; }
        public object customerSignature { get; set; }
        public object customerSignatureDate { get; set; }
        public object customerSignOffName { get; set; }
        public object customerPosition { get; set; }
        public object engineerRemarks { get; set; }
        public object createDate { get; set; }
        public object uploadDate { get; set; }
        public object assetSummaries { get; set; }
        public FormDataBucket formDataBucket { get; set; }
        public int signOffState { get; set; }
        public object thisServiceMachines { get; set; }
        public List<object> machinesForApproval { get; set; }
        public FormInfo formInfo { get; set; }
        public bool exportable { get; set; }
    }
}
