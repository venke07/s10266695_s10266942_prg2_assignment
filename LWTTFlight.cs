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
    internal class LWTTFlight : Flight
    {
        private double requestFee;

        public double RequestFee
        {
            get { return requestFee; }
            set { requestFee = value; }
        }

        public LWTTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            RequestFee = requestFee;
        }

        public override double CalculateFees()
        {
            double baseFee = 300; // Base boarding gate fee

            // Add fees based on direction
            if (Destination.Contains("SIN"))
                baseFee += 500; // Arriving flight fee
            else if (Origin.Contains("SIN"))
                baseFee += 800; // Departing flight fee

            // Add LWTT special request fee
            baseFee += 500;

            // Add request fee from constructor
            return baseFee + RequestFee;
        }


        public override string ToString()
        {
            return base.ToString() + $" (LWTTFlight, Request Fee: {requestFee})";
        }
    }
}
