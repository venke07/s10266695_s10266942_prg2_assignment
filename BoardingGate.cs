//==========================================================
// Student Number : S10266942
// Student Name :  Pugazhenthi Dharundev
// Partner Name : T Venkatesh
//==========================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s10266695_s10266942_prg2_assignment;

internal class BoardingGate
{
    private string gateName;
    public string GateName
    {
        get { return gateName; }
        set { gateName = value; }
    }

    private bool supportsCFFT;
    public bool SupportsCFFT
    {
        get { return supportsCFFT; }
        set { supportsCFFT = value; }
    }

    private bool supportsDDJB;
    public bool SupportsDDJB
    {
        get { return supportsDDJB; }
        set { supportsDDJB = value; }
    }

    private bool supportsLWTT;
    public bool SupportsLWTT
    {
        get { return supportsLWTT; }
        set { supportsLWTT = value; }
    }

    private Flight flight;
    public Flight Flight
    {
        get { return flight; }
        set { flight = value; }
    }

    public BoardingGate() { }

    public BoardingGate(string gateName, bool supportsCFFT, bool supportsDDJB, bool supportsLWTT, Flight flight)

    {

        GateName = gateName;
        SupportsCFFT = supportsCFFT;
        SupportsDDJB = supportsDDJB;
        SupportsLWTT = supportsLWTT;
        Flight = flight;
    }


    public double CalculateFees()
    {
        if (flight == null)
            return 0;

        double totalFees = 300; // Base boarding gate fee

        // Add fees based on special request support
        if (supportsDDJB && flight is DDJBFlight)
            totalFees += 300;

        if (supportsCFFT && flight is CFFTFlight)
            totalFees += 150;

        if (supportsLWTT && flight is LWTTFlight)
            totalFees += 500;

        return totalFees;
    }



    public override string ToString()
    {
        return "Gate Name : " + GateName + "\n" + "Supports CFFT : " + SupportsCFFT + "\n" + "Supports DDJB : " + SupportsDDJB + "\n" + "Supports LWTT : " + SupportsLWTT + "\n" + "Flight : " + Flight;
    }
}
