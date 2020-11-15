using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetFullOrderData
    {

public Int64 OrderNo             {set;get;} //bigint
public string IDNO                {set;get;}//varchar
public string UserName            {set;get;}//nvarchar
public DateTime BookingDate         {set;get;}//datetime
public Int16 CMS                 {set;get;}//tinyint
public Int16 BS                  {set;get;}//tinyint
public Int16 CS                  {set;get;}//tinyint
public Int16 MS                  {set;get;}//tinyint
public DateTime SD                  {set;get;}//datetime
public DateTime ED                  {set;get;}//datetime
public DateTime OStopTime           {set;get;}//datetime
public string LStation            {set;get;}//nvarchar
public string RStation            {set;get;}//nvarchar
public string CarTypeName         {set;get;}//varchar
public string CarNo               {set;get;}//varchar
public string PRONAME             {set;get;}//nvarchar
public Int16 InvoicKind          {set;get;}//tinyint
public string BCode               {set;get;}//varchar
public string TEL                 {set;get;}//varchar
public int PurePrice           {set;get;}//int
public string CARRIERID           {set;get;}//varchar
public string NPOBAN              {set;get;}//varchar
public string title               {set;get;}//nvarchar
public DateTime FS                  {set;get;}//datetime
public DateTime FE                  {set;get;}//datetime
public Single StartMile           {set;get;}//float
public Single StopMile            {set;get;}//float
public int CarRent             {set;get;}//int
public int FinalPrice          {set;get;}//int
public int FinePrice           {set;get;}//int
public int Mileage             {set;get;}//int
public int eTag                {set;get;}//int
public int TransDiscount       {set;get;}//int
public int CarPoint            {set;get;}//int
public int MotorPoint          {set;get;}//int
public DateTime FineTime            {set;get;}//datetime
public int Insurance           {set;get;}//int
public int InsurancePurePrice  {set;get;}//int
public string LFeedBack           {set;get;}//nvarchar
public string RFeedBack           {set;get;}//nvarchar
public Decimal P_LBA               {set;get;}//decimal
public Decimal P_RBA               {set;get;}//decimal
public Decimal P_TBA               {set;get;}//decimal
public Decimal P_MBA               {set;get;}//decimal
public Decimal P_lon               {set;get;}//decimal
public Decimal P_lat               {set;get;}//decimal
public Decimal R_LBA               {set;get;}//decimal
public Decimal R_RBA               {set;get;}//decimal
public Decimal R_TBA               {set;get;}//decimal
public Decimal R_MBA               {set;get;}//decimal
public Decimal R_lon               {set;get;}//decimal
public Decimal R_lat               {set;get;}//decimal
public int Reward              {set;get;}//int
public int CarDispatch         {set;get;}//int
public string DispatchRemark      {set;get;}//nvarchar
public int CleanFee            {set;get;}//int
public string CleanFeeRemark      {set;get;}//nvarchar
public int DestroyFee          {set;get;}//int
public string DestroyFeeRemark    {set;get;}//nvarchar
public int ParkingFee          {set;get;}//int
public string ParkingFeeRemark    {set;get;}//nvarchar
public int DraggingFee         {set;get;}//int
public string DraggingFeeRemark   {set;get;}//nvarchar
public int OtherFee            {set;get;}//int
public string OtherFeeRemark      {set;get;}//nvarchar
public int MachiFee            {set;get;}//int
public string EngineNO            {set;get;}//varchar
public string CarColor            {set;get;}//nvarchar
public string CarBrend            {set;get;}//varchar
public DateTime MEMBIRTH            {set;get;}//datetime
public string CityName            {set;get;}//nvarchar
public string AreaName            {set;get;}//nvarchar
public string MEMADDR             {set;get;}//nvarchar
    }
}
