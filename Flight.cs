//==========================================================
// Student Number : S10266695
// Student Name :  T Venkatesh
// Partner Name : Pugazhenthi Dharundev
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s10266695_s10266942_prg2_assignment
{
    abstract class Flight
    {
        private string flightNumber;
        private string origin;
        private string destination;
        private DateTime expectedTime;
        private string status;
        private string specialRequestCode;  // Added field

        public string FlightNumber
        {
            get { return flightNumber; }
            set { flightNumber = value; }
        }

        public string Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public DateTime ExpectedTime
        {
            get { return expectedTime; }
            set { expectedTime = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string SpecialRequestCode  // Added property
        {
            get { return specialRequestCode; }
            set { specialRequestCode = value; }
        }

        public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, string specialRequestCode = "")
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
            SpecialRequestCode = specialRequestCode;
        }

        public abstract double CalculateFees();

        public override string ToString()
        {
            return $"Flight: {FlightNumber}, Origin: {Origin}, Destination: {Destination}, Expected: {ExpectedTime}, Status: {Status}, Special Request: {SpecialRequestCode}";
        }
    }
}