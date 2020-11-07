using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_HandlePolygon:SPInput_Base
    {
        public string StationID  {set;get;}
        public int BlockID    {set;get;}
        public string BlockName  {set;get;}
        public string MAPColor   {set;get;}
        public string Longitude  {set;get;}
        public string Latitude   {set;get;}
        public DateTime StartDate  {set;get;}
        public DateTime EndDate    {set;get;}
        public int Mode       {set;get;}
        public string UserID { set; get; }
    }
}
