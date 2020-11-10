using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_BookingControl
    {
        
        public Int64 OrderNo { set; get; }// bigint
        public string PROCD      {set;get;}// char
        public string ODCUSTID   {set;get;}// varchar
        public string ODCUSTNM   {set;get;}// nvarchar
        public string TEL1       {set;get;}// varchar
        public string TEL2       {set;get;}// varchar
        public string TEL3       {set;get;}// varchar
        public string ODDATE     {set;get;}// varchar
        public string GIVEDATE   {set;get;}// varchar
        public string GIVETIME   {set;get;}// varchar
        public string RNTDATE    {set;get;}// varchar
        public string RNTTIME    {set;get;}// varchar
        public string CARTYPE    {set;get;}// varchar
        public string CARNO      {set;get;}// varchar
        public string OUTBRNH    {set;get;}// varchar
        public string INBRNH     {set;get;}// varchar
        public int ORDAMT     {set;get;}// int
        public string REMARK     {set;get;}// nvarchar
        public int PAYAMT     {set;get;}// int
        public int RPRICE     {set;get;}// int
        public int RINV       {set;get;}// int
        public float DISRATE    {set;get;}// float
        public int NETRPRICE  {set;get;}// int
        public int RNTAMT     {set;get;}// int
        public int INSUAMT    {set;get;}// int
        public float RENTDAY    {set;get;}// float
        public int EBONUS     {set;get;}// int
        public string PROJTYPE   {set;get;}// varchar
        public Int16 TYPE       {set;get;}// tinyint
        public Int16 INVKIND    {set;get;}// tinyint
        public string INVTITLE   {set;get;}// nvarchar
        public string UNIMNO     {set;get;}// varchar
        public string TSEQNO     {set;get;}// varchar
        public string CARRIERID  {set;get;}// varchar
        public string NPOBAN     {set;get;}// varchar
        public int NOCAMT     {set;get;}// int


    }
}
