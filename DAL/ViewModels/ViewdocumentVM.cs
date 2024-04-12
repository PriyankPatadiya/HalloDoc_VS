using DAL.DataModels;


namespace DAL.ViewModels
{
    public class ViewdocumentVM
    { 
        public List<RequestWiseFile>? RequestWiseFile { get; set; } 
        public string? patientName { get; set; }

        public int? requestid { get; set; }

        public string? confirmationNumber { get; set; }

    }
}
