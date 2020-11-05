using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_HandleStation:SPInput_Base
    {
        public int StationType { get; set; }
        public string StationID { get; set; }
        public string StationName { get; set; }
        public string ManagerStationID { get; set; }
        public string UniCode { get; set; }
        public Int16 CityID { get; set; }
        public int AreaID { get; set; }
        public string Addr { get; set; }
        public string TEL { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string in_description { get; set; }
        public string show_description { get; set; }
        public int IsRequired { get; set; }
        public int StationPick { get; set; }
        public string FCode { get; set; }
        public DateTime SDate { get; set; }
        public DateTime EDate { get; set; }
        public Int16 ParkingNum { get; set; }
        public int OnlineNum { get; set; }
        public string Area { get; set; }
        public string fileName1 { get; set; }
        public string fileName2 { get; set; }
        public string fileName3 { get; set; }
        public string fileName4 { get; set; }
        public string fileDescript1 { get; set; }
        public string fileDescript2 { get; set; }
        public string fileDescript3 { get; set; }
        public string fileDescript4 { get; set; }
        public string UserID { set; get; }
        public int Mode { get; set; }
    }
}
