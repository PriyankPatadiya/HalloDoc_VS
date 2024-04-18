


namespace DAL.ViewModels
{
    public class EncounterFormVM
    {
       
            public string FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Location { get; set; }
            public DateTime BirthDate { get; set; }
            public DateTime ServiceDate { get; set; }
            public string? IllnessOrInjury { get; set; }
            public string? MedicalHistory { get; set; }
            public string? Medications { get; set; }
            public string? Allergies { get; set; }
            public string? Temprature { get; set; }
            public string? HR { get; set; }
            public string? RR { get; set; }
            public string? SytolicBp { get; set; }
            public string? DistolicBp { get; set; }
            public string? O2 { get; set; }
            public string? Pain { get; set; }
            public string? Heent { get; set; }
            public string? Cv { get; set; }
            public string? Chest { get; set; }
            public string? ABD { get; set; }
            public string? Extr { get; set; }
            public string? Skin { get; set; }
            public string? Neuro { get; set; }
            public string? Other { get; set; }
            public string? Dignosis { get; set; }
            public string? TreatmentPlan { get; set; }
            public string? MedicationDispensed { get; set; }
            public string? Procedures { get; set; }
            public string? Followup { get; set; }
            public int requestid { get; set; }
            public int? physicianid { get; set; }

    }
}
