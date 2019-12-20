using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Runtime.Serialization;
using System.Data.Entity.Migrations;
using System.Linq;
using System.IO;
using System.Data.Entity.Validation;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel;
using System.Web.Mvc;

namespace Paho.Models
{
    [Authorize]
    // Models returned by CaseController actions.
    public class CaseViewModel
    {
        public string DatePickerConfig { get; set; }
        public string DateFormatDP { get; set; }
        public IEnumerable<CountryView> Countries { get; set; }
        public IEnumerable<LookupView<Area>> Areas { get; set; }
        public IEnumerable<LookupView<Region>> Regions { get; set; }
        public IEnumerable<LookupView<Institution>> Institutions { get; set; }
        public IEnumerable<LookupView<Lab>> Labs { get; set; }
        public IEnumerable<LookupView<CatReasonNotSampling>> ReasonNotSampling { get; set; }
        public IEnumerable<LookupView<Institution>> InstitutionsCaseGenerarting { get; set; }
        public Array LabsExternal { get; set; }
        public Array LabsHospital_cmb { get; set;  }
        public bool DisplayCountries { get; set; }
        public bool DisplayAreas { get; set; }
        public bool DisplayRegionals { get; set; }
        public bool DisplayHospitals { get; set; }
        public bool DisplayLabs { get; set; }
        //public int? UsrCtry { get; set; }

        // Regiones de usuario asignadas
        public string reg_inst_usr { get; set; }
        public string reg_salu_usr { get; set; }
        public string reg_pais_usr { get; set; }

        // Usuario de laboratorio NIC
        public bool lab_NIC_usr { get; set; }

        // Catalogos de laboratorio
        public IEnumerable<LookupView<CatSampleNoProcessed>> CSNP { get; set; }
        public IEnumerable<LookupView<CatTestType>> CTT { get; set; }
        public IEnumerable<LookupView<CatTestResult>> CTR { get; set; }
        public IEnumerable<LookupView<CatVirusType>> CVT { get; set; }
        public IEnumerable<LookupView<CatOtherVirusType>> COVT { get; set; }
        public IEnumerable<LookupView<CatVirusSubType>> CVST { get; set; }
        public IEnumerable<LookupView<CatVirusSubType>> CVST_Test { get; set; }
        public IEnumerable<LookupView<CatVirusLinaje>> CVL { get; set; }
        public IEnumerable<LookupView<CatNativePeople>> CNP { get; set; }
        public IEnumerable<LookupView<CatVaccinSource>> CVS { get; set; }
        public IEnumerable<LookupView<CatOccupation>> CatOccupations { get; set; }              //**** CAFQ
        public IEnumerable<LookupView<CatTrabSaludRama>> CatTrabSaludRamas { get; set; }        //**** CAFQ
        public IEnumerable<LookupView<CatTrabLaboRama>> CatTrabLaboRamas { get; set; }          //**** CAFQ
        public IEnumerable<LookupView<CatHospitalizedIn>> CHIn { get; set; }

        public object date_format_ { get; set; }

        // Paginador
        public Array caselist_interval_1 { get; set; }
        public Array caselist_interval_2 { get; set; }
    }

    public class LabTestViewModel : CaseBase
    {
        public int ID { get; set; }
        public int? CaseLabID  { get; set; }
        public long? LabID { get; set; }
        public bool? Processed { get; set; }
        public int? SampleNumber { get; set; }
        public int? TestType  { get; set; }
        public DateTime? TestDate { get; set; }
        public string TestResultID { get; set; }
        public int? VirusTypeID { get; set; }
        public Decimal? CTVirusType { get; set; }
        public Decimal? CTRLVirusType { get; set; }
        public int? OtherVirusTypeID { get; set; }
        public Decimal? CTOtherVirusType { get; set; }
        public Decimal? CTRLOtherVirusType { get; set; }
        public string OtherVirus { get; set; }
        public int? InfA { get; set; }
        public int? VirusSubTypeID { get; set; }
        public Decimal? CTSubType { get; set; }
        public Decimal? CTRLSubType { get; set; }
        public string TestResultID_VirusSubType { get; set; }
        public int? VirusSubTypeID_2 { get; set; }
        public Decimal? CTSubType_2 { get; set; }
        public Decimal? CTRLSubType_2 { get; set; }
        public string TestResultID_VirusSubType_2 { get; set; }
        public int? InfB { get; set; }
        public int? VirusLineageID { get; set; }
        public Decimal? CTLineage { get; set; }
        public Decimal? CTRLLineage { get; set; }
        public int? ParaInfI { get; set; }
        public int? ParaInfII { get; set; }
        public int? ParaInfIII { get; set; }
        public int? RSV { get; set; }
        public int? Adenovirus { get; set; }
        public int? Metapneumovirus { get; set; }
        public Decimal? RNP { get; set; }
        public Decimal? CTRLRNP { get; set; }
        public Decimal? CTRLNegative { get; set; }
        public DateTime? TestEndDate { get; set; }

        [ForeignKey("TestResultID")]
        public virtual CatTestResult CatTestResult { get; set; }

    };

    public class PrincipalViewModel
    {
        public int CountryID { get; set; }
        public int HospitalID { get; set; }
        public int? Year { get; set; }
        public IEnumerable<CountryView> Countries { get; set; }
        public IEnumerable<LookupView<Institution>> Institutions { get; set; }
        public IEnumerable<LookupView<Region>> Regions { get; set; }
        public IEnumerable<LookupView<State>> States { get; set; }
        public IEnumerable<LookupView<Area>> Areas { get; set; }
        public bool DisplayCountries { get; set; }
        public bool DisplayRegionals { get; set; }
        public bool DisplayHospitals { get; set; }
    }

    public class ExportarViewModel
    {
        public int CountryID { get; set; }
        public int HospitalID { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? SE { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<CountryView> Countries { get; set; }
        public IEnumerable<LookupView<Area>> Areas { get; set; }        //#### CAFQ: 180703
        public IEnumerable<LookupView<Institution>> Institutions { get; set; }
        public IEnumerable<LookupView<ReportCountry>> ReportsCountries { get; set; }
        public IEnumerable<LookupView<Region>> Regions { get; set; }
        public bool DisplayCountries { get; set; }
        public bool DisplayAreas { get; set; }                      //#### CAFQ: 180703
        public bool DisplayRegionals { get; set; }
        public bool DisplayHospitals { get; set; }
        public string DatePickerConfig { get; set; }
        public string DateFormatDP { get; set; }

    }

    public class ExportarLabViewModel
    {
        public int CountryID { get; set; }
        public int HospitalID { get; set; }
        public int? Year { get; set; }
        public int? WeekFrom { get; set; }
        public int? WeekTo { get; set; }
        public int? IDrecord { get; set; }
        public IEnumerable<CountryView> Countries { get; set; }
        public IEnumerable<LookupView<Institution>> Institutions { get; set; }
        public bool DisplayCountries { get; set; }
        public bool DisplayRegionals { get; set; }
        public bool DisplayHospitals { get; set; }
    }

    public class FluidViewModel
    {
        public int CountryID { get; set; }
        public int HospitalID { get; set; }
        public int? Year { get; set; }
        public int? WeekFrom { get; set; }
        public int? WeekTo { get; set; }
        public IEnumerable<CountryView> Countries { get; set; }
        public IEnumerable<LookupView<Institution>> Institutions { get; set; }
        public IEnumerable<LookupView<Region>> Regions { get; set; }
        public bool DisplayCountries { get; set; }
        public bool DisplayRegionals { get; set; }
        public bool DisplayHospitals { get; set; }
    }

    public class FluCaseExportViewModel
    {
        public int ID { get; set; }
        [Required]
        public string HospitalID { get; set; }
        public DateTime HospitalDate { get; set; }
        public DateTime RegDate { get; set; }
        [Required]
        public string FName1 { get; set; }
        public string FName2 { get; set; }
        [Required]
        public string LName1 { get; set; }
        public string LName2 { get; set; }
        public string NoExpediente { get; set; }
        public string NationalId { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age { get; set; }
        public string AMeasure { get; set; }
        public string AgeGroup { get; set; }
        public string Gender { get; set; }
        //public class CaseGEO : CaseBase
        public string Country { get; set; }
        public string Area { get; set; }
        public string State { get; set; }
        public string Local { get; set; }
        public string Neighborhood { get; set; }
        public string UrbanRural { get; set; }
        public string Address { get; set; }
        //public class CaseRisk : CaseBase
        public string Vaccin { get; set; }
        public string RiskFactors { get; set; }
        public bool? HDisease { get; set; }
        public bool? Diabetes { get; set; }
        public bool? Neuro { get; set; }
        public bool? Asthma { get; set; }
        public bool? Pulmonary { get; set; }
        public bool? Liver { get; set; }
        public bool? Renal { get; set; }
        public bool? Immunsupp { get; set; }

        public bool? ParaCerebral { get; set; }
        public bool? Indigena { get; set; }
        public bool? TrabSalud { get; set; }
        public bool? Desnutricion { get; set; }
        public bool? Prematuridad { get; set; }
        public bool? BajoPesoNacer { get; set; }
        public bool? AusLacMat { get; set; }
        public string Pregnant { get; set; }
        public bool? Pperium { get; set; }
        public string Trimester { get; set; }
        public int? PregnantWeek { get; set; }
        public bool? Smoking { get; set; }
        public bool? Alcohol { get; set; }
        public bool? DownSyn { get; set; }
        public string Obesity { get; set; }
        public string OtherRisk { get; set; }
        // public class CaseHospital : CaseBase
        public string CHNum { get; set; }
        public DateTime? FeverDate { get; set; }
        public int? FeverEW { get; set; }
        public string Antiviral { get; set; }
        public int? OseltaDose { get; set; }

        public DateTime? AStartDate { get; set; }

        public DateTime? DiagDate { get; set; }
        public int? DiagEW { get; set; }
        public int? Salon { get; set; }
        public int? DiagAdm { get; set; }
        public string DiagOtroAdm { get; set; }
        public bool? HallRadio { get; set; }
        public string HallRadioFindings { get; set; }           //**** CAFQ
        public bool? MecVent { get; set; }
        public int? DiagEg { get; set; }
        public bool? DifResp { get; set; }
        public bool? MedSatOxig { get; set; }
        public int? SatOxigPor { get; set; }

        public DateTime? HospAmDate { get; set; }
        public int? HospEW { get; set; }
        public DateTime? HospExDate { get; set; }
        public DateTime? ICUAmDate { get; set; }
        public int? ICUEW { get; set; }
        public DateTime? ICUExDate { get; set; }  
        public string Destin { get; set; }
        public bool? IsSample { get; set; }
        public DateTime? SampleDate { get; set; }
        public string SampleType { get; set; }
        public DateTime? ShipDate { get; set; }
        public string Lab { get; set; }        
        //public class CaseLab : CaseBase
        public DateTime? RecDate { get; set; }
        public bool? Processed { get; set; }
        public string NoProRen { get; set; }
        public DateTime? EndLabDate { get; set; }        
        public string FResult { get; set; }
        public string Comments { get; set; }
        //public virtual ICollection<CaseLabTest> CaseLabTests { get; set; }

}

    public class SummaryViewModel
    {
        public IEnumerable<CountryView> Countries { get; set; }
        public IEnumerable<LookupView<Institution>> Institutions { get; set; }
        public IEnumerable<CatAgeGroup> CatAgeGroup { get; set; }
        public bool DisplayCountries { get; set; }
        public bool DisplayRegionals { get; set; }
        public bool DisplayHospitals { get; set; }
        public string DatePickerConfig { get; set; }
        public string DateFormatDP { get; set; }
        //public int? UsrCtry { get; set; }
        public bool? SARI { get; set; }
        public bool? ILI { get; set; }
        //public int? AccessLevel_InstLogin { get; set; }
        //public string Name_InstLogin { get; set; }
        public IEnumerable<LookupView<Institution>> InstitutionsWithServices { get; set; }
    }

    public class SummaryJMViewModel             //#### CAFQ: Summary Jamaica
    {
        public IEnumerable<CountryView> Countries { get; set; }
        public IEnumerable<LookupView<Institution>> Institutions { get; set; }
        public IEnumerable<CatAgeGroupJM> CatAgeGroup { get; set; }
        public bool DisplayCountries { get; set; }
        public bool DisplayRegionals { get; set; }
        public bool DisplayHospitals { get; set; }
        public string DatePickerConfig { get; set; }
        public string DateFormatDP { get; set; }
        public bool DisplayTotalGroup { get; set; }
    }

    public class ReportViewModel
    {
        public int CountryID { get; set; }
        public string Name { get; set; }
        public string NationalID { get; set; }
        public string CaseStatus { get; set; }
        public long? HospitalID { get; set; }
        public long? LabID { get; set; }
        public DateTime? RStartDate { get; set; }
        public DateTime? REndDate { get; set; }
        public DateTime? HStartDate { get; set; }
        public DateTime? HEndDate { get; set; }
        public bool DisplayCountries { get; set; }
        public bool DisplayRegionals { get; set; }
        public bool DisplayHospitals { get; set; }
        public bool DisplayLabs { get; set; }
        public IEnumerable<CountryView> Countries { get; set; }
        public IEnumerable<LookupView<Hospital>> hospitals { get; set; }
        public IEnumerable<LookupView<Lab>> labs { get; set; }
    }

    public class FluCaseReportViewModel
    {
       public string CountryCode { get; set; }
       public long ID { get; set; }
       public long HospitalID { get; set; }
       public DateTime HospitalDate { get; set; }
       public DateTime RegDate { get; set; }
       public string Name { get; set; }
       public string NationalID { get; set; }
       public string CaseStatus { get; set; }
    }

    public class LookupView<T>
    {
        public string orden { get; set; } // Esta propiedad es para los catalogos del Laboratorio
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class CountryView : LookupView<Country>
    {
        public bool Active { get; set; }
    }

    public class RegionView : LookupView<Region>
    {
        public bool Active { get; set; }
    }

    //public class ReasonNotSamplingView : LookupView<CatReasonNotSampling>
    //{
    //    public string SPA { get; set; }
    //    public string ENG { get; set; }
    //}

    public abstract class CaseBase
    {
        public int? UserID { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public CaseBase()
        {
            InsertDate = LastUpdate = DateTime.Now;
        }
    }

    public class Country
    {
        public int ID { get; set; }
        public string Code { get; set; }
        [Display(Name = "Country")]
        public string Name { get; set; }
        public string ENG { get; set; }
        public int? NumberAdminisDivision { get; set; }
        public bool Active { get; set; }
        public virtual ICollection<Area> Areas { get; set; }
        public string Language { get; set; }
    }

    public class CatRegionType
    {
        public int ID { get; set; }
        public string SPA { get; set; }
        public string ENG { get; set; }
        public int? orden { get; set; }
    }

    public class Region
    {
        public int ID { get; set; }
        public int? CountryID { get; set; }
        public String Name { get; set; }
        public int? orig_country { get; set; }
        public int? tipo_region { get; set; }
        [ForeignKey("tipo_region")]
        public virtual CatRegionType CatRegionType { get; set; }

        [ForeignKey("CountryID")]
        public virtual Country Cat_Country { get; set; }
    }

    // Catalogos

    public class CatAntiviral
    {
        public int ID { get; set; }
        public string description { get; set; }
        public int? orden { get; set; }
        public string id_country { get; set; }
    }

    public class CatCaseStatus
    {
        public int ID { get; set; }
        public string description { get; set; }
        public int? id_country { get; set; }
    }

    public class CatAgeGroup
    {
        public int id { get; set; }
        [Display(Name = "ID")]
        public int id_conf_country { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Grupo de Edad")]
        public string AgeGroup { get; set; }
        public int id_country { get; set; }
        [Required]
        [Display(Name = "Mes Inicia")]
        public int month_begin { get; set; }
        [Required]
        [Display(Name = "Mes Finaliza")]
        public int month_end { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "FLUID")]
        public string label_fluid { get; set; }
    }

    public class CatAgeGroupJM                  //#### CAFQ: Summary Jamaica
    {
        public int id { get; set; }
        [Display(Name = "ID")]

        public int id_conf_country { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Grupo de Edad")]

        public string AgeGroup { get; set; }

        public int id_country { get; set; }
        [Required]
        [Display(Name = "Mes Inicia")]

        public int month_begin { get; set; }
        [Required]
        [Display(Name = "Mes Finaliza")]

        public int month_end { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "FLUID")]

        public string label_fluid { get; set; }
    }

    public class CatDiag
    {
        public int ID { get; set; }
        public string Diag { get; set; }
        public string ENG { get; set; }
        public string SPA { get; set; }
        public string code { get; set; }
    }
    
    public class CatDocumentType
    {
        public int ID { get; set; }
        public string description { get; set; }
        public int? orden { get; set; }
        public int? id_country { get; set; }
    }

    public class CatNativePeople
    {
        public int ID { get; set; }
        public string description { get; set; }
        public int? orden { get; set; }
        public int? active { get; set; }
        public int? id_country { get; set; }
    }

    public class CatServicios
    {
        public int ID { get; set; }
        public string description_service { get; set; }
    }

    public class CatVaccinSource
    {
        public int ID { get; set; }
        [Display(Name = "Descripción:")]
        public string description { get; set; }
        [Display(Name = "Orden:")]
        public int? orden { get; set; }
        public int? id_country { get; set; }
    }

    public class CatVaccinSourceConf
    {
        public int ID { get; set; }
        public int? id_catvaccinsource { get; set; }
        public int? id_country { get; set; }

        //public int? Conclusion { get; set; }  // Original y lo modifico AM 

        [ForeignKey("id_country")]
        public virtual Country CountryVaccine { get; set; }
        [ForeignKey("id_catvaccinsource")]
        public virtual CatVaccinSource CatVaccinSource { get; set; }
    }

    public class CatReasonNotSampling
    {
        public int? ID { get; set; }

        public string SPA { get; set; }

        public string ENG { get; set; }
    }

    // Catalogos para resultado de laboratorio

    public class CatSampleNoProcessed
    {
        public int ID { get; set; }
        [Display(Name = "Descripción SPA:")]
        public string SPA { get; set; }
        [Display(Name = "Descripción ENG:")]
        public string ENG { get; set; }
        [Display(Name = "Orden:")]
        public int? orden { get; set; }
        [Display(Name = "Activo:")]
        public bool? active { get; set; }
    }

    public class CatTestType
    {
        public int ID { get; set; }
        [Display(Name = "Descripción:")]
        public string description { get; set; }
        public int? id_country { get; set; }
        [Display(Name = "Orden:")]
        public int? orden { get; set; }
    }

    public class CatHospitalizedIn
    {
        public int ID { get; set; }
        [Display(Name = "Descripción SPA:")]
        public string SPA { get; set; }
        [Display(Name = "Descripción ENG:")]
        public string ENG { get; set; }
        [Display(Name = "Orden:")]
        public int? orden { get; set; }
    }

    public class CatTestResult
    {
        //public int ID { get; set; }

        //[Column("TestResultID")]
        //[ForeignKey("TestResultID")]

        //[DatabaseGenerated(PahoDbContext.Identity)]

        //public string TestResultID { get; set; }

        [Display(Name = "Descripción:")]
        public string description { get; set; }
        [Display(Name = "ENG:")]
        public string ENG { get; set; }
        [Key]
        public string TestResultID { get; set; }
        public string value { get; set; }
        public int? id_country { get; set; }
        [Display(Name = "Orden:")]
        public int? orden { get; set; }
    }

    public class CatVirusType
    {

        public int ID { get; set; }
        public string SPA { get; set; }
        public string ENG { get; set; }
        public int? orden { get; set; }

    }

    public class CatOtherVirusType
    {

        public int ID { get; set; }
        public string SPA { get; set; }
        public string ENG { get; set; }
        public int? orden { get; set; }

    }

    public class CatVirusSubType
    {
        public int ID { get; set; }
        public string SPA { get; set; }
        public string ENG { get; set; }
        public int? id_country { get; set; }
        public int? orden { get; set; }
    }

    public class CatVirusSubTypeConfByLab
    {
        public int ID { get; set; }
        public long id_Institution { get; set; }
        public int id_CatSubType { get; set; }
        public int? orden { get; set; }
    }

    public class CatVirusLinaje
    {
        public int ID { get; set; }
        public string SPA { get; set; }
        public string ENG { get; set; }
        public int? id_country { get; set; }
        public int? orden { get; set; }
    }

    // Termina catalogos para resultado de laboratorio

    // Catalogos generales

    public class CatStatusCase
    {
        public int ID { get; set; }
        public string SPA { get; set; }
        public string ENG { get; set; }
        public int? orden { get; set; }
    }

    public class CatInstitutionTypeHON
    {
        public int ID { get; set; }
        public string SPA { get; set; }
        public string ENG { get; set; }
        public int orden { get; set; }
    }
    // Termina catalogos generales

    public class CatCloseParameters			//**** NEW
    {
        public int Id { get; set; }
        [Required]
        public int? Surv { get; set; }
        //[Required]
        public int? IsSample { get; set; }
        public int? Processed { get; set; }
        public int? LabEndClosing { get; set; }
        public int? HospExDate { get; set; }
        public int? DiagEg { get; set; }

        //[NotMapped]
        [ForeignKey("Surv")]
        public virtual CatSurv SurvReg { get; set; }                    // Para vista Index
        [NotMapped]
        public List<CatSurv> SurvRegCollection { get; set; }            // Para form Create y Edit
        [NotMapped]
        public string IsSampleText { get; set; }
        [NotMapped]
        public string ProcessedText { get; set; }
        [NotMapped]
        public string LabEndClosingText { get; set; }
        [NotMapped]
        public string HospExDateText { get; set; }
        [NotMapped]
        public string DiagEgText { get; set; }
    }


    public class Area
    {
        public int ID { get; set; }
        public int CountryID { get; set; }
        public string Name { get; set; }
        public string orig_country { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<State> States { get; set; }
    }

    public class State
    {
        public int ID { get; set; }        
        public int AreaID { get; set; }
        public string Name { get; set; }
        public string orig_country { get; set; }
        public virtual Area Area { get; set; }
        public virtual ICollection<Local> Locals { get; set; }
        public virtual ICollection<Neighborhood> Neighborhoods { get; set; }
    }

    public class CatParishPostOfficeJM
    {
        public int ID { get; set; }
        public int AreaID { get; set; }
        public string PostOffice_PostalAgency { get; set; }
        public string orig_country { get; set; }
        public virtual Area Area { get; set; }
    }

    public class Local
    {
        public int ID { get; set; }
        public int StateID { get; set; }
        public int Code { get; set; }
        public virtual State State { get; set; }
    }

    public class Neighborhood
    {
        public int ID { get; set; }
        public int StateID { get; set; }
        public int? Code { get; set; }
        public string Name { get; set; }
        public int? RegionID { get; set; }
        public string orig_country { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public virtual State State { get; set; }
        public virtual ICollection<Hamlet> Hamlets { get; set; }
    }

	public class Hamlet                             //#### CAFQ: 181018
    {
        public int ID { get; set; }
        public int NeighborhoodID { get; set; }
        public string Name { get; set; }
        public string orig_country { get; set; }
        public virtual Neighborhood Neighborhood { get; set; } 
        public virtual ICollection<Colony> Colonies { get; set; }
    }

    public class Colony                                     //#### CAFQ: 181018
    {
        public int ID { get; set; }
        public int HamletID { get; set; }
        public string Name { get; set; }
        public string orig_country { get; set; }
        public virtual Hamlet Hamlet { get; set; }
    }

    public class SearchGeoReference
    {
        public string value { get; set; }
        public string label { get; set; }
        public string typeubic { get; set; }
        public int areaID { get; set; }
        public string areaName { get; set; }
        public int stateID { get; set; }
        public string stateName { get; set; }
        public int neighborhoodID { get; set; }
        public string neighborhoodName { get; set; }
        public int hamletID { get; set; }
        public string hamletName { get; set; }
        public int colonyID { get; set; }
        public string colonyName { get; set; }
    }

    public abstract class Institution
    {
        public long ID { get; set; }
        public int? CountryID { get; set; }
        public int AreaID { get; set; }
        [MaxLength(100)]
        [DisplayName("Nombre completo:")]
        public String FullName { get; set; }
        [Display(Name = "Nombre corto:")]
        public String Name { get; set; }
        [Display(Name = "Nivel de acceso:")]
        public AccessLevel AccessLevel { get; set; }
        [DisplayName("ID persolizado institución:")]
        public long? InstID { get; set; }
        [DisplayName("Activo:")]
        public bool Active { get; set; }
        public virtual Country Country { get; set; }
        public virtual Area Area { get; set; }
        public bool sentinel { get; set; }
        [DisplayName("IRAG:")]
        public bool SARI { get; set; }
        [DisplayName("ETI:")]
        public bool ILI { get; set; }
        public bool surv_unusual { get; set; }
        [DisplayName("PCR:")]
        public bool PCR { get; set; }
        [DisplayName("IFI:")]
        public bool IFI { get; set; }
        [DisplayName("Institución padre:")]
        public int? Father_ID { get; set; }
        // configurcion de region
        [DisplayName("Región institucional:")]
        public int? cod_region_institucional { get; set; }
        [DisplayName("Región salud:")]
        public int? cod_region_salud { get; set; }
        [DisplayName("Región país:")]
        public int? cod_region_pais { get; set; }
        [DisplayName("Código original país:")]
        public string orig_country { get; set; }
        [DisplayName("Orden de prioridad para el resultado final por institución:")]
        public int? OrdenPrioritybyLab { get; set; }
        [DisplayName("Laboratorio intermedio:")]
        public bool NPHL { get; set; }
        [NotMapped]
		public InstitutionType InstType { get; set; }
        public int? cod_institution_type { get; set; }
        //*****>                                            //#### CAFQ: 180911
        [DisplayName("Tipo Ubicación:")]
        public int? LocationTypeID { get; set; }

        [DisplayName("País Extranjero:")]
        public int? ForeignCountryID { get; set; }

        [DisplayName("Dirección Institución Extranjero:")]
        public string ForeignInstitutionAddress { get; set; }

        public bool ForeignLab { get; set; }
        //****<
        [DisplayName("Tipo Institución:")]
        public int? InstitutionTypeID { get; set; }
        [DisplayName("Laboratorio NIC:")]
        public bool LabNIC { get; set; }
    }

    public class InstitutionLocationType                    //#### CAFQ: 180911
    {
        public int ID { get; set; }
        public string Name_SPA { get; set; }
        public string Name_ENG { get; set; }
    }

    public class InstitutionLocationTypeView                //#### CAFQ: 180911
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Hospital : Institution
    {
        public virtual ICollection<FluCase> FluCases { get; set; }
    }

    public class Lab : Institution
    {
        public virtual ICollection<FluCase> FluCases { get; set; }

        public Lab()
        {

        }

        public Lab(Institution catalog)
        {
            CountryID = catalog.CountryID;

            LocationTypeID = catalog.LocationTypeID;
            ForeignCountryID = catalog.ForeignCountryID;
            AreaID = catalog.AreaID;
            FullName = catalog.FullName;
            Name = catalog.Name;
            ForeignInstitutionAddress = catalog.ForeignInstitutionAddress;
            AccessLevel = catalog.AccessLevel;
            InstID = catalog.InstID;
            cod_institution_type = catalog.cod_institution_type;
            Father_ID = catalog.Father_ID;
            orig_country = catalog.orig_country;
            cod_region_institucional = catalog.cod_region_institucional;
            cod_region_salud = catalog.cod_region_salud;
            cod_region_pais = catalog.cod_region_pais;
            SARI = catalog.SARI;
            ILI = catalog.ILI;
            surv_unusual = catalog.surv_unusual;
            sentinel = catalog.sentinel;
            PCR = catalog.PCR;
            IFI = catalog.IFI;
            Active = catalog.Active;
            NPHL = catalog.NPHL;
            LabNIC = catalog.LabNIC;
        }
    }

    public class AdminInstitution : Institution
    {
        public AdminInstitution()
        {

        }
        public AdminInstitution(Institution catalog)
        {
            CountryID = catalog.CountryID;
            AreaID = catalog.AreaID;
            FullName = catalog.FullName;
            Name = catalog.Name;
            AccessLevel = catalog.AccessLevel;
            InstID = catalog.InstID;
            Father_ID = catalog.Father_ID;
            orig_country = catalog.orig_country;
            cod_region_institucional = catalog.cod_region_institucional;
            cod_region_salud = catalog.cod_region_salud;
            cod_region_pais = catalog.cod_region_pais;
            SARI = catalog.SARI;
            ILI = catalog.ILI;
            PCR = catalog.PCR;
            IFI = catalog.IFI;
            Active = catalog.Active;
        }
    }

    public class ConfAlertCaseDefinition
    {
        [Key]
        public int id { get; set; }
        public int id_country { get; set; }
        [Required]
        [Display(Name = "Mes Inicia")]
        public int month_begin { get; set; }
        [Required]
        [Display(Name = "Mes Finaliza")]
        public int month_end { get; set; }
    }

    public class InstitutionConfiguration
    {
        public long ID { get; set; }
        public long? InstitutionFromID { get; set; }
        //[Key]
        //[Column(Order = 1)]
        public long? InstitutionToID { get; set; }
        //[Key]
        //[Column(Order = 2)]
        public long? InstitutionParentID { get; set; }
        [DisplayName("Flujo:")]
        public int Priority { get; set; }
        public bool Conclusion { get; set; }
        public bool OpenAlways { get; set; }

        //public int? Conclusion { get; set; }  // Original y lo modifico AM 

        [ForeignKey("InstitutionFromID")]
        public virtual Institution InstitutionFrom { get; set; }
        [ForeignKey("InstitutionToID")]
        public virtual Institution InstitutionTo { get; set; }
        [ForeignKey("InstitutionParentID")]
        public virtual Institution InstitutionParent { get; set; }

        [NotMapped]
        public MultiSelectList VirusFlowCollection { get; set; }        //**** NEW: 1911

        [NotMapped]
        public int[] VirusFlowSelected { get; set; }            //**** NEW: 1911
    }

    public class InstitutionForeignConf
    {
        [Column(Order = 1), Key]
        public long? InstitutionForeignID { get; set; }
        [Column(Order = 2), Key]
        public long? InstitutionLocalID { get; set; }
        //public int? InstitutionLocalCountryID { get; set; }


        [ForeignKey("InstitutionForeignID")]
        public virtual Institution InstitutionForeign { get; set; }
        [ForeignKey("InstitutionLocalID")]
        public virtual Institution InstitutionLocal { get; set; }
        //[ForeignKey("InstitutionLocalCountryID")]
        //public virtual Country CountryLocalLab { get; set; }
    }

    public class InstitutionConfEndFlowByVirus
    {
        
        public int ID { get; set; }
        public long id_Lab { get; set; }
        public int? id_Cat_TestType { get; set; }
        public string value_Cat_TestResult { get; set; }
        public int? id_Cat_VirusType { get; set; }
        public int id_priority_flow { get; set; }
        public long id_InstCnf { get; set; }
        public int? id_Cat_Subtype { get; set; }

        [ForeignKey("id_InstCnf")]
        public virtual InstitutionConfiguration InstitutionConfiguration { get; set; }
    }

    public class InstitutionConfFlowByVirus                     //**** NEW: 1911
    {
        public long? ID { get; set; }
        public long? InstConf_ID { get; set; }
        public int? VirusFlow_ID { get; set; }

    }

    public class EndFlowByVirus
    {
        public int ID { get; set; }
        public int FluCaseID { get; set; }
        public long LabID { get; set; }
        public string InstitutionName { get; set; }
        public bool Processed { get; set; }
        public int SampleNumber { get; set; }
        public int? VirusTypeID { get; set; }
        public int? OrdenVirusType { get; set; }
        public int? OtherVirusTypeID { get; set; }
        public string OtherVirus { get; set; }
        public int? TestType { get; set; }
        public int? OrdenTestType { get; set; }
        public int? VirusSubTypeID { get; set; }
        public int? OrdenVirusSubType { get; set; }
        public int? VirusLineageID { get; set; }
        public int? OrdenVirusLinaje { get; set; }
        public string TestResultID { get; set; }
        public int? OrdenTestResult { get; set; }
        public int? ICEFBVID { get; set; }
        public long? id_InstCnf { get; set; }
        public long? id_Lab { get; set; }
        public int? id_priority_flow { get; set; }
        public int? id_Cat_TestType { get; set; }
        public string value_Cat_TestResult { get; set; }
        public int? id_Cat_VirusType { get; set; }
        public int? id_Cat_Subtype { get; set; }
    }
    
    public class CatVirusFlow                                   //**** NEW: 1911
    {
        public int ID { get; set; }
        public int? TestType_ID { get; set; }
        public string TestResult_ID { get; set; }
        public int? VirusType_ID { get; set; }
        public int? VirusSubtype_ID { get; set; }
        public int? VirusLinaje_ID { get; set; }

        [ForeignKey("TestType_ID")]
        public virtual CatTestType CatTestTypes { get; set; }                   // Para Index
        [NotMapped]
        public List<CatTestType> CatTestTypeCollection { get; set; }            // Para form Create y Edit

        [ForeignKey("TestResult_ID")]
        public virtual CatTestResult CatTestResults { get; set; }
        [NotMapped]
        public List<CatTestResult> CatTestResultCollection { get; set; }

        [ForeignKey("VirusType_ID")]
        public virtual CatVirusType CatVirusTypes { get; set; }
        [NotMapped]
        public List<CatVirusType> CatVirusTypeCollection { get; set; }

        [ForeignKey("VirusSubtype_ID")]
        public virtual CatVirusSubType CatVirusSubTypes { get; set; }
        [NotMapped]
        public List<CatVirusSubType> CatVirusSubTypeCollection { get; set; }

        [ForeignKey("VirusLinaje_ID")]
        public virtual CatVirusLinaje CatVirusLinaje { get; set; }
        [NotMapped]
        public List<CatVirusLinaje> CatVirusLinajeCollection { get; set; }
    }
    
    public class FluCase : CaseBase
    {
         // Mapeo princifpal para grabar datos
        public int ID { get; set; }
        [Required]
        public long HospitalID { get; set; }
        public long? HospitalID_CaseGenerating { get; set; }
        public DateTime HospitalDate { get; set; }
        public DateTime RegDate { get; set; }
        public int? Surv { get; set; }
        public bool? SurvInusual { get; set; }
        public bool? Brote { get; set; }
        [Required]
        public string FName1 { get; set; }
        public string FName2 { get; set; }
        public string FullName { get; set; }
        [Required]
        public string LName1 { get; set; }
        public string LName2 { get; set; }
        public int? DocumentType { get; set; }
        public string NoExpediente { get; set; }
        public string NationalId { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age { get; set; }
        public AMeasure? AMeasure { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public AgeGroup? AgeGroup { get; private set; }
        public Gender Gender { get; set; }
        public string ResponsibleMinor { get; set; }
        public int? nativepeople { get;  set; }
        public int? nationality { get; set; }

        public int? Ocupacion { get; set; }                         //#### CAFQ
        public string TrabajoDirecc { get; set; }                   //#### CAFQ
        public string TrabajoEstablec { get; set; }                 //#### CAFQ
        public int? ContactoAnimVivos { get; set; }                 //#### CAFQ
        public int? OcupacMercAnimVivos { get; set; }               //#### CAFQ
        public int? ViajePrevSintoma { get; set; }                  //#### CAFQ
        public string DestPrevSintoma1 { get; set; }                //#### CAFQ
        public DateTime? DestFechaLlegada1 { get; set; }            //#### CAFQ
        public DateTime? DestFechaSalida1 { get; set; }             //#### CAFQ
        public string DestPrevSintoma2 { get; set; }                //#### CAFQ
        public DateTime? DestFechaLlegada2 { get; set; }            //#### CAFQ
        public DateTime? DestFechaSalida2 { get; set; }             //#### CAFQ
        public string DestPrevSintoma3 { get; set; }                //#### CAFQ
        public DateTime? DestFechaLlegada3 { get; set; }            //#### CAFQ
        public DateTime? DestFechaSalida3 { get; set; }             //#### CAFQ
        public int? ContacDirectoAnim { get; set; }                 //#### CAFQ
        public string AnimalNaturaContac { get; set; }              //#### CAFQ
        public int? ExpuextoSimilSintoma { get; set; }              //#### CAFQ
        public string NumeIdentContacto { get; set; }               //#### CAFQ
        public int? InfluConfirContacto { get; set; }               //#### CAFQ
        public string TipoRelaContacto { get; set; }                //#### CAFQ
        public int? FamiDirecContacto { get; set; }                 //#### CAFQ
        //public string TrabSaludRama { get; set; }                   //#### CAFQ
        public int? TrabSaludRama { get; set; }                     //#### CAFQ
        public bool? TrabLaboratorio { get; set; }                  //#### CAFQ
        //public string TrabLaboratorioRama { get; set; }             //#### CAFQ
        public int? TrabLaboratorioRama { get; set; }               //#### CAFQ
        public Decimal? Temperatura { get; set; }                   //#### CAFQ
        //public bool? DolorCabeza { get; set; }                    //#### CAFQ                YA EXISTE (Cefalea)
        public bool? Mialgia { get; set; }                          //#### CAFQ
        public bool? Erupcion { get; set; }                         //#### CAFQ
        public string ErupcionLocaliz { get; set; }                 //#### CAFQ
        //public bool? Dolor { get; set; }                          //#### CAFQ
        public string DolorMuscularLocaliz { get; set; }            //#### CAFQ
        //public bool? Disnea { get; set; }                         //#### CAFQ
        public bool? SintomHemorrag { get; set; }                   //#### CAFQ
        public string SintomHemorragDesc { get; set; }              //#### CAFQ
        public bool? AlteracEstadoMental { get; set; }              //#### CAFQ
        public bool? Altralgia { get; set; }                        //#### CAFQ
        public bool? Escalofrios { get; set; }                      //#### CAFQ
        public bool? Conjuntivitis { get; set; }                    //#### CAFQ
        public bool? Rinitis { get; set; }                          //#### CAFQ
        public bool? DiarreaAguda { get; set; }                     //#### CAFQ
        public bool? DiarreaCronica { get; set; }                   //#### CAFQ
        public bool? Mareo { get; set; }                            //#### CAFQ
        public bool? FalloDesarrollo { get; set; }                  //#### CAFQ
        public bool? Hepatomegalea { get; set; }                    //#### CAFQ
        public bool? Ictericia { get; set; }                        //#### CAFQ
        public bool? Linfadenopatia { get; set; }                   //#### CAFQ
        //public bool? MalestarGeneral { get; set; }                //#### CAFQ
        //public bool? Nausea { get; set; }                         //#### CAFQ
        public bool? RigidezNuca { get; set; }                      //#### CAFQ
        public bool? Paralisis { get; set; }                        //#### CAFQ
        public bool? RespiratSuperior { get; set; }                 //#### CAFQ
        public bool? RespiratInferior { get; set; }                 //#### CAFQ
        public bool? DolorRetrorobitario { get; set; }              //#### CAFQ
        public bool? PerdidaPeso { get; set; }                      //#### CAFQ
        public bool? Otro { get; set; }                             //#### CAFQ
        public string OtroDesc { get; set; }                        //#### CAFQ
        public int? InfeccHospit { get; set; }                      //#### CAFQ
        public DateTime? InfeccHospitFecha { get; set; }            //#### CAFQ

        public int? flow { get; set; }
        public int? statement { get; set; }

        //public class CaseGEO : CaseBase
        public int? CountryID { get; set; }
        public int? AreaID { get; set; }
        public int? StateID { get; set; }
        public int? ParishPostOfficeIDJMID { get; set;}
        public string StreetNo { get; set; }
        public string StreetName { get; set; }
        public string ApartmentSuiteLot { get; set; }
        public string Address2 { get; set; }
        public int? LocalID { get; set; }
        public int? NeighborhoodID { get; set; }
		public int? HamletID { get; set; }                      //#### CAFQ: 181018
        public int? ColonyID { get; set; }                      //#### CAFQ: 181018
        public UrbanRural UrbanRural { get; set; }
        public int? CountryID2weeks { get; set; }
        public int? AreaID2weeks { get; set; }
        public int? StateID2weeks { get; set; }
        public int? NeighborhoodID2weeks { get; set; }
        [MaxLength(250)]
        public string Address { get; set; }
        public int? RegionAddress { get; set; }             //#### CAFQ: 181008
        public string PhoneNumber { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        
        public int? CountryOrigin { get; set; }
        public virtual Country Country { get; set; }
        public virtual Area Area { get; set; }
        public virtual State State { get; set; }
        public virtual Local Local { get; set; }
        public virtual Neighborhood Neighborhood { get; set; }
        //public class CaseRisk : CaseBase
        public Vaccin? Vaccin { get; set; } 
        public RiskFactor? RiskFactors { get; set; }
        public int? Comorbidities { get; set; }
        public bool? HDisease { get; set; }
        public bool? Diabetes { get; set; }
        public bool? Neuro { get; set; }
        public bool? Asthma { get; set; }
        public bool? Pulmonary { get; set; }
        public bool? Liver { get; set; }
        public bool? Renal { get; set; }
        public bool? Immunsupp { get; set; }
        public bool? SickleCellDisease { get; set; }

        public bool? ParaCerebral { get; set; }
        public bool? Indigena { get; set; }
        public bool? TrabSalud { get; set; }
        public bool? Desnutricion { get; set; }
        public bool? Prematuridad { get; set; }
        public bool? BajoPesoNacer { get; set; }
        public bool? AusLacMat { get; set; }

        public Pregnant? Pregnant { get; set; }
        //public VaccineOptions? VacInfluenza { get; set; }
        public int? VacInfluenza { get; set; }
        public DateTime? VacInfluenzaDateFirst { get; set; }
        public DateTime? VacInfluenzaDateSecond { get; set; }
        public VaccineOptions? VacBcg { get; set; }
        public DateTime? VacBcgDate { get; set; }
        public int? VacBcgDosis { get; set; }
        public VaccineOptions? VacNeumococo { get; set; }
        public DateTime? VacNeumococoDate { get; set; }
        public int? VacNeumococoDosis { get; set; }
        public VaccineOptions? VacTosFerina { get; set; }
        public DateTime? VacTosFerinaDate { get; set; }
        public int? VacTosFerinaDosis { get; set; }
        public VaccineOptions? VacHaemophilus { get; set; }
        public DateTime? VacHaemophilusDate { get; set; }
        public int? VaccinFuente { get; set; }
        public int? AntiViral { get; set; }
        public DateTime? AntiViralDate { get; set; }
        public DateTime? AntiViralDateEnd { get; set; }
        public int? AntiViralType { get; set; }
        public string AntiViralTypeOther { get; set; }
        public int? OseltaDose { get; set; }
        public string AntiViralDose { get; set; }
        public int? AntiViralDays { get; set; }
        public int? Antibiotic { get; set; }
        public string AntibioticName { get; set; }

        public bool? Pperium { get; set; }
        public Trimester? Trimester { get; set; }
        public int? PregnantWeek { get; set; }
        public bool? Smoking { get; set; }
        public bool? Alcohol { get; set; }
        public bool? DownSyn { get; set; }
        public Obesity? Obesity { get; set; }
        public string OtherRisk { get; set; }
        public string OtherComment { get; set; }
        // public class CaseHospital : CaseBase
        [MaxLength(15)]
        public string CHNum { get; set; }
        public DateTime? FeverDate { get; set; }
        public int? FeverEW { get; set; }
        public int? FeverEY { get; set; }
        //[MaxLength(1)]
        //public string Antiviral { get; set; }
        public DateTime? AStartDate { get; set; }

        public DateTime? DiagDate { get; set; }
        public int? DiagEW { get; set; }
        public int? Salon { get; set; }
        public int? DiagAdm { get; set; }
        public string DiagOtroAdm { get; set; }
        public bool? HallRadio { get; set; }
        public string HallRadioFindings { get; set; }               //#### CAFQ
        public bool? MecVent { get; set; }
        public bool? UCInt { get; set; } //Unidad de cuidados intermedios 
        public bool? UCri { get; set; } //Unidad crítica
        public bool? MecVentNoInv { get; set; }
        public bool? ECMO { get; set; }
        public bool? VAFO { get; set; }
        public int? DiagEg { get; set; }
        public string DiagEgOtro { get; set; }                      //#### CAFQ
        public bool? DifResp { get; set; }
        public bool? MedSatOxig { get; set; }
        public int? SatOxigPor { get; set; }

        public DateTime? HospAmDate { get; set; }
        public int? HospEW { get; set; }
        public DateTime? HospExDate { get; set; }
        public int? ICU { get; set; }
        public DateTime? ICUAmDate { get; set; }
        public int? ICUEW { get; set; }
        public DateTime? ICUExDate { get; set; }
        public string HospitalizedInNumber { get; set; }
        public int? HospitalizedIn { get; set; }
        [MaxLength(1)]
        public string Destin { get; set; }
        public string DestinICU { get; set; }
        public string InstReferName { get; set; }
        public DateTime? FalleDate { get; set; }
        public bool? IsSample { get; set; }
        public int? ReasonNotSamplingID { get; set; }
        public string ReasonNotSamplingOther { get; set; }
        public DateTime? SampleDate { get; set; }           // Sample 1
        [MaxLength(2)]
        public string SampleType { get; set; }
        public DateTime? ShipDate { get; set; }
        public long? LabID { get; set; }
        public DateTime? SampleDate2 { get; set; }          // Sample 2
        [MaxLength(2)]
        public string SampleType2 { get; set; }
        public DateTime? ShipDate2 { get; set; }
        public long? LabID2 { get; set; }
        public DateTime? SampleDate3 { get; set; }          // Sample 3
        [MaxLength(2)]
        public string SampleType3 { get; set; }
        public DateTime? ShipDate3 { get; set; }
        public long? LabID3 { get; set; }
        public bool? Adenopatia { get; set; }
        public bool? AntecedentesFiebre { get; set; }
        public bool? Rinorrea { get; set; }
        public bool? Malestar { get; set; }
        public bool? Nauseas { get; set; }
        public bool? DolorMuscular { get; set; }
        public bool? Disnea { get; set; }
        public bool? DolorCabeza { get; set; }
        public bool? Estridor { get; set; }
        public bool? Tos { get; set; }
        public bool? Tiraje { get; set; }
        public bool? Odinofagia { get; set; }
        public bool? Wheezing { get; set; }             //#### CAFQ: 180619
        //public class CaseLab : CaseBase
        public DateTime? RecDate { get; set; }
        public string Identification_Test { get; set; }
        public bool? Processed { get; set; }
        public int? NoProRenId { get; set; }
        public string NoProRen { get; set; }
        public Decimal? TempSample1 { get; set; }
        public DateTime? RecDate2 { get; set; }
        public string Identification_Test2 { get; set; }
        public bool? Processed2 { get; set; }
        public string NoProRen2 { get; set; }
        public int? NoProRenId2 { get; set; }
        public Decimal? TempSample2 { get; set; }
        public DateTime? RecDate3 { get; set; }
        public string Identification_Test3 { get; set; }
        public bool? Processed3 { get; set; }
        public string NoProRen3 { get; set; }
        public int? NoProRenId3 { get; set; }
        public Decimal? TempSample3 { get; set; }

        // AM Laboratorio Nacional solo para HN
        public DateTime? RecDate_National { get; set; }
        public string Identification_Test_National { get; set; }
        public Decimal? TempSample_National { get; set; }
        public bool? Processed_National { get; set; }
        public int? NoProRenId_National { get; set; }
        public string NoProRen_National { get; set; }

        // AM Laboratorio intermedio
        public DateTime? Rec_Date_NPHL { get; set; }
        public string Identification_Test_NPHL { get; set; }
        public Decimal? Temp_NPHL { get; set; }
        public string Observation_NPHL { get; set; }
        public DateTime? Ship_Date_NPHL { get; set; }
        public bool? NPHL_Processed { get; set; }
        public int? NPHL_NoProRenId { get; set; }
        public string NPHL_NoProRen { get; set; }
        public string NPHL_Conclusion { get; set; }

        public DateTime? Rec_Date_NPHL_2 { get; set; }
        public string Identification_Test_NPHL_2 { get; set; }
        public Decimal? Temp_NPHL_2 { get; set; }
        public string Observation_NPHL_2 { get; set; }
        public DateTime? Ship_Date_NPHL_2 { get; set; }
        public bool? NPHL_Processed_2 { get; set; }
        public int? NPHL_NoProRenId_2 { get; set; }
        public string NPHL_NoProRen_2 { get; set; }
        public string NPHL_Conclusion_2 { get; set; }

        public DateTime? Rec_Date_NPHL_3 { get; set; }
        public string Identification_Test_NPHL_3 { get; set; }
        public Decimal? Temp_NPHL_3 { get; set; }
        public string Observation_NPHL_3 { get; set; }
        public DateTime? Ship_Date_NPHL_3 { get; set; }
        public bool? NPHL_Processed_3 { get; set; }
        public int? NPHL_NoProRenId_3 { get; set; }
        public string NPHL_NoProRen_3 { get; set; }
        public string NPHL_Conclusion_3 { get; set; }

        public DateTime? EndLabDate { get; set; }
        public string FResult { get; set; }
        public string Comments { get; set; }
        public string FinalResult { get; set; }
        public int? FinalResultVirusTypeID { get; set; }
        public int? FinalResultVirusSubTypeID { get; set; }
        public int? FinalResultVirusLineageID { get; set; }
        public string FinalResult_2 { get; set; }
        public int? FinalResultVirusTypeID_2 { get; set; }
        public int? FinalResultVirusSubTypeID_2 { get; set; }
        public int? FinalResultVirusLineageID_2 { get; set; }
        public string FinalResult_3 { get; set; }
        public int? FinalResultVirusTypeID_3 { get; set; }
        public int? FinalResultVirusSubTypeID_3 { get; set; }
        public int? FinalResultVirusLineageID_3 { get; set; }
        public string GeneticGroup { get; set; }
        public string GeneticGroup_2 { get; set; }
        public string GeneticGroup_3 { get; set; }
        public int? CaseStatus { get; set; }
        public DateTime? CloseDate { get; set; }
        public string ObservationCase { get; set; }
        public virtual ICollection<CaseLabTest> CaseLabTests { get; set; }
        public virtual Hospital Hospital { get; set; }
        public virtual Lab Lab { get; set; }
        [ForeignKey("FinalResultVirusTypeID")]
        public virtual CatVirusType CatVirusType_FR1 { get; set; }
        [ForeignKey("FinalResultVirusTypeID_2")]
        public virtual CatVirusType CatVirusType_FR2 { get; set; }
        [ForeignKey("FinalResultVirusTypeID_3")]
        public virtual CatVirusType CatVirusType_FR3 { get; set; }

        [ForeignKey("CaseStatus")]
        public virtual CatStatusCase CatStatusCase { get; set; }

        public int? MuestraID1 { get; set; }
        public int? MuestraID2 { get; set; }
        public int? MuestraID3 { get; set; }
        public int? MuestraID4 { get; set; }
        public int? MuestraID5 { get; set; }
        public int? MuestraID6 { get; set; }
        public int? MuestraID7 { get; set; }
        public int? MuestraID8 { get; set; }
        public int? MuestraID9 { get; set; }
        public int? MuestraID10 { get; set; }
        public int? MuestraID11 { get; set; }
        public int? MuestraID12 { get; set; }
        public int? MuestraID13 { get; set; }
        public int? MuestraID14 { get; set; }
        public int? MuestraID15 { get; set; }
    }

    public class CaseLabTest : CaseBase
    {
        public int ID { get; set; }
        public long? LabID { get; set; }
        public int FluCaseID { get; set; }
        //public long? LabID { get; set; }
        public bool? Processed { get; set; }
        public int? SampleNumber { get; set; }
        public int? VirusTypeID { get; set; }
        public Nullable<decimal> CTVirusType { get; set; }
        public decimal? CTRLVirusType { get; set; }
        public int? OtherVirusTypeID { get; set; }
        public decimal? CTOtherVirusType { get; set; }
        public decimal? CTRLOtherVirusType { get; set; }
        public string OtherVirus { get; set; }
        //public int? InfA { get; set; }
        public int? VirusSubTypeID { get; set; }
        public Decimal? CTSubType { get; set; }
        public decimal? CTRLSubType { get; set; }
        public string TestResultID_VirusSubType { get; set; }
        public int? VirusSubTypeID_2 { get; set; }
        public decimal? CTSubType_2 { get; set; }
        public decimal? CTRLSubType_2 { get; set; }
        public string TestResultID_VirusSubType_2 { get; set; }

        //public int? InfB { get; set; }
        public int? VirusLineageID { get; set; }
        public decimal? CTLineage { get; set; }
        public decimal? CTRLLineage { get; set; }
        //public int? ParaInfI { get; set; }
        //public int? ParaInfII { get; set; }
        //public int? ParaInfIII { get; set; }
        //public int? RSV { get; set; }
        //public int? Adenovirus { get; set; }
        //public int? Metapneumovirus { get; set; }
        public decimal? RNP { get; set; }
        public decimal? CTRLRNP { get; set; }
        public decimal? CTRLNegative { get; set; }
        public int? TestType { get; set; }

        //[ForeignKey("CatTestResult")]
        //[Column("value")]
        public string TestResultID { get; set; }
        public DateTime? TestDate { get; set; }
        public DateTime? TestEndDate { get; set; }
        //public virtual FluCase FluCase { get; set; }
        //public virtual VirusType VirusType { get; set; }
        // Control de flujo de las muestras
        public int? flow_test { get; set; }
        public int? statement_test { get; set; }
        public int? flow_flucase { get; set; }
        public int? statement_flucase { get; set; }
        public long? inst_cnf_orig { get; set; }
        public int? inst_conf_end_flow_by_virus { get; set; }

        [ForeignKey("TestType")]
        public virtual CatTestType CatTestType { get; set; }

        [ForeignKey("VirusTypeID")]
        public virtual CatVirusType CatVirusType { get; set; }
        [ForeignKey("VirusSubTypeID")]
        public virtual CatVirusSubType CatVirusSubType { get; set; }
        [ForeignKey("VirusSubTypeID_2")]
        public virtual CatVirusSubType CatVirusSubType_2 { get; set; }

        [ForeignKey("VirusLineageID")]
        public virtual CatVirusLinaje CatVirusLinaje { get; set; }

        [ForeignKey("LabID")]
        public virtual Institution Institution { get; set; }

        [ForeignKey("TestResultID")]
        public virtual CatTestResult CatTestResult { get; set; }
    }

    public class CatSurv			//**** NEW
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ENG { get; set; }
        public int? Orden { get; set; }
    }

    public class CatPopulationInstitution : CaseBase
    {
        public int id { get; set; }
        [Display(Name = "País")]
        public int country_id { get; set; }
        [Display(Name = "Institución")]
        public long id_institution { get; set; }
        [Display(Name = "Población")]
        public long population { get; set; }
        [Display(Name = "Año")]
        public int year { get; set; }

        [ForeignKey("country_id")]
        public virtual Country CountryPopulation { get; set; }

        [ForeignKey("id_institution")]
        public virtual Institution InstitutionPopulation { get; set; }

        [ForeignKey("CatPobInstId")]
        public virtual ICollection<CatPopulationInstitutionDetail> CatPopulationDetails { get; set; }

    }

    public class CatPopulationInstitutionDetail
    {
        public int Id { get; set; }
        //[Key]
        public int CatPobInstId { get; set; }
        public int? AgeGroup { get; set; }
        public int? PopulationFem { get; set; }
        public int? PopulationMaso { get; set; }
        public int? PopulationT { get; set; }

        [ForeignKey("AgeGroup")]
        public virtual CatAgeGroup AgeGroupbyCountry { get; set; }
    }

    public class CaseSummary : CaseBase
    {
        public int Id { get; set; }
        public int HosiptalId { get; set; }
        public DateTime StartDateOfWeek { get; set; }
        public int? EW { get; set; }
        public int? EpiYear { get; set; }
        public virtual ICollection<CaseSummaryDetail> CaseSummaryDetails { get; set; }
    }

    public class CaseSummaryJM : CaseBase                   //#### CAFQ: Summary Jamaica
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }                 //#### CAFQ: Summary Jamaica
        public DateTime WeekendDate { get; set; }           //#### CAFQ: Summary Jamaica
        public int? EW { get; set; }
        public int? EpiYear { get; set; }

        [ForeignKey("CaseSummaryId")]
        public virtual ICollection<CaseSummaryDetailJM> CaseSummaryDetailJM { get; set; }
    }

    public class CaseSummaryDetail
    {
        public int Id { get; set; }
        public int CaseSummaryId { get; set; }
        public int AgeGroup { get; set; }
        public int ETIDenoFem { get; set; }
        public int ETIDenoMaso { get; set; }
        public int ETIDenoST { get; set; }
        public int ETINumFem { get; set; }
        public int ETINumMaso { get; set; }
        public int ETINumST { get; set; }
        public int ETINumEmerFem { get; set; }
        public int ETINumEmerMaso { get; set; }
        public int ETINumEmerST { get; set; }
        public int HospFem { get; set; }
        public int HospMaso { get; set; }
        public int HospST { get; set; }
        public int HospSARIFem { get; set; }                      //#### CAFQ: 181101
        public int HospSARIMaso { get; set; }
        public int HospSARIST { get; set; }
        public int UCIFem { get; set; }
        public int UCIMaso { get; set; }
        public int UCIST { get; set; }
        public int UCISARIFem { get; set; }                      //#### CAFQ: 181101
        public int UCISARIMaso { get; set; }
        public int UCISARIST { get; set; }
        public int DefFem { get; set; }
        public int DefMaso { get; set; }
        public int DefST { get; set; }
        public int DefSARIFem { get; set; }                     //#### CAFQ: 181101
        public int DefSARIMaso { get; set; }
        public int DefSARIST { get; set; }
        public int? NeuFem { get; set; }
        public int? NeuMaso { get; set; }
        public int? NeuST { get; set; }
        public int? CCSARIFem { get; set; }
        public int? CCSARIMaso { get; set; }
        public int? CCSARIST { get; set; }
        public int? VentFem { get; set; }
        public int? VentMaso { get; set; }
        public int? VentST { get; set; }
    }

    public class CaseSummaryDetailJM                   //#### CAFQ: Summary Jamaica
    {
        public int Id { get; set; }
        public int CaseSummaryId { get; set; }
        public int AgeGroup { get; set; }
        public int ILICases { get; set; }
        public int ILISamplesTaken { get; set; }
        public int TotalVisits { get; set; }
    }

    public class Report
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Template { get; set; }
    }

    public class ReportCountry
    {
        public int ID { get; set; }
        public string description { get; set; }
        public int startCol { get; set; }
        public int startRow { get; set; }
        public bool active { get; set; }
		public string logo { get; set; }
        public int logoCol { get; set; }
        public int logoRow { get; set; }
        public int ReportID { get; set; }
        public int CountryID { get; set; }

    }

    public class ImportLog
    {
        public int ID { get; set; }
        public DateTime Fecha_Import { get; set; }
        public string User_Import { get; set; }
        public int Country_ID { get; set; }
        public string ImportedFilename { get; set; }
    }

    public class CatPadronCR_ImportLog
    {
        public int ID { get; set; }
        public DateTime Fecha_Import { get; set; }
        public string User_Import { get; set; }
        public int Country_ID { get; set; }
        public string ImportedFilename { get; set; }
    }

    public class RecordHistory
    {
        public int Id { get; set; }
        public string Userid { get; set; }
        public int? Recordid { get; set; }
        public int Action { get; set; }
        public int? flow { get; set; }
        public int? state { get; set; }
        public DateTime DateAction { get; set; }
    }

    public class GraphCache
    {
        public int ID { get; set; }
        public DateTime? FechaCache { get; set; }
        public string Graph { get; set; }
        public string Year { get; set; }
        public int CountryID { get; set; }
        public int? RegionID { get; set; }
        public int? StateID { get; set; }
        public int? HospitalID { get; set; }
        public int? IRAG { get; set; }
        public int? ETI { get; set; }
        public string GraphData { get; set; }

    }

    public class VirusType
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }

    public class TestResult
    {
        [MaxLength(1)]
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class CatDashboardLink          //**** CAFQ
    {
        public int id { get; set; }
        public int id_country { get; set; }
        public string link { get; set; }
        public string title { get; set; }
    }

    public class CatOccupation          //**** CAFQ
    {
        public int Id { get; set; }
        public string Occupation_SPA { get; set; }
        public string Occupation_ENG { get; set; }
        public int CIUO_08 { get; set; }
    }

    public class CatTrabSaludRama          //**** CAFQ
    {
        public int Id { get; set; }
        public string Rama_SPA { get; set; }
        public string Rama_ENG { get; set; }
    }

    public class CatTrabLaboRama          //**** CAFQ
    {
        public int Id { get; set; }
        public string Rama_SPA { get; set; }
        public string Rama_ENG { get; set; }
    }

    public enum TestType
    {
        IFA = 1,
        RT_PCR = 2,
        CULTURE = 3
    }

    public enum BooleanType
    {
        Si = 1,
        No = 0
    }

    public enum BooleanTypeNA
    {
        Si = 1,
        No = 0,
        [Description("No Aplica")]
        NA = 9
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Unknown = 3
    }

    public enum AgeGroup
    {
        Age_less_than_2_years = 1,
        Age_2_to_4_years = 2,
        Age_5_to_14_years = 3,
        Age_15_to_34_years = 4,
        Age_35_to_64_years = 5,
        Age_65_years_and_over = 6
    }

    public enum AgeGroup_SUR
    {
        Age_child_under_6_months = 1,
        Age_6_to_11_months = 2,
        Age_12_to_23_months = 3,
        Age_2_to_4_years = 4,
        Age_5_to_14_years = 5,
        Age_15_to_34_years = 6,
        Age_35_to_64_years = 7,
        Age_65_years_and_over = 8
    }

    public enum AMeasure
    {
        Day = 1,
        Month = 2,
        Year = 3
    }

    public enum Obesity
    {
        Without_Data_obesity = 0,
        BMI_between_30_40 = 1,
        BMI_over_40 = 2,
        Obesity_without_BMI_data = 3
    }

    public enum RiskFactor
    {
        NoRiskFactors = 0,
        ThereAreRiskFactors = 1,
        Without_Date_Unknown = 9
    }

    public enum UrbanRural
    {
        Urban = 1,
        Rural = 2,
        Unknow = 0,
    }

    public enum Vaccin
    {
        VaccinatedLastAvailabeToDate = 1,
        VaccinatedPreviousOrNotVaccinated = 2,
        UNknown = 3
    }

    public enum Trimester
    {
        First = 1,
        Second = 2,
        Third = 3,
        Unknown = 9
    }

    public enum Pregnant
    {
        Sí = 1,
        No = 0,
        Desconocido = 9
    }

    public enum VaccineOptions
    {
        Sí = 1,
        No = 2,
        Desconocido = 9
    }

    public enum InstitutionType
    {
        Hospital = 1,
        Lab = 2,
        Admin = 3
    }

    public enum AccessLevel
    {
        All = 1,
        Country = 2,
        Area = 3,
        State = 7,
        Regional = 5,
        SelfOnly = 4,
        Service = 6

    }

    //public enum AccessLevel_JAM
    //{
    //    All = 1,
    //    Country = 2,
    //    Parish = 3,
    //    SelfOnly = 4,
    //    Regional = 5,
    //    Service = 6
    //}

    public class PahoDbContext : IdentityDbContext<ApplicationUser>
    {
        public PahoDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static PahoDbContext Create()
        {
            return new PahoDbContext();
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Local> Locals { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<CatParishPostOfficeJM> CatParishPostOfficeJM { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }
		public DbSet<Hamlet> Hamlets { get; set; } 
        public DbSet<Colony> Colonies { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<InstitutionConfiguration> InstitutionsConfiguration { get; set; }
        public DbSet<InstitutionConfFlowByVirus> InstitutionConfFlowByVirus { get; set; }           //**** NEW: 1911
        public DbSet<InstitutionConfEndFlowByVirus> InstitutionConfEndFlowByVirus { get; set; }
        public DbSet<InstitutionLocationType> InstitutionLocationType { get; set; }
        public DbSet<CatDiag> CIE10 { get; set; }
        public DbSet<CatServicios> Salones { get; set; }
        public DbSet<FluCase> FluCases { get; set; }
        public DbSet<VirusType> VirusTypes { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<CatDashboardLink> CatDashboarLinks { get; set; }               //#### CAFQ
        public DbSet<CatOccupation> CatOccupations { get; set; }                    //#### CAFQ
        public DbSet<CatTrabSaludRama> CatTrabSaludRamas { get; set; }              //#### CAFQ
        public DbSet<CatTrabLaboRama> CatTrabLaboRamas { get; set; }                //#### CAFQ
        //Catalogos
        public DbSet<CatRegionType> CatRegionType { get; set; }
        public DbSet<CatSampleNoProcessed> CatSampleNoProcessed { get; set; }
        public DbSet<CatCaseStatus> CatCaseStatus { get; set; }
        public DbSet<CatTestType> CatTestType { get; set; }
        public DbSet<CatReasonNotSampling> CatReasonNotSampling { get; set; }
        public DbSet<CatTestResult> CatTestResult { get; set; }
        public DbSet<CatVirusType> CatVirusType { get; set; }
        public DbSet<CatOtherVirusType> CatOtherVirusType { get; set; }
        public DbSet<CatVirusSubType> CatVirusSubType { get; set; }
        public DbSet<CatVirusSubTypeConfByLab> CatVirusSubTypeConfByLab { get; set; }
        public DbSet<CatVirusLinaje> CatVirusLinaje { get; set; }
        public DbSet<CatAgeGroup> CatAgeGroup { get; set; }
        public DbSet<CatAgeGroupJM> CatAgeGroupJM { get; set; }
        public DbSet<CatAntiviral> CatAntiviral { get; set; }
        public DbSet<CatNativePeople> CatNativePeople { get; set; }
        public DbSet<CatVaccinSource> CatVaccinSource { get; set; }
        public DbSet<CatVaccinSourceConf> CatVaccinSourceConf { get; set; }
        public DbSet<CatPopulationInstitution> CatPopulationInstitutions { get; set; }
        public DbSet<CatPopulationInstitutionDetail> CatPopulationInstitutionsDetails { get; set; }
        public DbSet<CatStatusCase> CatStatusCase { get; set; }
        public DbSet<CatHospitalizedIn> CatHospitalizedIn { get; set; }
        public DbSet<CatSurv> CatSurv { get; set; }                                 //**** NEW: 1911
        public DbSet<CatCloseParameters> CatCloseParameters { get; set; }           //**** NEW: 1911
        ////public DbSet<CaseLabs> CaseLabses { get; set; }                             //**** NEW: 1911
        public DbSet<CatVirusFlow> CatViruFlows { get; set; }                       //**** NEW: 1911
        public DbSet<ConfAlertCaseDefinition> ConfAlertCaseDefinition { get; set; }

        public DbSet<CaseLabTest> CaseLabTests { get; set; }
        public DbSet<CaseSummary> CaseSummaries { get; set; }
        public DbSet<CaseSummaryJM> CaseSummariesJM { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportCountry> ReportsCountries { get; set; }
        public DbSet<ImportLog> ImportedFileList { get; set; }
        public DbSet<CatPadronCR_ImportLog> ImportedFileListPadron { get; set; }
        public DbSet<RecordHistory> RecordHistories { get; set; }

        //Laboratorios externos
        public DbSet<InstitutionForeignConf> InstitutionForeignConf { get; set; }
        // Tipo de institución para HON
        public DbSet<CatInstitutionTypeHON> CatInstitutionTypeHON { get; set; }

        // Graficas
        public DbSet<GraphCache> GraphCache { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<InstitutionConfiguration>();
            modelBuilder.Entity<ReportCountry>();
            modelBuilder.Entity<Institution>()
               .Map<Hospital>(m => m.Requires("InstitutionType").HasValue((int)InstitutionType.Hospital))
               .Map<Lab>(m => m.Requires("InstitutionType").HasValue((int)InstitutionType.Lab))
               .Map<AdminInstitution>(m => m.Requires("InstitutionType").HasValue((int)InstitutionType.Admin))
               ;
            modelBuilder.Entity<FluCase>()
                // Map to the Users table
           .Map(map =>
           {
               map.Properties(p => new
               {
                   p.ID,
                   p.HospitalID,
                   p.HospitalID_CaseGenerating,
                   p.HospitalDate,
                   p.RegDate,
                   p.FName1,
                   p.FName2,
                   p.FullName,
                   p.LName1,
                   p.LName2,
                   p.DocumentType,
                   p.NoExpediente,
                   p.NationalId,
                   p.DOB,
                   p.Age,
                   p.AMeasure,
                   p.AgeGroup,
                   p.Gender,
                   p.ResponsibleMinor,
                   p.UserID,
                   p.InsertDate,
                   p.LastUpdate,
                   p.Surv,
                   p.SurvInusual,
                   p.Brote,
                   p.nativepeople,
                   p.nationality,
                   p.Ocupacion,             //#### CAFQ
                   p.TrabajoDirecc,             //#### CAFQ
                   p.TrabajoEstablec,             //#### CAFQ
                   p.ContactoAnimVivos,             //#### CAFQ
                   p.OcupacMercAnimVivos,             //#### CAFQ
                   p.flow,
                   p.statement,
                   // CaseGeo
                   p.CountryID,
                   p.AreaID,
                   p.StateID,
                   p.ParishPostOfficeIDJMID,
                   p.StreetNo,
                   p.StreetName,
                   p.ApartmentSuiteLot,
                   p.Address2,
                   p.LocalID,
                   p.NeighborhoodID,
				   p.HamletID,                  //#### CAFQ: 181018
                   p.ColonyID,                  //#### CAFQ: 181018
                   p.UrbanRural,
                   p.CountryID2weeks,
                   p.AreaID2weeks,
                   p.StateID2weeks,
                   p.NeighborhoodID2weeks,
                   p.Address,
                   p.RegionAddress,             //#### CAFQ: 181008
                   p.CountryOrigin,
                   p.PhoneNumber,
                   p.Latitude,
                   p.Longitude,
                   //CaseRisk
                   p.Vaccin,
                   p.RiskFactors,
                   p.Comorbidities,
                   p.HDisease,
                   p.Diabetes,
                   p.Neuro,
                   p.Asthma,
                   p.Pulmonary,
                   p.Liver,
                   p.Renal,
                   p.Immunsupp,
                   p.ParaCerebral,
                   p.Indigena,
                   p.TrabSalud,
                   p.Desnutricion,
                   p.Prematuridad,
                   p.BajoPesoNacer,
                   p.AusLacMat,
                   p.Pregnant,
                   p.Pperium,
                   p.Trimester,
                   p.PregnantWeek,
                   p.Smoking,
                   p.Alcohol,
                   p.DownSyn,
                   p.Obesity,
                   p.OtherRisk,
                   p.OtherComment,
                   p.VacInfluenza,
                   p.VacInfluenzaDateFirst,
                   p.VacInfluenzaDateSecond,
                   p.VacBcg,
                   p.VacBcgDate,
                   p.VacBcgDosis,
                   p.VacNeumococo,
                   p.VacNeumococoDate,
                   p.VacNeumococoDosis,
                   p.VacTosFerina,
                   p.VacTosFerinaDate,
                   p.VacTosFerinaDosis,
                   p.VacHaemophilus,
                   p.VacHaemophilusDate,
                   p.VaccinFuente,
                   p.AntiViral,
                   p.AntiViralDate,
                   p.AntiViralDateEnd,
                   p.AntiViralType,
                   p.AntiViralTypeOther,
                   p.Antibiotic,
                   p.AntibioticName,
                   p.OseltaDose,
                   p.AntiViralDose,
                   p.AntiViralDays,
                   p.ViajePrevSintoma,             //#### CAFQ
                   p.DestPrevSintoma1,             //#### CAFQ
                   p.DestFechaLlegada1,             //#### CAFQ
                   p.DestFechaSalida1,             //#### CAFQ
                   p.DestPrevSintoma2,             //#### CAFQ
                   p.DestFechaLlegada2,             //#### CAFQ
                   p.DestFechaSalida2,             //#### CAFQ
                   p.DestPrevSintoma3,             //#### CAFQ
                   p.DestFechaLlegada3,             //#### CAFQ
                   p.DestFechaSalida3,             //#### CAFQ
                   p.ContacDirectoAnim,             //#### CAFQ
                   p.AnimalNaturaContac,             //#### CAFQ
                   p.ExpuextoSimilSintoma,             //#### CAFQ
                   p.NumeIdentContacto,             //#### CAFQ
                   p.InfluConfirContacto,             //#### CAFQ
                   p.TipoRelaContacto,             //#### CAFQ
                   p.FamiDirecContacto,             //#### CAFQ
                   p.TrabSaludRama,             //#### CAFQ
                   p.TrabLaboratorio,             //#### CAFQ
                   p.TrabLaboratorioRama,             //#### CAFQ
                   p.SickleCellDisease,
                   // CaseHospital
                   p.CHNum,
                   p.FeverDate,
                   p.FeverEW,
                   p.FeverEY,
                   p.DiagDate,
                   p.DiagEW,
                   p.Salon,
                   p.DiagAdm,
                   p.DiagOtroAdm,
                   p.HallRadio,
                   p.HallRadioFindings,             //#### CAFQ
                   p.UCInt,
                   p.UCri, 
                   p.MecVent,
                   p.MecVentNoInv,
                   p.ECMO,
                   p.VAFO,
                   p.DiagEg,
                   p.DiagEgOtro,            //#### CAFQ
                   p.DifResp,
                   p.MedSatOxig,
                   p.SatOxigPor,

                   p.AStartDate,
                   p.HospAmDate,
                   p.HospEW,
                   p.HospExDate,
                   p.ICU,
                   p.ICUAmDate,
                   p.ICUEW,
                   p.ICUExDate,
                   p.HospitalizedIn,
                   p.HospitalizedInNumber,
                   p.Destin,
                   p.DestinICU,
                   p.InstReferName,
                   p.FalleDate,
                   p.IsSample,
                   p.ReasonNotSamplingID,
                   p.ReasonNotSamplingOther,
                   p.SampleDate,
                   p.SampleType,
                   p.ShipDate,
                   p.LabID,
                   p.SampleDate2,
                   p.SampleType2,
                   p.ShipDate2,
                   p.LabID2,
                   p.SampleDate3,
                   p.SampleType3,
                   p.ShipDate3,
                   p.LabID3,
                   p.Adenopatia,
                   p.Wheezing,                        //#### CAFQ: 180619
                   p.AntecedentesFiebre,
                   p.Rinorrea,
                   p.Malestar,
                   p.Nauseas,
                   p.DolorMuscular,
                   p.Disnea,
                   p.DolorCabeza,
                   p.Estridor,
                   p.Tos,
                   p.Tiraje,
                   p.Odinofagia,
                   p.Temperatura,                          //#### CAFQ
                   p.Mialgia,                          //#### CAFQ
                   p.Erupcion,                          //#### CAFQ
                   p.ErupcionLocaliz,                          //#### CAFQ
                   //p.Dolor,                          //#### CAFQ
                   p.DolorMuscularLocaliz,                          //#### CAFQ
                   p.SintomHemorrag,                          //#### CAFQ
                   p.SintomHemorragDesc,                          //#### CAFQ
                   p.AlteracEstadoMental,                          //#### CAFQ
                   p.Altralgia,                          //#### CAFQ
                   p.Escalofrios,                          //#### CAFQ
                   p.Conjuntivitis,                          //#### CAFQ
                   p.Rinitis,                          //#### CAFQ
                   p.DiarreaAguda,                          //#### CAFQ
                   p.DiarreaCronica,                          //#### CAFQ
                   p.Mareo,                          //#### CAFQ
                   p.FalloDesarrollo,                          //#### CAFQ
                   p.Hepatomegalea,                          //#### CAFQ
                   p.Ictericia,                          //#### CAFQ
                   p.Linfadenopatia,                          //#### CAFQ
                   //p.MalestarGeneral,                          //#### CAFQ
                   //p.Nausea,                          //#### CAFQ
                   p.RigidezNuca,                          //#### CAFQ
                   p.Paralisis,                          //#### CAFQ
                   p.RespiratSuperior,                          //#### CAFQ
                   p.RespiratInferior,                          //#### CAFQ
                   p.DolorRetrorobitario,                          //#### CAFQ
                   p.PerdidaPeso,                          //#### CAFQ
                   p.Otro,                          //#### CAFQ
                   p.OtroDesc,                          //#### CAFQ
                   p.InfeccHospit,                          //#### CAFQ
                   p.InfeccHospitFecha,                          //#### CAFQ
                   // CaseLab
                   p.RecDate,
                   p.Identification_Test,
                   p.Processed,
                   p.NoProRen,
                   p.NoProRenId,
                   p.TempSample1,

                   p.RecDate2,
                   p.Identification_Test2,
                   p.Processed2,
                   p.NoProRenId2,
                   p.NoProRen2,
                   p.TempSample2,

                   p.RecDate3,
                   p.Identification_Test3,
                   p.Processed3,
                   p.NoProRenId3,
                   p.NoProRen3,
                   p.TempSample3,
                   // AM Laboratorio Nacional solo para HN
                    p.RecDate_National,
                    p.Identification_Test_National,
                    p.Processed_National,
                    p.NoProRenId_National,
                    p.NoProRen_National,
                    p.TempSample_National,

                   //AM Laboratorio intermedio
                   p.Rec_Date_NPHL, 
                   p.Identification_Test_NPHL,
                   p.Temp_NPHL,                   
                   p.Observation_NPHL,                   
                   p.Ship_Date_NPHL,                   
                   p.NPHL_Processed,
                   p.NPHL_NoProRenId,
                   p.NPHL_NoProRen,
                   p.NPHL_Conclusion,

                   p.Rec_Date_NPHL_2,
                   p.Identification_Test_NPHL_2,
                   p.Temp_NPHL_2,
                   p.Observation_NPHL_2,
                   p.Ship_Date_NPHL_2,
                   p.NPHL_Processed_2,
                   p.NPHL_NoProRenId_2,
                   p.NPHL_NoProRen_2,
                   p.NPHL_Conclusion_2,

                   p.Rec_Date_NPHL_3,
                   p.Identification_Test_NPHL_3,
                   p.Temp_NPHL_3,
                   p.Observation_NPHL_3,
                   p.Ship_Date_NPHL_3,
                   p.NPHL_Processed_3,
                   p.NPHL_NoProRenId_3,
                   p.NPHL_NoProRen_3,
                   p.NPHL_Conclusion_3,

                   p.EndLabDate,
                   p.FResult,
                   p.Comments,
                   p.FinalResult,
                   p.FinalResultVirusTypeID,
                   p.FinalResultVirusSubTypeID,
                   p.FinalResultVirusLineageID,
                   p.FinalResult_2,
                   p.FinalResultVirusTypeID_2,
                   p.FinalResultVirusSubTypeID_2,
                   p.FinalResultVirusLineageID_2,
                   p.FinalResult_3,
                   p.FinalResultVirusTypeID_3,
                   p.FinalResultVirusSubTypeID_3,
                   p.FinalResultVirusLineageID_3,
                   p.GeneticGroup,                              // Grupo genetico
                   p.GeneticGroup_2,
                   p.GeneticGroup_3,
                   p.CaseStatus,
                   p.CloseDate,
                   p.ObservationCase

               });
               map.ToTable("FluCase");
           })
           .Map(map =>
           {
               map.Properties(p => new
               {
                   p.MuestraID1,
                   p.MuestraID2,
                   p.MuestraID3,
                   p.MuestraID4,
                   p.MuestraID5,
                   p.MuestraID6,
                   p.MuestraID7,
                   p.MuestraID8,
                   p.MuestraID9,
                   p.MuestraID10,
                   p.MuestraID11,
                   p.MuestraID12,
                   p.MuestraID13,
                   p.MuestraID14,
                   p.MuestraID15
               });

               map.ToTable("LabTestReport");
           });
        }
        public override int SaveChanges()
        {
            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges
                return base.SaveChanges();

            }
            catch (DbEntityValidationException e)
            {
                StringBuilder message = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    message.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(message.ToString());
            }
        }

        public void LoadLookUpData()
        {
            var countries = new List<Country>
            {  
                 new Country {Active = false, Code ="AR",	Name = "Argentina"},                            
                 new Country {Active = false, Code ="BB",	Name = "Barbados"},                   
                 new Country {Active = true,  Code ="BO",	Name = "Bolivia"},                   
                 new Country {Active = false, Code ="BR",	Name = "Brasil"},                   
                 new Country {Active = false, Code ="BZ",	Name = "Belice"},                   
                 new Country {Active = false, Code ="CA",	Name = "Canadá"},                   
                 new Country {Active = true,  Code ="CL",	Name = "Chile"},                   
                 new Country {Active = false, Code ="CO",	Name = "Colombia"},                   
                 new Country {Active = false, Code ="CR",	Name = "Costa Rica"},                   
                 new Country {Active = false, Code ="CU",	Name = "Cuba"},                   
                 new Country {Active = false, Code ="DM",	Name = "Dominica"},                   
                 new Country {Active = false, Code ="DO",	Name = "Rep. Dominicana"},                   
                 new Country {Active = false, Code ="EC",	Name = "Ecuador"},                   
                 new Country {Active = false, Code ="GT",	Name = "Guatemala"},                   
                 new Country {Active = false, Code ="HN",	Name = "Honduras"},                   
                 new Country {Active = false, Code ="HT",	Name = "Haití"},                   
                 new Country {Active = false, Code ="JM",	Name = "Jamaica"},                   
                 new Country {Active = false, Code ="LC",	Name = "Santa Lucía"},                   
                 new Country {Active = false, Code ="MX",	Name = "México"},                   
                 new Country {Active = false, Code ="NI",	Name = "Nicaragua"},                   
                 new Country {Active = false, Code ="PA",	Name = "Panamá"},                   
                 new Country {Active = false, Code ="PE",	Name = "Perú"},                   
                 new Country {Active = false, Code ="PR",	Name = "Puerto Rico"},                   
                 new Country {Active = false, Code ="PY",	Name = "Paraguay"},                   
                 new Country {Active = false, Code ="SR",	Name = "Surinam"},                   
                 new Country {Active = false, Code ="SV",	Name = "El Salvador"},                   
                 new Country {Active = false, Code ="TT",	Name = "Trinidad y Tobago"},                   
                 new Country {Active = false, Code ="US",	Name = "Estados Unidos"},                   
                 new Country {Active = false, Code ="UY",	Name = "Uruguay"},                   
                 new Country {Active = false, Code ="VC",	Name = "S.Vicente y Granad."},                   
                 new Country {Active = false, Code ="VE",	Name = "Venezuela"},                   
                 new Country {Active = false, Code ="ZZ",	Name = "ZZZ Naciones Unidas"}
            };
            countries.ForEach(v => Countries.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var areas = new List<Area>
            {               
                    new Area {   CountryID = countries.Single(s => s.Code == "BO").ID,  Name = "Altiplano"},                                                                  
                    new Area {   CountryID = countries.Single(s => s.Code == "BO").ID,  Name = "Valles"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "BO").ID,  Name = "Llanos"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Tarapacá"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Antofagasta"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "CL	4	De Coquimbo"},                                            
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Valparaíso"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Del Libertador Bernardo O Higgins"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Del Maule"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Del Bío Bío"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De La Araucanía"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Los Lagos"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Aisén Del General Carlos Ibáñez"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Magallanes Y De La Antártica"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Metropolitana De Santiago"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Los Ríos"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Arica y Parinacota"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Ignorada o sin especificar"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "SIN DATO"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Guatemala"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "El Progreso"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Sacatepequez"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Chimaltenango"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Escuintla"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Santa Rosa"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Solola"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Totonicapan"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Quetzaltenango"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Suchitepequez"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Retalhuleu"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "San Marcos"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Huehuetenango"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "El Quiche"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Baja Verapaz"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Alta Verapaz"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Peten"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Izabal"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Zacapa"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Chiquimula"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Jalapa"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Jutiapa"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Belice"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "PY").ID,  Name = "No aplica"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "US").ID,  Name = "Este"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Sur"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Este"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Centro"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Oeste"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Norte"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "ZZ").ID,  Name = "Metropolitana"} 
            };
            areas.ForEach(v => Areas.AddOrUpdate(p => new { p.Name, p.CountryID }, v));
            SaveChanges();

            var virusTypes = new List<VirusType>
            {
                 new VirusType { Name= "Influenza A (no subtipificado)" },
                 new VirusType { Name= "Influenza A(H1N1)pdm09" },
                 new VirusType { Name= "Influenza A(H3N2)" },
                 new VirusType { Name= "Influenza A no subtipificable" },
                 new VirusType { Name= "Influenza B" },
                 new VirusType { Name= "Parainfluenza" },
                 new VirusType { Name= "Parainfluenza 1" },
                 new VirusType { Name= "Parainfluenza 2" },
                 new VirusType { Name= "Parainfluenza 3" },
                 new VirusType { Name= "VSR" },
                 new VirusType { Name= "Adenovirus" },
                 new VirusType { Name= "Metapneumovirus" },
                 new VirusType { Name= "Rinovirus" },
                 new VirusType { Name= "Coronavirus" },
                 new VirusType { Name= "Bocavirus" }
            };
            virusTypes.ForEach(v => VirusTypes.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var testResults = new List<TestResult>
            { 
                new TestResult { ID ="U", Name = "(Ninguno)"},
                new TestResult { ID ="N", Name = "Negativo" },
                new TestResult { ID ="I", Name = "Indeterminado" },
                new TestResult { ID ="P", Name = "Positivo" }
            };
            testResults.ForEach(v => TestResults.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var admininstitutions = new List<AdminInstitution>
            {  new AdminInstitution { InstID =100000001, FullName ="Pan American Health Organization PAHO-WHO",  	Name="PAHO-WHO", CountryID = countries.Single(s => s.Code =="ZZ").ID, AreaID=Areas.Single(s => s.Name == "Metropolitana").ID, AccessLevel = AccessLevel.All }, 
               new AdminInstitution { InstID = 59100000000, FullName ="OPS - Bolivia",  	Name="OPS - Bolivia", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID,  AccessLevel = AccessLevel.Country }, 
               new AdminInstitution { InstID = 59100100001, FullName ="Ministerio de Salud y Deportes",  	Name="MSD", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID,  AccessLevel = AccessLevel.Country }, 
               new AdminInstitution { InstID = 59100200000,  FullName ="Servicio Departamental de Salud La Paz",  	Name="SEDES LA PAZ", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 59100100004,  FullName ="Servicio Departamental de Salud Tarija",  	Name="SEDES TAR", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Valles").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 59100100010,  FullName ="Servicio Departamental de Salud Santa Cruz ",  	Name="SEDES SCZ", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Llanos").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000003,  FullName ="SEREMI Tarapacá",  	Name="SEREMI Tarapacá", CountryID = countries.Single(s => s.Code =="CL").ID,  AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600003350,  FullName ="CAN Antofagasta",  	Name="CAN",        CountryID = countries.Single(s => s.Code =="CL").ID,  AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000000,  FullName ="Epidemiología del Ministerio de Salud de Chile",  	Name="EPI-MINSAL-CL",   CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago ").ID, AccessLevel = AccessLevel.Country},
               new AdminInstitution { InstID = 5600000001,  FullName ="SEREMI Metropolitana",  	Name="SEREMI Metropolitana",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000002,  FullName ="SEREMI Valparaíso",  	Name="SEREMI Valparaíso",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Valparaíso").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000005, FullName ="SEREMI Bio Bío",  	Name="SEREMI Bio Bío",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Del Bío Bío").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000006,  FullName ="SEREMI Los Lagos ",  	Name="SEREMI Los Lagos",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Los Lagos").ID, AccessLevel = AccessLevel.Area},
            };
            admininstitutions.ForEach(v => Institutions.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var hospitals = new List<Hospital>
            {
               new Hospital { InstID =59100100002, FullName ="Hospital San Juan de Dios Tarija",  	Name="HSJDD TAR",     CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Valles").ID,  AccessLevel = AccessLevel.SelfOnly }, 
               new Hospital { InstID =59100100006, FullName ="hospital San Juan de Dios",  	Name="HSJDD SCZ",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Llanos").ID, AccessLevel = AccessLevel.Area},
               new Hospital { InstID =59100100007,  FullName ="Caja Nacional del Seguro Obrero Nro 3",  	Name="CNS Obrero 3",        CountryID = countries.Single(s => s.Code =="BO").ID,  AreaID=Areas.Single(s => s.Name =="Llanos").ID, AccessLevel = AccessLevel.Area},
               new Hospital { InstID =59100100008,  FullName ="Hospital del Niño Santa Cruz",  	Name="HN SCZ",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Llanos").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200004,  FullName ="Hospital Arco Iris",  	Name="Hospital Arco Iris",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200078,  FullName ="Hospital del Niño \"Ovidio Aliaga\"",  	Name="Hospital del Niño",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200081,  FullName ="Instituto Nacional del Tórax",  	Name="Inst. Nac. Tórax",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200082,  FullName ="Hospital Materno Infantil CNS",  	Name="CNS",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200143,  FullName ="Hospital Boliviano Holandés",  	Name="Boliviano Holandés",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600002100,  FullName ="Hospital Dr. Ernesto Torres Galdames (Iquique)",  	Name="Hospital Iquique",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600007100,  FullName ="Hospital Dr. Gustavo Fricke",  	Name="Hospital Fricke",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Valparaíso").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600010100,  FullName ="Hospital San Juan de Dios",  	Name="Hospital San Juan",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600012530,  FullName ="Hospital Militar",  	Name="Hospital Militar",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600018100,  FullName ="Hospital Grant Benavente",  	Name="Hospital G.Benavente",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600024105,  FullName ="Hospital Puerto Montt",  	Name="Hospital P. Montt",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Valparaíso").ID, AccessLevel = AccessLevel.SelfOnly}
                                                                                                                                                                                                           
            };
            hospitals.ForEach(v => Institutions.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var labs = new List<Lab>
             {                                                                                                                                                                                                     
                    new Lab { InstID =59100100003,  FullName ="Laboratorio Departamental DE PRUEBA ", Name ="LDRT PRUEBA", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Valles").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =59100100005,  FullName ="Coordinación Departamental de Laboratorios", Name ="CODELAB", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Valles").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =59100100009,  FullName ="Centro de Enfermedades Tropicales", Name ="CENETROP", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Llanos").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =59100299999,  FullName ="Instituto Nacional de Laboratorios de Salud \"Néstor Morales Villazán\"", Name ="INLASA Virologí­a", CountryID = countries.Single(s => s.Code =="BO").ID,AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.Country },
                    new Lab { InstID =5600000004,  FullName ="Instituto de Salud Pública de Chile", Name ="ISP-CL", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago ").ID,  AccessLevel = AccessLevel.Country },
                    new Lab { InstID =5600002101,  FullName ="Laboratorio del Hospital Dr. Ernesto Torres Galdames (Iquique)", Name ="Lab. H. Iquique", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600007101,  FullName ="Laboratorio del Hospital Dr. Gustavo Fricke", Name ="Lab. H. Fricke", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600008100,  FullName ="Laboratorio PCR San Camilo", Name ="Lab. PCR San Camilo", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600010101,  FullName ="Laboratorio del Hospital S.Juan de Dios", Name ="Lab. H. San Juan", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600010102,  FullName ="Laboratorio PCR del Hospital S.Juan de Dios", Name ="Lab. PCR H. San Juan", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600012531,  FullName ="Laboratorio del Hospital Militar", Name ="Lab. H. Militar", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600018101,  FullName ="Laboratorio del Hospital Gran Benavente", Name ="Lab. H. G. Benavente", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Del Bío Bío").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600018102,  FullName ="Laboratorio PCR Concepción", Name ="Lab. PCR Concepción", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Del Bío Bío").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600024106,  FullName ="Laboratorio del Hospital Puerto Montt", Name ="Lab. H. P. Montt", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Los Lagos").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600024107,  FullName ="Laboratorio PCR Puerto Montt", Name ="Lab. PCR P. Montt", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Los Lagos").ID,  AccessLevel = AccessLevel.SelfOnly }
             };
            labs.ForEach(v => Institutions.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var states = new List<State>();
            AddState(states, "Tarija", "BO", "Valles");
            AddState(states, "La Paz - El Aeto", "BO", "Altiplano");
            AddState(states, "La Paz", "BO", "Altiplano");
            AddState(states, "Santa Cruz", "BO", "Llanos");
            AddState(states, "Iquique", "CL", "De Tarapacá");
            AddState(states, "Tamarugal", "CL", "De Tarapacá");
            AddState(states, "Antofagasta", "CL", "De Antofagasta");
            AddState(states, "El Loa", "CL", "De Antofagasta");
            AddState(states, "Tocopilla", "CL", "De Antofagasta");
            AddState(states, "Copiapó", "CL", "De Atacama");
            AddState(states, "Chañaral", "CL", "De Atacama");
            AddState(states, "Huasco", "CL", "De Atacama");
            AddState(states, "Elqui", "CL", "De Coquimbo");
            AddState(states, "Choapa", "CL", "De Coquimbo");
            AddState(states, "Limarí", "CL", "De Coquimbo");
            AddState(states, "Valparaíso", "CL", "De Valparaíso");
            AddState(states, "Isla de Pascua", "CL", "De Valparaíso");
            AddState(states, "Los Andes", "CL", "De Valparaíso");
            AddState(states, "Petorca", "CL", "De Valparaíso");
            AddState(states, "Quillota", "CL", "De Valparaíso");
            AddState(states, "San Antonio", "CL", "De Valparaíso");
            AddState(states, "San Felipe de Aconcagua", "CL", "De Valparaíso");
            AddState(states, "Marga Marga", "CL", "De Valparaíso");
            AddState(states, "Cachapoal", "CL", "Del Libertador Bernardo O Higgins");
            AddState(states, "Cardenal Caro", "CL", "Del Libertador Bernardo O Higgins");
            AddState(states, "Colchagua", "CL", "Del Libertador Bernardo O Higgins");
            AddState(states, "Talca", "CL", "Del Maule");
            AddState(states, "Cauquenes", "CL", "Del Maule");
            AddState(states, "Curicó", "CL", "Del Maule");
            AddState(states, "Linares", "CL", "Del Maule");
            AddState(states, "Concepción", "CL", "Del Bío Bío");
            AddState(states, "Arauco", "CL", "Del Bío Bío");
            AddState(states, "Biobío", "CL", "Del Bío Bío");
            AddState(states, "ñuble", "CL", "Del Bío Bío");
            AddState(states, "Cautín", "CL", "De La Araucanía");
            AddState(states, "Malleco", "CL", "De La Araucanía");
            AddState(states, "Llanquihue", "CL", "De Los Lagos");
            AddState(states, "Chiloé", "CL", "De Los Lagos");
            AddState(states, "Osorno", "CL", "De Los Lagos");
            AddState(states, "Palena", "CL", "De Los Lagos");
            AddState(states, "Coihaique", "CL", "De Aisén Del General Carlos Ibáñez");
            AddState(states, "Aisén", "CL", "De Aisén Del General Carlos Ibáñez");
            AddState(states, "Capitán Prat", "CL", "De Aisén Del General Carlos Ibáñez");
            AddState(states, "General Carrera", "CL", "De Aisén Del General Carlos Ibáñez");
            AddState(states, "Magallanes", "CL", "De Magallanes Y De La Antártica");
            AddState(states, "Antártica Chilena", "CL", "De Magallanes Y De La Antártica");
            AddState(states, "Tierra del Fuego", "CL", "De Magallanes Y De La Antártica");
            AddState(states, "ñltima Esperanza", "CL", "De Magallanes Y De La Antártica");
            AddState(states, "Santiago", "CL", "Metropolitana De Santiago");
            AddState(states, "Cordillera", "CL", "Metropolitana De Santiago");
            AddState(states, "Chacabuco", "CL", "Metropolitana De Santiago");
            AddState(states, "Maipo", "CL", "Metropolitana De Santiago");
            AddState(states, "Melipilla", "CL", "Metropolitana De Santiago");
            AddState(states, "Talagante", "CL", "Metropolitana De Santiago");
            AddState(states, "Valdivia", "CL", "De Los Ríos");
            AddState(states, "Ranco", "CL", "De Los Ríos");
            AddState(states, "Arica", "CL", "De Arica y Parinacota");
            AddState(states, "Parinacota", "CL", "De Arica y Parinacota");
            AddState(states, "Ignorada o sin especificar", "CL", "Ignorada o sinespecificar");
            AddState(states, "SIN DATO", "GT", "SIN DATO");
            AddState(states, "GUATEMALA", "GT", "Guatemala");
            AddState(states, "SANTA CATARINA PINULA", "GT", "Guatemala");
            AddState(states, "SAN JOSE PINULA", "GT", "Guatemala");
            AddState(states, "SAN JOSE DEL GOLFO", "GT", "Guatemala");
            AddState(states, "PALENCIA", "GT", "Guatemala");
            AddState(states, "CHINAUTLA", "GT", "Guatemala");
            AddState(states, "SAN PEDRO AYAMPUC", "GT", "Guatemala");
            AddState(states, "MIXCO", "GT", "Guatemala");
            AddState(states, "SAN PEDRO SACATEPEQUEZ", "GT", "Guatemala");
            AddState(states, "SAN JUAN SACATEPEQUEZ", "GT", "Guatemala");
            AddState(states, "SAN RAYMUNDO", "GT", "Guatemala");
            AddState(states, "CHUARRANCHO", "GT", "Guatemala");
            AddState(states, "FRAIJANES", "GT", "Guatemala");
            AddState(states, "AMATITLAN", "GT", "Guatemala");
            AddState(states, "VILLA NUEVA", "GT", "Guatemala");
            AddState(states, "VILLA CANALES", "GT", "Guatemala");
            AddState(states, "SAN MIGUEL PETAPA", "GT", "Guatemala");
            AddState(states, "GUASTATOYA", "GT", "El Progreso");
            AddState(states, "MORAZAN", "GT", "El Progreso");
            AddState(states, "SAN AGUSTIN ACASAGUASTLAN", "GT", "El Progreso");
            AddState(states, "SAN CRISTOBAL ACASAGUASTLAN", "GT", "El Progreso");
            AddState(states, "EL JICARO", "GT", "El Progreso");
            AddState(states, "SANSARE", "GT", "El Progreso");
            AddState(states, "SANARATE", "GT", "El Progreso");
            AddState(states, "SAN ANTONIO LA PAZ", "GT", "El Progreso");
            AddState(states, "ANTIGUA GUATEMALA", "GT", "Sacatepequez");
            AddState(states, "JOCOTENANGO", "GT", "Sacatepequez");
            AddState(states, "PASTORES", "GT", "Sacatepequez");
            AddState(states, "SUMPANGO", "GT", "Sacatepequez");
            AddState(states, "SANTO DOMINGO XENACOJ", "GT", "Sacatepequez");
            AddState(states, "SANTIAGO SACATEPEQUEZ", "GT", "Sacatepequez");
            AddState(states, "SAN BARTOLOME MILPAS ALTAS", "GT", "Sacatepequez");
            AddState(states, "SAN LUCAS SACATEPEQUEZ", "GT", "Sacatepequez");
            AddState(states, "SANTA LUCIA MILPAS ALTAS", "GT", "Sacatepequez");
            AddState(states, "MAGDALENA MILPAS ALTAS", "GT", "Sacatepequez");
            AddState(states, "SANTA MARIA DE JESUS", "GT", "Sacatepequez");
            AddState(states, "CIUDAD VIEJA", "GT", "Sacatepequez");
            AddState(states, "SAN MIGUEL DUEñAS", "GT", "Sacatepequez");
            AddState(states, "SAN JUAN ALOTENANGO", "GT", "Sacatepequez");
            AddState(states, "SAN ANTONIO AGUAS CALIENTES", "GT", "Sacatepequez");
            AddState(states, "SANTA CATARINA BARAHONA", "GT", "Sacatepequez");
            AddState(states, "CHIMALTENANGO", "GT", "Chimaltenango");
            AddState(states, "SAN JOSE POAQUIL", "GT", "Chimaltenango");
            AddState(states, "SAN MARTIN JILOTEPEQUE", "GT", "Chimaltenango");
            AddState(states, "SAN JUAN COMALAPA", "GT", "Chimaltenango");
            AddState(states, "SANTA APOLONIA", "GT", "Chimaltenango");
            AddState(states, "TECPAN GUATEMALA", "GT", "Chimaltenango");
            AddState(states, "PATZUN", "GT", "Chimaltenango");
            AddState(states, "SAN MIGUEL POCHUTA", "GT", "Chimaltenango");
            AddState(states, "PATZICIA", "GT", "Chimaltenango");
            AddState(states, "SANTA CRUZ BALANYA", "GT", "Chimaltenango");
            AddState(states, "ACATENANGO", "GT", "Chimaltenango");
            AddState(states, "SAN PEDRO YEPOCAPA", "GT", "Chimaltenango");
            AddState(states, "SAN ANDRES ITZAPA", "GT", "Chimaltenango");
            AddState(states, "PARRAMOS", "GT", "Chimaltenango");
            AddState(states, "ZARAGOZA", "GT", "Chimaltenango");
            AddState(states, "EL TEJAR", "GT", "Chimaltenango");
            AddState(states, "ESCUINTLA", "GT", "Escuintla");
            AddState(states, "SANTA LUCIA COTZUMALGUAPA", "GT", "Escuintla");
            AddState(states, "LA DEMOCRACIA", "GT", "Escuintla");
            AddState(states, "SIQUINALA", "GT", "Escuintla");
            AddState(states, "MASAGUA", "GT", "Escuintla");
            AddState(states, "TIQUISATE", "GT", "Escuintla");
            AddState(states, "LA GOMERA", "GT", "Escuintla");
            AddState(states, "GUANAGAZAPA", "GT", "Escuintla");
            AddState(states, "SAN JOSE", "GT", "Escuintla");
            AddState(states, "IZTAPA", "GT", "Escuintla");
            AddState(states, "PALIN", "GT", "Escuintla");
            AddState(states, "SAN VICENTE PACAYA", "GT", "Escuintla");
            AddState(states, "NUEVA CONCEPCION", "GT", "Escuintla");
            AddState(states, "CUILAPA", "GT", "Santa Rosa");
            AddState(states, "BARBERENA", "GT", "Santa Rosa");
            AddState(states, "SANTA ROSA DE LIMA", "GT", "Santa Rosa");
            AddState(states, "CASILLAS", "GT", "Santa Rosa");
            AddState(states, "SAN RAFAEL LAS FLORES", "GT", "Santa Rosa");
            AddState(states, "ORATORIO", "GT", "Santa Rosa");
            AddState(states, "SAN JUAN TECUACO", "GT", "Santa Rosa");
            AddState(states, "CHIQUIMULILLA", "GT", "Santa Rosa");
            AddState(states, "TAXISCO", "GT", "Santa Rosa");
            AddState(states, "SANTA MARIA IXHUATAN", "GT", "Santa Rosa");
            AddState(states, "GUAZACAPAN", "GT", "Santa Rosa");
            AddState(states, "SANTA CRUZ NARANJO", "GT", "Santa Rosa");
            AddState(states, "PUEBLO NUEVO VIñAS", "GT", "Santa Rosa");
            AddState(states, "NUEVA SANTA ROSA", "GT", "Santa Rosa");
            AddState(states, "SOLOLA", "GT", "Solola");
            AddState(states, "SAN JOSE CHACAYA", "GT", "Solola");
            AddState(states, "SANTA MARIA VISITACION", "GT", "Solola");
            AddState(states, "SANTA LUCIA UTATLAN", "GT", "Solola");
            AddState(states, "NAHUALA", "GT", "Solola");
            AddState(states, "SANTA CATARINA IXTAHUACAN", "GT", "Solola");
            AddState(states, "SANTA CLARA LA LAGUNA", "GT", "Solola");
            AddState(states, "CONCEPCION", "GT", "Solola");
            AddState(states, "SAN ANDRES SEMETABAJ", "GT", "Solola");
            AddState(states, "PANAJACHEL", "GT", "Solola");
            AddState(states, "SANTA CATARINA PALOPO", "GT", "Solola");
            AddState(states, "SAN ANTONIO PALOPO", "GT", "Solola");
            AddState(states, "SAN LUCAS TOLIMAN", "GT", "Solola");
            AddState(states, "SANTA CRUZ LA LAGUNA", "GT", "Solola");
            AddState(states, "SAN PABLO LA LAGUNA", "GT", "Solola");
            AddState(states, "SAN MARCOS LA LAGUNA", "GT", "Solola");
            AddState(states, "SAN JUAN LA LAGUNA", "GT", "Solola");
            AddState(states, "SAN PEDRO LA LAGUNA", "GT", "Solola");
            AddState(states, "SANTIAGO ATITLAN", "GT", "Solola");
            AddState(states, "TOTONICAPAN", "GT", "Totonicapan");
            AddState(states, "SAN CRISTOBAL TOTONICAPAN", "GT", "Totonicapan");
            AddState(states, "SAN FRANCISCO EL ALTO", "GT", "Totonicapan");
            AddState(states, "SAN ANDRES XECUL", "GT", "Totonicapan");
            AddState(states, "MOMOSTENANGO", "GT", "Totonicapan");
            AddState(states, "SANTA MARIA CHIQUIMULA", "GT", "Totonicapan");
            AddState(states, "SANTA LUCIA LA REFORMA", "GT", "Totonicapan");
            AddState(states, "SAN BARTOLO AGUAS CALIENTES", "GT", "Totonicapan");
            AddState(states, "QUETZALTENANGO", "GT", "Quetzaltenango");
            AddState(states, "SALCAJA", "GT", "Quetzaltenango");
            AddState(states, "OLINTEPEQUE", "GT", "Quetzaltenango");
            AddState(states, "SAN CARLOS SIJA", "GT", "Quetzaltenango");
            AddState(states, "SIBILIA", "GT", "Quetzaltenango");
            AddState(states, "CABRICAN", "GT", "Quetzaltenango");
            AddState(states, "CAJOLA", "GT", "Quetzaltenango");
            AddState(states, "SAN MIGUEL SIGUILA", "GT", "Quetzaltenango");
            AddState(states, "SAN JUAN OSTUNCALCO", "GT", "Quetzaltenango");
            AddState(states, "SAN MATEO", "GT", "Quetzaltenango");
            AddState(states, "CONCEPCION CHIQUIRICHAPA", "GT", "Quetzaltenango");
            AddState(states, "SAN MARTIN SACATEPEQUEZ", "GT", "Quetzaltenango");
            AddState(states, "ALMOLONGA", "GT", "Quetzaltenango");
            AddState(states, "CANTEL", "GT", "Quetzaltenango");
            AddState(states, "HUITAN", "GT", "Quetzaltenango");
            AddState(states, "ZUNIL", "GT", "Quetzaltenango");
            AddState(states, "COLOMBA", "GT", "Quetzaltenango");
            AddState(states, "SAN FRANCISCO LA UNION", "GT", "Quetzaltenango");
            AddState(states, "EL PALMAR", "GT", "Quetzaltenango");
            AddState(states, "COATEPEQUE", "GT", "Quetzaltenango");
            AddState(states, "GENOVA", "GT", "Quetzaltenango");
            AddState(states, "FLORES COSTA CUCA", "GT", "Quetzaltenango");
            AddState(states, "LA ESPERANZA", "GT", "Quetzaltenango");
            AddState(states, "PALESTINA DE LOS ALTOS", "GT", "Quetzaltenango");
            AddState(states, "MAZATENANGO", "GT", "Suchitepequez");
            AddState(states, "CUYOTENANGO", "GT", "Suchitepequez");
            AddState(states, "SAN FRANCISCO ZAPOTITLAN", "GT", "Suchitepequez");
            AddState(states, "SAN BERNARDINO", "GT", "Suchitepequez");
            AddState(states, "SAN JOSE EL IDOLO", "GT", "Suchitepequez");
            AddState(states, "SANTO DOMINGO SUCHITEPEQUEZ", "GT", "Suchitepequez");
            AddState(states, "SAN LORENZO", "GT", "Suchitepequez");
            AddState(states, "SAMAYAC", "GT", "Suchitepequez");
            AddState(states, "SAN PABLO JOCOPILAS", "GT", "Suchitepequez");
            AddState(states, "SAN ANTONIO SUCHITEPEQUEZ", "GT", "Suchitepequez");
            AddState(states, "SAN MIGUEL PANAN", "GT", "Suchitepequez");
            AddState(states, "SAN GABRIEL", "GT", "Suchitepequez");
            AddState(states, "CHICACAO", "GT", "Suchitepequez");
            AddState(states, "PATULUL", "GT", "Suchitepequez");
            AddState(states, "SANTA BARBARA", "GT", "Suchitepequez");
            AddState(states, "SAN JUAN BAUTISTA", "GT", "Suchitepequez");
            AddState(states, "SANTO TOMAS LA UNION", "GT", "Suchitepequez");
            AddState(states, "ZUNILITO", "GT", "Suchitepequez");
            AddState(states, "PUEBLO NUEVO SUCHITEPEQUEZ", "GT", "Suchitepequez");
            AddState(states, "RIO BRAVO", "GT", "Suchitepequez");
            AddState(states, "RETALHULEU", "GT", "Retalhuleu");
            AddState(states, "SAN SEBASTIAN", "GT", "Retalhuleu");
            AddState(states, "SANTA CRUZ MULUA", "GT", "Retalhuleu");
            AddState(states, "SAN MARTIN ZAPOTITLAN", "GT", "Retalhuleu");

            states.ForEach(v => States.AddOrUpdate(p => new { p.Name, p.AreaID }, v));
            SaveChanges();

            var locals = new List<Local>();
            AddLocal(locals, 1, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 2, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20101, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20102, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20103, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20104, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21803, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21901, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21902, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 22001, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21602, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21701, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21702, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21703, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21801, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21802, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21307, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21401, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21402, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21501, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21502, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21601, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21301, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21302, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21303, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21304, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21305, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21306, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21104, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21105, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21201, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21202, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21203, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21204, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21004, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21005, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21006, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21101, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21102, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21103, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20903, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20904, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20905, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21001, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21002, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21003, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20804, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20805, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20806, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20807, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20901, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20902, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20608, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20701, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20702, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20801, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20802, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20803, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20602, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20603, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20604, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20605, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20606, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20607, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20402, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20403, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20501, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20502, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20503, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20601, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20304, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20305, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20306, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20307, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20308, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20401, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20105, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20201, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20202, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20301, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20302, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20303, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 60101, "Tarija", "BO", "Valles");
            AddLocal(locals, 60201, "Tarija", "BO", "Valles");
            AddLocal(locals, 60202, "Tarija", "BO", "Valles");
            AddLocal(locals, 60301, "Tarija", "BO", "Valles");
            AddLocal(locals, 60302, "Tarija", "BO", "Valles");
            AddLocal(locals, 60303, "Tarija", "BO", "Valles");
            AddLocal(locals, 60401, "Tarija", "BO", "Valles");
            AddLocal(locals, 60402, "Tarija", "BO", "Valles");
            AddLocal(locals, 60501, "Tarija", "BO", "Valles");
            AddLocal(locals, 60502, "Tarija", "BO", "Valles");
            AddLocal(locals, 60601, "Tarija", "BO", "Valles");
            AddLocal(locals, 70101, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70102, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70103, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70104, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70105, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70201, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71502, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71503, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71301, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71302, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71401, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71402, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71403, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71501, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71102, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71103, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71104, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71105, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71106, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71201, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71001, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71002, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71003, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71004, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71005, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71101, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70804, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70805, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70901, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70902, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70903, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70904, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70705, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70706, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70707, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70801, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70802, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70803, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70602, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70603, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70701, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70702, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70703, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70704, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70403, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70404, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70501, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70502, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70503, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70601, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70202, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70301, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70302, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70303, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70401, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70402, "Santa Cruz", "BO", "Llanos");

            locals.ForEach(v => Locals.AddOrUpdate(p => new { p.Code, p.StateID }, v));
            SaveChanges();

            var neighborhoods = new List<Neighborhood>();
            AddNeighborhood(neighborhoods, 1, "La Paz", "BO", "Altiplano");
            AddNeighborhood(neighborhoods, 2, "La Paz", "BO", "Altiplano");

            neighborhoods.ForEach(v => Neighborhoods.AddOrUpdate(p => new { p.Code, p.StateID }, v));
            SaveChanges();


            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this));

            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists("Admin"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Admin"));
            }

            //Create Role Tester if it does not exist
            if (!RoleManager.RoleExists("Tester"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Tester"));
            }

            //Create Role Reader if it does not exist
            if (!RoleManager.RoleExists("Reader"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Reader"));
            }

            //Create Role Reader if it does not exist
            if (!RoleManager.RoleExists("Staff"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Staff"));
            }

            //Create Role Reader if it does not exist
            if (!RoleManager.RoleExists("Report"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Report"));
            }

            Object[,] users = {
                                {"Admin","Paho201$","xionglijun@hotmail.com",  GetInstitutionID( "Pan American Health Organization PAHO-WHO"), "Lijun", "", "Xiong", "", "xionglijun@hotmail.com" },   
                                {"Staff","Paho201$","jCoro",  GetInstitutionID("OPS - Bolivia"),"Jorge","","Coro","",""  },
                                {"Staff","Paho201$","rSalamanca",  GetInstitutionID("OPS - Bolivia"),"Roxana","","Salamanca","",""  },
                                {"Staff","Paho201$","dPastor",  GetInstitutionID("OPS - Bolivia"),"Desiree","","Pastor","",""  },
                                {"Staff","Paho201$","aanez",  GetInstitutionID("OPS - Bolivia"),"Arletta","","Añez","","aanez@paho.org"  },
                                {"Staff","Paho201$","lSoraide",  GetInstitutionID("Ministerio de Salud y Deportes"),"Lizeth","","Soraide","Iriarte",""  },
                                {"Staff","Paho201$","aZarate",  GetInstitutionID("Ministerio de Salud y Deportes"),"Adolfo","","Zárate","",""  },
                                {"Staff","Paho201$","mMorales",  GetInstitutionID("Ministerio de Salud y Deportes"),"Martin","","Morales","",""  },
                                {"Staff","Paho201$","vTorrez",  GetInstitutionID("Hospital San Juan de Dios Tarija"),"Vivien","Silvana","Torrez","Choque",""  },
                                {"Staff","Paho201$","nAguado",  GetInstitutionID("Laboratorio Departamental de ReferenciaTarija"),"Nelly","","Aguado","Aparicio",""  },
                                {"Staff","Paho201$","wSantaCruz",  GetInstitutionID("Servicio Departamental de Salud Tarija"),"Walter","Horacio","Santa Cruz","",""  },
                                {"Staff","Paho201$","mRengel",  GetInstitutionID("Coordinación Departamental de Laboratorios"),"Maria","Dolores","Rengel","Estrada",""  },
                                {"Staff","Paho201$","rGross",  this.Institutions.Single(s => s.FullName =="hospital San Juan de Dios" && s.Name=="HSJDD SCZ").ID,"Rosmery","","Gross","Arteaga",""  },
                                {"Staff","Paho201$","fDominguez",  GetInstitutionID("Caja Nacional del Seguro Obrero Nro 3"),"Flora","","Dominguez","Quispe",""  },
                                {"Staff","Paho201$","eChavez",  GetInstitutionID("Caja Nacional del Seguro Obrero Nro 3"),"Ena","","Chavez","",""  },
                                {"Staff","Paho201$","eCabrera",  GetInstitutionID("Hospital del Niño Santa Cruz"),"Erika","","Cabrera","Albis",""  },
                                {"Staff","Paho201$","yRoca",  GetInstitutionID("Centro de Enfermedades Tropicales"),"Yelin","","Roca","Sanchez",""  },
                                {"Staff","Paho201$","sParedes",  GetInstitutionID("Servicio Departamental de Salud Santa Cruz"),"Susana","","Paredes","Montero",""  },
                                {"Staff","Paho201$","pruebas",  GetInstitutionID("Servicio Departamental de Salud La Paz"),"Pablo","","Bulba","",""  },
                                {"Staff","Paho201$","kkenta",  GetInstitutionID("Servicio Departamental de Salud La Paz"),"Karem","","Kenta","Vasquez",""  },
                                {"Staff","Paho201$","mfernandez",  GetInstitutionID("Hospital Arco Iris"),"Marcel","","Fernandez","Peralta",""  },
                                {"Staff","Paho201$","amendoza",  GetInstitutionID("Hospital del Niño \"Ovidio Aliaga\""),"Alfredo","","Mendoza","",""  },
                                {"Staff","Paho201$","jgutierrez",  GetInstitutionID("Instituto Nacional del Tórax"),"Juan","","Gutierrez","Quispe",""  },
                                {"Staff","Paho201$","msaavedra",  GetInstitutionID("Hospital Materno Infantil CNS"),"Margarita","","Saavedra","Cortez",""  },
                                {"Staff","Paho201$","aveizaga",  GetInstitutionID("Hospital Boliviano Holandés"),"Angel","","Veizaga","Quispe",""  },
                                {"Staff","Paho201$","pablo",  GetInstitutionID("Hospital Boliviano Holandés"),"Pablo","","Bulba","",""  },
                                {"Staff","Paho201$","acruz",  GetInstitutionID("Instituto Nacional de Laboratorios de Salud \"Néstor Morales Villazán\""),"Aleida","Nina","Cruz","",""  },
                                {"Staff","Paho201$","aCruzA",  GetInstitutionID("Instituto Nacional de Laboratorios de Salud \"Néstor Morales Villazán\""),"Angélica","Andrea","Cruz","Concha","angelica.cruz@redsalud.gov.cl"  },
                                {"Staff","Paho201$","jsantalla",  GetInstitutionID("Instituto Nacional de Laboratorios de Salud \"Néstor Morales Villazán\""),"Jose","Antonio","Santalla","Vargas","josesantalla@gmail.com"  },
                                {"Staff","Paho201$","bgalleguillos",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Bárbara","","Galleguillos","",""  },
                                {"Staff","Paho201$","nVergara",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Natalia","","Vergara","Mallegas",""  },
                                {"Staff","Paho201$","vSotomayor",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Viviana","","Sotomayor","",""  },
                                {"Staff","Paho201$","fSoto",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Franco","","Soto","",""  },
                                {"Staff","Paho201$","aEspinosa",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Angélica","","Espinosa","",""  },
                                {"Staff","Paho201$","mRojas",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Marcelo","","Rojas","",""  },
                                {"Staff","Paho201$","pruCL",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Prueba","","Chile","",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("SEREMI Metropolitana"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","mArenas",  GetInstitutionID("SEREMI Valparaíso"),"Myriam","","Arenas","",""  },
                                {"Staff","Paho201$","acruz",  GetInstitutionID("SEREMI Tarapacá"),"Aleida","Nina","Cruz","",""  },
                                {"Staff","Paho201$","aCruzA",  GetInstitutionID("SEREMI Tarapacá"),"Angélica","Andrea","Cruz","Concha","angelica.cruz@redsalud.gov.cl"  },
                                {"Staff","Paho201$","vDiaz",  GetInstitutionID("SEREMI Tarapacá"),"Verónica","","Díaz","",""  },
                                {"Staff","Paho201$","rFasce",  GetInstitutionID("Instituto de Salud Pública de Chile"),"Rodrigo","","Fasce","",""  },
                                {"Staff","Paho201$","aGutierrez",  GetInstitutionID("SEREMI Bio Bío"),"Andrea","","Gutierrez","",""  },
                                {"Staff","Paho201$","jUlloa",  GetInstitutionID("SEREMI Los Lagos"),"Juana","","Ulloa","",""  },
                                {"Staff","Paho201$","acruz",  GetInstitutionID("Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Aleida","Nina","Cruz","",""  },
                                {"Staff","Paho201$","aCruzA",  GetInstitutionID("Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Angélica","Andrea","Cruz","Concha","angelica.cruz@redsalud.gov.cl"  },
                                {"Staff","Paho201$","vDiaz",  GetInstitutionID("Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Verónica","","Díaz","",""  },
                                {"Staff","Paho201$","oLopez",  GetInstitutionID("Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Olga","","López","",""  },
                                {"Staff","Paho201$","efernandez",  GetInstitutionID("Laboratorio del Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Eduardo","","Fernández","",""  },
                                {"Staff","Paho201$","dOrtiz",  GetInstitutionID("Laboratorio del Hospital Dr. Ernesto Torres Galdames (Iquique)"),"David","","Ortíz","",""  },
                                {"Staff","Paho201$","eBejarano",  GetInstitutionID("Laboratorio del Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Enrique","","Bejarano","",""  },
                                {"Staff","Paho201$","eBejarano",  GetInstitutionID("CAN Antofagasta"),"Enrique","","Bejarano","",""  },
                                {"Staff","Paho201$","mBlanco",  GetInstitutionID("Hospital Dr. Gustavo Fricke"),"Myriam","","Blanco","",""  },
                                {"Staff","Paho201$","mArenas",  GetInstitutionID("Hospital Dr. Gustavo Fricke"),"Myriam","","Arenas","",""  },
                                {"Staff","Paho201$","bOsandon",  GetInstitutionID("Laboratorio del Hospital Dr. Gustavo Fricke"),"Berta","","Osandón","",""  },
                                {"Staff","Paho201$","pMursell",  GetInstitutionID("Laboratorio del Hospital Dr. Gustavo Fricke"),"Pablo","","Mursell","",""  },
                                {"Staff","Paho201$","pMursell",  GetInstitutionID("Laboratorio PCR San Camilo"),"Pablo","","Mursell","",""  },
                                {"Staff","Paho201$","tTarride",  this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios" && s.Name=="Hospital San Juan").ID,"Tamara","","Tarride","Muñoz",""  },
                                {"Staff","Paho201$","jVelasquez", this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios"&& s.Name=="Hospital San Juan").ID,"José","Luis","Velásquez","Mellado","joluveme@hotmail.com"  },
                                {"Staff","Paho201$","aCespedes",  this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios" && s.Name=="Hospital San Juan").ID,"Alejandra","","Céspedes","",""  },
                                {"Staff","Paho201$","lChanqueo",  this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios" && s.Name=="Hospital San Juan").ID,"Leonardo","","Chanqueo","",""  },
                                {"Staff","Paho201$","ePalta",   this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios" && s.Name=="Hospital San Juan").ID,"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","aCespedes",  GetInstitutionID("Laboratorio del Hospital S.Juan de Dios"),"Alejandra","","Céspedes","",""  },
                                {"Staff","Paho201$","lChanqueo",  GetInstitutionID("Laboratorio del Hospital S.Juan de Dios"),"Leonardo","","Chanqueo","",""  },
                                {"Staff","Paho201$","aRamirez",  GetInstitutionID("Laboratorio del Hospital S.Juan de Dios"),"Alejandro","","Ramí­rez","Acharan",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("Laboratorio del Hospital S.Juan de Dios"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","aRamirez",  GetInstitutionID("Laboratorio PCR del Hospital S.Juan de Dios"),"Alejandro","","Ramí­rez","Acharan",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("Laboratorio PCR del Hospital S.Juan de Dios"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","pSpalloni",  GetInstitutionID("Hospital Militar"),"Pía","","Spalloni","",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("Hospital Militar"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","aFicka",  GetInstitutionID("Hospital Militar"),"Alberto","","Ficka","",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("Laboratorio del Hospital Militar"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","gMarambio",  GetInstitutionID("Laboratorio del Hospital Militar"),"Gloria","","Marambio","",""  },
                                {"Staff","Paho201$","cAguayo",  GetInstitutionID("Hospital Grant Benavente"),"Claudia","","Aguayo","",""  },
                                {"Staff","Paho201$","cOrtega",  GetInstitutionID("Hospital Grant Benavente"),"Carlos","","Ortega","",""  },
                                {"Staff","Paho201$","aGutierrez",  GetInstitutionID("Hospital Grant Benavente"),"Andrea","","Gutierrez","",""  },
                                {"Staff","Paho201$","lTwele",  GetInstitutionID("Hospital Grant Benavente"),"Loreto","","Twele","",""  },
                                {"Staff","Paho201$","mOpazo",  GetInstitutionID("Laboratorio del Hospital Gran Benavente"),"Marina","","Opazo","",""  },
                                {"Staff","Paho201$","iRodriguez",  GetInstitutionID("Laboratorio del Hospital Gran Benavente"),"Iván","","Rodriguez","",""  },
                                {"Staff","Paho201$","iRodriguez",  GetInstitutionID("Laboratorio PCR Concepción"),"Iván","","Rodriguez","",""  },
                                {"Staff","Paho201$","jUlloa",  GetInstitutionID("Hospital Puerto Montt"),"Juana","","Ulloa","",""  },
                                {"Staff","Paho201$","dErnts",  GetInstitutionID("Hospital Puerto Montt"),"Diana","","Ernts","",""  },
                                {"Staff","Paho201$","pLanino",  GetInstitutionID("Hospital Puerto Montt"),"Paola","","Lanino","",""  },
                                {"Staff","Paho201$","lAcevedo",  GetInstitutionID("Laboratorio del Hospital Puerto Montt"),"Loreto","","Acevedo","",""  },
                                {"Staff","Paho201$","mlRioseco",  GetInstitutionID("Laboratorio del Hospital Puerto Montt"),"María Luisa","","Rioseco","",""  },
                                {"Staff","Paho201$","lAcevedo",  GetInstitutionID("Laboratorio PCR Puerto Montt"),"Loreto","","Acevedo","",""  },                              
                                {"Admin","Paho201$","ooliva",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Otavio","","Oliva","",""  },
                                {"Admin","Paho201$","ceisner",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Charles","","Eisner","",""  },
                                {"Admin","Paho201$","mcerpa",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Mauricio","","Cerpa","",""  },
                                {"Admin","Paho201$","rpalekar",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Rakhee","","Palekar","",""  },
                                {"Admin","Paho201$","epedroni",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Elena","","Pedroni","",""  },
                                {"Admin","Paho201$","obilbao",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Oona","","Bilbao","",""  },
                                {"Admin","Paho201$","lCatala",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Laura","","Catala","Pascual",""  },
                                {"Admin","Paho201$","mtdacosta",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Maria","Tereza","da Costa","Oliveira",""  },
                                {"Admin","Paho201$","voviedo",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Vladimir","","Oviedo","",""  },
                                {"Admin","Paho201$","pablo",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Pablo","","Bulba","",""  }
                              };

            for (int i = 0; i < users.GetLength(0); i++)
            {
                CreateUser(UserManager, new ApplicationUser()
                {
                    UserName = users[i, 2] as string,
                    InstitutionID = (long)users[i, 3],
                    FirstName1 = users[i, 4] as string,
                    FirstName2 = users[i, 5] as string,
                    LastName1 = users[i, 6] as string,
                    LastName2 = users[i, 7] as string,
                    Email = users[i, 8] as string,
                    EmailConfirmed = true
                }, users[i, 1] as string, users[i, 0] as string);
            }

        }

        public void LoadData()
        {
            var countries = new List<Country>
            {  
                 new Country {Active = false, Code ="AR",	Name = "Argentina"},                            
                 new Country {Active = false, Code ="BB",	Name = "Barbados"},                   
                 new Country {Active = true,  Code ="BO",	Name = "Bolivia"},                   
                 new Country {Active = false, Code ="BR",	Name = "Brasil"},                   
                 new Country {Active = false, Code ="BZ",	Name = "Belice"},                   
                 new Country {Active = false, Code ="CA",	Name = "Canadá"},                   
                 new Country {Active = true,  Code ="CL",	Name = "Chile"},                   
                 new Country {Active = false, Code ="CO",	Name = "Colombia"},                   
                 new Country {Active = false, Code ="CR",	Name = "Costa Rica"},                   
                 new Country {Active = false, Code ="CU",	Name = "Cuba"},                   
                 new Country {Active = false, Code ="DM",	Name = "Dominica"},                   
                 new Country {Active = false, Code ="DO",	Name = "Rep. Dominicana"},                   
                 new Country {Active = false, Code ="EC",	Name = "Ecuador"},                   
                 new Country {Active = false, Code ="GT",	Name = "Guatemala"},                   
                 new Country {Active = false, Code ="HN",	Name = "Honduras"},                   
                 new Country {Active = false, Code ="HT",	Name = "Haití"},                   
                 new Country {Active = false, Code ="JM",	Name = "Jamaica"},                   
                 new Country {Active = false, Code ="LC",	Name = "Santa Lucía"},                   
                 new Country {Active = false, Code ="MX",	Name = "México"},                   
                 new Country {Active = false, Code ="NI",	Name = "Nicaragua"},                   
                 new Country {Active = false, Code ="PA",	Name = "Panamá"},                   
                 new Country {Active = false, Code ="PE",	Name = "Perú"},                   
                 new Country {Active = false, Code ="PR",	Name = "Puerto Rico"},                   
                 new Country {Active = false, Code ="PY",	Name = "Paraguay"},                   
                 new Country {Active = false, Code ="SR",	Name = "Surinam"},                   
                 new Country {Active = false, Code ="SV",	Name = "El Salvador"},                   
                 new Country {Active = false, Code ="TT",	Name = "Trinidad y Tobago"},                   
                 new Country {Active = false, Code ="US",	Name = "Estados Unidos"},                   
                 new Country {Active = false, Code ="UY",	Name = "Uruguay"},                   
                 new Country {Active = false, Code ="VC",	Name = "S.Vicente y Granad."},                   
                 new Country {Active = false, Code ="VE",	Name = "Venezuela"},                   
                 new Country {Active = false, Code ="ZZ",	Name = "ZZZ Naciones Unidas"}
            };
            countries.ForEach(v => Countries.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var areas = new List<Area>
            {               
                    new Area {   CountryID = countries.Single(s => s.Code == "BO").ID,  Name = "Altiplano"},                                                                  
                    new Area {   CountryID = countries.Single(s => s.Code == "BO").ID,  Name = "Valles"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "BO").ID,  Name = "Llanos"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Tarapacá"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Antofagasta"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "CL	4	De Coquimbo"},                                            
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Valparaíso"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Del Libertador Bernardo O Higgins"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Del Maule"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Del Bío Bío"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De La Araucanía"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Los Lagos"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Aisén Del General Carlos Ibáñez"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Magallanes Y De La Antártica"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Metropolitana De Santiago"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Los Ríos"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "De Arica y Parinacota"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "CL").ID,  Name = "Ignorada o sin especificar"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "SIN DATO"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Guatemala"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "El Progreso"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Sacatepequez"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Chimaltenango"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Escuintla"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Santa Rosa"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Solola"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Totonicapan"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Quetzaltenango"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Suchitepequez"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Retalhuleu"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "San Marcos"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Huehuetenango"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "El Quiche"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Baja Verapaz"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Alta Verapaz"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Peten"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Izabal"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Zacapa"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Chiquimula"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Jalapa"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Jutiapa"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "GT").ID,  Name = "Belice"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "PY").ID,  Name = "No aplica"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "US").ID,  Name = "Este"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Sur"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Este"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Centro"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Oeste"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "UY").ID,  Name = "Norte"},                                    
                    new Area {   CountryID = countries.Single(s => s.Code == "ZZ").ID,  Name = "Metropolitana"} 
            };
            areas.ForEach(v => Areas.AddOrUpdate(p => new { p.Name, p.CountryID }, v));
            SaveChanges();

            var virusTypes = new List<VirusType>
            {
                 new VirusType { Name= "Influenza A (no subtipificado)" },
                 new VirusType { Name= "Influenza A(H1N1)pdm09" },
                 new VirusType { Name= "Influenza A(H3N2)" },
                 new VirusType { Name= "Influenza A no subtipificable" },
                 new VirusType { Name= "Influenza B" },
                 new VirusType { Name= "Parainfluenza" },
                 new VirusType { Name= "Parainfluenza 1" },
                 new VirusType { Name= "Parainfluenza 2" },
                 new VirusType { Name= "Parainfluenza 3" },
                 new VirusType { Name= "VSR" },
                 new VirusType { Name= "Adenovirus" },
                 new VirusType { Name= "Metapneumovirus" },
                 new VirusType { Name= "Rinovirus" },
                 new VirusType { Name= "Coronavirus" },
                 new VirusType { Name= "Bocavirus" }
            };
            virusTypes.ForEach(v => VirusTypes.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var testResults = new List<TestResult>
            { 
                new TestResult { ID ="U", Name = "(Ninguno)"},
                new TestResult { ID ="N", Name = "Negativo" },
                new TestResult { ID ="I", Name = "Indeterminado" },
                new TestResult { ID ="P", Name = "Positivo" }
            };
            testResults.ForEach(v => TestResults.AddOrUpdate(p => p.Name, v));
            SaveChanges();


            var admininstitutions = new List<AdminInstitution>
            {  new AdminInstitution { InstID =100000001, FullName ="Pan American Health Organization PAHO-WHO",  	Name="PAHO-WHO", CountryID = countries.Single(s => s.Code =="ZZ").ID, AreaID=Areas.Single(s => s.Name == "Metropolitana").ID, AccessLevel = AccessLevel.All }, 
               new AdminInstitution { InstID = 59100000000, FullName ="OPS - Bolivia",  	Name="OPS - Bolivia", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID,  AccessLevel = AccessLevel.Country }, 
               new AdminInstitution { InstID = 59100100001, FullName ="Ministerio de Salud y Deportes",  	Name="MSD", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID,  AccessLevel = AccessLevel.Country }, 
               new AdminInstitution { InstID = 59100200000,  FullName ="Servicio Departamental de Salud La Paz",  	Name="SEDES LA PAZ", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 59100100004,  FullName ="Servicio Departamental de Salud Tarija",  	Name="SEDES TAR", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Valles").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 59100100010,  FullName ="Servicio Departamental de Salud Santa Cruz ",  	Name="SEDES SCZ", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Llanos").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000003,  FullName ="SEREMI Tarapacá",  	Name="SEREMI Tarapacá", CountryID = countries.Single(s => s.Code =="CL").ID,  AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600003350,  FullName ="CAN Antofagasta",  	Name="CAN",        CountryID = countries.Single(s => s.Code =="CL").ID,  AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000000,  FullName ="Epidemiología del Ministerio de Salud de Chile",  	Name="EPI-MINSAL-CL",   CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago ").ID, AccessLevel = AccessLevel.Country},
               new AdminInstitution { InstID = 5600000001,  FullName ="SEREMI Metropolitana",  	Name="SEREMI Metropolitana",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000002,  FullName ="SEREMI Valparaíso",  	Name="SEREMI Valparaíso",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Valparaíso").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000005, FullName ="SEREMI Bio Bío",  	Name="SEREMI Bio Bío",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Del Bío Bío").ID, AccessLevel = AccessLevel.Area},
               new AdminInstitution { InstID = 5600000006,  FullName ="SEREMI Los Lagos ",  	Name="SEREMI Los Lagos",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Los Lagos").ID, AccessLevel = AccessLevel.Area},
            };
            admininstitutions.ForEach(v => Institutions.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var hospitals = new List<Hospital>
            {
               new Hospital { InstID =59100100002, FullName ="Hospital San Juan de Dios Tarija",  	Name="HSJDD TAR",     CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Valles").ID,  AccessLevel = AccessLevel.SelfOnly }, 
               new Hospital { InstID =59100100006, FullName ="hospital San Juan de Dios",  	Name="HSJDD SCZ",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Llanos").ID, AccessLevel = AccessLevel.Area},
               new Hospital { InstID =59100100007,  FullName ="Caja Nacional del Seguro Obrero Nro 3",  	Name="CNS Obrero 3",        CountryID = countries.Single(s => s.Code =="BO").ID,  AreaID=Areas.Single(s => s.Name =="Llanos").ID, AccessLevel = AccessLevel.Area},
               new Hospital { InstID =59100100008,  FullName ="Hospital del Niño Santa Cruz",  	Name="HN SCZ",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Llanos").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200004,  FullName ="Hospital Arco Iris",  	Name="Hospital Arco Iris",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200078,  FullName ="Hospital del Niño \"Ovidio Aliaga\"",  	Name="Hospital del Niño",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200081,  FullName ="Instituto Nacional del Tórax",  	Name="Inst. Nac. Tórax",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200082,  FullName ="Hospital Materno Infantil CNS",  	Name="CNS",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =59100200143,  FullName ="Hospital Boliviano Holandés",  	Name="Boliviano Holandés",        CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600002100,  FullName ="Hospital Dr. Ernesto Torres Galdames (Iquique)",  	Name="Hospital Iquique",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600007100,  FullName ="Hospital Dr. Gustavo Fricke",  	Name="Hospital Fricke",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Valparaíso").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600010100,  FullName ="Hospital San Juan de Dios",  	Name="Hospital San Juan",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600012530,  FullName ="Hospital Militar",  	Name="Hospital Militar",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600018100,  FullName ="Hospital Grant Benavente",  	Name="Hospital G.Benavente",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID, AccessLevel = AccessLevel.SelfOnly},
               new Hospital { InstID =5600024105,  FullName ="Hospital Puerto Montt",  	Name="Hospital P. Montt",        CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Valparaíso").ID, AccessLevel = AccessLevel.SelfOnly}
                                                                                                                                                                                                           
            };
            hospitals.ForEach(v => Institutions.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var labs = new List<Lab>
             {                                                                                                                                                                                                     
                    new Lab { InstID =59100100003,  FullName ="Laboratorio Departamental de ReferenciaTarija", Name ="LDRT", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Valles").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =59100100005,  FullName ="Coordinación Departamental de Laboratorios", Name ="CODELAB", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Valles").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =59100100009,  FullName ="Centro de Enfermedades Tropicales", Name ="CENETROP", CountryID = countries.Single(s => s.Code =="BO").ID, AreaID=Areas.Single(s => s.Name =="Llanos").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =59100299999,  FullName ="Instituto Nacional de Laboratorios de Salud \"Néstor Morales Villazán\"", Name ="INLASA Virologí­a", CountryID = countries.Single(s => s.Code =="BO").ID,AreaID=Areas.Single(s => s.Name =="Altiplano").ID, AccessLevel = AccessLevel.Country },
                    new Lab { InstID =5600000004,  FullName ="Instituto de Salud Pública de Chile", Name ="ISP-CL", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago ").ID,  AccessLevel = AccessLevel.Country },
                    new Lab { InstID =5600002101,  FullName ="Laboratorio del Hospital Dr. Ernesto Torres Galdames (Iquique)", Name ="Lab. H. Iquique", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600007101,  FullName ="Laboratorio del Hospital Dr. Gustavo Fricke", Name ="Lab. H. Fricke", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600008100,  FullName ="Laboratorio PCR San Camilo", Name ="Lab. PCR San Camilo", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Tarapacá").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600010101,  FullName ="Laboratorio del Hospital S.Juan de Dios", Name ="Lab. H. San Juan", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600010102,  FullName ="Laboratorio PCR del Hospital S.Juan de Dios", Name ="Lab. PCR H. San Juan", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600012531,  FullName ="Laboratorio del Hospital Militar", Name ="Lab. H. Militar", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Metropolitana De Santiago").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600018101,  FullName ="Laboratorio del Hospital Gran Benavente", Name ="Lab. H. G. Benavente", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Del Bío Bío").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600018102,  FullName ="Laboratorio PCR Concepción", Name ="Lab. PCR Concepción", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="Del Bío Bío").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600024106,  FullName ="Laboratorio del Hospital Puerto Montt", Name ="Lab. H. P. Montt", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Los Lagos").ID,  AccessLevel = AccessLevel.SelfOnly },
                    new Lab { InstID =5600024107,  FullName ="Laboratorio PCR Puerto Montt", Name ="Lab. PCR P. Montt", CountryID = countries.Single(s => s.Code =="CL").ID, AreaID=Areas.Single(s => s.Name =="De Los Lagos").ID,  AccessLevel = AccessLevel.SelfOnly }
             };
            labs.ForEach(v => Institutions.AddOrUpdate(p => p.Name, v));
            SaveChanges();

            var flucases = new List<FluCase>
            {
                new FluCase(){  
                     HospitalID =  hospitals.Single(s => s.FullName == "Hospital San Juan de Dios Tarija").ID,                        
                     Age = 18,
                     AMeasure = AMeasure.Year,                   
                     FName1 = "John",
                     LName1 = "Doe",
                     Gender = Gender.Male,
                     HospitalDate = new DateTime(2012, 12, 12),
                     NoExpediente = "123",
                     NationalId = "CI234",
                     RegDate = new DateTime(2012, 12, 12)
                },
                          
                new FluCase(){  
                     HospitalID =  hospitals.Single(s => s.FullName == "Hospital Grant Benavente").ID, 
                     Age = 1,
                     AMeasure = AMeasure.Month,
                     FName1 = "Jenny",
                     LName1 = "Smith",
                     Gender = Gender.Female,
                     HospitalDate = new DateTime(2013, 12, 12),
                     NoExpediente = "123",
                     NationalId = "CI1234",
                     RegDate = new DateTime(2013, 12, 12),
                     Comments = "comments2",
                     NoProRen = "NoProRen2",
                     FResult = "FResult2",
                     RecDate = DateTime.Now,
                     Processed = true,
                     EndLabDate = DateTime.Now
                },
                
                new FluCase(){  
                     HospitalID =  hospitals.Single(s => s.FullName == "Hospital Grant Benavente").ID,                   
                     DOB = new DateTime(2012, 12, 1),
                     FName1 = "David",
                     LName1 = "Chen",
                     Gender = Gender.Unknown,
                     HospitalDate = new DateTime(2013, 12, 12),
                     NoExpediente = "123",
                     NationalId = "CI1235",
                     RegDate = new DateTime(2013, 12, 12),
                     Comments = "comments1",
                     NoProRen = "NoProRen1",
                     FResult = "FResult1",
                     RecDate = DateTime.Now,
                     Processed = true,
                     EndLabDate = DateTime.Now
                }
            };
            flucases.ForEach(v => FluCases.AddOrUpdate(p => new { p.FName1, p.LName1 }, v));
            SaveChanges();


            DateTime? curentdate = DateTime.Today;
            var caselabtests = new List<CaseLabTest>
            {
                new CaseLabTest {  FluCaseID = flucases.Single(s => s.Comments =="comments1").ID, TestDate = curentdate.Value.AddDays(30),  TestEndDate = curentdate.Value.AddDays(60), VirusTypeID = virusTypes.Single(s => s.Name == "Parainfluenza 1").ID, TestResultID = "I", TestType=1},
                new CaseLabTest {  FluCaseID = flucases.Single(s => s.Comments =="comments1").ID, TestDate = curentdate.Value.AddDays(31),  TestEndDate = curentdate.Value.AddDays(60), VirusTypeID = virusTypes.Single(s => s.Name == "Parainfluenza 2").ID, TestResultID = "I", TestType=2},
                new CaseLabTest {  FluCaseID = flucases.Single(s => s.Comments =="comments1").ID, TestDate = curentdate.Value.AddDays(32),  TestEndDate = curentdate.Value.AddDays(60), VirusTypeID = virusTypes.Single(s => s.Name == "Parainfluenza 3").ID, TestResultID = "N", TestType=1},
                new CaseLabTest {  FluCaseID = flucases.Single(s => s.Comments =="comments1").ID, TestDate = curentdate.Value.AddDays(33),  TestEndDate = curentdate.Value.AddDays(60), VirusTypeID = virusTypes.Single(s => s.Name == "Parainfluenza 1" ).ID,TestResultID ="P", TestType=1},
                new CaseLabTest {  FluCaseID = flucases.Single(s => s.Comments =="comments2").ID, TestDate = curentdate.Value.AddDays(30),  TestEndDate = curentdate.Value.AddDays(60), VirusTypeID = virusTypes.Single(s => s.Name == "Parainfluenza 1").ID, TestResultID = "N", TestType=1},
                new CaseLabTest {  FluCaseID = flucases.Single(s => s.Comments =="comments2").ID, TestDate = curentdate.Value.AddDays(31),  TestEndDate = curentdate.Value.AddDays(60), VirusTypeID = virusTypes.Single(s => s.Name == "Parainfluenza 2").ID, TestResultID = "N", TestType=1},
                new CaseLabTest {  FluCaseID = flucases.Single(s => s.Comments =="comments2").ID, TestDate = curentdate.Value.AddDays(32),  TestEndDate = curentdate.Value.AddDays(60), VirusTypeID = virusTypes.Single(s => s.Name == "Parainfluenza 3").ID, TestResultID = "N", TestType=1},
                new CaseLabTest {  FluCaseID = flucases.Single(s => s.Comments =="comments2").ID, TestDate = curentdate.Value.AddDays(33),  TestEndDate = curentdate.Value.AddDays(60), VirusTypeID = virusTypes.Single(s => s.Name == "Parainfluenza 1").ID, TestResultID = "N", TestType=1},
            };

            foreach (CaseLabTest caselabtest in caselabtests)
            {
                if (CaseLabTests.FirstOrDefault(t => t.FluCaseID == caselabtest.FluCaseID && t.TestDate == caselabtest.TestDate && t.TestEndDate == caselabtest.TestEndDate && t.VirusTypeID == caselabtest.VirusTypeID) == null)
                {
                    CaseLabTests.Add(caselabtest);
                }
            };

            SaveChanges();

            var states = new List<State>();
            AddState(states, "Tarija", "BO", "Valles");
            AddState(states, "La Paz - El Aeto", "BO", "Altiplano");
            AddState(states, "La Paz", "BO", "Altiplano");
            AddState(states, "Santa Cruz", "BO", "Llanos");
            AddState(states, "Iquique", "CL", "De Tarapacá");
            AddState(states, "Tamarugal", "CL", "De Tarapacá");
            AddState(states, "Antofagasta", "CL", "De Antofagasta");
            AddState(states, "El Loa", "CL", "De Antofagasta");
            AddState(states, "Tocopilla", "CL", "De Antofagasta");
            AddState(states, "Copiapó", "CL", "De Atacama");
            AddState(states, "Chañaral", "CL", "De Atacama");
            AddState(states, "Huasco", "CL", "De Atacama");
            AddState(states, "Elqui", "CL", "De Coquimbo");
            AddState(states, "Choapa", "CL", "De Coquimbo");
            AddState(states, "Limarí", "CL", "De Coquimbo");
            AddState(states, "Valparaíso", "CL", "De Valparaíso");
            AddState(states, "Isla de Pascua", "CL", "De Valparaíso");
            AddState(states, "Los Andes", "CL", "De Valparaíso");
            AddState(states, "Petorca", "CL", "De Valparaíso");
            AddState(states, "Quillota", "CL", "De Valparaíso");
            AddState(states, "San Antonio", "CL", "De Valparaíso");
            AddState(states, "San Felipe de Aconcagua", "CL", "De Valparaíso");
            AddState(states, "Marga Marga", "CL", "De Valparaíso");
            AddState(states, "Cachapoal", "CL", "Del Libertador Bernardo O Higgins");
            AddState(states, "Cardenal Caro", "CL", "Del Libertador Bernardo O Higgins");
            AddState(states, "Colchagua", "CL", "Del Libertador Bernardo O Higgins");
            AddState(states, "Talca", "CL", "Del Maule");
            AddState(states, "Cauquenes", "CL", "Del Maule");
            AddState(states, "Curicó", "CL", "Del Maule");
            AddState(states, "Linares", "CL", "Del Maule");
            AddState(states, "Concepción", "CL", "Del Bío Bío");
            AddState(states, "Arauco", "CL", "Del Bío Bío");
            AddState(states, "Biobío", "CL", "Del Bío Bío");
            AddState(states, "ñuble", "CL", "Del Bío Bío");
            AddState(states, "Cautín", "CL", "De La Araucanía");
            AddState(states, "Malleco", "CL", "De La Araucanía");
            AddState(states, "Llanquihue", "CL", "De Los Lagos");
            AddState(states, "Chiloé", "CL", "De Los Lagos");
            AddState(states, "Osorno", "CL", "De Los Lagos");
            AddState(states, "Palena", "CL", "De Los Lagos");
            AddState(states, "Coihaique", "CL", "De Aisén Del General Carlos Ibáñez");
            AddState(states, "Aisén", "CL", "De Aisén Del General Carlos Ibáñez");
            AddState(states, "Capitán Prat", "CL", "De Aisén Del General Carlos Ibáñez");
            AddState(states, "General Carrera", "CL", "De Aisén Del General Carlos Ibáñez");
            AddState(states, "Magallanes", "CL", "De Magallanes Y De La Antártica");
            AddState(states, "Antártica Chilena", "CL", "De Magallanes Y De La Antártica");
            AddState(states, "Tierra del Fuego", "CL", "De Magallanes Y De La Antártica");
            AddState(states, "ñltima Esperanza", "CL", "De Magallanes Y De La Antártica");
            AddState(states, "Santiago", "CL", "Metropolitana De Santiago");
            AddState(states, "Cordillera", "CL", "Metropolitana De Santiago");
            AddState(states, "Chacabuco", "CL", "Metropolitana De Santiago");
            AddState(states, "Maipo", "CL", "Metropolitana De Santiago");
            AddState(states, "Melipilla", "CL", "Metropolitana De Santiago");
            AddState(states, "Talagante", "CL", "Metropolitana De Santiago");
            AddState(states, "Valdivia", "CL", "De Los Ríos");
            AddState(states, "Ranco", "CL", "De Los Ríos");
            AddState(states, "Arica", "CL", "De Arica y Parinacota");
            AddState(states, "Parinacota", "CL", "De Arica y Parinacota");
            AddState(states, "Ignorada o sin especificar", "CL", "Ignorada o sinespecificar");
            AddState(states, "SIN DATO", "GT", "SIN DATO");
            AddState(states, "GUATEMALA", "GT", "Guatemala");
            AddState(states, "SANTA CATARINA PINULA", "GT", "Guatemala");
            AddState(states, "SAN JOSE PINULA", "GT", "Guatemala");
            AddState(states, "SAN JOSE DEL GOLFO", "GT", "Guatemala");
            AddState(states, "PALENCIA", "GT", "Guatemala");
            AddState(states, "CHINAUTLA", "GT", "Guatemala");
            AddState(states, "SAN PEDRO AYAMPUC", "GT", "Guatemala");
            AddState(states, "MIXCO", "GT", "Guatemala");
            AddState(states, "SAN PEDRO SACATEPEQUEZ", "GT", "Guatemala");
            AddState(states, "SAN JUAN SACATEPEQUEZ", "GT", "Guatemala");
            AddState(states, "SAN RAYMUNDO", "GT", "Guatemala");
            AddState(states, "CHUARRANCHO", "GT", "Guatemala");
            AddState(states, "FRAIJANES", "GT", "Guatemala");
            AddState(states, "AMATITLAN", "GT", "Guatemala");
            AddState(states, "VILLA NUEVA", "GT", "Guatemala");
            AddState(states, "VILLA CANALES", "GT", "Guatemala");
            AddState(states, "SAN MIGUEL PETAPA", "GT", "Guatemala");
            AddState(states, "GUASTATOYA", "GT", "El Progreso");
            AddState(states, "MORAZAN", "GT", "El Progreso");
            AddState(states, "SAN AGUSTIN ACASAGUASTLAN", "GT", "El Progreso");
            AddState(states, "SAN CRISTOBAL ACASAGUASTLAN", "GT", "El Progreso");
            AddState(states, "EL JICARO", "GT", "El Progreso");
            AddState(states, "SANSARE", "GT", "El Progreso");
            AddState(states, "SANARATE", "GT", "El Progreso");
            AddState(states, "SAN ANTONIO LA PAZ", "GT", "El Progreso");
            AddState(states, "ANTIGUA GUATEMALA", "GT", "Sacatepequez");
            AddState(states, "JOCOTENANGO", "GT", "Sacatepequez");
            AddState(states, "PASTORES", "GT", "Sacatepequez");
            AddState(states, "SUMPANGO", "GT", "Sacatepequez");
            AddState(states, "SANTO DOMINGO XENACOJ", "GT", "Sacatepequez");
            AddState(states, "SANTIAGO SACATEPEQUEZ", "GT", "Sacatepequez");
            AddState(states, "SAN BARTOLOME MILPAS ALTAS", "GT", "Sacatepequez");
            AddState(states, "SAN LUCAS SACATEPEQUEZ", "GT", "Sacatepequez");
            AddState(states, "SANTA LUCIA MILPAS ALTAS", "GT", "Sacatepequez");
            AddState(states, "MAGDALENA MILPAS ALTAS", "GT", "Sacatepequez");
            AddState(states, "SANTA MARIA DE JESUS", "GT", "Sacatepequez");
            AddState(states, "CIUDAD VIEJA", "GT", "Sacatepequez");
            AddState(states, "SAN MIGUEL DUEñAS", "GT", "Sacatepequez");
            AddState(states, "SAN JUAN ALOTENANGO", "GT", "Sacatepequez");
            AddState(states, "SAN ANTONIO AGUAS CALIENTES", "GT", "Sacatepequez");
            AddState(states, "SANTA CATARINA BARAHONA", "GT", "Sacatepequez");
            AddState(states, "CHIMALTENANGO", "GT", "Chimaltenango");
            AddState(states, "SAN JOSE POAQUIL", "GT", "Chimaltenango");
            AddState(states, "SAN MARTIN JILOTEPEQUE", "GT", "Chimaltenango");
            AddState(states, "SAN JUAN COMALAPA", "GT", "Chimaltenango");
            AddState(states, "SANTA APOLONIA", "GT", "Chimaltenango");
            AddState(states, "TECPAN GUATEMALA", "GT", "Chimaltenango");
            AddState(states, "PATZUN", "GT", "Chimaltenango");
            AddState(states, "SAN MIGUEL POCHUTA", "GT", "Chimaltenango");
            AddState(states, "PATZICIA", "GT", "Chimaltenango");
            AddState(states, "SANTA CRUZ BALANYA", "GT", "Chimaltenango");
            AddState(states, "ACATENANGO", "GT", "Chimaltenango");
            AddState(states, "SAN PEDRO YEPOCAPA", "GT", "Chimaltenango");
            AddState(states, "SAN ANDRES ITZAPA", "GT", "Chimaltenango");
            AddState(states, "PARRAMOS", "GT", "Chimaltenango");
            AddState(states, "ZARAGOZA", "GT", "Chimaltenango");
            AddState(states, "EL TEJAR", "GT", "Chimaltenango");
            AddState(states, "ESCUINTLA", "GT", "Escuintla");
            AddState(states, "SANTA LUCIA COTZUMALGUAPA", "GT", "Escuintla");
            AddState(states, "LA DEMOCRACIA", "GT", "Escuintla");
            AddState(states, "SIQUINALA", "GT", "Escuintla");
            AddState(states, "MASAGUA", "GT", "Escuintla");
            AddState(states, "TIQUISATE", "GT", "Escuintla");
            AddState(states, "LA GOMERA", "GT", "Escuintla");
            AddState(states, "GUANAGAZAPA", "GT", "Escuintla");
            AddState(states, "SAN JOSE", "GT", "Escuintla");
            AddState(states, "IZTAPA", "GT", "Escuintla");
            AddState(states, "PALIN", "GT", "Escuintla");
            AddState(states, "SAN VICENTE PACAYA", "GT", "Escuintla");
            AddState(states, "NUEVA CONCEPCION", "GT", "Escuintla");
            AddState(states, "CUILAPA", "GT", "Santa Rosa");
            AddState(states, "BARBERENA", "GT", "Santa Rosa");
            AddState(states, "SANTA ROSA DE LIMA", "GT", "Santa Rosa");
            AddState(states, "CASILLAS", "GT", "Santa Rosa");
            AddState(states, "SAN RAFAEL LAS FLORES", "GT", "Santa Rosa");
            AddState(states, "ORATORIO", "GT", "Santa Rosa");
            AddState(states, "SAN JUAN TECUACO", "GT", "Santa Rosa");
            AddState(states, "CHIQUIMULILLA", "GT", "Santa Rosa");
            AddState(states, "TAXISCO", "GT", "Santa Rosa");
            AddState(states, "SANTA MARIA IXHUATAN", "GT", "Santa Rosa");
            AddState(states, "GUAZACAPAN", "GT", "Santa Rosa");
            AddState(states, "SANTA CRUZ NARANJO", "GT", "Santa Rosa");
            AddState(states, "PUEBLO NUEVO VIñAS", "GT", "Santa Rosa");
            AddState(states, "NUEVA SANTA ROSA", "GT", "Santa Rosa");
            AddState(states, "SOLOLA", "GT", "Solola");
            AddState(states, "SAN JOSE CHACAYA", "GT", "Solola");
            AddState(states, "SANTA MARIA VISITACION", "GT", "Solola");
            AddState(states, "SANTA LUCIA UTATLAN", "GT", "Solola");
            AddState(states, "NAHUALA", "GT", "Solola");
            AddState(states, "SANTA CATARINA IXTAHUACAN", "GT", "Solola");
            AddState(states, "SANTA CLARA LA LAGUNA", "GT", "Solola");
            AddState(states, "CONCEPCION", "GT", "Solola");
            AddState(states, "SAN ANDRES SEMETABAJ", "GT", "Solola");
            AddState(states, "PANAJACHEL", "GT", "Solola");
            AddState(states, "SANTA CATARINA PALOPO", "GT", "Solola");
            AddState(states, "SAN ANTONIO PALOPO", "GT", "Solola");
            AddState(states, "SAN LUCAS TOLIMAN", "GT", "Solola");
            AddState(states, "SANTA CRUZ LA LAGUNA", "GT", "Solola");
            AddState(states, "SAN PABLO LA LAGUNA", "GT", "Solola");
            AddState(states, "SAN MARCOS LA LAGUNA", "GT", "Solola");
            AddState(states, "SAN JUAN LA LAGUNA", "GT", "Solola");
            AddState(states, "SAN PEDRO LA LAGUNA", "GT", "Solola");
            AddState(states, "SANTIAGO ATITLAN", "GT", "Solola");
            AddState(states, "TOTONICAPAN", "GT", "Totonicapan");
            AddState(states, "SAN CRISTOBAL TOTONICAPAN", "GT", "Totonicapan");
            AddState(states, "SAN FRANCISCO EL ALTO", "GT", "Totonicapan");
            AddState(states, "SAN ANDRES XECUL", "GT", "Totonicapan");
            AddState(states, "MOMOSTENANGO", "GT", "Totonicapan");
            AddState(states, "SANTA MARIA CHIQUIMULA", "GT", "Totonicapan");
            AddState(states, "SANTA LUCIA LA REFORMA", "GT", "Totonicapan");
            AddState(states, "SAN BARTOLO AGUAS CALIENTES", "GT", "Totonicapan");
            AddState(states, "QUETZALTENANGO", "GT", "Quetzaltenango");
            AddState(states, "SALCAJA", "GT", "Quetzaltenango");
            AddState(states, "OLINTEPEQUE", "GT", "Quetzaltenango");
            AddState(states, "SAN CARLOS SIJA", "GT", "Quetzaltenango");
            AddState(states, "SIBILIA", "GT", "Quetzaltenango");
            AddState(states, "CABRICAN", "GT", "Quetzaltenango");
            AddState(states, "CAJOLA", "GT", "Quetzaltenango");
            AddState(states, "SAN MIGUEL SIGUILA", "GT", "Quetzaltenango");
            AddState(states, "SAN JUAN OSTUNCALCO", "GT", "Quetzaltenango");
            AddState(states, "SAN MATEO", "GT", "Quetzaltenango");
            AddState(states, "CONCEPCION CHIQUIRICHAPA", "GT", "Quetzaltenango");
            AddState(states, "SAN MARTIN SACATEPEQUEZ", "GT", "Quetzaltenango");
            AddState(states, "ALMOLONGA", "GT", "Quetzaltenango");
            AddState(states, "CANTEL", "GT", "Quetzaltenango");
            AddState(states, "HUITAN", "GT", "Quetzaltenango");
            AddState(states, "ZUNIL", "GT", "Quetzaltenango");
            AddState(states, "COLOMBA", "GT", "Quetzaltenango");
            AddState(states, "SAN FRANCISCO LA UNION", "GT", "Quetzaltenango");
            AddState(states, "EL PALMAR", "GT", "Quetzaltenango");
            AddState(states, "COATEPEQUE", "GT", "Quetzaltenango");
            AddState(states, "GENOVA", "GT", "Quetzaltenango");
            AddState(states, "FLORES COSTA CUCA", "GT", "Quetzaltenango");
            AddState(states, "LA ESPERANZA", "GT", "Quetzaltenango");
            AddState(states, "PALESTINA DE LOS ALTOS", "GT", "Quetzaltenango");
            AddState(states, "MAZATENANGO", "GT", "Suchitepequez");
            AddState(states, "CUYOTENANGO", "GT", "Suchitepequez");
            AddState(states, "SAN FRANCISCO ZAPOTITLAN", "GT", "Suchitepequez");
            AddState(states, "SAN BERNARDINO", "GT", "Suchitepequez");
            AddState(states, "SAN JOSE EL IDOLO", "GT", "Suchitepequez");
            AddState(states, "SANTO DOMINGO SUCHITEPEQUEZ", "GT", "Suchitepequez");
            AddState(states, "SAN LORENZO", "GT", "Suchitepequez");
            AddState(states, "SAMAYAC", "GT", "Suchitepequez");
            AddState(states, "SAN PABLO JOCOPILAS", "GT", "Suchitepequez");
            AddState(states, "SAN ANTONIO SUCHITEPEQUEZ", "GT", "Suchitepequez");
            AddState(states, "SAN MIGUEL PANAN", "GT", "Suchitepequez");
            AddState(states, "SAN GABRIEL", "GT", "Suchitepequez");
            AddState(states, "CHICACAO", "GT", "Suchitepequez");
            AddState(states, "PATULUL", "GT", "Suchitepequez");
            AddState(states, "SANTA BARBARA", "GT", "Suchitepequez");
            AddState(states, "SAN JUAN BAUTISTA", "GT", "Suchitepequez");
            AddState(states, "SANTO TOMAS LA UNION", "GT", "Suchitepequez");
            AddState(states, "ZUNILITO", "GT", "Suchitepequez");
            AddState(states, "PUEBLO NUEVO SUCHITEPEQUEZ", "GT", "Suchitepequez");
            AddState(states, "RIO BRAVO", "GT", "Suchitepequez");
            AddState(states, "RETALHULEU", "GT", "Retalhuleu");
            AddState(states, "SAN SEBASTIAN", "GT", "Retalhuleu");
            AddState(states, "SANTA CRUZ MULUA", "GT", "Retalhuleu");
            AddState(states, "SAN MARTIN ZAPOTITLAN", "GT", "Retalhuleu");

            states.ForEach(v => States.AddOrUpdate(p => new { p.Name, p.AreaID }, v));
            SaveChanges();

            var locals = new List<Local>();
            AddLocal(locals, 1, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 2, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20101, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20102, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20103, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20104, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21803, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21901, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21902, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 22001, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21602, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21701, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21702, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21703, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21801, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21802, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21307, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21401, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21402, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21501, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21502, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21601, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21301, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21302, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21303, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21304, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21305, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21306, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21104, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21105, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21201, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21202, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21203, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21204, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21004, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21005, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21006, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21101, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21102, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21103, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20903, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20904, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20905, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21001, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21002, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 21003, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20804, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20805, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20806, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20807, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20901, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20902, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20608, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20701, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20702, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20801, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20802, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20803, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20602, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20603, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20604, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20605, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20606, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20607, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20402, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20403, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20501, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20502, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20503, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20601, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20304, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20305, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20306, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20307, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20308, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20401, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20105, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20201, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20202, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20301, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20302, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 20303, "La Paz", "BO", "Altiplano");
            AddLocal(locals, 60101, "Tarija", "BO", "Valles");
            AddLocal(locals, 60201, "Tarija", "BO", "Valles");
            AddLocal(locals, 60202, "Tarija", "BO", "Valles");
            AddLocal(locals, 60301, "Tarija", "BO", "Valles");
            AddLocal(locals, 60302, "Tarija", "BO", "Valles");
            AddLocal(locals, 60303, "Tarija", "BO", "Valles");
            AddLocal(locals, 60401, "Tarija", "BO", "Valles");
            AddLocal(locals, 60402, "Tarija", "BO", "Valles");
            AddLocal(locals, 60501, "Tarija", "BO", "Valles");
            AddLocal(locals, 60502, "Tarija", "BO", "Valles");
            AddLocal(locals, 60601, "Tarija", "BO", "Valles");
            AddLocal(locals, 70101, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70102, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70103, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70104, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70105, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70201, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71502, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71503, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71301, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71302, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71401, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71402, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71403, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71501, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71102, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71103, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71104, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71105, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71106, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71201, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71001, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71002, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71003, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71004, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71005, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 71101, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70804, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70805, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70901, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70902, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70903, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70904, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70705, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70706, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70707, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70801, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70802, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70803, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70602, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70603, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70701, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70702, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70703, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70704, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70403, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70404, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70501, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70502, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70503, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70601, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70202, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70301, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70302, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70303, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70401, "Santa Cruz", "BO", "Llanos");
            AddLocal(locals, 70402, "Santa Cruz", "BO", "Llanos");

            locals.ForEach(v => Locals.AddOrUpdate(p => new { p.Code, p.StateID }, v));
            SaveChanges();

            var neighborhoods = new List<Neighborhood>();
            AddNeighborhood(neighborhoods, 1, "La Paz", "BO", "Altiplano");
            AddNeighborhood(neighborhoods, 2, "La Paz", "BO", "Altiplano");

            neighborhoods.ForEach(v => Neighborhoods.AddOrUpdate(p => new { p.Code, p.StateID }, v));
            SaveChanges();


            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this));

            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists("Admin"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Admin"));
            }

            //Create Role Tester if it does not exist
            if (!RoleManager.RoleExists("Tester"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Tester"));
            }

            //Create Role Reader if it does not exist
            if (!RoleManager.RoleExists("Reader"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Reader"));
            }

            //Create Role Reader if it does not exist
            if (!RoleManager.RoleExists("Staff"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Staff"));
            }

            if (!RoleManager.RoleExists("Report"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Report"));
            }

            Object[,] users = {
                                {"Admin","Paho201$","xusheng@hotmail.com",  GetInstitutionID( "Pan American Health Organization PAHO-WHO"), "Sheng", "", "Xu", "", "xusheng@hotmail.com"  }, 
                                {"Admin","Paho201$","xionglijun@hotmail.com",  GetInstitutionID( "Pan American Health Organization PAHO-WHO"), "Lijun", "", "Xiong", "", "xionglijun@hotmail.com" },   
                                {"Staff","Paho201$","jCoro",  GetInstitutionID("OPS - Bolivia"),"Jorge","","Coro","",""  },
                                {"Staff","Paho201$","rSalamanca",  GetInstitutionID("OPS - Bolivia"),"Roxana","","Salamanca","",""  },
                                {"Staff","Paho201$","dPastor",  GetInstitutionID("OPS - Bolivia"),"Desiree","","Pastor","",""  },
                                {"Staff","Paho201$","aanez",  GetInstitutionID("OPS - Bolivia"),"Arletta","","Añez","","aanez@paho.org"  },
                                {"Staff","Paho201$","lSoraide",  GetInstitutionID("Ministerio de Salud y Deportes"),"Lizeth","","Soraide","Iriarte",""  },
                                {"Staff","Paho201$","aZarate",  GetInstitutionID("Ministerio de Salud y Deportes"),"Adolfo","","Zárate","",""  },
                                {"Staff","Paho201$","mMorales",  GetInstitutionID("Ministerio de Salud y Deportes"),"Martin","","Morales","",""  },
                                {"Staff","Paho201$","vTorrez",  GetInstitutionID("Hospital San Juan de Dios Tarija"),"Vivien","Silvana","Torrez","Choque",""  },
                                {"Staff","Paho201$","nAguado",  GetInstitutionID("Laboratorio Departamental de ReferenciaTarija"),"Nelly","","Aguado","Aparicio",""  },
                                {"Staff","Paho201$","wSantaCruz",  GetInstitutionID("Servicio Departamental de Salud Tarija"),"Walter","Horacio","Santa Cruz","",""  },
                                {"Staff","Paho201$","mRengel",  GetInstitutionID("Coordinación Departamental de Laboratorios"),"Maria","Dolores","Rengel","Estrada",""  },
                                {"Staff","Paho201$","rGross",  this.Institutions.Single(s => s.FullName =="hospital San Juan de Dios" && s.Name=="HSJDD SCZ").ID,"Rosmery","","Gross","Arteaga",""  },
                                {"Staff","Paho201$","fDominguez",  GetInstitutionID("Caja Nacional del Seguro Obrero Nro 3"),"Flora","","Dominguez","Quispe",""  },
                                {"Staff","Paho201$","eChavez",  GetInstitutionID("Caja Nacional del Seguro Obrero Nro 3"),"Ena","","Chavez","",""  },
                                {"Staff","Paho201$","eCabrera",  GetInstitutionID("Hospital del Niño Santa Cruz"),"Erika","","Cabrera","Albis",""  },
                                {"Staff","Paho201$","yRoca",  GetInstitutionID("Centro de Enfermedades Tropicales"),"Yelin","","Roca","Sanchez",""  },
                                {"Staff","Paho201$","sParedes",  GetInstitutionID("Servicio Departamental de Salud Santa Cruz"),"Susana","","Paredes","Montero",""  },
                                {"Staff","Paho201$","pruebas",  GetInstitutionID("Servicio Departamental de Salud La Paz"),"Pablo","","Bulba","",""  },
                                {"Staff","Paho201$","kkenta",  GetInstitutionID("Servicio Departamental de Salud La Paz"),"Karem","","Kenta","Vasquez",""  },
                                {"Staff","Paho201$","mfernandez",  GetInstitutionID("Hospital Arco Iris"),"Marcel","","Fernandez","Peralta",""  },
                                {"Staff","Paho201$","amendoza",  GetInstitutionID("Hospital del Niño \"Ovidio Aliaga\""),"Alfredo","","Mendoza","",""  },
                                {"Staff","Paho201$","jgutierrez",  GetInstitutionID("Instituto Nacional del Tórax"),"Juan","","Gutierrez","Quispe",""  },
                                {"Staff","Paho201$","msaavedra",  GetInstitutionID("Hospital Materno Infantil CNS"),"Margarita","","Saavedra","Cortez",""  },
                                {"Staff","Paho201$","aveizaga",  GetInstitutionID("Hospital Boliviano Holandés"),"Angel","","Veizaga","Quispe",""  },
                                {"Staff","Paho201$","pablo",  GetInstitutionID("Hospital Boliviano Holandés"),"Pablo","","Bulba","",""  },
                                {"Staff","Paho201$","acruz",  GetInstitutionID("Instituto Nacional de Laboratorios de Salud \"Néstor Morales Villazán\""),"Aleida","Nina","Cruz","",""  },
                                {"Staff","Paho201$","aCruzA",  GetInstitutionID("Instituto Nacional de Laboratorios de Salud \"Néstor Morales Villazán\""),"Angélica","Andrea","Cruz","Concha","angelica.cruz@redsalud.gov.cl"  },
                                {"Staff","Paho201$","jsantalla",  GetInstitutionID("Instituto Nacional de Laboratorios de Salud \"Néstor Morales Villazán\""),"Jose","Antonio","Santalla","Vargas","josesantalla@gmail.com"  },
                                {"Staff","Paho201$","bgalleguillos",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Bárbara","","Galleguillos","",""  },
                                {"Staff","Paho201$","nVergara",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Natalia","","Vergara","Mallegas",""  },
                                {"Staff","Paho201$","vSotomayor",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Viviana","","Sotomayor","",""  },
                                {"Staff","Paho201$","fSoto",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Franco","","Soto","",""  },
                                {"Staff","Paho201$","aEspinosa",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Angélica","","Espinosa","",""  },
                                {"Staff","Paho201$","mRojas",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Marcelo","","Rojas","",""  },
                                {"Staff","Paho201$","pruCL",  GetInstitutionID("Epidemiología del Ministerio de Salud de Chile"),"Prueba","","Chile","",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("SEREMI Metropolitana"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","mArenas",  GetInstitutionID("SEREMI Valparaíso"),"Myriam","","Arenas","",""  },
                                {"Staff","Paho201$","acruz",  GetInstitutionID("SEREMI Tarapacá"),"Aleida","Nina","Cruz","",""  },
                                {"Staff","Paho201$","aCruzA",  GetInstitutionID("SEREMI Tarapacá"),"Angélica","Andrea","Cruz","Concha","angelica.cruz@redsalud.gov.cl"  },
                                {"Staff","Paho201$","vDiaz",  GetInstitutionID("SEREMI Tarapacá"),"Verónica","","Díaz","",""  },
                                {"Staff","Paho201$","rFasce",  GetInstitutionID("Instituto de Salud Pública de Chile"),"Rodrigo","","Fasce","",""  },
                                {"Staff","Paho201$","aGutierrez",  GetInstitutionID("SEREMI Bio Bío"),"Andrea","","Gutierrez","",""  },
                                {"Staff","Paho201$","jUlloa",  GetInstitutionID("SEREMI Los Lagos"),"Juana","","Ulloa","",""  },
                                {"Staff","Paho201$","acruz",  GetInstitutionID("Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Aleida","Nina","Cruz","",""  },
                                {"Staff","Paho201$","aCruzA",  GetInstitutionID("Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Angélica","Andrea","Cruz","Concha","angelica.cruz@redsalud.gov.cl"  },
                                {"Staff","Paho201$","vDiaz",  GetInstitutionID("Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Verónica","","Díaz","",""  },
                                {"Staff","Paho201$","oLopez",  GetInstitutionID("Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Olga","","López","",""  },
                                {"Staff","Paho201$","efernandez",  GetInstitutionID("Laboratorio del Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Eduardo","","Fernández","",""  },
                                {"Staff","Paho201$","dOrtiz",  GetInstitutionID("Laboratorio del Hospital Dr. Ernesto Torres Galdames (Iquique)"),"David","","Ortíz","",""  },
                                {"Staff","Paho201$","eBejarano",  GetInstitutionID("Laboratorio del Hospital Dr. Ernesto Torres Galdames (Iquique)"),"Enrique","","Bejarano","",""  },
                                {"Staff","Paho201$","eBejarano",  GetInstitutionID("CAN Antofagasta"),"Enrique","","Bejarano","",""  },
                                {"Staff","Paho201$","mBlanco",  GetInstitutionID("Hospital Dr. Gustavo Fricke"),"Myriam","","Blanco","",""  },
                                {"Staff","Paho201$","mArenas",  GetInstitutionID("Hospital Dr. Gustavo Fricke"),"Myriam","","Arenas","",""  },
                                {"Staff","Paho201$","bOsandon",  GetInstitutionID("Laboratorio del Hospital Dr. Gustavo Fricke"),"Berta","","Osandón","",""  },
                                {"Staff","Paho201$","pMursell",  GetInstitutionID("Laboratorio del Hospital Dr. Gustavo Fricke"),"Pablo","","Mursell","",""  },
                                {"Staff","Paho201$","pMursell",  GetInstitutionID("Laboratorio PCR San Camilo"),"Pablo","","Mursell","",""  },
                                {"Staff","Paho201$","tTarride",  this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios" && s.Name=="Hospital San Juan").ID,"Tamara","","Tarride","Muñoz",""  },
                                {"Staff","Paho201$","jVelasquez", this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios"&& s.Name=="Hospital San Juan").ID,"José","Luis","Velásquez","Mellado","joluveme@hotmail.com"  },
                                {"Staff","Paho201$","aCespedes",  this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios" && s.Name=="Hospital San Juan").ID,"Alejandra","","Céspedes","",""  },
                                {"Staff","Paho201$","lChanqueo",  this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios" && s.Name=="Hospital San Juan").ID,"Leonardo","","Chanqueo","",""  },
                                {"Staff","Paho201$","ePalta",   this.Institutions.Single(s => s.FullName =="Hospital San Juan de Dios" && s.Name=="Hospital San Juan").ID,"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","aCespedes",  GetInstitutionID("Laboratorio del Hospital S.Juan de Dios"),"Alejandra","","Céspedes","",""  },
                                {"Staff","Paho201$","lChanqueo",  GetInstitutionID("Laboratorio del Hospital S.Juan de Dios"),"Leonardo","","Chanqueo","",""  },
                                {"Staff","Paho201$","aRamirez",  GetInstitutionID("Laboratorio del Hospital S.Juan de Dios"),"Alejandro","","Ramí­rez","Acharan",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("Laboratorio del Hospital S.Juan de Dios"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","aRamirez",  GetInstitutionID("Laboratorio PCR del Hospital S.Juan de Dios"),"Alejandro","","Ramí­rez","Acharan",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("Laboratorio PCR del Hospital S.Juan de Dios"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","pSpalloni",  GetInstitutionID("Hospital Militar"),"Pía","","Spalloni","",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("Hospital Militar"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","aFicka",  GetInstitutionID("Hospital Militar"),"Alberto","","Ficka","",""  },
                                {"Staff","Paho201$","ePalta",  GetInstitutionID("Laboratorio del Hospital Militar"),"Eliana","","Palta","",""  },
                                {"Staff","Paho201$","gMarambio",  GetInstitutionID("Laboratorio del Hospital Militar"),"Gloria","","Marambio","",""  },
                                {"Staff","Paho201$","cAguayo",  GetInstitutionID("Hospital Grant Benavente"),"Claudia","","Aguayo","",""  },
                                {"Staff","Paho201$","cOrtega",  GetInstitutionID("Hospital Grant Benavente"),"Carlos","","Ortega","",""  },
                                {"Staff","Paho201$","aGutierrez",  GetInstitutionID("Hospital Grant Benavente"),"Andrea","","Gutierrez","",""  },
                                {"Staff","Paho201$","lTwele",  GetInstitutionID("Hospital Grant Benavente"),"Loreto","","Twele","",""  },
                                {"Staff","Paho201$","mOpazo",  GetInstitutionID("Laboratorio del Hospital Gran Benavente"),"Marina","","Opazo","",""  },
                                {"Staff","Paho201$","iRodriguez",  GetInstitutionID("Laboratorio del Hospital Gran Benavente"),"Iván","","Rodriguez","",""  },
                                {"Staff","Paho201$","iRodriguez",  GetInstitutionID("Laboratorio PCR Concepción"),"Iván","","Rodriguez","",""  },
                                {"Staff","Paho201$","jUlloa",  GetInstitutionID("Hospital Puerto Montt"),"Juana","","Ulloa","",""  },
                                {"Staff","Paho201$","dErnts",  GetInstitutionID("Hospital Puerto Montt"),"Diana","","Ernts","",""  },
                                {"Staff","Paho201$","pLanino",  GetInstitutionID("Hospital Puerto Montt"),"Paola","","Lanino","",""  },
                                {"Staff","Paho201$","lAcevedo",  GetInstitutionID("Laboratorio del Hospital Puerto Montt"),"Loreto","","Acevedo","",""  },
                                {"Staff","Paho201$","mlRioseco",  GetInstitutionID("Laboratorio del Hospital Puerto Montt"),"María Luisa","","Rioseco","",""  },
                                {"Staff","Paho201$","lAcevedo",  GetInstitutionID("Laboratorio PCR Puerto Montt"),"Loreto","","Acevedo","",""  },                              
                                {"Admin","Paho201$","ooliva",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Otavio","","Oliva","",""  },
                                {"Admin","Paho201$","ceisner",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Charles","","Eisner","",""  },
                                {"Admin","Paho201$","mcerpa",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Mauricio","","Cerpa","",""  },
                                {"Admin","Paho201$","rpalekar",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Rakhee","","Palekar","",""  },
                                {"Admin","Paho201$","epedroni",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Elena","","Pedroni","",""  },
                                {"Admin","Paho201$","obilbao",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Oona","","Bilbao","",""  },
                                {"Admin","Paho201$","lCatala",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Laura","","Catala","Pascual",""  },
                                {"Admin","Paho201$","mtdacosta",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Maria","Tereza","da Costa","Oliveira",""  },
                                {"Admin","Paho201$","voviedo",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Vladimir","","Oviedo","",""  },
                                {"Admin","Paho201$","pablo",  GetInstitutionID("Pan American Health Organization PAHO-WHO"),"Pablo","","Bulba","",""  }
                              };

            for (int i = 0; i < users.GetLength(0); i++)
            {
                CreateUser(UserManager, new ApplicationUser()
                {
                    UserName = users[i, 2] as string,
                    InstitutionID = (long)users[i, 3],
                    FirstName1 = users[i, 4] as string,
                    FirstName2 = users[i, 5] as string,
                    LastName1 = users[i, 6] as string,
                    LastName2 = users[i, 7] as string,
                    Email = users[i, 8] as string,
                    EmailConfirmed = true
                }, users[i, 1] as string, users[i, 0] as string);
            }
        }

        private long GetInstitutionID(String FullName)
        {
            try
            {
                return this.Institutions.Single(s => s.FullName == FullName).ID;
            }
            catch (System.InvalidOperationException)
            {
                throw new Exception(FullName);
            }
        }

        private void CreateUser(UserManager<ApplicationUser> UserManager, ApplicationUser user, string password, string rolename)
        {

            //Create User
            if (UserManager.FindByName(user.UserName) == null)
            {
                try
                {
                    var adminresult = UserManager.Create(user, password);

                    //Add User Admin to Role Admin
                    if (adminresult.Succeeded)
                    {
                        var result = UserManager.AddToRole(user.Id, rolename);
                    }
                }
                catch (DbEntityValidationException e)
                {
                    StringBuilder message = new StringBuilder();
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        message.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            message.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw new Exception(message.ToString());
                }
            }
        }
        private void AddState(List<State> states, String name, string countryCode, string areaName)
        {
            var area = Areas.FirstOrDefault(s => s.Country.Code == countryCode && s.Name == areaName);
            if (area != null)
            {
                states.Add(new State { Name = name, AreaID = area.ID });
            };

        }

        private void AddLocal(List<Local> locals, int code, String statename, string countryCode, string areaName)
        {
            var state = States.FirstOrDefault(s => s.Name == statename && s.Area.Country.Code == countryCode && s.Area.Name == areaName);
            if (state != null)
            {
                locals.Add(new Local { StateID = state.ID, Code = code });
            };

        }

        private void AddNeighborhood(List<Neighborhood> neighborhoods, int code, String statename, string countryCode, string areaName)
        {
            var state = States.FirstOrDefault(s => s.Name == statename && s.Area.Country.Code == countryCode && s.Area.Name == areaName);
            if (state != null)
            {
                neighborhoods.Add(new Neighborhood { StateID = state.ID, Code = code });
            };
        }
    }

    public class PahoDbInitializer : CreateDatabaseIfNotExists<PahoDbContext>
    {
        protected override void Seed(PahoDbContext context)
        {
            context.LoadData();

            var sqlFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.sql").OrderBy(x => x);
            foreach (string file in sqlFiles)
            {
                context.Database.ExecuteSqlCommand(File.ReadAllText(file));
            }

            base.Seed(context);
        }
    }

}