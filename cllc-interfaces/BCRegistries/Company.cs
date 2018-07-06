using System.Runtime.Serialization;
using System;
using System.Globalization;

namespace BCRegWrapper
{
    public class Company
    {
        //[DataMember(Name="name")]
        public string name { get; set; }
        
        //[DataMember(Name="number")]
        public string number { get; set; }
        
        //[DataMember(Name="corporationType")]
        public string corporationType { get; set; }
        
        //[DataMember(Name="corporationClass")]
        public string corporationClass { get; set; }
        
        //[DataMember(Name="bn9")]
        public string bn9 { get; set; }
        
        //[DataMember(Name="bn15")]
        public string bn15 { get; set; }
        
        //[DataMember(Name="jurisdiction")]
        public string jurisdiction { get; set; }
        
        //[DataMember(Name="stateCode")]
        public string stateCode { get; set; }
        
        //[DataMember(Name="stateActive")]
        public string stateActive { get; set; }
        
        //[DataMember(Name="stateDescription")]
        public string stateDescription { get; set; }
    }
}