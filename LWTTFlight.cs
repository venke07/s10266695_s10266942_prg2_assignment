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

        public override double CalculateFees() // Example logic 200
        {
            // Base fee calculation for all flights
            double baseFee = 300; // Boarding Gate Base Fee

            // Add fee based on if it's arriving or departing
            if (destination.Equals("Singapore(SIN)", StringComparison.OrdinalIgnoreCase))
            {
                baseFee += 500; // Arriving Flight Fee
            }
            else if (origin.Equals("Singapore(SIN)", StringComparison.OrdinalIgnoreCase))
            {
                baseFee += 800; // Departing Flight Fee
            }

            return baseFee;
        }


        public override string ToString()
        {
            return base.ToString() + $" (LWTTFlight, Request Fee: {requestFee})";
        }
    }
}
